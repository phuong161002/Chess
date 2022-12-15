using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualBoard
{
    public int[,] Grid;

    public static VirtualBoard CreateVirtualBoard(Board board)
    {
        VirtualBoard virtualBoard = new VirtualBoard();
        virtualBoard.Grid = new int[Board.BOARD_SIZE, Board.BOARD_SIZE];
        for(int x = 0; x < Board.BOARD_SIZE; x++)
        {
            for (int y = 0; y < Board.BOARD_SIZE; y++)
            {
                Piece piece = board.GetPieceAtSquare(new Vector2Int(x, y));
                if (piece != null)
                {
                    virtualBoard.Grid[x, y] = ((int)piece.team << 3) | (int)piece.pieceType;
                }
                else
                {
                    virtualBoard.Grid[x, y] = (int)PieceType.None;
                }
            }
        }

        return virtualBoard;
    }

    public void Show()
    {
        string s = "";

        for (int y = 0; y < Board.BOARD_SIZE; y++)
        {
            for (int x = 0; x < Board.BOARD_SIZE; x++)
            {
                s += Grid[x, y] + " ";
            }

            s += Environment.NewLine;
        }

        Debug.Log(s);
    }
}
