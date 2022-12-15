using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class AIPlayer : ChessPlayer
{
    private Search _search;

    public event Action OnAITurn;

    public AIPlayer(TeamColor team, Board board)
    {
        Team = team;
        Board = board;
        ActivePieces = new List<Piece>();
        moveGenerator = new MoveGenerator(this, board);
        
        _search = new Search(Board, this, moveGenerator);
        _search.onSearchComplete += OnSearchComplete;
        _search.searchDiagnostics = new Search.SearchDiagnostics();

        OnAITurn += _search.StartSearch;
    }

    public void ComputerMove()
    {
        OnAITurn?.Invoke();
    }

    private void OnSearchComplete(Move move)
    {
        if (move == Move.InvalidMove)
        {
            ChooseRandomMove();
            return;
        }
        Board.OnSelectedPieceMoved(move, move.pieceAtSource);
    }

    private void ChooseRandomMove()
    {
        var moves = GenerateMoves();
        Move move = moves[Random.Range(0, moves.Count)];
        Board.OnSelectedPieceMoved(move, move.pieceAtSource);
    }
}