using System;
using System.Collections.Concurrent;
using UnityEngine;
using WebSocketSharp;

/// <summary>
/// WebSocket Client class.
/// Responsible for handling communication between server and client.
/// </summary>
public class GameClient
{
    // // WebSocket
    // private ClientWebSocket ws = new ClientWebSocket();
    // private UTF8Encoding encoder; // For websocket text message encoding.
    // private const UInt64 MAXREADSIZE = 1 * 1024 * 1024;
    // // Server address
    // private Uri serverUri;
    // Queues
    public ConcurrentQueue<String> ReceiveQueue { get; }
    public BlockingCollection<ArraySegment<byte>> SendQueue { get; }

    private bool _stop;
    private WebSocket _client;

    public GameClient(string url)
    {
        _client = new WebSocket(url);
        ReceiveQueue = new ConcurrentQueue<string>();
        SendQueue = new BlockingCollection<ArraySegment<byte>>();
        
        _client.OnMessage += (sender, e) =>
        {
            var body = !e.IsPing ? e.Data : "A ping was received.";
            Debug.Log($"WebSocket Message: {body}");
            if (!e.IsPing)
            {
                ReceiveQueue.Enqueue(e.Data);
            }
        };
        
        _client.OnError += (sender, e) => {
            Debug.LogError($"WebSocket Error: {e.Message}");
        };

        _client.OnClose += (sender, e) =>
        {
             Debug.LogError($"WebSocket Close: {e.Code}  {e.Reason}");
        };
    }

    public void ConnectToServer()
    {
        _client.Connect();
    }

    public void Close()
    {
        _client.CloseAsync(CloseStatusCode.Normal);
    }

    public void SendMessage(string message)
    {
        _client.Send(message);
    }

    public void SendMessage<T>(T message)
    {
        var msg = NetworkHelper.ParseString(message);
        Debug.Log($"Send Message: {msg}");
        _client.Send(msg);
    }
}