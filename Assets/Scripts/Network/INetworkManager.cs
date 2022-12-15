using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INetworkManager
{
    void OnMessageReceived(string msg);
    void SendMsg<T>(Message<T> message);
}
