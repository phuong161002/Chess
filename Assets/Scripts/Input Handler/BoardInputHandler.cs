using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Board))]
public class BoardInputHandler : MonoBehaviour, IInputHandler
{
    private Board board;
    private bool enabledInput = true;

    private void Awake()
    {
        board = GetComponent<Board>();
    }

    public void ProcessInput(Vector3 inputPosition, GameObject selectedObject, Action callback)
    {
        if (enabledInput)
        {
            board.OnSquareSelected(inputPosition);
        }
    }

    public void DisableInput()
    {
        enabledInput = false;
    }

    public void EnableInput()
    {
        enabledInput = true;
    }
}