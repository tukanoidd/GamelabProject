using System;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(ConnectionPoint))]
public class ConnectionPointEditor : Editor
{
    private ConnectionPoint _connectionPoint;
    
    private void OnEnable()
    {
        _connectionPoint = (ConnectionPoint) target;
    }

    public override void OnInspectorGUI()
    {
        if (_connectionPoint)
        {
            if (GUILayout.Button("Remove Connections"))
            {
                GameManager.current.RemoveConnectionsFromConnectionPoint(_connectionPoint);
            }
        }

        DrawDefaultInspector();
    }
}