using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverUIManager : MonoBehaviour
{
    [SerializeField] private UIButton restartButton;
    [SerializeField] private TextMeshProUGUI resultText;

    public void OnGameFinished(string winner)
    {
        resultText.text = $"{winner} won";
        gameObject.SetActive(true);
    }

    public void RestartGame()
    {
        if (ChessGameController.Instance == null)
        {
            return;
        }

        if (GameManager.Instance.PlayMode == PlayMode.PvE)
        {
            ChessGameController.Instance.RestartGame();
            return;
        }
        
        Service.Instance.RequestRestartGame();
        UIManager.Instance.SwitchTo(CanvasTags.PlayRoom);
    }
}