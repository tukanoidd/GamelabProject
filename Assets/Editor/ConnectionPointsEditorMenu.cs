﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

public class ConnectionPointsEditorMenu
{
    [MenuItem("MapBuilder/All Points Destroy Colliders")]
    private static void AllPointsUpdateColliderRadius()
    {
        ConnectionPoint[] conPoints = GameObject.FindObjectsOfType<ConnectionPoint>();
        foreach (ConnectionPoint conPoint in conPoints)
        {
            Collider collider = conPoint.GetComponent<Collider>();
            if (collider)
            {
                if (Application.isPlaying) GameObject.Destroy(collider);
                else if (Application.isEditor)  GameObject.DestroyImmediate(collider);
            }
        }
    }
    
    [MenuItem("MapBuilder/All Points Find Nearby Connection")]
    private static void AllPointsFindNearbyConnection()
    {
        ConnectionPoint[] conPoints = GameObject.FindObjectsOfType<ConnectionPoint>();
        foreach (ConnectionPoint conPoint in conPoints)
        {
            conPoint.CheckForNearbyConnectionPoint();
        }
    }
    
    [MenuItem("MapBuilder/Connect TWO Selected Points")]
    private static void ConnectTwoSelectedPoints()
    {
        ConnectionPoint[] connectionPoints = GetSelectedConnectionPoints();
        
        if (connectionPoints.Length == 2 &&
            connectionPoints[0].transform.parent != connectionPoints[1].transform.parent)
        {
            connectionPoints[0].PointConnect(connectionPoints[1]);
            connectionPoints[1].PointConnect(connectionPoints[0]);

            UpdateCustomConnection(connectionPoints[0]);
            UpdateCustomConnection(connectionPoints[1]);
        }
    }

    private static void UpdateCustomConnection(ConnectionPoint conPoint)
    {
        conPoint.hasConnection = true;
        conPoint.isConnectedNearby = false;
        conPoint.hasCustomConnection = true;
    }

    [MenuItem("MapBuilder/Disconnect/All Points")]
    private static void DisconnectAllPoints()
    {
        ConnectionPoint[] conPoints = GameObject.FindObjectsOfType<ConnectionPoint>();
        DisconnectPoints(conPoints);
    }
    
    [MenuItem("MapBuilder/Disconnect/All Points Except With Custom Connection")]
    private static void DisconnectAllPointsExceptCustomConnection()
    {
        ConnectionPoint[] conPoints = GameObject.FindObjectsOfType<ConnectionPoint>().Where(conPoint => !conPoint.hasCustomConnection).ToArray();
        DisconnectPoints(conPoints);
    }

    private static void DisconnectPoints(ConnectionPoint[] conPoints)
    {
        foreach (ConnectionPoint conPoint in conPoints)
        {
            if (conPoint.connection) conPoint.connection.SetNoConnections();
            conPoint.SetNoConnections();
        }
    }

    [MenuItem("MapBuilder/Disconnect/Selected Points")]
    private static void DisconnectSelectedPoints()
    {
        ConnectionPoint[] connectionPoints = GetSelectedConnectionPoints();
        DisconnectPoints(connectionPoints);
    }
    
    private static ConnectionPoint[] GetSelectedConnectionPoints()
    {
        return Selection.gameObjects
            .Where(obj => obj.GetComponent<ConnectionPoint>() != null)
            .Select(obj => obj.GetComponent<ConnectionPoint>()).ToArray();
    }
}
#endif