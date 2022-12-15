using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.UIElements;
using Quaternion = System.Numerics.Quaternion;
using Vector2 = UnityEngine.Vector2;

public class MoveGenerator
{
    public bool inCheck;
    public bool inDoubleCheck;
    public bool pinsExistInPosition;
    private bool genQuiets;

    private ChessPlayer player;
    private ChessPlayer opponent;
    private Board board;

    private List<Move> moves;

    

    private ulong pinRayBitmask;
    private ulong checkRayBitmask;
    private ulong opponentKnightAttacks;
    public ulong opponentPawnAttackMap;
    public ulong opponentAttackMapNoPawns;
    private ulong opponentSlidingAttackMap;
    public ulong opponentAttackMap;

    public static readonly ulong[] kingAttackBitboards;
    public static readonly ulong[] knightAttackBitboards;
    public static readonly ulong[][] pawnAttackBitboards;
    public static readonly int[] directionOffsets = { 8, -8, -1, 1, 7, -7, 9, -9 };

    private Vector2Int[] directions =
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right,
        new Vector2Int(1, 1),
        new Vector2Int(-1, 1),
        new Vector2Int(1, -1),
        new Vector2Int(-1, -1)
    };


    static MoveGenerator()
    {
        int[] allKnightJumps = { 15, 17, -17, -15, 10, -6, 6, -10 };
        kingAttackBitboards = new ulong[64];
        knightAttackBitboards = new ulong[64];
        pawnAttackBitboards = new ulong[64][];
        for (int squareIndex = 0; squareIndex < 64; squareIndex++)
        {
            int y = squareIndex / 8;
            int x = squareIndex - y * 8;

            // Calculate all squares king can move to from current square (not including castling)
            foreach (int kingMoveDelta in directionOffsets)
            {
                int kingMoveSquare = squareIndex + kingMoveDelta;
                if (kingMoveSquare >= 0 && kingMoveSquare < 64)
                {
                    int kingSquareY = kingMoveSquare / 8;
                    int kingSquareX = kingMoveSquare - kingSquareY * 8;
                    // Ensure king has moved max of 1 square on x/y axis (to reject indices that have wrapped around side of board)
                    int maxCoordMoveDst =
                        System.Math.Max(System.Math.Abs(x - kingSquareX), System.Math.Abs(y - kingSquareY));
                    if (maxCoordMoveDst == 1)
                    {
                        kingAttackBitboards[squareIndex] |= 1ul << kingMoveSquare;
                    }
                }
            }

            // Calculate all squares knight can jump to from current square
            ulong knightBitboard = 0;
            foreach (int knightJumpDelta in allKnightJumps)
            {
                int knightJumpSquare = squareIndex + knightJumpDelta;
                if (knightJumpSquare >= 0 && knightJumpSquare < 64)
                {
                    int knightSquareY = knightJumpSquare / 8;
                    int knightSquareX = knightJumpSquare - knightSquareY * 8;
                    // Ensure knight has moved max of 2 squares on x/y axis (to reject indices that have wrapped around side of board)
                    int maxCoordMoveDst = System.Math.Max(System.Math.Abs(x - knightSquareX),
                        System.Math.Abs(y - knightSquareY));
                    if (maxCoordMoveDst == 2)
                    {
                        knightBitboard |= 1ul << knightJumpSquare;
                    }
                }
            }

            knightAttackBitboards[squareIndex] = knightBitboard;

            // Calculate legal pawn captures for white and black
            pawnAttackBitboards[squareIndex] = new ulong[2];
            if (x > 0)
            {
                if (y < 7)
                {
                    pawnAttackBitboards[squareIndex][(int)TeamColor.WHITE] |= 1ul << (squareIndex + 7);
                }

                if (y > 0)
                {
                    pawnAttackBitboards[squareIndex][(int)TeamColor.BLACK] |= 1ul << (squareIndex - 9);
                }
            }

            if (x < 7)
            {
                if (y < 7)
                {
                    pawnAttackBitboards[squareIndex][(int)TeamColor.WHITE] |= 1ul << (squareIndex + 9);
                }

                if (y > 0)
                {
                    pawnAttackBitboards[squareIndex][(int)TeamColor.BLACK] |= 1ul << (squareIndex - 7);
                }
            }
        }
    }

    public MoveGenerator(ChessPlayer chessPlayer, Board board)
    {
        this.player = chessPlayer;
        this.board = board;
    }

    public List<Move> GenerateMoves(bool includeQuietMoves = true)
    {
        genQuiets = includeQuietMoves;
        Init();
        CalculateAttackData();
        GenerateKingMoves();

        if (inDoubleCheck)
        {
            foreach (Piece piece in player.ActivePieces)
            {
                if (!(piece is King))
                {
                    piece.AvailableMoves.Clear();
                }
            }

            return moves;
        }

        GenerateSlidingMoves();
        GenerateKnightMoves();
        GeneratePawnMoves();

        return moves;
    }

    private void Init()
    {
        moves = new List<Move>();
        inCheck = false;
        inDoubleCheck = false;
        pinsExistInPosition = false;
        checkRayBitmask = 0;
        pinRayBitmask = 0;
        opponent = player.opponent;
        pinRayBitmask = 0;
        checkRayBitmask = 0;
        opponentKnightAttacks = 0;
        opponentPawnAttackMap = 0;
        opponentAttackMapNoPawns = 0;
        opponentSlidingAttackMap = 0;
        opponentAttackMap = 0;
        player.UpdatePiecesOnBoard();
    }

   

    void CalculateAttackData()
    {
        opponentSlidingAttackMap = 0;
        GenSlidingAttackMap();

        int startDirIndex = 0;
        int endDirIndex = 8;

        if (player.opponentQueens.Length == 0)
        {
            startDirIndex = (player.opponentRooks.Length > 0) ? 0 : 4;
            endDirIndex = (player.opponentBishops.Length > 0) ? 8 : 4;
        }

        for (int dir = startDirIndex; dir < endDirIndex; dir++)
        {
            bool isDiagonal = dir > 3;
            Vector2Int directionOffset = directions[dir];
            bool isFriendlyPieceAlongRay = false;
            ulong rayMask = 0;

            Vector2Int coords = player.friendlyKing.occupiedSquare + directionOffset;
            while (board.CheckIfCoordsAreOnBoard(coords))
            {
                int squareIndex = coords.x + coords.y * Board.BOARD_SIZE;
                rayMask |= 1ul << squareIndex;
                Piece piece = board.GetPieceAtSquare(coords);
                // This square contains a piece
                if (piece != null)
                {
                    if (piece.team == player.Team)
                    {
                        // First friendly piece we have come across in this direction
                        if (!isFriendlyPieceAlongRay)
                        {
                            isFriendlyPieceAlongRay = true;
                        }
                        // This is the second friendly piece we've found in this direction, therefore pin is not possible
                        else
                        {
                            break;
                        }
                    }
                    // This square contains an enemy piece
                    else
                    {
                        if ((isDiagonal && piece is Bishop) || (!isDiagonal && piece is Rook) || piece is Queen)
                        {
                            // Friendly piece blocks the check, so this is a pin
                            if (isFriendlyPieceAlongRay)
                            {
                                pinsExistInPosition = true;
                                pinRayBitmask |= rayMask;
                            }
                            // No friendly piece blocking the attack, so this is a check
                            else
                            {
                                checkRayBitmask |= rayMask;
                                inDoubleCheck = inCheck; // if already in check, then this is double check
                                inCheck = true;
                            }

                            break;
                        }
                        else
                        {
                            // This enemy piece is not able to move in the current direction,
                            // and so is blocking any checks/pins
                            break;
                        }
                    }
                }

                coords += directionOffset;
            }

            // Stop searching for pins if in double check,
            // as the king is the only piece able to move in that case anyway
            if (inDoubleCheck)
            {
                break;
            }
        }

        // Knight attacks
        opponentKnightAttacks = 0;
        bool isKnightCheck = false;
        foreach (Knight opponentKnight in player.opponentKnights)
        {
            Vector2Int startCoords = opponentKnight.occupiedSquare;
            int squareIndex = startCoords.x + startCoords.y * Board.BOARD_SIZE;
            opponentKnightAttacks |= knightAttackBitboards[squareIndex];

            if (!isKnightCheck && BitBoardUtility.ContainsSquare(opponentKnightAttacks, player.friendlyKing.occupiedSquare))
            {
                isKnightCheck = true;
                inDoubleCheck = inCheck;
                inCheck = true;
                checkRayBitmask |= 1ul << squareIndex;
            }
        }

        // Pawn attacks
        opponentPawnAttackMap = 0;
        bool isPawnCheck = false;
        foreach (Pawn opponentPawn in player.opponentPawns)
        {
            Vector2Int startCoords = opponentPawn.occupiedSquare;
            int squareIndex = startCoords.x + startCoords.y * Board.BOARD_SIZE;
            ulong pawnAttacks = pawnAttackBitboards[squareIndex][(int)opponent.Team];
            opponentPawnAttackMap |= pawnAttacks;

            if (!isPawnCheck && BitBoardUtility.ContainsSquare(pawnAttacks, player.friendlyKing.occupiedSquare))
            {
                isPawnCheck = true;
                inDoubleCheck = inCheck;
                inCheck = true;
                checkRayBitmask |= 1ul << squareIndex;
            }
        }

        Vector2Int enemyKingSquare = player.opponentKing.occupiedSquare;
        int enemyKingSquareIndex = enemyKingSquare.x + enemyKingSquare.y * Board.BOARD_SIZE;
        opponentAttackMapNoPawns =
            opponentSlidingAttackMap | opponentKnightAttacks | kingAttackBitboards[enemyKingSquareIndex];
        opponentAttackMap = opponentAttackMapNoPawns | opponentPawnAttackMap;
    }

    public bool SquareIsAttacked(Vector2Int coords)
    {
        return BitBoardUtility.ContainsSquare(opponentAttackMap, coords);
    }

    void GenSlidingAttackMap()
    {
        foreach (Rook rook in player.opponentRooks)
        {
            UpdateSlidingAttackPiece(rook, 0, 4);
        }

        foreach (Bishop bishop in player.opponentBishops)
        {
            UpdateSlidingAttackPiece(bishop, 4, 8);
        }

        foreach (Queen queen in player.opponentQueens)
        {
            UpdateSlidingAttackPiece(queen, 0, 8);
        }
    }

    void GenerateKingMoves()
    {
        List<Move> kingMoves = player.friendlyKing.TryGetAvailableMoves(player.friendlyKing.occupiedSquare);
        List<Move> movesToRemove = new List<Move>();
        foreach (Move kingMove in kingMoves)
        {
            bool isCapture = kingMove.pieceAtTarget != null;
            Vector2Int targetCoords = kingMove.targetCoords;

            if (!isCapture)
            {
                if (!genQuiets || SquareIsInCheckRay(targetCoords))
                {
                    movesToRemove.Add(kingMove);
                    continue;
                }
            }

            if (SquareIsAttacked(targetCoords) ||
                (inCheck && (kingMove.flag == MoveFlag.LeftCastling || kingMove.flag == MoveFlag.RightCastling)))
            {
                movesToRemove.Add(kingMove);
            }
        }

        foreach (var move in movesToRemove)
        {
            kingMoves.Remove(move);
        }

        moves.AddRange(kingMoves);
    }

    void UpdateSlidingAttackPiece(Piece piece, int startDir, int endDir)
    {
        for (int dirIndex = startDir; dirIndex < endDir; dirIndex++)
        {
            Vector2Int dir = directions[dirIndex];
            Vector2Int targetCoords = piece.occupiedSquare + dir;
            while (board.CheckIfCoordsAreOnBoard(targetCoords))
            {
                int squareIndex = targetCoords.x + targetCoords.y * Board.BOARD_SIZE;
                opponentSlidingAttackMap |= 1ul << squareIndex;
                Piece targetPiece = board.GetPieceAtSquare(targetCoords);
                if (targetPiece != player.friendlyKing)
                {
                    if (targetPiece != null)
                    {
                        break;
                    }
                }

                targetCoords += dir;
            }
        }
    }

    public bool IsPinned(Vector2Int coords)
    {
        int square = coords.x + coords.y * Board.BOARD_SIZE;
        return pinsExistInPosition && ((pinRayBitmask >> square) & 1) != 0;
    }

    bool SquareIsInCheckRay(Vector2Int coords)
    {
        int square = coords.x + coords.y * Board.BOARD_SIZE;
        return inCheck && ((checkRayBitmask >> square) & 1) != 0;
    }

    void GenerateSlidingMoves()
    {
        foreach (var rook in player.friendlyRooks)
        {
            GenerateSlidingPieceMoves(rook);
        }

        foreach (var bishop in player.friendlyBishops)
        {
            GenerateSlidingPieceMoves(bishop);
        }

        foreach (var queen in player.friendlyQueens)
        {
            GenerateSlidingPieceMoves(queen);
        }
    }

    void GenerateSlidingPieceMoves(Piece piece)
    {
        bool isPinned = IsPinned(piece.occupiedSquare);

        if (inCheck && isPinned)
        {
            piece.AvailableMoves.Clear();
            return;
        }

        List<Move> availableMoves = piece.TryGetAvailableMoves(piece.occupiedSquare);
        List<Move> movesToRemove = new List<Move>();
        foreach (Move move in availableMoves)
        {
            bool movePreventCheck = SquareIsInCheckRay(move.targetCoords);
            bool isCapture = move.pieceAtTarget != null;
            Vector2Int direction = move.targetCoords - move.sourceCoords;
            if ((inCheck && !movePreventCheck) || (!genQuiets && isCapture)
                || (isPinned && !IsMoveAlongRay(direction,
                    player.friendlyKing.occupiedSquare, move.sourceCoords)))
            {
                movesToRemove.Add(move);
            }
        }

        foreach (var move in movesToRemove)
        {
            availableMoves.Remove(move);
        }

        moves.AddRange(availableMoves);
    }

    void GenerateKnightMoves()
    {
        foreach (var knight in player.friendlyKnights)
        {
            if (IsPinned(knight.occupiedSquare))
            {
                knight.AvailableMoves.Clear();
                continue;
            }

            List<Move> availableMoves = knight.TryGetAvailableMoves(knight.occupiedSquare);
            List<Move> movesToRemove = new List<Move>();
            foreach (var move in availableMoves)
            {
                if ((!genQuiets && move.pieceAtTarget == null) || (inCheck && !SquareIsInCheckRay(move.targetCoords)))
                {
                    movesToRemove.Add(move);
                }
            }

            foreach (var move in movesToRemove)
            {
                availableMoves.Remove(move);
            }

            moves.AddRange(availableMoves);
        }
    }

    void GeneratePawnMoves()
    {
        foreach (var pawn in player.friendlyPawns)
        {
            List<Move> availableMoves = pawn.TryGetAvailableMoves(pawn.occupiedSquare);
            List<Move> movesToRemove = new List<Move>();
            foreach (var move in availableMoves)
            {
                Vector2Int direction = move.targetCoords - move.sourceCoords;
                if ((!genQuiets && direction.x == 0) || (inCheck && !SquareIsInCheckRay(move.targetCoords)) ||
                    (IsPinned(pawn.occupiedSquare) &&
                     !IsMoveAlongRay(direction,
                         pawn.occupiedSquare,
                         player.friendlyKing.occupiedSquare)))
                {
                    movesToRemove.Add(move);
                }
            }

            foreach (var move in movesToRemove)
            {
                availableMoves.Remove(move);
            }

            moves.AddRange(availableMoves);
        }
    }

    bool IsMoveAlongRay(Vector2Int moveDir, Vector2Int startCoords, Vector2Int targetCoords)
    {
        moveDir = convertToDiretion(moveDir);
        Vector2Int rayDir = convertToDiretion(targetCoords - startCoords);
        return moveDir == rayDir || moveDir == -rayDir;
    }

    Vector2Int convertToDiretion(Vector2Int moveDir)
    {
        if (moveDir.x == 0)
        {
            return new Vector2Int(0, Math.Sign(moveDir.y));
        }

        if (moveDir.y == 0)
        {
            return new Vector2Int(Math.Sign(moveDir.x), 0);
        }

        return new Vector2Int(Math.Sign(moveDir.x), Math.Sign(moveDir.y));
    }
}

public static class BitBoardUtility
{
    public static bool ContainsSquare(ulong bitboard, int square)
    {
        return ((bitboard >> square) & 1) != 0;
    }

    public static bool ContainsSquare(ulong bitboard, Vector2Int square)
    {
        return ((bitboard >> (square.x + square.y * Board.BOARD_SIZE)) & 1) != 0;
    }
}