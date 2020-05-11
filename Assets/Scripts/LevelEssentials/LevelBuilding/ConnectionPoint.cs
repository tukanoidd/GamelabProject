using System;
using System.Collections.Generic;
using System.Linq;
using DataTypes;
using UnityEditor;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.UIElements;
using Plane = DataTypes.Plane;

[ExecuteAlways]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class ConnectionPoint : MonoBehaviour
{
    //---------Public and Private Visible In Inspector---------\\
    public static float scale = 0.1f;
    public static float tpTriggerHeightDivider = 2;
    public static float tpTriggerLengthDivider = 1.25f;
    public static float tpTriggerDepthDivider = 5;

    public float customMaxCamOffset = 0.5f;

    public bool HasParent => parentBlock != null;
    //---------Public and Private Visible In Inspector---------\\

    //--------Private and Public Invisible In Inspector--------\\
#if UNITY_EDITOR
    private MeshRenderer _meshRenderer;
    private Material _standardMat;
    private Material _connectedMat;
#endif

    private List<BoxCollider> _tpTriggers;

    public Block parentBlock;
    public bool isConnected;
    public bool isConnectedNearby;

    public ConnectionPointPositionDirections posDirs;
    //--------Private and Public Invisible In Inspector--------\\

    private void Awake()
    {
        InitPrivateVars();
        FindPosDirs();
        AddTpTriggers();
    }

    private void Update()
    {
#if UNITY_EDITOR
        UpdateDebugMaterials();
#endif
    }

    void InitPrivateVars()
    {
#if UNITY_EDITOR
        _meshRenderer = GetComponent<MeshRenderer>();
        _standardMat = Resources.Load<Material>("Materials/ConnectionPointMat");
        _connectedMat = Resources.Load<Material>("Materials/ConnectedConnectionPoint");
#endif
    }

    public void FindPosDirs()
    {
        Vector3 connectionPointPos = transform.localPosition;
        float x = connectionPointPos.x;
        float y = connectionPointPos.y;
        float z = connectionPointPos.z;

        List<GravitationalPlane> gravitationalPlanes = new List<GravitationalPlane>();

        if (y > 0 || y < 0)
        {
            gravitationalPlanes.Add(new GravitationalPlane(Plane.XZ, GravitationalPlane.GetPlaneSide(y)));

            if (x > 0 || x < 0)
                gravitationalPlanes.Add(new GravitationalPlane(Plane.YZ, GravitationalPlane.GetPlaneSide(x)));
            else if (z < 0 || z > 0)
                gravitationalPlanes.Add(new GravitationalPlane(Plane.XY, GravitationalPlane.GetPlaneSide(z)));
        }
        else
        {
            gravitationalPlanes.Add(new GravitationalPlane(Plane.XY, GravitationalPlane.GetPlaneSide(z)));
            gravitationalPlanes.Add(new GravitationalPlane(Plane.YZ, GravitationalPlane.GetPlaneSide(x)));
        }

        posDirs = new ConnectionPointPositionDirections(gravitationalPlanes,
            new List<AxisPositionDirection>()
            {
                new AxisPositionDirection(Axis.X, AxisPositionDirection.GetDirection(x)),
                new AxisPositionDirection(Axis.Y, AxisPositionDirection.GetDirection(y)),
                new AxisPositionDirection(Axis.Z, AxisPositionDirection.GetDirection(z)),
            });
    }

    public void AddTpTriggers()
    {
        HelperMethods.DestroyObjects(GetComponents<Collider>());

        _tpTriggers = new List<BoxCollider>();

        foreach (GravitationalPlane gravitationalPlane in posDirs.gravitationalPlanes)
        {
            BoxCollider tpTrigger = gameObject.AddComponent<BoxCollider>();
            tpTrigger.isTrigger = true;
            tpTrigger.tag = "TpTrigger";

            SetupTpTrigger(tpTrigger, gravitationalPlane.plane, posDirs.axisPositionDirections);

            _tpTriggers.Add(tpTrigger);
        }
    }

    private void SetupTpTrigger(BoxCollider tpTrigger, Plane plane, List<AxisPositionDirection> dirs)
    {
#if UNITY_EDITOR
        InitPrivateVars();
#endif
        AxisDirection xDir = dirs.First(dir => dir.axis == Axis.X).dir;
        AxisDirection yDir = dirs.First(dir => dir.axis == Axis.Y).dir;
        AxisDirection zDir = dirs.First(dir => dir.axis == Axis.Z).dir;

        Vector3 tpTriggerSize = new Vector3();
        Vector3 tpTriggerCenter = tpTrigger.center;
        PlaneSide planeSide;

        Vector3 localScale = transform.localScale;
        Vector3 meshSize = HelperMethods.Divide(_meshRenderer.bounds.size, localScale);
        Vector3 relativeBlockSize =
            HelperMethods.Divide(meshSize, HelperMethods.Divide(_meshRenderer.bounds.size, Block.size));

        if (plane == Plane.XY)
        {
            tpTriggerSize.z = relativeBlockSize.z / tpTriggerHeightDivider;

            planeSide = GravitationalPlane.GetPlaneSide(zDir);

            if (xDir == AxisDirection.Zero)
            {
                tpTriggerSize.x = relativeBlockSize.x / tpTriggerLengthDivider;
                tpTriggerSize.y = relativeBlockSize.y / tpTriggerDepthDivider;
            }
            else if (yDir == AxisDirection.Zero)
            {
                tpTriggerSize.x = relativeBlockSize.x / tpTriggerDepthDivider;
                tpTriggerSize.y = relativeBlockSize.y / tpTriggerLengthDivider;
            }

            if (planeSide == PlaneSide.PlaneNormalNegative) tpTriggerCenter.z -= tpTriggerSize.z / 2;
            else if (planeSide == PlaneSide.PlaneNormalPositive) tpTriggerCenter.z += tpTriggerSize.z / 2;
        }
        else if (plane == Plane.XZ)
        {
            tpTriggerSize.y = relativeBlockSize.y / tpTriggerHeightDivider;

            planeSide = GravitationalPlane.GetPlaneSide(yDir);

            if (xDir == AxisDirection.Zero)
            {
                tpTriggerSize.x = relativeBlockSize.x / tpTriggerLengthDivider;
                tpTriggerSize.z = relativeBlockSize.z / tpTriggerDepthDivider;
            }
            else if (zDir == AxisDirection.Zero)
            {
                tpTriggerSize.x = relativeBlockSize.x / tpTriggerDepthDivider;
                tpTriggerSize.z = relativeBlockSize.z / tpTriggerLengthDivider;
            }

            if (planeSide == PlaneSide.PlaneNormalNegative) tpTriggerCenter.y -= tpTriggerSize.y / 2;
            else if (planeSide == PlaneSide.PlaneNormalPositive) tpTriggerCenter.y += tpTriggerSize.y / 2;
        }
        else if (plane == Plane.YZ)
        {
            tpTriggerSize.x = relativeBlockSize.x / tpTriggerHeightDivider;

            planeSide = GravitationalPlane.GetPlaneSide(xDir);

            if (yDir == AxisDirection.Zero)
            {
                tpTriggerSize.y = relativeBlockSize.y / tpTriggerLengthDivider;
                tpTriggerSize.z = relativeBlockSize.z / tpTriggerDepthDivider;
            }
            else if (zDir == AxisDirection.Zero)
            {
                tpTriggerSize.y = relativeBlockSize.y / tpTriggerDepthDivider;
                tpTriggerSize.z = relativeBlockSize.z / tpTriggerLengthDivider;
            }

            if (planeSide == PlaneSide.PlaneNormalNegative) tpTriggerCenter.x -= tpTriggerSize.x / 2;
            else if (planeSide == PlaneSide.PlaneNormalPositive) tpTriggerCenter.x += tpTriggerSize.x / 2;
        }

        SetTpTriggerSizeCenter(tpTrigger, tpTriggerSize, tpTriggerCenter);
    }

    private void SetTpTriggerSizeCenter(BoxCollider tpTrigger, Vector3 size, Vector3 center)
    {
        tpTrigger.size = size;
        tpTrigger.center = center;
    }

    public void FindNearbyConnections()
    {
        ConnectionPoint[] connectionPoints = FindObjectsOfType<ConnectionPoint>().Where(connectionPoint =>
            connectionPoint != this && parentBlock != connectionPoint.parentBlock &&
            HelperMethods.CheckIsNear(connectionPoint, this)).ToArray();

        foreach (ConnectionPoint connectionPoint in connectionPoints)
        {
            GameManager.current.ConnectBlocks(
                connectionPoint,
                this,
                HelperMethods.CheckIsNear(connectionPoint, this)
            );
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isConnectedNearby && isConnected)
        {
            Player player = other.GetComponent<Player>();
            if (!player) return;
            
            if (name.Contains("2") && parentBlock.name.Contains("5"))
            {
                Debug.Log("-------");
                Debug.Log(player.canTeleport);
                Debug.Log("-------");
            }

            if (player.canTeleport)
            {
                //Debug.Log("InitTp from " + name + " : " + parentBlock.name);
                player.TeleportFrom(this);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isConnectedNearby)
        {
            Player player = other.GetComponent<Player>();
            if (!player) return;
            if (!player.canTeleport && player.teleportedLastTo == this) player.canTeleport = true;
        }
    }

#if UNITY_EDITOR

    private void UpdateDebugMaterials()
    {
        if (isConnected && _meshRenderer.sharedMaterial != _connectedMat) _meshRenderer.sharedMaterial = _connectedMat;
        else if (!isConnected && _meshRenderer.sharedMaterial != _standardMat)
            _meshRenderer.sharedMaterial = _standardMat;
    }

    private void OnDrawGizmos()
    {
        if (GameManager.current.connectionPointsDebugDrawHasParentBlock)
        {
            Handles.Label(transform.position + Vector3.up, HasParent ? "Y" : "N");   
        }
    }
#endif
}