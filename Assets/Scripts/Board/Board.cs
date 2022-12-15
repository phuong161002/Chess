using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SquareSelectorCreator))]
public class Board : MonoBehaviour
{
    [SerializeField] private Transform bottomLeftSquare;
    public const int BOARD_SIZE = 8;
    [SerializeField] private float squareSize;

    private Piece[,] grid;
    private Piece selectedPiece;
    private SquareSelectorCreator _selectorCreator;

    private void Awake()
    {
        _selectorCreator = GetComponent<SquareSelectorCreator>();
        CreateGrid();
    }

    private void CreateGrid()
    {
        grid = new Piece[BOARD_SIZE, BOARD_SIZE];
    }

    public Vector2Int CalculateCoordsFromPosition(Vector3 position)
    {
        Vector3 boardPos = position - bottomLeftSquare.transform.position +
                           new Vector3(squareSize / 2, 0, squareSize / 2);
        return new Vector2Int(Mathf.FloorToInt(boardPos.x / squareSize), Mathf.FloorToInt(boardPos.z / squareSize));
    }

    public Vector3 CalculatePositionFromCoords(Vector2Int coords)
    {
        return new Vector3(coords.x * squareSize + bottomLeftSquare.transform.position.x, 0,
            coords.y * squareSize + bottomLeftSquare.transform.position.z);
    }

    public bool CheckIfCoordsAreOnBoard(Vector2Int coords)
    {
        if (coords.x < 0 || coords.x >= BOARD_SIZE || coords.y < 0 || coords.y >= BOARD_SIZE)
            return false;
        return true;
    }

    public void SetPiece(Piece piece, Vector2Int coords)
    {
        if (!CheckIfCoordsAreOnBoard(coords))
        {
            Debug.LogError("Index coords " + coords.ToString() + " was out of range");
            return;
        }

        grid[coords.x, coords.y] = piece;
    }

    public Piece GetPieceAtSquare(Vector2Int coords)
    {
        if (!CheckIfCoordsAreOnBoard(coords))
        {
            Debug.LogError("Index coords " + coords.ToString() + " was out of range");
            return null;
        }

        return grid[coords.x, coords.y];
    }

    public void OnSquareSelected(Vector3 position)
    {
        if (!ChessGameController.Instance.IsGameInProgress() 
            || ChessGameController.Instance.ActivePlayer is AIPlayer 
            || (GameManager.Instance.PlayMode == PlayMode.PvP && ChessGameController.Instance.ActivePlayer.Team != GameManager.Instance.MyTeamColor))
        {
            return;
        }

        Vector2Int coords = CalculateCoordsFromPosition(position);
        Piece piece = GetPieceAtSquare(coords);

        if (selectedPiece != null)
        {
            if (piece != null && piece == selectedPiece)
            {
                DeselectPiece();
            }
            else if (piece != null && selectedPiece != piece &&
                     ChessGameController.Instance.IsTeamTurnActive(piece.team))
            {
                DeselectPiece();
                SelectPiece(piece);
            }
            else if (selectedPiece.CanMoveTo(coords))
            {
                Move move = selectedPiece.GetMove(coords);
                OnSelectedPieceMoved(move, move.pieceAtSource);
                if (GameManager.Instance.PlayMode == PlayMode.PvP)
                {
                    Service.Instance.MovePiece(move.sourceCoords, move.targetCoords);
                }
            }
        }
        else
        {
            if (piece != null && ChessGameController.Instance.IsTeamTurnActive(piece.team))
            {
                SelectPiece(piece);
            }
        }
    }

    public void ShowSelectionSquares(List<Vector2Int> coords)
    {
        Dictionary<Vector3, bool> squaresData = new Dictionary<Vector3, bool>();
        for (int i = 0; i < coords.Count; i++)
        {
            Vector3 position = CalculatePositionFromCoords(coords[i]);
            bool isFreeSquare = GetPieceAtSquare(coords[i]) == null;
            squaresData.Add(position, isFreeSquare);
        }

        _selectorCreator.ShowSelection(squaresData);
    }

    public void DeselectPiece()
    {
        selectedPiece = null;
        _selectorCreator.ClearSelection();
    }

    public void SelectPiece(Piece piece)
    {
        selectedPiece = piece;
        _selectorCreator.ShowSelectedSquare(CalculatePositionFromCoords(piece.occupiedSquare));
        ShowSelectionSquares(piece.AvailableMoves.ConvertAll(m => m.targetCoords));
    }

    public bool HasPiece(Piece piece)
    {
        foreach (var item in grid)
        {
            if (item == piece)
                return true;
        }

        return false;
    }

    public void OnSelectedPieceMoved(Move move, Piece piece)
    {
        _selectorCreator.ClearMove();
        TryToTakeOppositePiece(move);
        MakeMove(move);
        piece.MovePiece(move.targetCoords);
        DeselectPiece();
        ShowMove(move);
        
        EndTurn();
    }

    private void EndTurn()
    {
        ChessGameController.Instance.EndTurn();
    }

