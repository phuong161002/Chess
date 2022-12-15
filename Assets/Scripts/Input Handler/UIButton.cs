using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
    
[RequireComponent(typeof(UIInputReceiver))]
public class UIButton : Button
{
    private InputReceiver _inputReceiver;

    protected override void Awake()
    {
        base.Awake();

        _inputReceiver = GetComponent<UIInputReceiver>();
        onClick.AddListener(() => _inputReceiver.OnInputReceived());
    }
}
