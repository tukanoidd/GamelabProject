﻿using System;
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
    private Material _testBlockMat;
    private Material _isWalkablePointMat;
    private GameDefaultSettings _defaultGameSettings;

    private List<Edge> _blockEdges = new List<Edge>();

    [NonSerialized] public List<ConnectionPoint> connectionPoints = new List<ConnectionPoint>();

    public bool isTesting = true;

    public bool isWalkable = true;
    public float isWalkablePointScale = 0.25f;
    private GameObject isWalkablePoint;

    private void Awake()
    {
        InitPrivateVars();

        if (isTesting) _meshRenderer.sharedMaterial = _testBlockMat;

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

        _testBlockMat = Resources.Load<Material>("Materials/TestBlockMat");
        _isWalkablePointMat = Resources.Load<Material>("Materials/WalkableBlockPointMat");

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
            else isWalkablePoint = checkWalkablePoint.gameObject;
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

                newConnectionPoint.name = "ConnectionPoint" + (i + 1);
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


                if (connectionPointComponent) connectionPoints.Add(connectionPointComponent);
            }
        }
    }

    void CreateIsWalkablePoint()
    {
        isWalkablePoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        if (isWalkablePoint)
        {
            MeshRenderer isWalkablePointMeshRenderer = isWalkablePoint.GetComponent<MeshRenderer>();
            isWalkablePointMeshRenderer.sharedMaterial = _isWalkablePointMat;

            isWalkablePoint.name = "IsWalkablePoint";
            isWalkablePoint.transform.localScale = Vector3.one * isWalkablePointScale;

            Bounds meshRendererBounds = _meshRenderer.bounds;
            isWalkablePoint.transform.position = meshRendererBounds.center + Vector3.up * meshRendererBounds.size.y / 2;
            isWalkablePoint.layer = LayerMask.NameToLayer("Debug");

            isWalkablePoint.transform.SetParent(transform);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
}