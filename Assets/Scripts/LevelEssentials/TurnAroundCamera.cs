using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class TurnAroundCamera : MonoBehaviour
{
    //---------Public and Private Visible In Inspector---------\\
    [SerializeField] private float accelerometerThreshold = 0.3f;
    [SerializeField] private float rotationSpeed = 1.5f;
    
    public GameObject targetToLookAt;
    //---------Public and Private Visible In Inspector---------\\
    
    //--------Private and Public Invisible In Inspector--------\\
    private Vector3 _offsetFromTarget = Vector3.zero;

    private Player _player;
    
    [NonSerialized] public Camera cam;

    [NonSerialized] public Dictionary<Vector3, int> snappingPoints;
    [NonSerialized] public int selDeg = 0;
    [NonSerialized] public string[] degOptions = new string[8] {"0", "45", "90", "135", "180", "225", "270", "315"};
    [NonSerialized] public int degToSnap = 0;
    [NonSerialized] public bool circleCalc = false;
    //--------Private and Public Invisible In Inspector--------\\

    private void Awake()
    {
        cam = GetComponent<Camera>();
        _player = FindObjectOfType<Player>();
    }
    
    private void Start()
    {
        CreateTargetToLookAt();
        
        _offsetFromTarget = targetToLookAt.transform.position - transform.position;
    }

    private void LateUpdate()
    {
        TurnCamera();
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

    private void TurnCamera()
    {
        if (!_player.isMoving && !GameManager.current.gamePaused)
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
    }

    private float GetHorizontalRotation()
    {
        // Based on device type get different input
        switch (GameManager.current.deviceType)
        {
            case DeviceType.Desktop: return Input.GetAxis("Horizontal");
            case DeviceType.Handheld:
                float acceleration = Input.acceleration.x;
                return Mathf.Abs(acceleration) < accelerometerThreshold ? 0 : Mathf.Sign(acceleration) * rotationSpeed;
            default: return 0;
        }
    }
}