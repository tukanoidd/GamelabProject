using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Helpers;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Block : MonoBehaviour
{
    private Mesh _mesh;
    private MeshRenderer _meshRenderer;
    private Material _isWalkablePointMat;
    private Material _isNotWalkablePointMat;
    private GameDefaultSettings _defaultGameSettings;

    private List<Edge> _blockEdges = new List<Edge>();

    [NonSerialized] public List<ConnectionPoint> connectionPoints = new List<ConnectionPoint>();
    [NonSerialized] public MapBlockData thisBlocksMapData;

    public bool isTesting = true;

    public bool isWalkable = true;
    
    private GameObject _isWalkablePoint;
    private MeshRenderer _isWalkablePointMeshRenderer;
    private float _isWalkablePointScale = 0.25f;

    private void Awake()
    {
        InitPrivateVars();

        Vector3 meshRendererblockSize = _meshRenderer.bounds.size;
        transform.localScale = BlockHelpers.ScaleToDefaultSize(new BlockSize(Mathf.RoundToInt(meshRendererblockSize.x),
            Mathf.RoundToInt(meshRendererblockSize.y),
            Mathf.RoundToInt(meshRendererblockSize.z)), _defaultGameSettings.defaultBlockSize);

        CreatePoints();
    }

    void InitPrivateVars()
    {
        _mesh = GetComponent<MeshFilter>().sharedMesh;
        _meshRenderer = GetComponent<MeshRenderer>();
        
        _isWalkablePointMat = Resources.Load<Material>("Materials/WalkableBlockPointMat");
        _isNotWalkablePointMat = Resources.Load<Material>("Materials/NotWalkableBlockPointMat");

        _blockEdges = EdgeHelpers.GetEdges(_mesh.triangles).FindBoundary().SortEdges();

        _defaultGameSettings = Resources.Load<GameDefaultSettings>("ScriptableObjects/DefaultGameSettings");
    }

    void CreatePoints()
    {
        ConnectionPoint[] checkConnectionPoints = GetComponentsInChildren<ConnectionPoint>();
        if (checkConnectionPoints.Length == 0) CreateConnectionPoints();
        else connectionPoints = checkConnectionPoints.ToList();

        if (isWalkable && _meshRenderer)
        {
            Transform checkWalkablePoint = transform.Find("IsWalkablePoint");
            if (!checkWalkablePoint) CreateIsWalkablePoint();
            else _isWalkablePoint = checkWalkablePoint.gameObject;
        }
    }

    void CreateConnectionPoints()
    {
        Vector3[] vertices = _mesh.vertices;
        HashSet<Vector3> connectionPointsPositions = new HashSet<Vector3>();

        for (int i = 0; i < _blockEdges.Count; i++)
        {
            Edge edge = _blockEdges[i];
            connectionPointsPositions.Add((vertices[edge.v1] + vertices[edge.v2]) / 2);
        }

        List<Vector3> connectionPointsPositionsList = connectionPointsPositions.ToList();

        for (int i = 0; i < connectionPointsPositionsList.Count; i++)
        {
            GameObject newConnectionPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            if (newConnectionPoint)
            {
                newConnectionPoint.transform.SetParent(transform);

                newConnectionPoint.name = "ConnectionPoint " + (i + 1);
                newConnectionPoint.transform.localScale = Vector3.one * ConnectionPoint.scale;
                newConnectionPoint.transform.localPosition = connectionPointsPositionsList[i];
                newConnectionPoint.tag = "ConnectionPoint";
                newConnectionPoint.layer = LayerMask.NameToLayer("Debug");

                Collider conPointCollider = newConnectionPoint.GetComponent<Collider>();
                if (conPointCollider)
                {
                    conPointCollider.isTrigger = true;
                }

                ConnectionPoint connectionPointComponent = newConnectionPoint.AddComponent<ConnectionPoint>();
                
                if (connectionPointComponent)
                {
                    connectionPointComponent.parentBlock = this;
                    connectionPoints.Add(connectionPointComponent);
                }
            }
        }
    }

    void CreateIsWalkablePoint()
    {
        _isWalkablePoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        if (_isWalkablePoint)
        {
            _isWalkablePointMeshRenderer = _isWalkablePoint.GetComponent<MeshRenderer>();
            _isWalkablePointMeshRenderer.sharedMaterial = _isWalkablePointMat;

            _isWalkablePoint.name = "IsWalkablePoint";
            _isWalkablePoint.transform.localScale = Vector3.one * _isWalkablePointScale;

            Bounds meshRendererBounds = _meshRenderer.bounds;
            _isWalkablePoint.transform.position = meshRendererBounds.center + Vector3.up * meshRendererBounds.size.y / 2;
            _isWalkablePoint.layer = LayerMask.NameToLayer("Debug");

            Collider isWalkablePointCollider = _isWalkablePoint.GetComponent<Collider>();
            if (isWalkablePointCollider) Destroy(isWalkablePointCollider);

            _isWalkablePoint.transform.SetParent(transform);
        }
    }

    private void Update()
    {
        if (connectionPoints.Count == 0) UpdateConnectionPoints();

        if (_isWalkablePoint)
        {
            if (!_isWalkablePointMeshRenderer) _isWalkablePointMeshRenderer = _isWalkablePoint.GetComponent<MeshRenderer>();

            if (_isWalkablePointMeshRenderer)
            {
                if (!isWalkable && _isWalkablePointMeshRenderer.sharedMaterial == _isWalkablePointMat)
                    _isWalkablePointMeshRenderer.sharedMaterial = _isNotWalkablePointMat;
                if (isWalkable && _isWalkablePointMeshRenderer.sharedMaterial == _isNotWalkablePointMat)
                    _isWalkablePointMeshRenderer.sharedMaterial = _isWalkablePointMat;
            }   
        }
    }

    public void UpdateConnectionPoints()
    {
        connectionPoints = GetComponentsInChildren<ConnectionPoint>().ToList();
        foreach (ConnectionPoint conPoint in connectionPoints)
        {
            conPoint.parentBlock = this;
        }
        if (connectionPoints.Count == 0) CreateConnectionPoints();
    }

    private void OnDrawGizmos()
    {
        #if UNITY_EDITOR
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 3);
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * 3);
        #endif
    }
}