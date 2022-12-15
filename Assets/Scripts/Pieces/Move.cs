using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Move
{
    public Vector2Int sourceCoords;
    public Vector2Int targetCoords;
    public Piece pieceAtSource;
    public Piece pieceAtTarget;
    public static Move InvalidMove = new Move();
    public MoveFlag flag;
    public Piece leftRook;
    public Piece rightRook;
    public Piece oppositePawn;

    private Move()
    {
        
    }
    
    public Move(Vector2Int source, Vector2Int target, Piece pieceAtSource)
    {
        this.sourceCoords = source;
        this.targetCoords = target;
        this.pieceAtSource = pieceAtSource;
    }

    public override string ToString()
    {
        return $"{sourceCoords} => {targetCoords}";
    }
}
