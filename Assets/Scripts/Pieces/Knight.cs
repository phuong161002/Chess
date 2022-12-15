using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
    private Vector2Int[] offsets = new Vector2Int[]
    {
        new Vector2Int(2, 1),
        new Vector2Int(2, -1),
        new Vector2Int(1, 2),
        new Vector2Int(1, -2),
        new Vector2Int(-2, 1),
        new Vector2Int(-2, -1),
        new Vector2Int(-1, 2),
        new Vector2Int(-1, -2),
    };

    public override List<Move> TryGetAvailableMoves(Vector2Int startCoords, bool inSearch = false)
    {
        AvailableMoves.Clear();
        foreach (Vector2Int offset in offsets)
        {
            Vector2Int newCoords = occupiedSquare + offset;
            if (Board.CheckIfCoordsAreOnBoard(newCoords) && !isFromSameTeam(Board.GetPieceAtSquare(newCoords)))
            {
                TryToAddMove(new Move(startCoords, newCoords, this));
            }
        }

        return AvailableMoves;
    }
}