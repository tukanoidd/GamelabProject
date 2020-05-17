using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataTypes;
using UnityEngine;
using Plane = DataTypes.Plane;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class Player : MonoBehaviour
{
    //---------Public and Private Visible In Inspector---------\\
    [SerializeField] private bool drawDebugLineToTarget = true;
    [SerializeField] private bool drawDebugLineForward = true;
    [SerializeField] private float debugLineForwardLength = 3f;

    [SerializeField] private bool drawPathWhenMoving;
    [SerializeField] private float movementSpeed = 10;

    [SerializeField] private float gravity = 10f;

    [SerializeField] private float groundCheckDist = 0.4f;
    [SerializeField] private float groundedGravity = 2f;
    [SerializeField] private LayerMask groundMask;

    [SerializeField] private float customCameraPositionMaxOffset = 0.5f;

    [SerializeField] private bool checkForCamera = false;

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
    private CharacterController _characterController;
    private float _height = 0;

    private Vector3? _targetPosition = null;

    private Material _testBlockMat;
    private Material _savedBlockMat;

    private BlockConnection _currentMovementConnection = null;
    private Block _targetBlock = null;

    private Vector3 _velocity = Vector3.zero;

    private Transform _groundCheck;

    [HideInInspector] public bool grounded = false;
    [HideInInspector] public bool isMoving = false;

    [HideInInspector] public bool canTeleport = true;
    [HideInInspector] public ConnectionPoint teleportedLastTo = null;

    //--------Private and Public Invisible In Inspector--------\\

    private void Awake()
    {
        InitPrivateVars();
        UpdateRotation();
    }

    private void FixedUpdate()
    {
        CalculateGravity();
        UpdateRotation();

        _characterController.Move(_velocity * Time.fixedDeltaTime);
    }

    private void CalculateGravity()
    {
        float planeSideGravity = gravity * -GravitationalPlane.PlaneSideToInt(gravitationalPlane.planeSide);

        Collider[] colliders = Physics.OverlapSphere(_groundCheck.position, groundCheckDist, groundMask);
        grounded = colliders.Length > 0;

        if (grounded)
        {
            Block[] blocks = colliders.Where(coll => coll.GetComponent<Block>() != null)
                .Select(coll => coll.GetComponent<Block>()).ToArray();

            if (blocks.Length > 0)
                BlockStandingOn = blocks
                    .OrderBy(block => Vector3.Distance(_groundCheck.position, block.transform.position)).First();
        }

        if (gravitationalPlane.plane == Plane.XY)
        {
            if (grounded)
            {
                if (gravitationalPlane.planeSide == PlaneSide.PlaneNormalPositive && _velocity.z < 0)
                    _velocity.z = -groundedGravity;
                else if (gravitationalPlane.planeSide == PlaneSide.PlaneNormalNegative && _velocity.z > 0)
                    _velocity.z = groundedGravity;
            }

            _velocity.z += planeSideGravity;
        }
        else if (gravitationalPlane.plane == Plane.XZ)
        {
            if (grounded)
            {
                if (gravitationalPlane.planeSide == PlaneSide.PlaneNormalPositive && _velocity.y < 0)
                    _velocity.y = -groundedGravity;
                else if (gravitationalPlane.planeSide == PlaneSide.PlaneNormalNegative && _velocity.y > 0)
                    _velocity.y = groundedGravity;
            }

            _velocity.y += planeSideGravity;
        }
        else if (gravitationalPlane.plane == Plane.YZ)
        {
            if (grounded)
            {
                if (gravitationalPlane.planeSide == PlaneSide.PlaneNormalPositive && _velocity.x < 0)
                    _velocity.x = -groundedGravity;
                else if (gravitationalPlane.planeSide == PlaneSide.PlaneNormalNegative && _velocity.x > 0)
                    _velocity.x = groundedGravity;
            }

            _velocity.x += planeSideGravity;
        }
    }

    private void OnGravityChanged()
    {
        _velocity = Vector3.zero;
        UpdateRotation();
    }

    private void InitPrivateVars()
    {
        _characterController = GetComponent<CharacterController>();
        _groundCheck = transform.Find("GroundCheck");
        _height = GetComponent<MeshRenderer>().bounds.size.y;
        _testBlockMat = Resources.Load<Material>("Materials/PathFindingBlockTest");
    }

    public void UpdateRotation() => transform.localEulerAngles =
        gravitationalPlane.ToRotationEuler(transform.localRotation.eulerAngles);

    public IEnumerator MoveAlongPath(List<PathFindingLocation> path)
    {
        isMoving = true;

        int lastI = -1;

        PathFindingLocation loc = null;
        PathFindingLocation nextLoc = null;
        MapLocation offset;
        MapLocation prevOffset = new MapLocation(-1, -1);
        Vector2 targetBlockPos2D = Vector2.zero;
        Vector2 playerPos2D = Vector2.zero;

        int pathLen = path.Count();

        for (int i = 0; i < pathLen - 1;)
        {
            if (lastI != i)
            {
                lastI = i;

                loc = path[i];
                nextLoc = path[i + 1];

                _currentMovementConnection = loc.connection;
            }

            if (_currentMovementConnection != null && checkForCamera)
            {
                if (!_currentMovementConnection.customCameraPositions.Any(pos =>
                    Vector3.Distance(GameManager.current.mainCamera.transform.position, pos) <=
                    customCameraPositionMaxOffset) && !_currentMovementConnection.isNear)
                {
                    _velocity = Vector3.zero;
                    yield break;
                }
            }

            if (loc != null && nextLoc != null)
            {
                offset = nextLoc.mapLoc - loc.mapLoc;

                _targetBlock = nextLoc.mapBlockData.block;

                Vector3 targetBlockPos = nextLoc.mapBlockData.block.transform.position;
                Vector3 playerPos = transform.position;

                if (!prevOffset.Equals(offset)) Rotate(offset);

                prevOffset = offset;

                if (gravitationalPlane.plane == Plane.XY)
                {
                    _velocity.x = offset.col * movementSpeed;
                    _velocity.y = offset.row * movementSpeed;

                    targetBlockPos2D = new Vector2(targetBlockPos.x, targetBlockPos.y);
                    playerPos2D = new Vector2(playerPos.x, playerPos.y);
                }
                else if (gravitationalPlane.plane == Plane.XZ)
                {
                    _velocity.x = offset.col * movementSpeed;
                    _velocity.z = offset.row * movementSpeed;

                    targetBlockPos2D = new Vector2(targetBlockPos.x, targetBlockPos.z);
                    playerPos2D = new Vector2(playerPos.x, playerPos.z);
                }
                else if (gravitationalPlane.plane == Plane.YZ)
                {
                    _velocity.z = offset.col * movementSpeed;
                    _velocity.y = offset.row * movementSpeed;

                    targetBlockPos2D = new Vector2(targetBlockPos.y, targetBlockPos.z);
                    playerPos2D = new Vector2(playerPos.y, playerPos.z);
                }

                yield return new WaitForFixedUpdate();

                if (Vector2.Distance(playerPos2D, targetBlockPos2D) <= 0.1f)
                {
                    TeleportTo(HelperMethods.SnapToBlockGridPlane(transform.position, gravitationalPlane.plane));
                    i++;
                }
            }
        }

        _targetBlock = null;
        _currentMovementConnection = null;
        _velocity = Vector3.zero;
        isMoving = false;
        GameManager.current.cameraLockedMovement = false;
    }

    private void TeleportTo(Vector3 newPos)
    {
        _characterController.enabled = false;
        transform.position = newPos;
        _characterController.enabled = true;
    }

    private void Rotate(MapLocation offset)
    {
        int row = offset.row;
        int col = offset.col;

        transform.localEulerAngles = new Vector3(
            transform.localEulerAngles.x,
            row != 0 ? (row < 0 ? 180 : 0) : (col != 0 ? (col < 0 ? -90 : 90) : 0),
            transform.localEulerAngles.z
        );
    }

    public void TeleportFrom(ConnectionPoint fromTargetConnectionPoint)
    {
        if (_currentMovementConnection == null) return;

        ConnectionPoint targetConnectionPoint =
            _currentMovementConnection.connectionPoints.Contains(fromTargetConnectionPoint)
                ? _currentMovementConnection.connectionPoints.First(connectionPoint =>
                    connectionPoint != fromTargetConnectionPoint)
                : null;

        if (targetConnectionPoint == null) return;
        if (targetConnectionPoint.parentBlock != _targetBlock) return;

        canTeleport = false;

        teleportedLastTo = targetConnectionPoint;

        Vector3 offset = transform.position - fromTargetConnectionPoint.transform.position;

        TeleportTo(targetConnectionPoint.transform.position + new Vector3(
            offset.x,
            _height,
            offset.z
        ));
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