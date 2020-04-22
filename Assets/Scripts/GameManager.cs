﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataTypes;
using UnityEngine;

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
        while(normalizedTime <= 1f)
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
            DisconnectPoints(blockConnections.Where(blockConnection => blockConnection.connectionPoints.Contains(connectionPoint) && blockConnection.isNear).ToArray());
            blockConnections.RemoveAll(blockConnection => blockConnection.connectionPoints.Contains(connectionPoint) && blockConnection.isNear);   
        }
        else
        {
            DisconnectPoints(blockConnections.Where(blockConnection => blockConnection.connectionPoints.Contains(connectionPoint)).ToArray());
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
                blockConnection.customCameraPositions = new SortedSet<Vector3>(); 
            }
        }
    }

    public void AddCameraPosition(ConnectionPoint[] connectionPoints)
    {
        //todo add logic
    }

    public void ConnectBlocks(ConnectionPoint connectionPoint1, ConnectionPoint connectionPoint2)
    {
     //todo add logic   
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (drawDebugConnectionLines)
        {
            Gizmos.color = Color.yellow;
            foreach (BlockConnection blockConnection in GameManager.current.blockConnections)
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
