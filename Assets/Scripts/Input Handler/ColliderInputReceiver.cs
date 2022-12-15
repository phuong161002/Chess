using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderInputReceiver : InputReceiver
{
    private Vector3 clickPosition;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                clickPosition = hit.point;
                Debug.DrawLine(Camera.main.transform.position, clickPosition, Color.white, 1f);
                OnInputReceived();
            }
        }
    }

    public override void OnInputReceived()
    {
        foreach (var inputHandler in _inputHandlers)
        {
            inputHandler.ProcessInput(clickPosition, null, null);
        }
    }
}
