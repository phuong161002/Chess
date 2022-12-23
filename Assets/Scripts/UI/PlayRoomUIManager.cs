using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayRoomUIManager : MonoBehaviour
{
    public bool isGameStarted;
    [SerializeField] private TextMeshProUGUI myDisplayNameTMP;
    [SerializeField] private TextMeshProUGUI opponentDisplayNameTMP;
    [SerializeField] private UIButton btnExitRoomOnPlaying;
    [SerializeField] private UIButton btnExitRoom;
    [SerializeField] private UIButton btnStartGame;

    public string MyDisplayName
    {
        get => myDisplayNameTMP.text;
        set => myDisplayNameTMP.text = value;
    }

    public string OpponentDisplayName
    {
        get => opponentDisplayNameTMP.text;
        set => opponentDisplayNameTMP.text = value;
    }

    private void OnEnable()
    {
        if (isGameStarted)
        {
            btnStartGame.gameObject.SetActive(false);
        }
        else
        {
            btnStartGame.gameObject.SetActive(true);
        }

        if (GameManager.Instance.PlayMode == PlayMode.PvE)
        {
            myDisplayNameTMP.gameObject.SetActive(false);
            opponentDisplayNameTMP.gameObject.SetActive(false);
            return;
        }

        myDisplayNameTMP.gameObject.SetActive(true);
        opponentDisplayNameTMP.gameObject.SetActive(true);
        Debug.Log("Setup name");
        var myRoom = GameManager.Instance.MyRoom;
        if (myRoom == null)
        {
            MyDisplayName = "NoName";
            OpponentDisplayName = "NoName";
            return;
        }

        var myUserInfo = myRoom.GetMyUserInfo();
        var opponentUserInfo = myRoom.GetOpponentUserInfo();
        MyDisplayName = myUserInfo.DisplayName;
        OpponentDisplayName = opponentUserInfo.DisplayName;
    }

    public void StartGame()
    {
        Service.Instance.RequestStartGame();
    }

    public void ExitRoom()
    {
        if (GameManager.Instance.PlayMode == PlayMode.PvE)
        {
            GameManager.Instance.JoinLobby();
            UIManager.Instance.OnPlayerExitPlayRoom();
            return;
        }
        Service.Instance.ExitRoom();
    }
}