using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bishop : Piece
{
    private Vector2Int[] directions =
    {
        new Vector2Int(1, 1),
        new Vector2Int(-1, 1),
        new Vector2Int(1, -1),
        new Vector2Int(-1, -1)
    };

    public override List<Move> TryGetAvailableMoves(Vector2Int startCoords, bool inSearch = false)
    {
        AvailableMoves.Clear();
        foreach (Vector2Int dir in directions)
        {
            Vector2Int newCoords = occupiedSquare + dir;
            while (Board.CheckIfCoordsAreOnBoard(newCoords))
            {
                Piece piece = Board.GetPieceAtSquare(newCoords);
                if (!isFromSameTeam(piece))
                {
                    TryToAddMove(new Move(startCoords, newCoords, this));
                }
                
                if(piece != null)
                {
                    break;
                }
                newCoords += dir;
            } 
        }

        return AvailableMoves;
    }
}
