using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class PieceCreator : MonoBehaviour
{
    [SerializeField] private Pawn whitePawn;
    [SerializeField] private Pawn blackPawn;
    [SerializeField] private Bishop whiteBishop;
    [SerializeField] private Bishop blackBishop;
    [SerializeField] private Rook whiteRook;
    [SerializeField] private Rook blackRook;
    [SerializeField] private Knight whiteKnight;
    [SerializeField] private Knight blackKnight;
    [SerializeField] private Queen whiteQueen;
    [SerializeField] private Queen blackQueen;
    [SerializeField] private King whiteKing;
    [SerializeField] private King blackKing;

    private void Awake()
    {
        
    }

    public Piece GetPiecePrefab(PieceType pieceType, TeamColor teamColor)
    {
        bool isWhite = teamColor == TeamColor.WHITE;
        switch (pieceType)
        {
            case PieceType.Pawn:
                return isWhite ? whitePawn : blackPawn;
            case PieceType.Bishop:
                return isWhite ? whiteBishop : blackBishop;
            case  PieceType.Rook :
                return isWhite ? whiteRook : blackRook;
            case  PieceType.Knight:
                return isWhite ? whiteKnight : blackKnight;
            case  PieceType.Queen:
                return isWhite ? whiteQueen : blackQueen;
            case  PieceType.King:
                return isWhite ? whiteKing : blackKing;
            default:
                return null;
        }
    }

    public Piece CreatePiece(PieceType pieceType, TeamColor teamColor)
    {
        Piece prefab = GetPiecePrefab(pieceType, teamColor);
        if (prefab == null)
        {
            return null;
        }

        Piece piece = Instantiate(prefab, transform);
        return piece;
    }
}
