using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataTypes;
using UnityEngine;
using Plane = DataTypes.Plane;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(Rigidbody))]
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

    public Block blockStandingOn;

    public Block BlockStandingOn
    {
        get => blockStandingOn;
        set
        {
            MeshRenderer valueMeshRenderer = value.GetComponent<MeshRenderer>();

            if (blockStandingOn)
            {
                MeshRenderer blockMeshRenderer = blockStandingOn.GetComponent<MeshRenderer>();
                blockMeshRenderer.material = _savedBlockMat;
            }

            _savedBlockMat = valueMeshRenderer.material;
            valueMeshRenderer.material = _testBlockMat;
            blockStandingOn = value;
        }
    }
    //---------Public and Private Visible In Inspector---------\\

    //--------Private and Public Invisible In Inspector--------\\
    private Rigidbody _rigidbody;
    private float _height = 0;

    private Vector3? _targetPosition = null;

    private Material _testBlockMat;
    private Material _savedBlockMat;

    private BlockConnection _currentMovementConnection = null;
    private Block _targetBlock = null;

    [HideInInspector] public bool isMoving = false;
    [HideInInspector] public MovementAxisConstraints movementAxisConstraints = null;

    [HideInInspector] public bool canTeleport = true;
    [HideInInspector] public ConnectionPoint teleportedLastTo = null;

    [HideInInspector] public bool canMove = false;
    //--------Private and Public Invisible In Inspector--------\\

    private void Awake()
    {
        InitPrivateVars();

        UpdateGravity();
    }

    private void Update()
    {
        if (GameManager.current.gamePaused) return;
    }

    private void InitPrivateVars()
    {
        _rigidbody = GetComponent<Rigidbody>();

        _height = GetComponent<MeshRenderer>().bounds.size.y;

        _testBlockMat = Resources.Load<Material>("Materials/PathFindingBlockTest");
    }

    public void UpdateRotation()
    {
        transform.localEulerAngles = gravitationalPlane.ToRotationEuler(transform.localRotation.eulerAngles);
    }

    private void UpdateConstraints()
    {
        RigidbodyConstraints newConstraints = RigidbodyConstraints.None;

        if (gravitationalPlane.plane == Plane.XY)
        {
            newConstraints |= RigidbodyConstraints.FreezeRotationX;
            newConstraints |= RigidbodyConstraints.FreezeRotationY;
        }
        else if (gravitationalPlane.plane == Plane.XZ)
        {
            newConstraints |= RigidbodyConstraints.FreezeRotationX;
            newConstraints |= RigidbodyConstraints.FreezeRotationZ;
        }
        else if (gravitationalPlane.plane == Plane.YZ)
        {
            newConstraints |= RigidbodyConstraints.FreezeRotationY;
            newConstraints |= RigidbodyConstraints.FreezeRotationZ;
        }

        if (movementAxisConstraints != null)
        {
            if (movementAxisConstraints.x) newConstraints |= RigidbodyConstraints.FreezePositionX;
            if (movementAxisConstraints.y) newConstraints |= RigidbodyConstraints.FreezePositionY;
            if (movementAxisConstraints.z) newConstraints |= RigidbodyConstraints.FreezePositionZ;
        }

        _rigidbody.constraints = newConstraints;
    }

    private void UpdateGravity()
    {
        Physics.gravity = gravitationalPlane.ToGravityVector() * GameManager.current.gravitationalAcceleration;

        UpdateRotation();
        UpdateConstraints();
    }

    public IEnumerator MoveAlongPath(List<PathFindingLocation> path)
    {
        isMoving = true;
        int i = 0;

        Vector3 playerPos = Vector3.zero, targetPos = Vector3.zero;
        MapLocation targetMapPos = MapLocation.Zero, currentBlockMapPos = MapLocation.Zero;
        float dist = 0;

        while (i < path.Count)
        {
            playerPos = transform.position;
            _currentMovementConnection = i == 0 ? path[i].connection : path[i - 1].connection;

            if (path[i].mapBlockData.block != _targetBlock)
            {
                _targetBlock = path[i].mapBlockData.block;

                targetPos = path[i].mapBlockData.worldLoc;
                targetMapPos = path[i].mapLoc;
                
                currentBlockMapPos = i == 0 ? targetMapPos : path[i - 1].mapLoc;

                float offset = GravitationalPlane.PlaneSideToInt(gravitationalPlane.planeSide) * _height;

                if (gravitationalPlane.plane == Plane.XY) targetPos.z += offset;
                else if (gravitationalPlane.plane == Plane.XZ) targetPos.y += offset;
                else if (gravitationalPlane.plane == Plane.YZ) targetPos.x += offset;

                if (i != 0) movementAxisConstraints = new MovementAxisConstraints(currentBlockMapPos-targetMapPos, gravitationalPlane.plane);

                UpdateConstraints();
            }

            dist = Vector3.Distance(targetPos, playerPos);

            if (dist < 0.1f) i++;
            else
            {
                //_rigidbody.MovePosition(targetPos);
                transform.Translate((targetPos - playerPos) * Time.deltaTime * movementSpeed);
            }

            yield return new WaitForFixedUpdate();
        }

        _currentMovementConnection = null;
        _targetBlock = null;
        isMoving = false;
    }

    public void TeleportFrom(ConnectionPoint fromTargetConnectionPoint)
    {
        if (_currentMovementConnection == null) return;

        Debug.Log(fromTargetConnectionPoint.parentBlock.name);
        ConnectionPoint targetConnectionPoint =
            _currentMovementConnection.connectionPoints.Contains(fromTargetConnectionPoint)
                ? _currentMovementConnection.connectionPoints.First(connectionPoint =>
                    connectionPoint != fromTargetConnectionPoint)
                : null;

        if (targetConnectionPoint == null) return;
        Debug.Log(targetConnectionPoint.parentBlock.name + " : " + _targetBlock.name);
        if (targetConnectionPoint.parentBlock != _targetBlock) return;

        canTeleport = false;

        teleportedLastTo = targetConnectionPoint;

        Vector3 offset = transform.position - fromTargetConnectionPoint.transform.position;
        Debug.Log(offset);

        transform.position = targetConnectionPoint.transform.position + new Vector3(
            offset.x,
            _height,
            offset.z
        );
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<Block>())
        {
            if (!canMove) canMove = true;
        }
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