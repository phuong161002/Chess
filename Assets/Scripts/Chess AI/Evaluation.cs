using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class Evaluation
{
    public const int pawnValue = 100;
    public const int knightValue = 300;
    public const int rookValue = 500;
    public const int bishopValue = 320;
    public const int queenValue = 900;

    public static int[,] orthogonalDistance;
    public static int[] centreManhattanDistance;


    static Evaluation()
    {
        orthogonalDistance = new int[64, 64];
        centreManhattanDistance = new int[64];
        for (int squareA = 0; squareA < 64; squareA++)
        {
            Vector2Int coordA = Board.CoordFromIndex(squareA);
            int fileDstFromCentre = Mathf.Max(3 - coordA.x, coordA.x - 4);
            int rankDstFromCentre = Mathf.Max(3 - coordA.y, coordA.y - 4);
            centreManhattanDistance[squareA] = fileDstFromCentre + rankDstFromCentre;

            for (int squareB = 0; squareB < 64; squareB++)
            {
                Vector2Int coordB = Board.CoordFromIndex(squareB);
                int rankDistance = Mathf.Abs(coordA.y - coordB.y);
                int fileDistance = Mathf.Abs(coordA.x - coordB.x);
                orthogonalDistance[squareA, squareB] = fileDistance + rankDistance;
                // kingDistance[squareA, squareB] = Max (fileDistance, rankDistance);
            }
        }
    }

    private const float endgameMaterialStart = rookValue * 2 + bishopValue + knightValue;

    public static int Evaluate(ChessPlayer chessPlayer)
    {
        ChessPlayer whitePlayer = ChessGameController.Instance.GetChessPlayerByTeamColor(TeamColor.WHITE);
        ChessPlayer blackPlayer = ChessGameController.Instance.GetChessPlayerByTeamColor(TeamColor.BLACK);
        whitePlayer.UpdatePiecesOnBoard();
        blackPlayer.UpdatePiecesOnBoard();

        int whiteEval = 0;
        int blackEval = 0;

        int whiteMaterial = CountMaterial(whitePlayer);
        int blackMaterial = CountMaterial(blackPlayer);

        int whiteMaterialWithoutPawns = whiteMaterial - whitePlayer.GetPiecesOfType<Pawn>().Length * pawnValue;
        int blackMaterialWithoutPawns = blackMaterial - blackPlayer.GetPiecesOfType<Pawn>().Length * pawnValue;
        float whiteEndgamePhaseWeight = EndgamePhaseWeight(whiteMaterialWithoutPawns);
        float blackEndgamePhaseWeight = EndgamePhaseWeight(blackMaterialWithoutPawns);

        whiteEval += whiteMaterial;
        blackEval += blackMaterial;
        whiteEval += MopUpEval(whitePlayer, blackPlayer, whiteMaterial, blackMaterial, blackEndgamePhaseWeight);
        blackEval += MopUpEval(blackPlayer, whitePlayer, blackMaterial, whiteMaterial, whiteEndgamePhaseWeight);

        whiteEval += EvaluatePieceSquareTables(whitePlayer, blackEndgamePhaseWeight);
        blackEval += EvaluatePieceSquareTables(blackPlayer, whiteEndgamePhaseWeight);

        int evaluation = whiteEval - blackEval;
        int perspective = chessPlayer.Team == TeamColor.WHITE ? 1 : -1;
        return evaluation * perspective;
    }

    static int CountMaterial(ChessPlayer chessPlayer)
    {
        int material = 0;

        material += chessPlayer.friendlyPawns.Length * pawnValue;
        material += chessPlayer.friendlyKnights.Length * knightValue;
        material += chessPlayer.friendlyBishops.Length * bishopValue;
        material += chessPlayer.friendlyRooks.Length * rookValue;
        material += chessPlayer.friendlyQueens.Length * queenValue;

        return material;
    }

    static float EndgamePhaseWeight(int materialCountWithoutPawns)
    {
        const float multiplier = 1 / endgameMaterialStart;
        return 1 - System.Math.Min(1, materialCountWithoutPawns * multiplier);
    }

    static int MopUpEval(ChessPlayer player, ChessPlayer opponent, int myMaterial, int opponentMaterial,
        float endgameWeight)
    {
        int mopUpScore = 0;
        if (myMaterial > opponentMaterial + pawnValue * 2 && endgameWeight > 0)
        {
            King friendlyKing = player.friendlyKing;
            King opponentKing = opponent.friendlyKing;
            int friendlyKingSquare = friendlyKing.occupiedSquare.x + friendlyKing.occupiedSquare.y * Board.BOARD_SIZE;
            int opponentKingSquare = opponentKing.occupiedSquare.x + opponentKing.occupiedSquare.y * Board.BOARD_SIZE;
            mopUpScore += centreManhattanDistance[opponentKingSquare] * 10;
            // use ortho dst to promote direct opposition
            mopUpScore += (14 - NumRookMovesToReachSquare(friendlyKingSquare, opponentKingSquare)) * 4;

            return (int)(mopUpScore * endgameWeight);
        }

        return 0;
    }

    static int EvaluatePieceSquareTables(ChessPlayer player, float endgamePhaseWeight)
    {
        int value = 0;
        bool isWhite = player.Team == TeamColor.WHITE;
        value += EvaluatePieceSquareTable(PieceSquareTable.pawns, player.friendlyPawns, isWhite);
        value += EvaluatePieceSquareTable(PieceSquareTable.rooks, player.friendlyRooks, isWhite);
        value += EvaluatePieceSquareTable(PieceSquareTable.knights, player.friendlyKnights, isWhite);
        value += EvaluatePieceSquareTable(PieceSquareTable.bishops, player.friendlyBishops, isWhite);
        value += EvaluatePieceSquareTable(PieceSquareTable.queens, player.friendlyQueens, isWhite);
        int kingEarlyPhase =
            PieceSquareTable.Read(PieceSquareTable.kingMiddle, player.friendlyKing.occupiedSquare, isWhite);
        value += (int)(kingEarlyPhase * (1 - endgamePhaseWeight));
        //value += PieceSquareTable.Read (PieceSquareTable.kingMiddle, board.KingSquare[colourIndex], isWhite);

        return value;
    }

    static int EvaluatePieceSquareTable<T>(int[] table, T[] pieces, bool isWhite) where T : Piece
    {
        int value = 0;
        for (int i = 0; i < pieces.Length; i++)
        {
            value += PieceSquareTable.Read(table, pieces[i].occupiedSquare, isWhite);
        }

        return value;
    }

    public static int GetPieceValue(PieceType pieceType)
    {
        switch (pieceType)
        {
            case PieceType.Pawn:
                return Evaluation.pawnValue;
            case PieceType.Bishop:
                return Evaluation.bishopValue;
            case PieceType.Rook:
                return Evaluation.rookValue;
            case PieceType.Knight:
                return Evaluation.knightValue;
            case PieceType.Queen:
                return Evaluation.queenValue;
            default:
                return 0;
        }
    }

    public static int NumRookMovesToReachSquare(int startSquare, int targetSquare)
    {
        return orthogonalDistance[startSquare, targetSquare];
    }
}