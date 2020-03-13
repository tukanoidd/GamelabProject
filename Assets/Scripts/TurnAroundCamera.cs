using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnAroundCamera : MonoBehaviour
{
    private DeviceType _deviceType;
    
    [SerializeField] private float accelerometerThreshold = 0.3f;
    [SerializeField] private float rotationSpeed = 1.5f;
    [SerializeField] private GameObject targetToLookAt;

    private Vector3 _offsetFromTarget = Vector3.zero;

    void Start()
    {
        _deviceType = SystemInfo.deviceType;

        CreateTargetToLookAt();

        _offsetFromTarget = targetToLookAt.transform.position - transform.position;
    }

    public void CreateTargetToLookAt()
    {
        GameObject target = GameObject.FindGameObjectWithTag("TargetToLookAt");
        if (!target)
        {
            targetToLookAt = new GameObject("Target To Look At");
            targetToLookAt.tag = "TargetToLookAt";
            targetToLookAt.transform.position = Vector3.zero;
        }
        else targetToLookAt = target;
    }

    private void LateUpdate()
    {
        float horizontal = -GetHorizontalRotation();

        // Rotate with value that got from in[ut
        targetToLookAt.transform.Rotate(0, horizontal, 0);
        
        // Find desired position of the camera based on objects position and rotation
        float desiredAngle = targetToLookAt
            .transform.eulerAngles.y;
        Quaternion rotation = Quaternion.Euler(0, desiredAngle, 0);
        transform.position = targetToLookAt.transform.position - (rotation * _offsetFromTarget);

        // Look at the object
        transform.LookAt(targetToLookAt.transform);
    }

    private float GetHorizontalRotation()
    {
        // Based on device type get different input
        switch (_deviceType)
        {
            case DeviceType.Desktop: return Input.GetAxis("Horizontal");
            case DeviceType.Handheld:
                float acceleration = Input.acceleration.x;
                return  Mathf.Abs(acceleration) < accelerometerThreshold ? 0 : Mathf.Sign(acceleration) * rotationSpeed;
            default: return 0;
        }
    }
}