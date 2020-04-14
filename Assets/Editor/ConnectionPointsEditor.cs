/*using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(ConnectionPoint))]
public class ConnectionPointsEditor : Editor
{
    private ConnectionPoint _conPoint;

    private void OnEnable()
    {
        _conPoint = (ConnectionPoint) target;
    }

    public override void OnInspectorGUI()
    {
        if (_conPoint)
        {
            if (GUILayout.Button("Set No Connections"))
            {
                ConnectionPoint[] conPoints = Selection.gameObjects
                    .Where(obj => obj.GetComponent<ConnectionPoint>())
                    .Select(obj => obj.GetComponent<ConnectionPoint>()).ToArray();
                    
                foreach (ConnectionPoint conPoint in conPoints)
                {
                    conPoint.SetNoConnections();   
                }
            }

            if (GUILayout.Button("Add Custom Camera Position"))
            {
                TurnAroundCamera camera = FindObjectOfType<TurnAroundCamera>();
                if (camera)
                {
                    ConnectionPoint[] conPoints = Selection.gameObjects
                        .Where(obj => obj.GetComponent<ConnectionPoint>())
                        .Select(obj => obj.GetComponent<ConnectionPoint>()).ToArray();
                    
                    foreach (ConnectionPoint conPoint in conPoints)
                    {
                        if (!conPoint.customCameraPositions.Contains(camera.transform.position))
                        {
                            conPoint.customCameraPositions.Add(camera.transform.position);   
                        }   
                    }
                }
            }
        }

        DrawDefaultInspector();
    }
}
#endif*/