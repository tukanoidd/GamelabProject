﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

public class ConnectionPointsEditor
{
    private static List<ConnectionPoint> GetSelectedConnectionPoints()
    {
        return Selection.gameObjects
            .Where(obj => obj.GetComponent<ConnectionPoint>() != null)
            .Select(obj => obj.GetComponent<ConnectionPoint>()).ToList();
    }
    
    [MenuItem("MapBuilder/Connect TWO Selected Points")]
    private static void ConnectTwoSelectedPoints()
    {
        List<ConnectionPoint> connectionPoints = GetSelectedConnectionPoints();
        
        if (connectionPoints.Count == 2 &&
            connectionPoints[0].transform.parent != connectionPoints[1].transform.parent)
        {
            connectionPoints[0].PointConnect(connectionPoints[1]);
        }
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
}
#endif