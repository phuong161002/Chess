using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RegisterUIManager : MonoBehaviour
{
    [SerializeField] private UIButton registerButton;
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private TMP_InputField displayNameInputField;

    public void DoRegister()
    {
        string user = usernameInputField.text;
        string pass = passwordInputField.text;
        string displayName = displayNameInputField.text;
        Service.Instance.DoRegister(user, pass, displayName);
    }
}