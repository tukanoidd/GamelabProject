using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TMPro;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshRenderer))]
public class ConnectionPoint : MonoBehaviour
{
    public enum PosDir
    {
        UpRight,
        UpLeft,
        UpForward,
        UpBackward,

        WIP
    }

    private MeshRenderer _meshRenderer;
    private Material _standardMaterial;
    private Material _connectedMaterial;
    private GameDefaultSettings _defaultGameSettings;
    private Player _player;

    public static float scale = 0.1f;

    public bool drawDebugConnectionLines = true;

    public bool hasConnection = true;
    public bool isConnectedNearby = true;
    public bool hasCustomConnection = false;

    public float nearbyRadius = 1;

    public ConnectionPoint connection;

    public List<Vector3> customCameraPositions = new List<Vector3>();
    public float customMaxCamOffset = 0.5f;

    [NonSerialized] public Block parentBlock;
    [NonSerialized] public PosDir posDir;
    [NonSerialized] public Vector3 offsetFromParentBlock;
    
    [NonSerialized] public bool canTeleport = false;
    [NonSerialized] public float tpWait = 1;

    private void Awake()
    {
        SetPrivateVars();
        GetPosDir();

        if (Application.isPlaying)
        {
            if (!connection) CheckForNearbyConnectionPoint();
        }
    }

    private void SetPrivateVars()
    {
        _defaultGameSettings = Resources.Load<GameDefaultSettings>("ScriptableObjects/DefaultGameSettings");
        if (_defaultGameSettings) nearbyRadius = BlockHelpers.Min(_defaultGameSettings.defaultBlockSize) / 2;

        _meshRenderer = GetComponent<MeshRenderer>();
        _standardMaterial = Resources.Load<Material>("Materials/ConnectionPointMat");
        _connectedMaterial = Resources.Load<Material>("Materials/ConnectedConnectionPoint");

        _meshRenderer.sharedMaterial = _standardMaterial;

        _player = FindObjectOfType<Player>();
    }

    private void GetPosDir()
    {
        if (transform.parent)
        {
            MeshRenderer parentRenderer = transform.parent.GetComponent<MeshRenderer>();
            if (parentRenderer)
            {
                Vector3 parentCenter = parentRenderer.bounds.center;
                Vector3 pointPos = transform.position;

                Vector3 offset = (pointPos - parentCenter).normalized;

                String up = offset.y >= 0 ? (offset.y == 0 ? "Center" : "Up") : "Down";
                String right = offset.x >= 0 ? (offset.x == 0 ? "Center" : "Right") : "Left";
                String forward = offset.z >= 0 ? (offset.z == 0 ? "Center" : "Forward") : "Backward";

                String strPosDir = up + right + forward;

                if (strPosDir == "UpCenterForward") posDir = PosDir.UpForward;
                else if (strPosDir == "UpCenterBackward") posDir = PosDir.UpBackward;
                else if (strPosDir == "UpRightCenter") posDir = PosDir.UpRight;
                else if (strPosDir == "UpLeftCenter") posDir = PosDir.UpLeft;
                else posDir = PosDir.WIP;

                if (posDir == PosDir.UpBackward || posDir == PosDir.UpForward || posDir == PosDir.UpLeft ||
                    posDir == PosDir.UpRight)
                {
                    offsetFromParentBlock = new Vector3(offset.x, 0, offset.z).normalized;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        CheckLoadedMaterials();
        CheckParentBlock();
#endif
        if (Application.isPlaying) CheckPlayerNearby();
    }

    void CheckPlayerNearby()
    {
        if (_player)
        {
            Vector3 playerPos = _player.transform.position;
            Vector3 conPointPos = transform.position;

            if (
                hasConnection && hasCustomConnection && connection
            )
            {
                if (CheckCentersClose(playerPos, conPointPos) &&
                    !_player.teleporting && canTeleport && _player.isMoving)
                {
                    canTeleport = false;
                    connection.canTeleport = false;
                    
                    _player.TeleportToConPoint(connection, playerPos - conPointPos);

                    StartCoroutine(TpWait());
                    connection.StartCoroutine(connection.TpWait());
                } else if (Vector3.Distance(playerPos, conPointPos) >= 0.5f && !_player.teleporting && tpWait >= 1f)
                {
                    canTeleport = true;
                }
            }
        }
    }
    
    private IEnumerator TpWait()
    {
        float duration = 1f;
        tpWait = 0;
        
        while(tpWait <= 1f)
        {
            tpWait += Time.deltaTime / duration;
            yield return null;
        }
    }

    private bool CheckCentersClose(Vector3 pos1, Vector3 pos2)
    {
        return XZDist(pos1, pos2) <= 0.5f;
    }

    private float XZDist(Vector3 pos1, Vector3 pos2)
    {
        return Vector2.Distance(
            new Vector2(pos1.x, pos1.z),
            new Vector2(pos2.x, pos2.z)
        );
    }

    private void CheckParentBlock()
    {
        if (!parentBlock && transform.parent) parentBlock = transform.parent.GetComponent<Block>();
    }

    private void CheckLoadedMaterials()
    {
        if (!_standardMaterial) _standardMaterial = Resources.Load<Material>("Materials/ConnectionPointMat");
        if (!_connectedMaterial) _connectedMaterial = Resources.Load<Material>("Materials/ConnectedConnectionPoint");

        UpdateDebugMaterials();
    }

    private float DistBtwPoints(ConnectionPoint pt1, ConnectionPoint pt2)
    {
        return Vector3.Distance(pt1.transform.position, pt2.transform.position);
    }

    private void OnDrawGizmos()
    {
        if (hasConnection && connection && drawDebugConnectionLines)
        {
#if UNITY_EDITOR
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, connection.transform.position);
#endif
        }
    }

    private void UpdateDebugMaterials()
    {
        if (connection) _meshRenderer.sharedMaterial = _connectedMaterial;
        else _meshRenderer.sharedMaterial = _standardMaterial;
    }

    public void CheckForNearbyConnectionPoint()
    {
        if (connection) return;

        List<ConnectionPoint> connectionPoints = FindObjectsOfType<ConnectionPoint>()
            .Where(conPoint => conPoint != this && transform.parent != conPoint.transform.parent).Where(conPoint =>
                DistBtwPoints(this, conPoint) <= nearbyRadius).OrderBy((conPoint) => DistBtwPoints(this, conPoint))
            .ToList();

        if (connectionPoints.Count > 0)
        {
            isConnectedNearby = true;
            hasCustomConnection = false;
            PointConnect(connectionPoints[0]);
            return;
        }

        SetNoConnections();
    }

    public void PointConnect(ConnectionPoint customConnectionPoint)
    {
        connection = customConnectionPoint;
        hasConnection = true;

        UpdateDebugMaterials();
    }

    public void SetNoConnections()
    {
        hasConnection = false;
        hasCustomConnection = false;
        isConnectedNearby = false;
        connection = null;

        UpdateDebugMaterials();
    }
}