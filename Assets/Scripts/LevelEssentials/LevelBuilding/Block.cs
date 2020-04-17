using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DataTypes;
using MeshEdge;
using UnityEditor;
using Object = UnityEngine.Object;
using Plane = DataTypes.Plane;

/// <summary>
/// Class that is used by every object
/// that can be used to navigate the character on in the scene 
/// </summary>
[ExecuteAlways]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Collider))]
public class Block : MonoBehaviour
{
    //---------Public and Private Visible In Inspector---------\\
#if UNITY_EDITOR
    [SerializeField] private bool drawDebugConnectionLines = true;

    [SerializeField] private bool drawDebugAxisLines = true;
    [SerializeField] private float debugAxisLinesLength = 3f;
    [SerializeField] private bool drawDebugAxisLinesTitles = false;
    [SerializeField] private Vector3 drawDebugAxisLinesTitlesOffset = Vector3.up * 0.3f;
#endif

    public static BlockSize size = new BlockSize(1, 1, 1);
    public static float nearbyRadius = 1;

    public static readonly List<GravitationalPlane> BlockGravitationalPlanes = new List<GravitationalPlane>()
    {
        new GravitationalPlane(Plane.XY, PlaneSide.PlaneNormalNegative),
        new GravitationalPlane(Plane.XY, PlaneSide.PlaneNormalPositive),
        new GravitationalPlane(Plane.XZ, PlaneSide.PlaneNormalNegative),
        new GravitationalPlane(Plane.XZ, PlaneSide.PlaneNormalPositive),
        new GravitationalPlane(Plane.YZ, PlaneSide.PlaneNormalNegative),
        new GravitationalPlane(Plane.YZ, PlaneSide.PlaneNormalPositive),
    };

    public bool HasConnections => blockConnections.Count > 0;
    public bool HasNearbyConnections => blockConnections.Any(blockConnection => blockConnection.isNear);

    public List<BlockConnection> NearbyConnections =>
        blockConnections.Where(blockConnection => blockConnection.isNear).ToList();
    //---------Public and Private Visible In Inspector---------\\

    //--------Private and Public Invisible In Inspector--------\\
    private Mesh _mesh;
    private MeshRenderer _meshRenderer;
    private List<Edge> _meshEdges = new List<Edge>();

    private PathFinder _pathFinder;

    [NonSerialized] public int id;

    [NonSerialized] public bool pointsReset = false;
    [NonSerialized] public bool initPoints = true;

    [NonSerialized] public List<ConnectionPoint> connectionPoints = new List<ConnectionPoint>();

    [NonSerialized] public Dictionary<GravitationalPlane, IsWalkablePoint> isWalkablePoints =
        new Dictionary<GravitationalPlane, IsWalkablePoint>();

    [NonSerialized] public List<BlockConnection> blockConnections = new List<BlockConnection>();

    [NonSerialized] public MapBlockData mapData;
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

    private void Update()
    {
    }

    private void InitPrivateVars()
    {
        _mesh = GetComponent<MeshFilter>().sharedMesh;
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshEdges = MeshEdgeTools.GetEdges(_mesh.triangles).FindBoundary().SortEdges();
    }

    public void FindPathFinder()
    {
        _pathFinder = FindObjectOfType<PathFinder>();
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
        ConnectionPoint[] checkConnectionPoints = GetComponentsInChildren<ConnectionPoint>();
        if (checkConnectionPoints.Length > 0)
        {
            if (pointsReset || initPoints)
            {
                ObjectsHelpers.DestroyObjects(checkConnectionPoints);
                CreateConnectionPoints();
            }
            else connectionPoints = checkConnectionPoints.ToList();
        }
        else CreateConnectionPoints();

        // Creating new isWalkablePoint
        IsWalkablePoint[] checkIsWalkablePoints = GetComponentsInChildren<IsWalkablePoint>();
        if (checkIsWalkablePoints.Length > 0)
        {
            if (pointsReset || initPoints)
            {
                ObjectsHelpers.DestroyObjects(checkIsWalkablePoints);
                CreateIsWalkablePoints();
            }
            else
            {
                isWalkablePoints = new Dictionary<GravitationalPlane, IsWalkablePoint>();
                foreach (IsWalkablePoint isWalkablePoint in checkIsWalkablePoints)
                {
                    isWalkablePoints.Add(isWalkablePoint.gravitationalPlane, isWalkablePoint);
                }
            }
        }
        else CreateIsWalkablePoints();

        pointsReset = false;
        initPoints = true;
    }

