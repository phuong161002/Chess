using UnityEngine;
using DG.Tweening;

public class LineTweener : MonoBehaviour, IObjectTweener
{
    [SerializeField] private float speed = 1;
    
    public void MoveTo(Transform transform, Vector3 position)
    {
        float distance = Vector3.Distance(transform.position, position);
        transform.DOMove(position, distance / speed);
    }
}
