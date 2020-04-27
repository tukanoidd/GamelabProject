using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataTypes;
using UnityEngine;

[ExecuteAlways]
public class GameManager : MonoBehaviour
{
    //---------Public and Private Visible In Inspector---------\\
#if UNITY_EDITOR
    [SerializeField] private bool drawDebugConnectionLines = true;
    public bool connectionPointsDebugDrawHasParentBlock = true;
    public TpTriggerDebugDrawMode tpTriggerDebugDrawMode = TpTriggerDebugDrawMode.DimensionLines;
#endif

    private static GameManager s_current = null;
    
    public static GameManager current
    {
        get
        {
            if (s_current == null) s_current = FindObjectOfType<GameManager>();
 
            return s_current;
        }

        set => s_current = value;
    }
    
    public DeviceType deviceType;
    
    public Player player;
    public TurnAroundCamera mainCamera;
    public PathFinder pathFinder;
    
    public bool gamePaused = false;

    public bool playerLockedMovement = false;
    public bool cameraLockedMovement = false;

    public List<BlockConnection> blockConnections = new List<BlockConnection>();
    //---------Public and Private Visible In Inspector---------\\

    //--------Private and Public Invisible In Inspector--------\\
    
    //--------Private and Public Invisible In Inspector--------\\

    //--------Static Behavior--------\\
    public static IEnumerator Countdown(float duration, Action funcToExecute)
    {
        float normalizedTime = 0;
        while (normalizedTime <= 1f)
        {
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }

        funcToExecute();
    }
    //--------Static Behavior--------\\

    private void Awake()
    { 
        InitVars();   
    }

    public void InitVars()
    {
        s_current = this;

        deviceType = SystemInfo.deviceType;

        player = FindObjectOfType<Player>();
        mainCamera = FindObjectOfType<TurnAroundCamera>();
        pathFinder = FindObjectOfType<PathFinder>();
    }

    public void RemoveConnectionsFromPoints(ConnectionPoint[] connectionPoints, bool nearby = false)
    {
        foreach (ConnectionPoint connectionPoint in connectionPoints)
        {
            RemoveConnectionsFromConnectionPoint(connectionPoint, nearby);
        }
    }

    public void RemoveConnectionsFromConnectionPoint(ConnectionPoint connectionPoint, bool nearby = false)
    {
        if (nearby)
        {
            DisconnectPoints(blockConnections.Where(blockConnection =>
                blockConnection.connectionPoints.Contains(connectionPoint) && blockConnection.isNear).ToArray());
            blockConnections.RemoveAll(blockConnection =>
                blockConnection.connectionPoints.Contains(connectionPoint) && blockConnection.isNear);
        }
        else
        {
            DisconnectPoints(blockConnections
                .Where(blockConnection => blockConnection.connectionPoints.Contains(connectionPoint)).ToArray());
            blockConnections.RemoveAll(blockConnection => blockConnection.connectionPoints.Contains(connectionPoint));
        }
    }

    public void DisconnectPoints(BlockConnection[] removeConnectionsList)
    {
        foreach (BlockConnection connection in removeConnectionsList)
        {
            foreach (ConnectionPoint conPoint in connection.connectionPoints)
            {
                DisconnectPoint(conPoint);
            }
        }
    }

    public void RemoveConnectionBetweenTwoPoints(ConnectionPoint conPoint1, ConnectionPoint conPoint2)
    {
        BlockConnection[] removeConnections = blockConnections.Where(blockConnection =>
            blockConnection.connectionPoints.Contains(conPoint1) &&
            blockConnection.connectionPoints.Contains(conPoint2)).ToArray();

        if (removeConnections.Length > 0)
        {
            DisconnectPoint(conPoint1);
            DisconnectPoint(conPoint2);

            foreach (BlockConnection blockConnection in removeConnections)
            {
                blockConnections.Remove(blockConnection);
            }
        }
    }

    public void DisconnectPoint(ConnectionPoint connectionPoint)
    {
        connectionPoint.isConnected = false;
        connectionPoint.isConnectedNearby = false;
    }

    public void RemoveAllCameraPositions(ConnectionPoint[] connectionPoints)
    {
        foreach (ConnectionPoint connectionPoint in connectionPoints)
        {
            BlockConnection[] neededBlockConnections = blockConnections
                .Where(blockConnection => blockConnection.connectionPoints.Contains(connectionPoint)).ToArray();

            foreach (BlockConnection blockConnection in neededBlockConnections)
            {
                mainCamera.customPositions.RemoveAll(customPosition =>
                    blockConnection.customCameraPositions.Contains(customPosition));;
                blockConnection.customCameraPositions = new List<Vector3>();
            }
        }
    }

    public void AddCameraPosition(ConnectionPoint[] connectionPoints)
    {
        Vector3 camPos = mainCamera.transform.position;

        BlockConnection[] connections = blockConnections.Where(connection =>
            connectionPoints.Any(connectionPoint => connection.connectionPoints.Contains(connectionPoint))).ToArray();
        foreach (BlockConnection connection in connections)
        {
            if (!mainCamera.customPositions.Contains(camPos)) mainCamera.customPositions.Add(camPos);
            if (!connection.customCameraPositions.Contains(camPos)) connection.customCameraPositions.Add(camPos);
        }
    }

    public void ConnectBlocks(ConnectionPoint connectionPoint1, ConnectionPoint connectionPoint2, bool isNear)
    {
        if (connectionPoint1.parentBlock == connectionPoint2.parentBlock) return;

        if (blockConnections.Any(blockConnection =>
            blockConnection.connectionPoints.Contains(connectionPoint1) &&
            blockConnection.connectionPoints.Contains(connectionPoint2))) return;

        GravitationalPlane sameGravitationalPlane = null;
        
        foreach (GravitationalPlane gravitationalPlane in connectionPoint1.posDirs.gravitationalPlanes)
        {
            foreach (GravitationalPlane otherGravitationalPlane in connectionPoint2.posDirs.gravitationalPlanes)
            {
                if (gravitationalPlane.IsEqual(otherGravitationalPlane))
                {
                    sameGravitationalPlane = gravitationalPlane;
                    goto FoundSameGravitationalPlane;
                }
            }
        }
        
        FoundSameGravitationalPlane:

        if (sameGravitationalPlane == null) return;
        
        List<Block> connectedBlocks = new List<Block>();
        List<ConnectionPoint> connectionPoints = new List<ConnectionPoint>();
            
        connectedBlocks.Add(connectionPoint1.parentBlock);
        connectedBlocks.Add(connectionPoint2.parentBlock);

        connectionPoints.Add(connectionPoint1);
        connectionPoints.Add(connectionPoint2);
        
        blockConnections.Add(new BlockConnection(
            connectedBlocks,
            sameGravitationalPlane,
            connectionPoints,
            new List<Vector3>(),
            isNear
        ));

        ConnectPoint(connectionPoint1, isNear);
        ConnectPoint(connectionPoint2, isNear);
    }

    private void ConnectPoint(ConnectionPoint connectionPoint, bool isNear)
    {
        connectionPoint.isConnected = true;
        connectionPoint.isConnectedNearby = isNear;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (drawDebugConnectionLines)
        {
            Gizmos.color = Color.yellow;
            foreach (BlockConnection blockConnection in blockConnections)
            {
                ConnectionPoint[] connectionPoints = blockConnection.connectionPoints.ToArray();
                if (connectionPoints.Length > 1)
                {
                    Gizmos.DrawLine(
                        connectionPoints[0].transform.position,
                        connectionPoints[1].transform.position
                    );
                }
            }
        }
    }
#endif
}