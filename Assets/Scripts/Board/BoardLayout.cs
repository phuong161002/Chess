using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Board/Layout")]
public class BoardLayout : ScriptableObject
{
    [Serializable]
    private class BoardSquareSetup
    {
        public Vector2Int coords;
        public PieceType pieceType;
        public TeamColor team;
    }

    [SerializeField] private BoardSquareSetup[] boardSquareSetups;

    public int GetPiecesCount()
    {
        return boardSquareSetups.Length;
    }

    public Vector2Int GetPieceCoordsAtIndex(int index)
    {
        if (boardSquareSetups.Length <= index)
        {
            Debug.LogError("Index was out of range");
            return new Vector2Int(-1, -1);
        }

        return boardSquareSetups[index].coords;
    }
    
    public TeamColor GetPieceTeamColorAtIndex(int index)
    {
        if (boardSquareSetups.Length <= index)
        {
            Debug.LogError("Index was out of range");
            return TeamColor.WHITE;
        }

        return boardSquareSetups[index].team;
    }
    
    public PieceType GetPieceTypeAtIndex(int index)
    {
        if (boardSquareSetups.Length <= index)
        {
            Debug.LogError("Index was out of range");
            return PieceType.None;
        }

        return boardSquareSetups[index].pieceType;
    }
}
