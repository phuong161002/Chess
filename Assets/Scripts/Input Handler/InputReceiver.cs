using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputReceiver : MonoBehaviour
{
    protected IInputHandler[] _inputHandlers;

    private void Awake()
    {
        _inputHandlers = GetComponents<IInputHandler>();
    }

    private void OnEnable()
    {
        EventHandler.OnBeginSwitchTurn += DisableInput;
        EventHandler.OnEndSwitchTurn += EnableInput;
    }

    private void OnDisable()
    {
        EventHandler.OnBeginSwitchTurn -= DisableInput;
        EventHandler.OnEndSwitchTurn -= EnableInput;
    }

    public abstract void OnInputReceived();

    public void DisableInput()
    {
        foreach (IInputHandler handler in _inputHandlers)
        {
            handler.DisableInput();
        }
    }

    public void EnableInput()
    {
        foreach (IInputHandler handler in _inputHandlers)
        {
            handler.EnableInput();
        }
    }
}
