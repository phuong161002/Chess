using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Piece
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
    
    public override List<Move> TryGetAvailableMoves(Vector2Int startCoords, bool inSearch = false)
    {
        AvailableMoves.Clear();
        foreach (Vector2Int dir in directions)
        {
            Vector2Int newCoords = startCoords + dir;
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
