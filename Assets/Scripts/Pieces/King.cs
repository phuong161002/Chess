using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class King : Piece
{
    private Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(1, 1),
        new Vector2Int(1, -1),
        new Vector2Int(-1, 1),
        new Vector2Int(-1, -1),
        Vector2Int.down, 
        Vector2Int.left, 
        Vector2Int.right, 
        Vector2Int.up
    };

    private Vector2Int leftCastlingMove;
    private Vector2Int rightCastlingMove;

    private Piece leftRook;
    private Piece rightRook;

    public override List<Move> TryGetAvailableMoves(Vector2Int startCoords, bool inSearch = false)
    {
        AvailableMoves.Clear();
        AssignStandardMoves(startCoords);
        AssignCastlingMoves(startCoords);
        return AvailableMoves;
    }

    private void AssignStandardMoves(Vector2Int startCoords)
    {
        foreach (Vector2Int dir in directions)
        {
            Vector2Int newCoords = startCoords + dir;
            if (Board.CheckIfCoordsAreOnBoard(newCoords))
            {
                Piece piece = Board.GetPieceAtSquare(newCoords);
                if (piece == null || !isFromSameTeam(piece))
                {
                    TryToAddMove(new Move(startCoords, newCoords, this));
                }
            }
           
        }
    }

    private void AssignCastlingMoves(Vector2Int startCoords)
    {
        if (numberOfPieceMoves != 0)
        {
            return;
        }

        leftRook = GetPieceInDirection<Rook>(team, startCoords, Vector2Int.left);
        rightRook = GetPieceInDirection<Rook>(team, startCoords, Vector2Int.right);
        
        if (leftRook != null && leftRook.numberOfPieceMoves == 0)
        {
            leftCastlingMove = startCoords + Vector2Int.left * 2;
            TryToAddMove(new Move(startCoords, leftCastlingMove, this) {flag = MoveFlag.LeftCastling, leftRook = leftRook});
        }

        if (rightRook != null && rightRook.numberOfPieceMoves == 0)
        {
            rightCastlingMove = startCoords + Vector2Int.right * 2;
            TryToAddMove(new Move(startCoords, rightCastlingMove, this) {flag = MoveFlag.RightCastling, rightRook = rightRook});
        }
    }

    public override void MovePiece(Vector2Int targetCoords)
    {
        if (!hasMoved)
        {
            if (targetCoords == leftCastlingMove)
            {
                Vector2Int newCoords = targetCoords + Vector2Int.right;
                leftRook.MovePiece(newCoords);
            }
            if (targetCoords == rightCastlingMove)
            {
                Vector2Int newCoords = targetCoords + Vector2Int.left;
                rightRook.MovePiece(newCoords);
            }
        }
        base.MovePiece(targetCoords);
    }
}
