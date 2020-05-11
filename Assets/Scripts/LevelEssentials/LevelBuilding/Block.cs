using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DataTypes;
using UnityEditor;
using UnityEngine.PlayerLoop;
using Plane = DataTypes.Plane;

/// <summary>
/// Class that is used by every object
/// that can be used to navigate the character on in the scene 
/// </summary>
[ExecuteAlways]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(MeshFilter))]
public class Block : MonoBehaviour
{
    //---------Public and Private Visible In Inspector---------\\
#if UNITY_EDITOR
    
#endif

    public static BlockSize size = new BlockSize(1, 1, 1);
    public static float nearbyRadius = 0.5f;
    
    public Dictionary<GravitationalPlane, HashSet<MapBlockData>> mapBlockDatas = new Dictionary<GravitationalPlane, HashSet<MapBlockData>>();

    public static readonly List<GravitationalPlane> BlockGravitationalPlanes = new List<GravitationalPlane>()
    {
        new GravitationalPlane(Plane.XY, PlaneSide.PlaneNormalNegative),
        new GravitationalPlane(Plane.XY, PlaneSide.PlaneNormalPositive),
        new GravitationalPlane(Plane.XZ, PlaneSide.PlaneNormalNegative),
        new GravitationalPlane(Plane.XZ, PlaneSide.PlaneNormalPositive),
        new GravitationalPlane(Plane.YZ, PlaneSide.PlaneNormalNegative),
        new GravitationalPlane(Plane.YZ, PlaneSide.PlaneNormalPositive),
    };
    //---------Public and Private Visible In Inspector---------\\

    //--------Private and Public Invisible In Inspector--------\\
    private MeshRenderer _meshRenderer;

    public int id;

    public List<ConnectionPoint> connectionPoints = new List<ConnectionPoint>();

    public Dictionary<GravitationalPlane, IsWalkablePoint> isWalkablePoints =
        new Dictionary<GravitationalPlane, IsWalkablePoint>();

    public MapPartBuilder mapPartBuilderParent;
    //--------Private and Public Invisible In Inspector--------\\

    private void Awake()
    {
        id = GetInstanceID();

        InitPrivateVars();
        ScaleToSetSize();
        CreatePoints();
    }

    private void Start()
    {
        if (Application.isPlaying)
        {
            LevelEventSystem.current.onBlockClicked += BlockClickedTapped;
        }
    }

    private void InitPrivateVars()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    void ScaleToSetSize()
    {
        Vector3 blockSize = _meshRenderer.bounds.size;

        transform.localScale = new Vector3(
            blockSize.x / size.x,
            blockSize.y / size.y,
            blockSize.z / size.z
        );
    }

    public void CreatePoints()
    {
#if UNITY_EDITOR
        InitPrivateVars();
#endif

        // Creating new connection points
        ConnectionPoint[] checkConnectionPoints = gameObject.GetComponentsInChildren<ConnectionPoint>();

        if (checkConnectionPoints.Length > 0)
        {
            connectionPoints = checkConnectionPoints.ToList();
            SetParentBlockToConnectionPoints();
        }
        else CreateConnectionPoints();

        // Creating new isWalkablePoint
        IsWalkablePoint[] checkIsWalkablePoints = gameObject.GetComponentsInChildren<IsWalkablePoint>();
        if (checkIsWalkablePoints.Length > 0)
        {
            isWalkablePoints = new Dictionary<GravitationalPlane, IsWalkablePoint>();
            foreach (IsWalkablePoint isWalkablePoint in checkIsWalkablePoints)
            {
                isWalkablePoints.Add(isWalkablePoint.gravitationalPlane, isWalkablePoint);
                isWalkablePoint.parentBlock = this;
            }
        }
        else CreateIsWalkablePoints();
    }

