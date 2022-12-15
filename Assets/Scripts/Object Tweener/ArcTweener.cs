using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ArcTweener : MonoBehaviour, IObjectTweener
{
    [SerializeField] private float speed = 1;
    [SerializeField] private float height = 2;
    
    public void MoveTo(Transform transform, Vector3 position)
    {
        float distance = Vector3.Distance(transform.position, position);
        transform.DOJump(position, height, 1, distance / speed);
    }
}
