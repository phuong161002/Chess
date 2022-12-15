using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece
{
    private Vector2Int[] directions = new Vector2Int[]
    {
        Vector2Int.down, 
        Vector2Int.up, 
        Vector2Int.left, 
        Vector2Int.right
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
