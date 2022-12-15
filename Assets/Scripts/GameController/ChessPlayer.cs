using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ChessPlayer
{
    public TeamColor Team { get; protected set; }
    public Board Board { get; protected set; }
    public List<Piece> ActivePieces { get; protected set; }

    public ChessPlayer opponent;

    protected MoveGenerator moveGenerator;
    
    public King friendlyKing;
    public Pawn[] friendlyPawns;
    public Rook[] friendlyRooks;
    public Bishop[] friendlyBishops;
    public Queen[] friendlyQueens;
    public Knight[] friendlyKnights;

    public King opponentKing;
    public Pawn[] opponentPawns;
    public Rook[] opponentRooks;
    public Bishop[] opponentBishops;
    public Queen[] opponentQueens;
    public Knight[] opponentKnights;

    protected ChessPlayer()
    {
    }

    public ChessPlayer(TeamColor team, Board board)
    {
        this.Team = team;
        this.Board = board;
        ActivePieces = new List<Piece>();
        moveGenerator = new MoveGenerator(this, board);
        Debug.Log(MoveGenerator.kingAttackBitboards);
    }

    public void AddPiece(Piece piece)
    {
        ActivePieces.Add(piece);
    }

    public void RemovePiece(Piece piece)
    {
        ActivePieces.Remove(piece);
    }

    public List<Move> GenerateMoves(bool includeQuietMoves = true)
    {
        if (moveGenerator == null)
        {
            Debug.LogError("null");
            return new List<Move>();
        }
        return moveGenerator.GenerateMoves(includeQuietMoves);
        // List<Move> moves = new List<Move>();
        // for(int i = 0; i < ActivePieces.Count; i++)
        // {
        //     if (Board.HasPiece(ActivePieces[i]))
        //     {
        //         moves.AddRange(ActivePieces[i].TryGetAvailableMoves(ActivePieces[i].occupiedSquare));
        //     }
        // }
        //
        // // for (int i = 0; i < Board.BOARD_SIZE; i++)
        // // {
        // //     for (int j = 0; j < Board.BOARD_SIZE; j++)
        // //     {
        // //         Piece piece = Board.GetPieceAtSquare(new Vector2Int(i, j));
        // //         if (piece != null && piece.team == Team)
        // //         {
        // //             moves.AddRange(piece.TryGetAvailableMoves(new Vector2Int(i, j)));
        // //         }
        // //     }
        // // }
        //
        // return moves;
    }

    public Piece[] GetPiecesAttackingOppositePieceOfType<T>() where T : Piece
    {
        return ActivePieces.Where(p => p.IsAttackingPieceOfType<T>()).ToArray();
    }

    public Piece[] GetPiecesOfType<T>() where T : Piece
    {
        return ActivePieces.Where(p => p is T && Board.HasPiece(p)).ToArray();
    }

    public void OnGameRestarted()
    {
        ActivePieces.Clear();
    }

    public bool IsInCheck()
    {
        return moveGenerator.inCheck;
    }

    public List<Vector2Int> GetPinsMap()
    {
        List<Vector2Int> res = new List<Vector2Int>();
        for (int i = 0; i < Board.BOARD_SIZE; i++)
        {
            for (int j = 0; j < Board.BOARD_SIZE; j++)
            {
                Vector2Int coords = new Vector2Int(i, j);
                if (moveGenerator.IsPinned(coords))
                {
                    res.Add(coords);
                }
            }
        }

        return res;
    }

    public List<Vector2Int> GetAttackMap()
    {
        List<Vector2Int> res = new List<Vector2Int>();
        for (int i = 0; i < Board.BOARD_SIZE; i++)
        {
            for (int j = 0; j < Board.BOARD_SIZE; j++)
            {
                Vector2Int coords = new Vector2Int(i, j);
                if (moveGenerator.SquareIsAttacked(coords))
                {
                    res.Add(coords);
                }
            }
        }

        return res;
    }
    
    public void UpdatePiecesOnBoard()
    {
        List<Piece> friendlyPieces = new List<Piece>();
        List<Piece> opponentPieces = new List<Piece>();
        for (int i = 0; i < Board.BOARD_SIZE; i++)
        {
            for (int j = 0; j < Board.BOARD_SIZE; j++)
            {
                Vector2Int coords = new Vector2Int(i, j);
                Piece piece = Board.GetPieceAtSquare(coords);
                if (piece != null)
                {
                    if (piece is King)
                    {
                        if (piece.team == this.Team)
                        {
                            friendlyKing = (King)piece;
                        }
                        else
                        {
                            opponentKing = (King)piece;
                        }
                    }
                    if (piece.team == this.Team)
                    {
                        friendlyPieces.Add(piece);
                    }
                    else
                    {
                        opponentPieces.Add(piece);
                    }
                }
            }
        }

        // friendlyKing = friendlyPieces.Where(p => p is King).First().ConvertTo<King>();
        friendlyQueens = friendlyPieces.Where(p => p is Queen).ConvertTo<Queen[]>();
        friendlyPawns = friendlyPieces.Where(p => p is Pawn).ConvertTo<Pawn[]>();
        friendlyRooks = friendlyPieces.Where(p => p is Rook).ConvertTo<Rook[]>();
        friendlyBishops = friendlyPieces.Where(p => p is Bishop).ConvertTo<Bishop[]>();
        friendlyKnights = friendlyPieces.Where(p => p is Knight).ConvertTo<Knight[]>();

        // opponentKing = opponentPieces.Where(p => p is King).First().ConvertTo<King>();
        opponentQueens = opponentPieces.Where(p => p is Queen).ConvertTo<Queen[]>();
        opponentPawns = opponentPieces.Where(p => p is Pawn).ConvertTo<Pawn[]>();
        opponentRooks = opponentPieces.Where(p => p is Rook).ConvertTo<Rook[]>();
        opponentBishops = opponentPieces.Where(p => p is Bishop).ConvertTo<Bishop[]>();
        opponentKnights = opponentPieces.Where(p => p is Knight).ConvertTo<Knight[]>();
    }
}