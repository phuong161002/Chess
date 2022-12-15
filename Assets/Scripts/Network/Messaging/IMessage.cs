using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMessage<T>
{
    WsTags Tags { get; set; }
    T Data { get; set; }
}
