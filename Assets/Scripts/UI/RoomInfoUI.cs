using TMPro;
using UnityEngine;

public class RoomInfoUI : MonoBehaviour
{
    public string Id
    {
        get => _roomInfo.Id;
    }

    public int PlayerQuantity
    {
        get => _roomInfo.Players.Count;
    }
    
    [SerializeField] private TextMeshProUGUI RoomIdTMP;
    [SerializeField] private TextMeshProUGUI PlayerInfoTMP;
    [SerializeField] private UIButton JoinBtn;
    private RoomInfo _roomInfo;

    public void Setup(RoomInfo roomInfo)
    {
        _roomInfo = roomInfo;
        RoomIdTMP.text = roomInfo.Id;
        PlayerInfoTMP.text = roomInfo.Players.Count + "/" + 2;
        JoinBtn.enabled = roomInfo.Players.Count < 2;
    }
    
    private void OnEnable()
    {
        LobbyUIManager.roomInfoList.Add(this);
    }

    private void OnDisable()
    {
        LobbyUIManager.roomInfoList.Remove(this);
    }

    public void DoJoinRoom()
    {
        Service.Instance.JoinRoom(Id);
    }
}