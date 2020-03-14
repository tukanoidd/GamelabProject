using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

public class ConnectionPointsEditorMenu
{
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
        List<ConnectionPoint> connectionPoints = GetSelectedConnectionPoints();
        
        if (connectionPoints.Count == 2 &&
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

    [MenuItem("MapBuilder/Disconnect Selected Points")]
    private static void DisconnectSelectedPoints()
    {
        List<ConnectionPoint> connectionPoints = GetSelectedConnectionPoints();

        foreach (ConnectionPoint connectionPoint in connectionPoints)
        {
            if (connectionPoint.connection) connectionPoint.connection = null;
            connectionPoint.connection = null;
        }
    }
    
    private static List<ConnectionPoint> GetSelectedConnectionPoints()
    {
        return Selection.gameObjects
            .Where(obj => obj.GetComponent<ConnectionPoint>() != null)
            .Select(obj => obj.GetComponent<ConnectionPoint>()).ToList();
    }
}
#endif