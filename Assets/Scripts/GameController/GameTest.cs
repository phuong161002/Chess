using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameTest : MonoBehaviour
{
    [SerializeField] private Board board;
    private void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(GenerateMoves());
        }
    }

    IEnumerator GenerateMoves()
    {
        Debug.Log("Generate Move");
        var player = ChessGameController.Instance.GetChessPlayerByTeamColor(TeamColor.WHITE);
        var moves = player.GenerateMoves();
        foreach (Move move in moves)
        {
            board.MakeMove(move);
            var black = player.opponent;
            var moves2 = black.GenerateMoves();
            foreach (var move2 in moves2)
            {
                board.MakeMove(move2);
                yield return new WaitForSeconds(0.1f);
                board.UnmakeMove(move2);
            }
            board.UnmakeMove(move);
        }
    }
}