using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Message<T> : IMessage<T>
{
    public WsTags Tags { get; set; }
    public T Data { get; set; }

    public Message(WsTags tags, T data)
    {
        Tags = tags;
        Data = data;
    }
}
