using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIInputReceiver : InputReceiver
{
    [SerializeField] private UnityEvent clickEvent;

    public override void OnInputReceived()
    {
        foreach (IInputHandler inputHandler in _inputHandlers)
        {
            inputHandler.ProcessInput(Input.mousePosition, gameObject, () => clickEvent.Invoke());
        }
    }
}
