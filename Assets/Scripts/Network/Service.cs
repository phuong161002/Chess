using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Service : SingletonMonobehavior<Service>
{
    public void DoLogin(string username, string password)
    {
        Debug.Log($"Do Login : user={username}  pass={password}");
        Message<LoginData> loginData = new Message<LoginData>(WsTags.Login, new LoginData()
        {
            Username = username,
            Password = password
        });
        NetworkManager.Instance.SendMsg(loginData);
    }

    public void DoRegister(string username, string password, string displayName)
    {
        Debug.Log($"Do Register : user={username}  pass={password}  displayName={displayName}");
        Message<RegisterData> registerData = new Message<RegisterData>(WsTags.Register, new RegisterData()
        {
            Username = username,
            Password = password,
            DisplayName = displayName
        });
        NetworkManager.Instance.SendMsg(registerData);
    }

    public void MovePiece(Vector2Int startCoords, Vector2Int targetCoords)
    {
        Debug.Log($"Move piece {startCoords} => {targetCoords}");
        Message<MoveData> message = new Message<MoveData>(WsTags.MovePiece, new MoveData()
        {
            FromX = startCoords.x,
            FromY = startCoords.y,
            ToX = targetCoords.x,
            ToY = targetCoords.y
        });
        NetworkManager.Instance.SendMsg(message);
    }

    public void RequestCreateRoom()
    {
        Debug.Log($"Request create room");
        Message<object> message = new Message<object>(WsTags.CreateRoom, null);
        NetworkManager.Instance.SendMsg(message);
    }

    public void GetRoomList()
    {
        Debug.Log("Get Room List");
        Message<object> message = new Message<object>(WsTags.RoomList, null);
        NetworkManager.Instance.SendMsg(message);
    }

    public void JoinRoom(string roomId)
    {
        Debug.Log($"Join Room {roomId}");
        Message<string> message = new Message<string>(WsTags.JoinRoom, roomId);
        NetworkManager.Instance.SendMsg(message);
    }

    public void RequestStartGame()
    {
        Debug.Log($"Start Game");
        Message<object> message = new Message<object>(WsTags.StartGame, null);
        NetworkManager.Instance.SendMsg(message);
    }

    public void RequestRestartGame()
    {
        Debug.Log("Restart Game");
        Message<object> message = new Message<object>(WsTags.RestartGame, null);
        NetworkManager.Instance.SendMsg(message);
    }

    public void ExitRoom()
    {
        Debug.Log("Exit room");
        Message<object> message = new Message<object>(WsTags.ExitRoom, null);
        NetworkManager.Instance.SendMsg(message);
    }

    public void EndGame()
    {
        Debug.Log("End Game");
        Message<object> message = new Message<object>(WsTags.EndGame, null);
        NetworkManager.Instance.SendMsg(message);
    }
}