    private void CreateConnectionPoints()
    {
        connectionPoints = new List<ConnectionPoint>();
        DestroyImmediate(transform.Find("ConnectionPoints")?.gameObject);

        float halfX = size.x / 2f;
        float halfY = size.y / 2f;
        float halfZ = size.z / 2f;

        List<Vector3> connectionPointsPositions = new List<Vector3>()
        {
            //Upper
            new Vector3(-halfX, halfY, 0),
            new Vector3(0, halfY, halfZ),
            new Vector3(halfX, halfY, 0),
            new Vector3(0, halfY, -halfZ),
            //Middle
            new Vector3(-halfX, 0, -halfZ),
            new Vector3(-halfX, 0, halfZ),
            new Vector3(halfX, 0, halfZ),
            new Vector3(halfX, 0, -halfZ),
            //Lower
            new Vector3(-halfX, -halfY, 0),
            new Vector3(0, -halfY, halfZ),
            new Vector3(halfX, -halfY, 0),
            new Vector3(0, -halfY, -halfZ)
        };

        GameObject connectionPointsHolder = new GameObject("ConnectionPoints");
        connectionPointsHolder.transform.SetParent(transform);

        for (int i = 0; i < connectionPointsPositions.Count; i++)
        {
            GameObject newConnectionPointObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            newConnectionPointObject.name = "ConnectionPoint " + (i + 1);
            newConnectionPointObject.tag = "ConnectionPoint";
            newConnectionPointObject.layer = LayerMask.NameToLayer("Debug");

            Transform newConnectionPointObjectTransform = newConnectionPointObject.transform;
            newConnectionPointObjectTransform.SetParent(connectionPointsHolder.transform);
            newConnectionPointObjectTransform.localScale = Vector3.one * ConnectionPoint.scale;
            newConnectionPointObjectTransform.localPosition = connectionPointsPositions[i];

            SphereCollider conPointCollider = newConnectionPointObject.GetComponent<SphereCollider>();
            if (conPointCollider) DestroyImmediate(conPointCollider);

            ConnectionPoint newConnectionPointComponent = newConnectionPointObject.AddComponent<ConnectionPoint>();
            newConnectionPointComponent.parentBlock = this;

            connectionPoints.Add(newConnectionPointComponent);
        }

        connectionPointsHolder.transform.localPosition = Vector3.zero;
    }

    public void SetParentBlockToConnectionPoints()
    {
        if (connectionPoints.Count < 1)
        {
            ConnectionPoint[] checkConnectionPoints = GetComponentsInChildren<ConnectionPoint>();
            connectionPoints = checkConnectionPoints.ToList();
            SetParentBlockToConnectionPoints();
        }
        else
        {
            foreach (ConnectionPoint connectionPoint in connectionPoints)
            {
                connectionPoint.parentBlock = this;
            }
        }
    }

    private void CreateIsWalkablePoints()
    {
        isWalkablePoints = new Dictionary<GravitationalPlane, IsWalkablePoint>();
        DestroyImmediate(transform.Find("IsWalkablePoints")?.gameObject);

        GameObject isWalkablePointsHolder = new GameObject("IsWalkablePoints");
        isWalkablePointsHolder.transform.SetParent(transform);

        float halfX = size.x / 2f;
        float halfY = size.y / 2f;
        float halfZ = size.z / 2f;

        for (int i = 0; i < BlockGravitationalPlanes.Count; i++)
        {
            GravitationalPlane gravitationalPlane = BlockGravitationalPlanes[i];

            GameObject newIsWalkablePoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            newIsWalkablePoint.name = "IsWalkablePoint" + (i + 1);
            newIsWalkablePoint.tag = "IsWalkablePoint";
            newIsWalkablePoint.layer = LayerMask.NameToLayer("Debug");

            Transform newIsWalkablePointTransform = newIsWalkablePoint.transform;
            newIsWalkablePoint.transform.SetParent(isWalkablePointsHolder.transform);
            newIsWalkablePoint.transform.localScale = Vector3.one * IsWalkablePoint.scale;

            if (gravitationalPlane.plane == Plane.XY)
            {
                if (gravitationalPlane.planeSide == PlaneSide.PlaneNormalPositive)
                    newIsWalkablePointTransform.localPosition = new Vector3(0, 0, halfZ);
                if (gravitationalPlane.planeSide == PlaneSide.PlaneNormalNegative)
                    newIsWalkablePointTransform.localPosition = new Vector3(0, 0, -halfZ);
            }
            else if (gravitationalPlane.plane == Plane.XZ)
            {
                if (gravitationalPlane.planeSide == PlaneSide.PlaneNormalPositive)
                    newIsWalkablePointTransform.localPosition = new Vector3(0, halfY, 0);
                if (gravitationalPlane.planeSide == PlaneSide.PlaneNormalNegative)
                    newIsWalkablePointTransform.localPosition = new Vector3(0, -halfY, 0);
            }
            else if (gravitationalPlane.plane == Plane.YZ)
            {
                if (gravitationalPlane.planeSide == PlaneSide.PlaneNormalPositive)
                    newIsWalkablePointTransform.localPosition = new Vector3(halfX, 0, 0);
                if (gravitationalPlane.planeSide == PlaneSide.PlaneNormalNegative)
                    newIsWalkablePointTransform.localPosition = new Vector3(-halfX, 0, 0);
            }

            Collider isWalkablePointCollider = newIsWalkablePoint.GetComponent<Collider>();
            if (isWalkablePointCollider) isWalkablePointCollider.isTrigger = true;

            IsWalkablePoint newIsWalkablePointComponent = newIsWalkablePoint.AddComponent<IsWalkablePoint>();
            newIsWalkablePointComponent.parentBlock = this;
            newIsWalkablePointComponent.gravitationalPlane = gravitationalPlane;

            isWalkablePoints.Add(gravitationalPlane, newIsWalkablePointComponent);

            isWalkablePointsHolder.transform.localPosition = Vector3.zero;
        }
    }
    
