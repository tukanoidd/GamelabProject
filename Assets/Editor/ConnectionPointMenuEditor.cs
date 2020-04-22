#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using DataTypes;
using UnityEditor;
using UnityEngine;

public static class ConnectionPointMenuEditor
{
    [MenuItem("MapBuilder/Connection Points/All Points/Camera Positions/Remove")]
    private static void AllConnectionPointsRemoveCameraPositions()
    {
        GameManager.current.RemoveAllCameraPositions(GetAllConnectionPoints());
    }
    
    [MenuItem("MapBuilder/Connection Points/Selected Points/Camera Positions/Add")]
    private static void SelectedConnectionPointsAddCameraPositions()
    {
        GameManager.current.AddCameraPosition(GetSelectedConnectionPoints());
    }
    
    [MenuItem("MapBuilder/Connection Points/Selected Points/Camera Positions/Remove")]
    private static void SelectedConnectionPointsRemoveCameraPositions()
    {
        GameManager.current.RemoveAllCameraPositions(GetSelectedConnectionPoints());
    }

    [MenuItem("MapBuilder/Connection Points/All Points/Connect/To Nearby")]
    private static void AllConnectionPointsConnectToNearby()
    {
        ConnectionPoint[] connectionPoints = GetAllConnectionPoints();
        foreach (ConnectionPoint connectionPoint in connectionPoints)
        {
            connectionPoint.FindNearbyConnections();
        }
    }

    [MenuItem("MapBuilder/Connection Points/Selected Points/Connect TWO")]
    private static void ConnectTwoSelectedPoints()
    {
        ConnectionPoint[] connectionPoints = GetSelectedConnectionPoints();
        if (connectionPoints.Length > 1)
        {
            GameManager.current.ConnectBlocks(connectionPoints[0], connectionPoints[1]);
        }
    }

    [MenuItem("MapBuilder/Connection Points/All Points/Disconnect/All")]
    private static void DisconnectAllConnectionPoints()
    {
        GameManager.current.RemoveConnectionsFromPoints(GetAllConnectionPoints());
    }

    [MenuItem("MapBuilder/Connection Points/All Points/Disconnect/Only Nearby Connected")]
    private static void DisconnectAllConnectionPointsOnlyNearbyConnected()
    {
        GameManager.current.RemoveConnectionsFromPoints(GetAllConnectionPoints(), true);
    }

    [MenuItem("MapBuilder/Connection Points/Selected Points/Disconnect/All")]
    private static void DisconnectSelectedPoints()
    {
        GameManager.current.RemoveConnectionsFromPoints(GetSelectedConnectionPoints());
    }

    [MenuItem("MapBuilder/Connection Points/Selected Points/Disconnect/TWO")]
    private static void RemoveConnectionBetweenTwoPoints()
    {
        ConnectionPoint[] connectionPoints = GetSelectedConnectionPoints();
        if (connectionPoints.Length > 1)
        {
            GameManager.current.RemoveConnectionBetweenTwoPoints(connectionPoints[0], connectionPoints[1]);
        }
    }

    private static ConnectionPoint[] GetAllConnectionPoints()
    {
        return GameObject.FindObjectsOfType<ConnectionPoint>();
    }

    private static ConnectionPoint[] GetSelectedConnectionPoints()
    {
        return Selection.gameObjects.Where(obj => obj.GetComponent<ConnectionPoint>() != null)
            .Select(obj => obj.GetComponent<ConnectionPoint>()).ToArray();
    }
}
#endif