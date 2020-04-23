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
#endif

    public static GameManager current;
    //---------Public and Private Visible In Inspector---------\\

    //--------Private and Public Invisible In Inspector--------\\
    [NonSerialized] public DeviceType deviceType;
    [NonSerialized] public bool gamePaused = false;

    [NonSerialized] public Player player;
    [NonSerialized] public TurnAroundCamera mainCamera;
    [NonSerialized] public PathFinder pathFinder;

    [NonSerialized] public bool playerLockedMovement = false;
    [NonSerialized] public bool cameraLockedMovement = false;

    [NonSerialized] public List<BlockConnection> blockConnections = new List<BlockConnection>();
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
        current = this;

        deviceType = SystemInfo.deviceType;

        player = FindObjectOfType<Player>();
        mainCamera = FindObjectOfType<TurnAroundCamera>();
        pathFinder = FindObjectOfType<PathFinder>();
    }

    public void RemoveConnectionsFromPoints(ConnectionPoint[] connectionPoints, bool nearby = false)
    {
        foreach (ConnectionPoint connectionPoint in connectionPoints)
        {
            RemoveConnectionsFromConnectionPoint(connectionPoint);
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

        if (removeConnections.Length > 1)
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
                mainCamera.customPositions.RemoveWhere(customPosition =>
                    blockConnection.customCameraPositions.Contains(customPosition));
                blockConnection.customCameraPositions = new SortedSet<Vector3>();
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
            mainCamera.customPositions.Add(camPos);
            connection.customCameraPositions.Add(camPos);
        }
    }

    public void ConnectBlocks(ConnectionPoint connectionPoint1, ConnectionPoint connectionPoint2, bool isNear)
    {
        if (connectionPoint1.parentBlock == connectionPoint2.parentBlock) return;

        GravitationalPlane sameGravitationalPlane = connectionPoint1.posDirs.Key.FirstOrDefault(gravitationalPlane =>
            connectionPoint2.posDirs.Key.Contains(gravitationalPlane));

        if (sameGravitationalPlane == null) return;
        
        if (!blockConnections.Any(blockConnection =>
            blockConnection.connectionPoints.Contains(connectionPoint1) &&
            blockConnection.connectionPoints.Contains(connectionPoint2)))
        {
            HashSet<Block> connectedBlocks = new HashSet<Block>();
            HashSet<ConnectionPoint> connectionPoints = new HashSet<ConnectionPoint>();
            
            connectedBlocks.Add(connectionPoint1.parentBlock);
            connectedBlocks.Add(connectionPoint2.parentBlock);

            connectionPoints.Add(connectionPoint1);
            connectionPoints.Add(connectionPoint2);

            blockConnections.Add(new BlockConnection(
               connectedBlocks,
               sameGravitationalPlane,
               connectionPoints,
               new SortedSet<Vector3>(),
               isNear
            ));
        }
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