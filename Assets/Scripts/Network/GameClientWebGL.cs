using UnityEngine;
using System;
using System.Collections.Concurrent;
using System.Text;
using HybridWebSocket;


public class GameClientWebGL : IGameClient
{
    private WebSocket _client;

    public ConcurrentQueue<string> ReceiveQueue { get; }
    public BlockingCollection<ArraySegment<byte>> SendQueue { get; }

    public GameClientWebGL(string url)
    {
        _client = WebSocketFactory.CreateInstance(url);
        ReceiveQueue = new ConcurrentQueue<string>();
        SendQueue = new BlockingCollection<ArraySegment<byte>>();
        _client.OnOpen += () => { Debug.Log("WebSocket Connected"); };
        _client.OnMessage += (data =>
        {
            string message = Encoding.UTF8.GetString(data);
            Debug.Log($"Receive Msg: {message}");
            ReceiveQueue.Enqueue(message);
        });
        
        _client.OnError += msg =>  {
            Debug.LogError($"WebSocket Error: {msg}");
        };

        _client.OnClose += code =>
        {
            Debug.LogError($"WebSocket Close: code={code}");
        };
    }
    
    
    public void ConnectToServer()
    {
        _client.Connect();
    }

    public void Close()
    {
        _client.Close();
    }

    public void SendMessage(string message)
    {
        _client.Send(Encoding.UTF8.GetBytes(message));
    }

    public void SendMessage<T>(T message)
    {
        var msg = NetworkHelper.ParseString(message);
        SendMessage(msg);
    }
}