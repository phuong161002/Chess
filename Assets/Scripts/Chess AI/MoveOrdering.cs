using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOrdering
{
    int[] moveScores;
    const int maxMoveCount = 218;

    const int squareControlledByOpponentPawnPenalty = 350;
    const int capturedPieceValueMultiplier = 10;

    MoveGenerator moveGenerator;

    public MoveOrdering(MoveGenerator moveGenerator)
    {
        moveScores = new int[maxMoveCount];
        this.moveGenerator = moveGenerator;
    }

    public void OrderMoves(List<Move> moves)
    {
        for(int i = 0; i < moves.Count; i++)
        {
            int score = 0;
            Piece movePiece = moves[i].pieceAtSource;
            Piece capturePiece = moves[i].pieceAtTarget;
            if (capturePiece != null)
            {
                // Order moves to try capturing the most valuable opponent piece with least valuable of own pieces first
                // The capturedPieceValueMultiplier is used to make even 'bad' captures like QxP rank above non-captures
                score = capturedPieceValueMultiplier * Evaluation.GetPieceValue(capturePiece.pieceType) -
                        Evaluation.GetPieceValue(movePiece.pieceType);
            }

            if (movePiece.pieceType == PieceType.Pawn)
            {
                // Promotion Score
                if (moves[i].flag == MoveFlag.PawnPromotion)
                {
                    score += Evaluation.queenValue;
                }
            }
            else
            {
                if (BitBoardUtility.ContainsSquare(moveGenerator.opponentPawnAttackMap, moves[i].targetCoords))
                {
                    score -= squareControlledByOpponentPawnPenalty;
                }
            }
            
            moveScores[i] = score;
        }
        Sort(moves);   
    }
    
    void Sort (List<Move> moves) {
        // Sort the moves list based on scores
        for (int i = 0; i < moves.Count - 1; i++) {
            for (int j = i + 1; j > 0; j--) {
                int swapIndex = j - 1;
                if (moveScores[swapIndex] < moveScores[j]) {
                    (moves[j], moves[swapIndex]) = (moves[swapIndex], moves[j]);
                    (moveScores[j], moveScores[swapIndex]) = (moveScores[swapIndex], moveScores[j]);
                }
            }
        }
    }
}