    public void TryToTakeOppositePiece(Move move)
    {
        Piece piece = move.pieceAtTarget;
        if (piece != null && !move.pieceAtSource.isFromSameTeam(piece))
        {
            TakePiece(piece);
        }
    }

    private void TakePiece(Piece piece)
    {
        grid[piece.occupiedSquare.x, piece.occupiedSquare.y] = null;
        ChessGameController.Instance.OnPieceRemoved(piece);
    }

    public void OnGameRestarted()
    {
        selectedPiece = null;
        _selectorCreator.ClearAll();
        CreateGrid();
    }

    public void PromotePiece(Piece piece)
    {
        TakePiece(piece);
        ChessGameController.Instance.CreatePieceAndInitialize(piece.team, piece.occupiedSquare, PieceType.Queen);
    }

    public void MakeMove(Move move, bool inSearch = false)
    {
        if (move.pieceAtTarget is King)
        {
            Debug.Log(move);
            UpdateBoard();
            throw new Exception();
        }
        
        if (move.flag == MoveFlag.PawnEnPassant)
        {
            Vector2Int enemyCoords = move.targetCoords +
                                     (move.pieceAtSource.team == TeamColor.WHITE ? Vector2Int.down : Vector2Int.up);
            grid[enemyCoords.x, enemyCoords.y] = null;
        }
        else if (move.flag == MoveFlag.LeftCastling)
        {
            Piece rook = move.leftRook;
            Vector2Int newCoords = move.targetCoords + Vector2Int.right;
            grid[0, rook.team == TeamColor.WHITE ? 0 : 7] = null;
            grid[newCoords.x, newCoords.y] = rook;
            rook.numberOfPieceMoves++;
        }
        else if (move.flag == MoveFlag.RightCastling)
        {
            Piece rook = move.rightRook;
            Vector2Int newCoords = move.targetCoords + Vector2Int.left;
            grid[7, rook.team == TeamColor.WHITE ? 0 : 7] = null;
            grid[newCoords.x, newCoords.y] = rook;
            rook.numberOfPieceMoves++;
        }

        grid[move.sourceCoords.x, move.sourceCoords.y] = null;
        grid[move.targetCoords.x, move.targetCoords.y] = move.pieceAtSource;
        if (move.pieceAtSource == null)
        {
            // Debug.LogError(move.ToString());
        }
        else
        {
            move.pieceAtSource.occupiedSquare = move.targetCoords;
            move.pieceAtSource.numberOfPieceMoves++;
        }
    }

    public void UnmakeMove(Move move, bool inSearch = false)
    {
        if (move.flag == MoveFlag.PawnEnPassant)
        {
            Vector2Int enemyCoords = move.targetCoords +
                                     (move.pieceAtSource.team == TeamColor.WHITE ? Vector2Int.down : Vector2Int.up);

            grid[enemyCoords.x, enemyCoords.y] = move.oppositePawn;
        }
        else if (move.flag == MoveFlag.LeftCastling)
        {
            Piece rook = move.leftRook;
            grid[0, rook.team == TeamColor.WHITE ? 0 : 7] = rook;
            Vector2Int newCoords = move.targetCoords + Vector2Int.right;
            grid[newCoords.x, newCoords.y] = null;
            rook.numberOfPieceMoves--;
        }
        else if (move.flag == MoveFlag.RightCastling)
        {
            Piece rook = move.rightRook;
            grid[7, rook.team == TeamColor.WHITE ? 0 : 7] = rook;
            Vector2Int newCoords = move.targetCoords + Vector2Int.left;
            grid[newCoords.x, newCoords.y] = null;
            rook.numberOfPieceMoves--;
        }

        if (move.pieceAtSource == null)
        {
            // Debug.LogError(move.ToString());
        }

        grid[move.sourceCoords.x, move.sourceCoords.y] = move.pieceAtSource;
        grid[move.targetCoords.x, move.targetCoords.y] = move.pieceAtTarget;

        if (move.pieceAtTarget != null)
        {
            move.pieceAtTarget.occupiedSquare = move.targetCoords;
        }

        move.pieceAtSource.occupiedSquare = move.sourceCoords;
        move.pieceAtSource.numberOfPieceMoves--;

        // if (move.pieceAtTarget != null)
        // {
        //     move.pieceAtTarget.transform.position = CalculatePositionFromCoords(move.targetCoords);
        // }
        //
        // move.pieceAtSource.transform.position = CalculatePositionFromCoords(move.sourceCoords);
    }

    public void UpdateBoard()
    {
        for (int i = 0; i < BOARD_SIZE; i++)
        {
            for (int j = 0; j < BOARD_SIZE; j++)
            {
                Vector2Int coords = new Vector2Int(i, j);
                Piece piece = GetPieceAtSquare(coords);
                if (piece != null)
                {
                    piece.gameObject.SetActive(true);
                    piece.MovePiece(coords);
                }
            }
        }
    }
    
    public static Vector2Int CoordFromIndex(int index)
    {
        return new Vector2Int(index % BOARD_SIZE, index / BOARD_SIZE);
    }

    public void ShowMove(Move move)
    {
        _selectorCreator.ShowMove(move);
    }
        
}