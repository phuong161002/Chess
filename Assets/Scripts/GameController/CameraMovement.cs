using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Camera cameraToMove;
    [SerializeField] private Transform whitePlayerSight;
    [SerializeField] private Transform blackPlayerSight;

    private Transform currentSight;
    private float radius;
    private float angle;
    private Vector3 origin;
    [SerializeField] private float angularSpeed;

    public bool IsMovingCamera { get; private set; }

    private void Start()
    {
        cameraToMove = Camera.main;
        var sight = GameManager.Instance.MyTeamColor == TeamColor.WHITE ? whitePlayerSight : blackPlayerSight;
        cameraToMove.transform.SetPositionAndRotation(sight.position, sight.rotation);
        currentSight = sight;
        radius = Vector3.Distance(whitePlayerSight.position, blackPlayerSight.position) / 2;
        origin = (whitePlayerSight.position + blackPlayerSight.position) / 2;
    }

    public void StartMove()
    {
        IsMovingCamera = true;
        if (currentSight == whitePlayerSight)
        {
            currentSight = blackPlayerSight;
        }
        else
        {
            currentSight = whitePlayerSight;
        }
    }

    private void Update()
    {
        if (IsMovingCamera)
        {
            MoveCamera();
        }
    }

    private void MoveCamera()
    {
        Vector3 current = cameraToMove.transform.position - origin;
        Vector3 target = currentSight.transform.position - origin;
        // Vector3 newPosition = Vector3.MoveTowards(cameraToMove.transform.position, currentSight.transform.position,
        //     angularSpeed * Time.deltaTime * radius);
        Vector3 newPosition = Vector3.RotateTowards(current, target, angularSpeed * Time.deltaTime, 0) + origin;
        Vector3 newEulerAngles = Vector3.MoveTowards(cameraToMove.transform.rotation.eulerAngles,
            currentSight.transform.rotation.eulerAngles,
            angularSpeed * Time.deltaTime * 180 / Mathf.PI);
        if (Vector3.Distance(currentSight.transform.position, cameraToMove.transform.position) < 0.1f)
        {
            IsMovingCamera = false;
            cameraToMove.transform.position = currentSight.transform.position;
            return;
        }

        cameraToMove.transform.SetPositionAndRotation(newPosition, Quaternion.Euler(newEulerAngles));
    }
}