using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class LobbyUIManager : MonoBehaviour
{
    [SerializeField] private UIButton PVPButton;
    [SerializeField] private UIButton PVEButton;
    [SerializeField] private TextMeshProUGUI displayNameText;
    [SerializeField] private GameObject roomInfoUIPerfab;
    [SerializeField] private Transform roomListTransform;

    public static List<RoomInfoUI> roomInfoList = new List<RoomInfoUI>();

    private void OnEnable()
    {
        var user = GameManager.Instance.User;
        if (user != null)
        {
            SetDisplayName(user.DisplayName);
        }
        else
        {
            SetDisplayName("No Name");
        }
        
        Service.Instance.GetRoomList();
    }

    private void SetDisplayName(string displayName)
    {
        displayNameText.text = displayName;
    }
    
    public void CreateRoom()
    {
        Service.Instance.RequestCreateRoom();
    }
    
    public void PlayPvP()
    {
        UIManager.Instance.ShowDialogue("Searching player...", DialogueType.Info);
    }

    public void PlayPvE()
    {
        UIManager.Instance.HideUI();
        GameManager.Instance.StartGame(PlayMode.PvE, TeamColor.WHITE);
    }

    public void ShowRoomInfo(RoomList roomList)
    {
        List<RoomInfoUI> UIToDestroy = new List<RoomInfoUI>();
        foreach (var roomInfoUI in roomInfoList)
        {
            var roomInfos = roomList.Rooms.FindAll(r => r.Id == roomInfoUI.Id);
            if (roomInfos.Count <= 0)
            {
                UIToDestroy.Add(roomInfoUI);
            }
        }

        foreach (var roomInfoUI in UIToDestroy)
        {
            Destroy(roomInfoUI.gameObject);
        }
        
        foreach (var room in roomList.Rooms)
        {
            var roomInfoUI = roomInfoList.FirstOrDefault(r => r.Id == room.Id);
            if (roomInfoUI != default)
            {
                roomInfoUI.Setup(room);
            }
            else
            {
                var gO = Instantiate(roomInfoUIPerfab, roomListTransform);
                roomInfoUI = gO.GetComponent<RoomInfoUI>();
                roomInfoUI.Setup(room);
            }
        }
    }
}
