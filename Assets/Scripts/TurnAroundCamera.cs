using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnAroundCamera : MonoBehaviour
{
    private DeviceType _deviceType;
    private Gyroscope _gyro;

    [SerializeField] private GameObject targetToLookAt;

    private Vector3 _offsetFromTarget = Vector3.zero;

    void Start()
    {
        _deviceType = SystemInfo.deviceType;
        
        _gyro = Input.gyro;

        if (_deviceType == DeviceType.Handheld) _gyro.enabled = true;

        if (!targetToLookAt)
        {
            targetToLookAt = new GameObject("Target To Look At");
            targetToLookAt.transform.position = Vector3.zero;
        }

        _offsetFromTarget = targetToLookAt.transform.position - transform.position;
    }

    private void LateUpdate()
    {
        float horizontal = GetHorizontalRotation();

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
            case DeviceType.Handheld: return _gyro.rotationRate.y;
            default: return 0;
        }
    }
}