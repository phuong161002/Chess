using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class ChessGameController : MonoBehaviour
{
    [SerializeField] private Board board;
    [SerializeField] private BoardLayout startingLayout;
    [SerializeField] private PieceCreator _pieceCreator;
    
    private ChessPlayer whitePlayer;
    private ChessPlayer blackPlayer;
    public ChessPlayer ActivePlayer { get; private set; }

    private CameraMovement _cameraMovement;

    private GameState gameState;
    private static ChessGameController _instance;
    
    public static ChessGameController Instance
    {
        get => _instance;
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }
        _cameraMovement = GetComponent<CameraMovement>();
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    private void Start()
    {
        CreatePlayer();
        StartNewGame();
    }

    private void CreatePlayer()
    {
        whitePlayer = new ChessPlayer(TeamColor.WHITE, board);
        if (GameManager.Instance.PlayMode == PlayMode.PvE)
        {
            blackPlayer = new AIPlayer(TeamColor.BLACK, board);
        }
        else
        {
            blackPlayer = new ChessPlayer(TeamColor.BLACK, board);
        }

        whitePlayer.opponent = blackPlayer;
        blackPlayer.opponent = whitePlayer;
    }

    private void StartNewGame()
    {
        UIManager.Instance.SwitchTo(CanvasTags.PlayRoom);
        SetGameState(GameState.Init);
        CreatePieceFromLayout(startingLayout);
        ActivePlayer = whitePlayer;
        GenerateAllPossiblePlayerMoves(ActivePlayer);
        GenerateAllPossiblePlayerMoves(GetOpponentToPlayer(ActivePlayer));
        SetGameState(GameState.Play);

        if (ActivePlayer is AIPlayer)
        {
            ((AIPlayer)ActivePlayer).ComputerMove();
        }
    }

    private void CreatePieceFromLayout(BoardLayout layout)
    {
        int length = layout.GetPiecesCount();
        for (int i = 0; i < length; i++)
        {
            Vector2Int coords = layout.GetPieceCoordsAtIndex(i);
            TeamColor team = layout.GetPieceTeamColorAtIndex(i);
            PieceType pieceType = layout.GetPieceTypeAtIndex(i);
            CreatePieceAndInitialize(team, coords, pieceType);
        }
    }

    public void CreatePieceAndInitialize(TeamColor team, Vector2Int coords, PieceType pieceType)
    {
        Piece piece = _pieceCreator.CreatePiece(pieceType, team);
        piece.name = team.ToString() + " " + pieceType.ToString();
        piece.pieceType = pieceType;
        piece.Setup(coords, team, board);
        if (piece.team == TeamColor.BLACK)
        {
            blackPlayer.AddPiece(piece);
        }
        else
        {
            whitePlayer.AddPiece(piece);
        }
    }

    private void SetGameState(GameState state)
    {
        gameState = state;
    }

    public bool IsGameInProgress()
    {
        return gameState == GameState.Play;
    }

    private ChessPlayer GetOpponentToPlayer(ChessPlayer player)
    {
        if (player.Team == TeamColor.WHITE)
        {
            return blackPlayer;
        }

        return whitePlayer;
    }

    public ChessPlayer GetChessPlayerByTeamColor(TeamColor teamColor)
    {
        if (teamColor == TeamColor.WHITE)
        {
            return whitePlayer;
        }

        return blackPlayer;
    }

    public bool IsTeamTurnActive(TeamColor team)
    {
        return team == ActivePlayer.Team;
    }

    private void GenerateAllPossiblePlayerMoves(ChessPlayer player)
    {
        player.GenerateMoves();
    }

    private void ChangeActiveTeam()
    {
        // StartCoroutine(SwitchTurnRoutine());
        ActivePlayer = GetOpponentToPlayer(ActivePlayer);

        if (ActivePlayer is AIPlayer)
        {
            ((AIPlayer)ActivePlayer).ComputerMove();
        }
    }

    public void EndTurn()
    {
        GenerateAllPossiblePlayerMoves(ActivePlayer);
        GenerateAllPossiblePlayerMoves(GetOpponentToPlayer(ActivePlayer));
        if (ActivePlayer.opponent.IsInCheck())
        {
            Piece opponentKing = ActivePlayer.opponent.GetPiecesOfType<King>().FirstOrDefault();
            List<Vector2Int> squares = new List<Vector2Int>();
            squares.Add(opponentKing.occupiedSquare);
            board.ShowSelectionSquares(squares);
        }

        if (CheckIfGameIsFinish())
        {
            EndGame();
        }
        else
        {
            ChangeActiveTeam();
        }
    }

    private void EndGame()
    {
        gameState = GameState.Finish;
        Service.Instance.EndGame();
        UIManager.Instance.OnGameFinished(ActivePlayer.Team.ToString());
    }

    public bool CheckIfGameIsFinish()
    {
        List<Move> moves = ActivePlayer.opponent.GenerateMoves();
        if (moves.Count == 0 && ActivePlayer.opponent.IsInCheck())
        {
            return true;
        }

        return false;
    }

    public void OnPieceRemoved(Piece piece)
    {
        ChessPlayer pieceOwner = GetChessPlayerByTeamColor(piece.team);
        pieceOwner.RemovePiece(piece);
        Destroy(piece.gameObject);
    }

    public void RestartGame()
    {
        DestroyPieces();
        board.OnGameRestarted();
        whitePlayer.OnGameRestarted();
        blackPlayer.OnGameRestarted();
        StartNewGame();
    }

    private void DestroyPieces()
    {
        whitePlayer.ActivePieces.ForEach(p => Destroy(p.gameObject));
        blackPlayer.ActivePieces.ForEach(p => Destroy(p.gameObject));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            whitePlayer.GenerateMoves();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            whitePlayer.ActivePieces.ForEach(p => p.gameObject.SetActive(false));
            board.UpdateBoard();
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            if (ActivePlayer is AIPlayer)
            {
                ((AIPlayer)ActivePlayer).ComputerMove();
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            board.DeselectPiece();
            board.ShowSelectionSquares(ActivePlayer.GetPinsMap());
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            board.DeselectPiece();
            board.ShowSelectionSquares(ActivePlayer.GetAttackMap());
        }
    }

    public void MakeMove(Vector2Int startCoords, Vector2Int targetCoords)
    {
        Piece piece = board.GetPieceAtSquare(startCoords);
        Move move = piece.AvailableMoves.First(m => m.sourceCoords == startCoords && m.targetCoords == targetCoords);
        board.OnSelectedPieceMoved(move, piece);
    }
}