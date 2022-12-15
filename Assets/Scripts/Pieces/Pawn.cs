using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{
    public override List<Move> TryGetAvailableMoves(Vector2Int startCoords, bool inSearch = false)
    {
        AvailableMoves.Clear();
        Vector2Int direction = team == TeamColor.WHITE ? Vector2Int.up : Vector2Int.down;
        int range = hasMoved ? 1 : 2;

        for (int i = 1; i <= range; i++)
        {
            Vector2Int newCoords = startCoords + direction * i;
            if (!Board.CheckIfCoordsAreOnBoard(newCoords) || Board.GetPieceAtSquare(newCoords) != null)
            {
                break;
            }

            TryToAddMove(new Move(startCoords, newCoords, this));
        }

        Vector2Int[] takeDirections = new Vector2Int[]
        {
            Vector2Int.left + direction,
            Vector2Int.right + direction
        };

        // Find opposite piece to take
        for (int i = 0; i < takeDirections.Length; i++)
        {
            Vector2Int newCoords = startCoords + takeDirections[i];
            if (Board.CheckIfCoordsAreOnBoard(newCoords))
            {
                Piece piece = Board.GetPieceAtSquare(newCoords);
                if (piece != null && !isFromSameTeam(piece))
                {
                    TryToAddMove(new Move(startCoords, newCoords, this));
                }
            }
        }

        int endYOfBoard = team == TeamColor.WHITE ? Board.BOARD_SIZE - 1 : 0;
        foreach (Move move in AvailableMoves)
        {
            if (move.targetCoords.y == endYOfBoard)
            {
                move.flag = MoveFlag.PawnPromotion;
            }
        }

        // if (DistanceFromStartSquare() == new Vector2Int(0, 3))
        // {
        //     Vector2Int[] enPassantDirection = new Vector2Int[]
        //     {
        //         Vector2Int.left,
        //         Vector2Int.right,
        //     };
        //     for (int i = 0; i < enPassantDirection.Length; i++)
        //     {
        //         Vector2Int newCoords = startCoords + enPassantDirection[i];
        //         if (Board.CheckIfCoordsAreOnBoard(newCoords))
        //         {
        //             Piece piece = Board.GetPieceAtSquare(newCoords);
        //             if (piece != null && !isFromSameTeam(piece) && piece is Pawn && piece.moveIndex == Piece.numberOfMoves - 1)
        //             {
        //                 TryToAddMove(new Move(startCoords, startCoords + enPassantDirection[i] + direction)
        //                     { isEnPassantMove = true, oppositePawn = piece });
        //             }
        //         }
        //     }
        // }

        return AvailableMoves;
    }

    public override void MovePiece(Vector2Int targetCoords)
    {
        base.MovePiece(targetCoords);
        // checkEnPassant();
        checkPromotion();
    }

    private void checkPromotion()
    {
        int endYOfBoard = team == TeamColor.WHITE ? Board.BOARD_SIZE - 1 : 0;
        if (occupiedSquare.y == endYOfBoard)
        {
            Board.PromotePiece(this);
        }
    }

    private void checkEnPassant()
    {
        if (DistanceFromStartSquare() == new Vector2Int(1, 4))
        {
            Piece piece = Board.GetPieceAtSquare(occupiedSquare + 
                (team == TeamColor.WHITE ? Vector2Int.down : Vector2Int.up));
            if (piece != null && !isFromSameTeam(piece) && piece is Pawn &&
                piece.moveIndex == Piece.numberOfMoves - 2)
            {
                // Board.TryToTakeOppositePiece(piece.occupiedSquare);
            }
        }
    }
}