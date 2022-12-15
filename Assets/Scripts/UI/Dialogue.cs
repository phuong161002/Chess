using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Dialogue : MonoBehaviour
{
    [SerializeField] private Color InfoBg = Color.blue;
    [SerializeField] private Color ErrorBg = Color.red;
    [SerializeField] private Color WarningBg = Color.yellow;

    [SerializeField] private Color InfoTextColor = Color.white;
    [SerializeField] private Color ErrorTextColor = Color.black;
    [SerializeField] private Color WarningTextColor = Color.red;

    [SerializeField] private float minWidth;

    private Image _background;
    [SerializeField] private TextMeshProUGUI _text;
    private DialogueType _dialogueType;
    private string _message;

    private void Awake()
    {
        _background = GetComponent<Image>();
    }


    public void Setup(string message, DialogueType type)
    {
        _message = message;
        _dialogueType = type;
        _text.SetText(message);
        _background.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Max(minWidth, _text.preferredWidth + 50));

        switch (type)
        {
            case DialogueType.Info:
                _background.color = InfoBg;
                _text.color = InfoTextColor;
                break;
            case DialogueType.Error:
                _background.color = ErrorBg;
                _text.color = ErrorTextColor;
                break;
            case DialogueType.Warning:
                _background.color = WarningBg;
                _text.color = WarningTextColor;
                break;
        }
    }

    public void ShowDialogue()
    {
        gameObject.SetActive(true);
    }

    public void HideDialogue()
    {
        gameObject.SetActive(false);
    }
}