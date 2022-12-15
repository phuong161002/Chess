using TMPro;
using UnityEngine;

public class LoginUIManager : MonoBehaviour
{
    [SerializeField] private UIButton loginButton;
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private TMP_InputField passwordInputField;

    public void DoLogin()
    {
        string user = usernameInputField.text;
        string pass = passwordInputField.text;
        Service.Instance.DoLogin(user, pass);
    }
}
