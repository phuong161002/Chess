using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayRoomUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI myDisplayNameTMP;
    [SerializeField] private TextMeshProUGUI opponentDisplayNameTMP;

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
        Service.Instance.ExitRoom();
    }
}