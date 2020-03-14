using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

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
                _conPoint.SetNoConnections();
            }
        
            if (GUILayout.Button("Set Custom Camera Position"))
            {
                TurnAroundCamera camera = FindObjectOfType<TurnAroundCamera>();
                if (_conPoint && camera)
                {
                    _conPoint.customCameraPosition = camera.transform.position;
                }    
            }   
        }

        DrawDefaultInspector();
    }
}
#endif