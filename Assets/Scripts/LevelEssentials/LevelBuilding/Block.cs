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

    [NonSerialized] public bool meshReset = false;

    [NonSerialized] public List<ConnectionPoint> connectionPoints = new List<ConnectionPoint>();

    [NonSerialized] public MapBlockData mapData;
    //--------Private and Public Invisible In Inspector--------\\

    //--------Static Behavior--------\\
    static void ScaleToSetSize(Block block, Vector3 blockSize)
    {
        block.transform.localScale = new Vector3(
            blockSize.x / Block.size.x,
            blockSize.y / Block.size.y,
            blockSize.z / Block.size.z
        );
    }
    //--------Static Behavior--------\\

    private void Awake()
    {
#if UNITY_EDITOR
        InitPrivateVars();
        ScaleToSetSize(this, _meshRenderer.bounds.size);
        CreatePoints();
#endif
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

    public void CreatePoints()
    {
        #if UNITY_EDITOR
        InitPrivateVars();
        #endif
        
        // Creating new connection points
        ConnectionPoint[] checkConnectionPoints = GetComponentsInChildren<ConnectionPoint>();
        if (checkConnectionPoints.Length > 0)
        {
            if (meshReset) ObjectsHelpers.DestroyObjects(checkConnectionPoints);
            else connectionPoints = checkConnectionPoints.ToList();
        }
        else CreateConnectionPoints();

        // Creating new isWalkablePoint
        Transform checkIsWalkablePoint = transform.Find("IsWalkablePoint");
        if (checkIsWalkablePoint)
        {
            if (meshReset) DestroyImmediate(checkIsWalkablePoint.gameObject);
            else _isWalkablePoint = checkIsWalkablePoint.gameObject;
        } else CreateIsWalkablePoint();
    }

    private void CreateConnectionPoints()
    {
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

            //todo add box collider on the edge of the block

            ConnectionPoint newConnectionPointComponent = newConnectionPointObject.AddComponent<ConnectionPoint>();
            newConnectionPointComponent.parentBlock = this;
            //todo find posDirs of connectionPoint
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
    
    //todo onClick/onTap event
}