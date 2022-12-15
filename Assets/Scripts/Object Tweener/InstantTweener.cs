using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantTweener : MonoBehaviour, IObjectTweener
{
    public void MoveTo(Transform transform, Vector3 position)
    {
        transform.position = position;
    }
}
