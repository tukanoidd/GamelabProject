#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(ConnectionPoint))]
public class ConnectionPointEditor : Editor
{
    private ConnectionPoint[] _connectionPoints;
    
    private void OnEnable()
    {
        _connectionPoints = Selection.gameObjects.Where(obj => obj.GetComponent<ConnectionPoint>() != null)
            .Select(obj => obj.GetComponent<ConnectionPoint>()).ToArray();
    }

    public override void OnInspectorGUI()
    {
        if (_connectionPoints.Length > 0)
        {
            if (GUILayout.Button("Remove Connections"))
            {
                foreach (ConnectionPoint connectionPoint in _connectionPoints)
                {
                    GameManager.current.RemoveConnectionsFromConnectionPoint(connectionPoint);   
                }
            }

            if (_connectionPoints.Length == 2)
            {
                if (GUILayout.Button("Add Custom Camera Position To This Connection"))
                {
                    GameManager.current.AddCameraPosition(_connectionPoints);
                }   
            }
        }

        DrawDefaultInspector();
    }
}
#endif