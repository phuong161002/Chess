using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Piece : MonoBehaviour
{
    public Vector2Int occupiedSquare;
    public TeamColor team;
    public PieceType pieceType;
    public Board Board { get; private set; }
    public bool hasMoved;
    public int moveIndex;
    public static int numberOfMoves;
    public int numberOfPieceMoves;
    protected Vector2Int startSquare;
    private IObjectTweener _tweener;
    public List<Move> AvailableMoves
    {
        get;
        private set;
    }
    
    private MaterialSetter _materialSetter;
    public MaterialSetter MaterialSetter
    {
        get
        {
            if (_materialSetter == null)
            {
                _materialSetter = GetComponent<MaterialSetter>();
            }

            return _materialSetter;
        }
    }

    private void Awake()
    {
        AvailableMoves = new List<Move>();
        _tweener = GetComponent<InstantTweener>();
    }

    public void Setup(Vector2Int coords, TeamColor teamColor, Board board)
    {
        occupiedSquare = coords;
        startSquare = coords;
        team = teamColor;
        Board = board;
        transform.position = board.CalculatePositionFromCoords(coords);
        board.SetPiece(this, coords);
        SetMaterial(PieceCreator.Instance.GetTeamMaterial(teamColor));
    }

    public void SetMaterial(Material material)
    {
        MaterialSetter.SetMaterial(material);
    }

    public bool isFromSameTeam(Piece piece)
    {
        if (piece == null)
        {
            return false;
        }
        return this.team == piece.team;
    }

    public virtual List<Move> TryGetAvailableMoves(Vector2Int startCoords, bool inSearch = false)
    {
        return null;
    }

    protected void TryToAddMove(Move move)
    {
        if (!AvailableMoves.Contains(move))
        {
            // move.pieceAtSource = Board.GetPieceAtSquare(move.sourceCoords);
            move.pieceAtTarget = Board.GetPieceAtSquare(move.targetCoords);
            AvailableMoves.Add(move);
        }
    }

    public bool CanMoveTo(Vector2Int coords)
    {
        foreach (Move move in AvailableMoves)
        {
            if (move.targetCoords == coords)
            {
                return true;
            }
        }

        return false;
    }
    
    public Move GetMove(Vector2Int coords)
    {
        foreach (Move move in AvailableMoves)
        {
            if (move.targetCoords == coords)
            {
                return move;
            }
        }

        return Move.InvalidMove;
    }

    public virtual void MovePiece(Vector2Int targetCoords)
    {
        Vector3 targetPosition = Board.CalculatePositionFromCoords(targetCoords);
        occupiedSquare = targetCoords;
        hasMoved = true;
        moveIndex = numberOfMoves;
        numberOfMoves++;
        transform.position = targetPosition;
    }
    
    public bool IsAttackingPieceOfType<T>() where T : Piece
    {
        foreach (Move move in AvailableMoves)
        {
            if (Board.GetPieceAtSquare(move.targetCoords) is T)
            {
                return true;
            }
        }

        return false;
    }

    public Piece GetPieceInDirection<T>(TeamColor team, Vector2Int startCoords,Vector2Int direction) where T : Piece
    {
        Vector2Int coords = startCoords + direction;
        while (Board.CheckIfCoordsAreOnBoard(coords))
        {
            Piece piece = Board.GetPieceAtSquare(coords);
            if (piece != null)
            {
                if (piece.team == team && piece is T)
                {
                    return piece;
                }

                return null;
            }
            coords += direction;
        }

        return null;
    }

    public Vector2Int DistanceFromStartSquare()
    {
        return new Vector2Int(Math.Abs(occupiedSquare.x - startSquare.x), Math.Abs(occupiedSquare.y - startSquare.y));
    }
}