    private void CreateConnectionPoints()
    {
        connectionPoints = new List<ConnectionPoint>();
        DestroyImmediate(transform.Find("ConnectionPoints")?.gameObject);

        Vector3[] vertices = _mesh.vertices;
        List<Vector3> connectionPointsPositions = new List<Vector3>();

        foreach (Edge edge in _meshEdges)
        {
            Vector3 newConnectionPointPosition = (vertices[edge.v1] + vertices[edge.v2]) / 2;
            if (!connectionPointsPositions.Contains(newConnectionPointPosition))
                connectionPointsPositions.Add(newConnectionPointPosition);
        }

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
    }

    private void CreateIsWalkablePoints()
    {
        isWalkablePoints = new Dictionary<GravitationalPlane, IsWalkablePoint>();
        DestroyImmediate(transform.Find("IsWalkablePoints")?.gameObject);

        GameObject isWalkablePointsHolder = new GameObject("IsWalkablePoints");
        isWalkablePointsHolder.transform.SetParent(transform);

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
                    newIsWalkablePointTransform.localPosition = new Vector3(0, 0, size.z / 2f);
                if (gravitationalPlane.planeSide == PlaneSide.PlaneNormalNegative)
                    newIsWalkablePointTransform.localPosition = new Vector3(0, 0, -size.z / 2f);
            }
            else if (gravitationalPlane.plane == Plane.XZ)
            {
                if (gravitationalPlane.planeSide == PlaneSide.PlaneNormalPositive)
                    newIsWalkablePointTransform.localPosition = new Vector3(0, size.y / 2f, 0);
                if (gravitationalPlane.planeSide == PlaneSide.PlaneNormalNegative)
                    newIsWalkablePointTransform.localPosition = new Vector3(0, -size.y / 2f, 0);
            }
            else if (gravitationalPlane.plane == Plane.YZ)
            {
                if (gravitationalPlane.planeSide == PlaneSide.PlaneNormalPositive)
                    newIsWalkablePointTransform.localPosition = new Vector3(size.x / 2f, 0, 0);
                if (gravitationalPlane.planeSide == PlaneSide.PlaneNormalNegative)
                    newIsWalkablePointTransform.localPosition = new Vector3(-size.x / 2f, 0, 0);
            }

            Collider isWalkablePointCollider = newIsWalkablePoint.GetComponent<Collider>();
            if (isWalkablePointCollider) DestroyImmediate(isWalkablePointCollider);

            IsWalkablePoint newIsWalkablePointComponent = newIsWalkablePoint.AddComponent<IsWalkablePoint>();
            newIsWalkablePointComponent.parentBlock = this;
            newIsWalkablePointComponent.gravitationalPlane = gravitationalPlane;

            isWalkablePoints.Add(gravitationalPlane, newIsWalkablePointComponent);
        }
    }

    private void BlockClickedTapped(int id)
    {
        if (this.id == id)
        {
            //todo logic for player movement on click   
            Debug.Log(name + " clicked");
        }
    }

    public void TeleportPlayerFrom(Player player, ConnectionPoint connectionPoint)
    {
        //todo add teleportation logic
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (drawDebugConnectionLines)
        {
            Gizmos.color = Color.yellow;
            foreach (BlockConnection blockConnection in blockConnections)
            {
                foreach (KeyValuePair<ConnectionPoint, ConnectionPoint> connectionPointsDictionary in blockConnection
                    .connectionPoints)
                {
                    Gizmos.DrawLine(connectionPointsDictionary.Key.transform.position,
                        connectionPointsDictionary.Value.transform.position);
                }
            }
        }

        if (drawDebugAxisLines)
        {
            Vector3 position = transform.position;
            Vector3 endPos;

            // X Axis
            Gizmos.color = Color.red;
            endPos = position + transform.right * debugAxisLinesLength;
            Gizmos.DrawLine(position, endPos);
            if (drawDebugAxisLinesTitles) Handles.Label(endPos + drawDebugAxisLinesTitlesOffset, "Right");

            // Y Axis
            Gizmos.color = Color.green;
            endPos = position + transform.up * debugAxisLinesLength;
            Gizmos.DrawLine(position, endPos);
            if (drawDebugAxisLinesTitles) Handles.Label(endPos + drawDebugAxisLinesTitlesOffset, "Up");

            // Z Axis
            Gizmos.color = Color.blue;
            endPos = position + transform.forward * debugAxisLinesLength;
            Gizmos.DrawLine(position, endPos);
            if (drawDebugAxisLinesTitles) Handles.Label(endPos + drawDebugAxisLinesTitlesOffset, "Forward");
        }
    }
#endif

    private void OnDestroy()
    {
        if (Application.isPlaying)
        {
            LevelEventSystem.current.onBlockClicked -= BlockClickedTapped;
        }
    }
}