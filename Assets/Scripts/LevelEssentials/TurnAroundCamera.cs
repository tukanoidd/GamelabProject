using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

[RequireComponent(typeof(Camera))]
public class TurnAroundCamera : MonoBehaviour
{
    //---------Public and Private Visible In Inspector---------\\
    [SerializeField] private float accelerometerThreshold = 0.3f;
    [SerializeField] private float rotationSpeed = 1.5f;
    [SerializeField] private float waitTillSnapping = 1f;
    [SerializeField] private float snappingThreshold = 0.5f;
    [SerializeField] private float snappingSpeed = 1;
    [SerializeField] private float snapMaxDistanceDelta = 0.05f;

    public GameObject targetToLookAt;
    //---------Public and Private Visible In Inspector---------\\

    //--------Private and Public Invisible In Inspector--------\\
    private Vector3 _offsetFromTarget = Vector3.zero;

    private bool _snapping = false;
    private Vector3? _snapTarget;

    public Camera cam;

    public Dictionary<Vector3, int> snappingPoints = new Dictionary<Vector3, int>();
    public int selDeg = 0;
    public string[] degOptions = new string[8] {"0", "45", "90", "135", "180", "225", "270", "315"};
    public int degToSnap = 0;

    public List<Vector3> customPositions = new List<Vector3>();
    //--------Private and Public Invisible In Inspector--------\\

    private void Awake()
    {
        cam = GetComponent<Camera>();
        CreateTargetToLookAt();
    }

    private void Start()
    {
        _offsetFromTarget = targetToLookAt.transform.position - transform.position;
    }

    private void LateUpdate()
    {
        TurnCamera();
        
        // Look at the object
        transform.LookAt(targetToLookAt.transform);
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
        if (!GameManager.current.player.isMoving
            && !GameManager.current.cameraLockedMovement)
        {
            float horizontal = -GetHorizontalRotation();
            if (Math.Abs(horizontal) > 0.05f)
            {
                
                StopAllCoroutines();
                _snapping = false;

                RotateCamera(horizontal);
            }
            else
            {
                StartCoroutine(CheckForSnapping());
            }
        }
    }

    private void RotateCamera(float speed)
    {
        // Rotate with value that got from in[ut
        targetToLookAt.transform.Rotate(0, speed, 0);

        // Find desired position of the camera based on objects position and rotation
        float desiredAngle = targetToLookAt
            .transform.eulerAngles.y;
        Quaternion rotation = Quaternion.Euler(0, desiredAngle, 0);
        transform.position = targetToLookAt.transform.position - (rotation * _offsetFromTarget);
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

    private IEnumerator CheckForSnapping()
    {
        if (_snapping) yield break;

        yield return new WaitForSeconds(waitTillSnapping);

        Vector3 pos = transform.position;
        
        Vector3[] checkTargets = customPositions.Where(customPosition =>
            Vector3.Distance(customPosition, pos) <= snappingThreshold && Vector3.Distance(customPosition, pos) > 0.01f).ToArray();

        if (checkTargets.Length > 0)
        {
            _snapTarget = checkTargets[0];
            
            StartCoroutine(MoveToSnapPoint());
        }
    }

    private IEnumerator MoveToSnapPoint()
    {
        if (!_snapTarget.HasValue) yield break;
            
        _snapping = true;

        Vector3 offset = transform.position - _snapTarget.Value;
        float rotSpeed = snappingSpeed * Math.Sign(offset.z) * Math.Sign(_snapTarget.Value.x);
        
        while (Vector3.Distance(transform.position, _snapTarget.Value) > snapMaxDistanceDelta)
        {
            RotateCamera(rotSpeed);
            
            yield return new WaitForFixedUpdate();
        }

        transform.position = _snapTarget.Value;

        _snapping = false;
        _snapTarget = null;
    }
}