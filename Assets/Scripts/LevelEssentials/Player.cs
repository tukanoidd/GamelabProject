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
    private Block _targetBlock = null;
    private PathFindingLocation _current = null;

    private Material _testBlockMat;
    private Material _savedBlockMat;

    public bool isMoving = false;
    public MovementAxisConstraints movementAxisConstraints = null;

    public bool isTeleporting = false;
    public bool canTeleport = true;
    public bool canMove = false;

    public MovementDirection? movementDirection = null;
    //--------Private and Public Invisible In Inspector--------\\

    private void Awake()
    {
        InitPrivateVars();

        UpdateGravity();
        UpdateRotation();
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

    private void UpdateGravity()
    {
        Physics.gravity = gravitationalPlane.ToGravityVector() * GameManager.current.gravitationalAcceleration;
    }

    public IEnumerator MoveAlongPath(List<PathFindingLocation> path)
    {
        isMoving = true;
        int i = 0;

        Vector3 targetBlockPos, playerPos, targetPos;
        float dist = 0;

        while (i < path.Count)
        {
            targetBlockPos = path[i].mapBlockData.worldLoc;
            playerPos = transform.position;
            targetPos = targetBlockPos;

            float offset = GravitationalPlane.PlaneSideToInt(gravitationalPlane.planeSide) * _height;

            if (gravitationalPlane.plane == Plane.XY) targetPos.z += offset;
            else if (gravitationalPlane.plane == Plane.XZ) targetPos.y += offset;
            else if (gravitationalPlane.plane == Plane.YZ) targetPos.x += offset;

            dist = Vector3.Distance(targetPos, playerPos);

            if (dist < 0.1f) i++;
            else
            {
                _rigidbody.MovePosition(targetPos);
                transform.Translate((targetPos - playerPos) * Time.deltaTime * movementSpeed);
            }

            yield return new WaitForFixedUpdate();
        }

        isMoving = false;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<Block>())
        {
            canMove = true;
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