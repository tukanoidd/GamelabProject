using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

public class ConnectionPointsEditor
{
    [MenuItem("MapBuilder/Connect TWO Selected Points")]
    private static void ConnectTwoSelectedPoints()
    {
        List<ConnectionPoint> connectionPoints = Selection.gameObjects
            .Where(obj => obj.GetComponent<ConnectionPoint>() != null)
            .Select(obj => obj.GetComponent<ConnectionPoint>()).ToList();
        
        if (connectionPoints.Count == 2 &&
            connectionPoints[0].transform.parent != connectionPoints[1].transform.parent)
        {
            connectionPoints[0].CustomPointConnect(connectionPoints[1]);
        }
    }
}
#endif