using System;
using System.Collections;
using System.Collections.Generic;
using DataTypes;
using UnityEngine;
using Plane = DataTypes.Plane;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    //---------Public and Private Visible In Inspector---------\\
    [SerializeField] private bool drawDebugLineToTarget = true;
    [SerializeField] private bool drawDebugLineForward = true;
    [SerializeField] private float debugLineForwardLength = 3f;
    
    [SerializeField] private bool drawPathWhenMoving;
    [SerializeField] private float movementSpeed = 10;

    [SerializeField] private float customCameraPositionMaxOffset = 0.5f;
    
    public GravitationalPlane gravitationalPlane = new GravitationalPlane(Plane.XZ, PlaneSide.PlaneNormalPositive);
    //---------Public and Private Visible In Inspector---------\\
    
    //--------Private and Public Invisible In Inspector--------\\
    private PathFinder _pathFinder;
    private CharacterController _characterController;
    private TurnAroundCamera _mainCamera;
    private float _height = 0;

    private Vector3? _targetPosition = null;
    private Block _targetBlock = null;
    private PathFindingLocation _current = null;

    private Material _testBlockMat;

    [NonSerialized] public bool isMoving = false;
    [NonSerialized] public MovementAxisConstraints movementAxisConstraints = null;
    
    [NonSerialized] public bool isTeleporting = false;
    [NonSerialized] public bool canTeleport = true;

    [NonSerialized] public MovementDirection? movementDirection = null;
    //--------Private and Public Invisible In Inspector--------\\

    private void Awake()
    {
        InitPrivateVars();
    }

    private void Start()
    {
        if (_characterController.isGrounded) MoveToGround();
    }

    private void Update()
    {
        if (GameManager.current.gamePaused) return;
        
        if (isMoving) {} //todo smooth movement logic

        LockRotation();
    }

    private void InitPrivateVars()
    {
        _pathFinder = FindObjectOfType<PathFinder>();
        _characterController = GetComponent<CharacterController>();
        _mainCamera = FindObjectOfType<TurnAroundCamera>();
        
        _height = GetComponent<MeshRenderer>().bounds.size.y;
        
        _testBlockMat = Resources.Load<Material>("Materials/PathFindingBlockTest");
    }

    private void MoveToGround()
    {
        //todo logic to move to ground
    }

    private void LockRotation()
    {
        Vector3 newRot = transform.rotation.eulerAngles;
        newRot.x = 0;
        newRot.z = 0;

        transform.rotation = Quaternion.Euler(newRot);
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (drawDebugLineToTarget && _targetPosition.HasValue)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, _targetPosition.Value);
        }

        if (drawDebugLineForward)
        {
            Gizmos.color = Color.blue;
            Vector3 lineStart = transform.position + Vector3.up * _height / 2;
            Gizmos.DrawLine(lineStart, lineStart + transform.forward * debugLineForwardLength);   
        }
    }
    #endif
}
