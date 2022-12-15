using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Board))]
public class SquareLabeler : MonoBehaviour
{
    private Board board;
    private TextMeshPro[,] labels;
    [SerializeField] private GameObject labelPrefab;

    private void Awake()
    {
        board = GetComponent<Board>();
    }

    private void Start()
    {
        Setup();
    }

    private void Setup()
    {
        labels = new TextMeshPro[Board.BOARD_SIZE, Board.BOARD_SIZE];
        for (int i = 0; i < Board.BOARD_SIZE; i++)
        {
            for (int j = 0; j < Board.BOARD_SIZE; j++)
            {
                labels[i, j] =
                    Instantiate(labelPrefab, board.CalculatePositionFromCoords(new Vector2Int(i, j)),
                        Quaternion.identity).GetComponentInChildren<TextMeshPro>();
            }
        }
    }

    private void Update()
    {
        for (int i = 0; i < Board.BOARD_SIZE; i++)
        {
            for (int j = 0; j < Board.BOARD_SIZE; j++)
            {
                Piece piece = board.GetPieceAtSquare(new Vector2Int(i, j));
                string textLabel;
                if (piece == null)
                {
                    textLabel = "None";
                }
                else
                {
                    textLabel = piece.name;
                }
                labels[i,j].SetText(textLabel);
            }
        }
    }
}