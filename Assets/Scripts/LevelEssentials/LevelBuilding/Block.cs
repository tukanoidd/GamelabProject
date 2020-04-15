using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DataTypes;
using MeshEdge;
using Object = UnityEngine.Object;

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
    [SerializeField] private bool isTesting = true;

    public static BlockSize size = new BlockSize(1, 1, 1);
    public static float isWalkablePointScale = 0.25f;
    
    public bool isWalkable = true;

    public float IsWalkablePointScale => isWalkablePointScale;
    //---------Public and Private Visible In Inspector---------\\

    //--------Private and Public Invisible In Inspector--------\\
    private Mesh _mesh;
    private MeshRenderer _meshRenderer;
    private List<Edge> _meshEdges = new List<Edge>();

    private GameObject _isWalkablePoint;
    private MeshRenderer _isWalkablePointMeshRenderer;
    private Material _isWalkablePointMat;
    private Material _isNotWalkablePointMat;

    private PathFinder _pathFinder;

    [NonSerialized] public int id;

    [NonSerialized] public bool pointsReset = false;
    [NonSerialized] public bool initPoints = true;

    [NonSerialized] public List<ConnectionPoint> connectionPoints = new List<ConnectionPoint>();

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

    private void InitPrivateVars()
    {
        _mesh = GetComponent<MeshFilter>().sharedMesh;
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshEdges = MeshEdgeTools.GetEdges(_mesh.triangles).FindBoundary().SortEdges();

        _isWalkablePointMat = Resources.Load<Material>("Materials/WalkableBlockPointMat");
        _isNotWalkablePointMat = Resources.Load<Material>("Materials/NotWalkableBlockPointMat");

        FindPathFinder();
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
        Transform checkIsWalkablePoint = transform.Find("IsWalkablePoint");
        if (checkIsWalkablePoint)
        {
            if (pointsReset || initPoints)
            {
                DestroyImmediate(checkIsWalkablePoint.gameObject);
                CreateIsWalkablePoint();
            }
            else _isWalkablePoint = checkIsWalkablePoint.gameObject;
        } else CreateIsWalkablePoint();

        pointsReset = false;
        initPoints = true;
    }

    private void CreateConnectionPoints()
    {
        connectionPoints = new List<ConnectionPoint>();
        
        Vector3[] vertices = _mesh.vertices;
        List<Vector3> connectionPointsPositions = new List<Vector3>();

        foreach (Edge edge in _meshEdges)
        {
            Vector3 newConnectionPointPosition = (vertices[edge.v1] + vertices[edge.v2]) / 2;
            if (!connectionPointsPositions.Contains(newConnectionPointPosition))
                connectionPointsPositions.Add(newConnectionPointPosition);
        }

        for (int i = 0; i < connectionPointsPositions.Count; i++)
        {
            GameObject newConnectionPointObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            newConnectionPointObject.transform.SetParent(transform);

            newConnectionPointObject.name = "ConnectionPoint " + (i + 1);

            Transform newConnectionPointObjectTransform = newConnectionPointObject.transform;
            newConnectionPointObjectTransform.localScale = Vector3.one * ConnectionPoint.scale;
            newConnectionPointObjectTransform.localPosition = connectionPointsPositions[i];

            newConnectionPointObject.tag = "ConnectionPoint";
            newConnectionPointObject.layer = LayerMask.NameToLayer("Debug");

            SphereCollider conPointCollider = newConnectionPointObject.GetComponent<SphereCollider>();
            if (conPointCollider) DestroyImmediate(conPointCollider);

            ConnectionPoint newConnectionPointComponent = newConnectionPointObject.AddComponent<ConnectionPoint>();
            newConnectionPointComponent.parentBlock = this;
            
            connectionPoints.Add(newConnectionPointComponent);
        }
    }

    private void CreateIsWalkablePoint()
    {
        _isWalkablePoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        _isWalkablePointMeshRenderer = _isWalkablePoint.GetComponent<MeshRenderer>();
        _isWalkablePointMeshRenderer.sharedMaterial = _isWalkablePointMat;

        _isWalkablePoint.name = "IsWalkablePoint";
        _isWalkablePoint.transform.localScale = Vector3.one * isWalkablePointScale;

        Bounds meshRendererBounds = _meshRenderer.bounds;
        _isWalkablePoint.transform.position = meshRendererBounds.center + Vector3.up * meshRendererBounds.size.y / 2;
        _isWalkablePoint.layer = LayerMask.NameToLayer("Debug");

        Collider isWalkablePointCollider = _isWalkablePoint.GetComponent<Collider>();
        if (isWalkablePointCollider) DestroyImmediate(isWalkablePointCollider);

        _isWalkablePoint.transform.SetParent(transform);
    }
    
    private void BlockClickedTapped(int id)
    {
        if (this.id == id)
        {
            //todo logic for player movement on click   
            Debug.Log(name + " clicked");
        }
    }
}