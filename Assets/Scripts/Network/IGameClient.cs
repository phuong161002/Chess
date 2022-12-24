
using System;
using System.Collections.Concurrent;

public interface IGameClient
{
    ConcurrentQueue<String> ReceiveQueue { get; }    
    BlockingCollection<ArraySegment<byte>> SendQueue { get; }

    void ConnectToServer();
    void Close();
    void SendMessage(string message);
    void SendMessage<T>(T message);
}