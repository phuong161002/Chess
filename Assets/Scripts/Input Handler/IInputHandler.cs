using UnityEngine;
using System;

public interface IInputHandler
{
    void ProcessInput(Vector3 inputPosition, GameObject selectedObject, Action callback);

    void DisableInput();
    
    void EnableInput();
}
