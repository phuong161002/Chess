using System;
using System.Data;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Threading.Tasks;
using UnityEngine;


public class NetworkManager : SingletonMonobehavior<NetworkManager>, INetworkManager
{
    private GameClient _client;
    [SerializeField] private string host = "127.0.0.1";
    [SerializeField] private int port = 3000;

    protected override void Awake()
    {
        base.Awake();
        _client = new GameClient($"ws://{host}:{port}");
    }

    private void Start()
    {
        ConnectToServer();
    }

    private void Update()
    {
        var cqueue = _client.ReceiveQueue;
        string msg;
        while (cqueue.TryPeek(out msg))
        {
            cqueue.TryDequeue(out msg);
            OnMessageReceived(msg);
        }
    }

    private void ConnectToServer()
    {
        Debug.Log("Connecting to server...");
        _client.ConnectToServer();
        Debug.Log("Done!");
    }

    private void OnApplicationQuit()
    {
        _client.Close();
    }

    public void OnMessageReceived(string msg)
    {
        var msgModel = NetworkHelper.ParseObject<Message<object>>(msg);
        WsTags wsTag = msgModel.Tags;
        string data = msgModel.Data.ToString();
        switch (wsTag)
        {
            case WsTags.Invalid:
                UIManager.Instance.ShowDialogue(data, DialogueType.Info, 2f);
                break;
            case WsTags.Login:
                break;
            case WsTags.Register:
                break;
            case WsTags.UserInfo:
                var userInfo = NetworkHelper.ParseObject<UserInfo>(data);
                GameManager.Instance.User =
                    new User(userInfo.Username, userInfo.DisplayName, userInfo.Avatar, userInfo.Level, userInfo.Amount);
                UIManager.Instance.SwitchTo(CanvasTags.Lobby);
                break;
            case WsTags.RoomInfo:
                var roomInfo = NetworkHelper.ParseObject<RoomInfo>(data);
                if (roomInfo.RoomType == RoomType.PlayRoom)
                {
                    GameManager.Instance.UpdateRoom(roomInfo);
                }
                break;
            case WsTags.StartGame:
                var startGameData = NetworkHelper.ParseObject<StartGameData>(data);
                GameManager.Instance.StartGame(PlayMode.PvP, startGameData.MyTeamColor);
                break;
            case WsTags.MovePiece:
                var moveData = NetworkHelper.ParseObject<MoveData>(data);
                Vector2Int startCoords = new Vector2Int(moveData.FromX, moveData.FromY);
                Vector2Int targetCoords = new Vector2Int(moveData.ToX, moveData.ToY);
                ChessGameController.Instance.MakeMove(startCoords, targetCoords);
                break;
            case WsTags.RoomList:
                var roomList = NetworkHelper.ParseObject<RoomList>(data);
                UIManager.Instance.ShowRoomInfo(roomList);
                break;
            case WsTags.ExitRoom:
                GameManager.Instance.JoinLobby();
                break;
            default: break;
        }
    }

    public void SendMsg<T>(Message<T> message)
    {
        _client.SendMessage(message);
    }
}