    public void IsWalkablePointsCheckIfWalkable()
    {
        foreach (IsWalkablePoint isWalkablePoint in isWalkablePoints.Values)
        {
            isWalkablePoint.CheckIfWalkable();
        }   
    }
    
    public void IsWalkablePointsSetActiveAndSetParentBlock()
    {
        foreach (IsWalkablePoint isWalkablePoint in isWalkablePoints.Values)
        {
            isWalkablePoint.enabled = true;
            isWalkablePoint.parentBlock = this;
        }
    }

    private void BlockClickedTapped(int id)
    {
        GameManager gm = GameManager.current;
        
        if (this.id == id && gm.player.canMove && gm.mapBuilder.PathFindingMapsDataExists)
        {
            StartCoroutine(
                gm.player.MoveAlongPath(
                    gm.pathFinder.FindShortestPath(
                        gm.player.blockStandingOn, this, gm.player.gravitationalPlane
                        )
                    )
                );
            GameManager.current.cameraLockedMovement = false;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        CheckIfPlayerCollided(other);
    }

    private void OnCollisionStay(Collision other)
    {
        CheckIfPlayerCollided(other);
    }

    private void CheckIfPlayerCollided(Collision other)
    {
        if (other.gameObject != GameManager.current.player.gameObject) return;
        if (GameManager.current.player.BlockStandingOn != this) GameManager.current.player.BlockStandingOn = this;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (GameManager.current.blocksDebugDrawAxisLines)
        {
            Vector3 position = transform.position;
            Vector3 endPos;

            // X Axis
            Gizmos.color = Color.red;
            endPos = position + transform.right * GameManager.current.blocksDebugAxisLinesLength;
            Gizmos.DrawLine(position, endPos);
            if (GameManager.current.blocksDebugDrawAxisLinesTitles) 
                Handles.Label(endPos + GameManager.current.blocksDebugDrawAxisLinesTitlesOffset, "Right");

            // Y Axis
            Gizmos.color = Color.green;
            endPos = position + transform.up * GameManager.current.blocksDebugAxisLinesLength;
            Gizmos.DrawLine(position, endPos);
            if (GameManager.current.blocksDebugDrawAxisLinesTitles) 
                Handles.Label(endPos + GameManager.current.blocksDebugDrawAxisLinesTitlesOffset, "Up");

            // Z Axis
            Gizmos.color = Color.blue;
            endPos = position + transform.forward * GameManager.current.blocksDebugAxisLinesLength;
            Gizmos.DrawLine(position, endPos);
            if (GameManager.current.blocksDebugDrawAxisLinesTitles) 
                Handles.Label(endPos + GameManager.current.blocksDebugDrawAxisLinesTitlesOffset, "Forward");
        }

        if (GameManager.current.blocksDebugDrawLocalPositions)
        {
            Handles.Label(transform.position + Vector3.up, transform.localPosition.ToString());
        }
    }
#endif

    private void OnDestroy()
    {
        if (Application.isPlaying && LevelEventSystem.current)
        {
            LevelEventSystem.current.onBlockClicked -= BlockClickedTapped;
        }
    }
}