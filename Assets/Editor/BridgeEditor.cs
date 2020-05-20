#if UNITY_EDITOR
using System;
using System.Linq;
using DataTypes;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Bridge))]
public class BridgeEditor : Editor
{
    private Axis[] _availableAxis = {Axis.X, Axis.Z};
    private AxisDirection[] _availableAxisDirections = {AxisDirection.Negative, AxisDirection.Positive};

    private Bridge _targetBridge;
    
    private GUIStyle bridgeStartLabelStyle = new GUIStyle();

    private void OnEnable()
    {
        bridgeStartLabelStyle.alignment = TextAnchor.MiddleCenter;
        bridgeStartLabelStyle.normal.textColor = Color.white;
        
        _targetBridge = (Bridge) target;
    }

    public override void OnInspectorGUI()
    {
        if (!_targetBridge) return;

        GUILayout.Label("Bridge Orientation");
        using (new GUILayout.HorizontalScope())
        {
            GUILayout.Space(20);
            GUILayout.BeginVertical();
            GUILayout.Label("AxisPositionDirection");

            Axis selectedAxis = _targetBridge.bridgeOrientation.axis;

            _targetBridge.bridgeOrientation.axis = _availableAxis[
                EditorGUILayout.Popup(
                    "Axis",
                    Array.IndexOf(_availableAxis, selectedAxis),
                    _availableAxis.Select(axis => axis.ToString()).ToArray()
                )
            ];

            AxisDirection selectedAxisDirection = _targetBridge.bridgeOrientation.dir;

            _targetBridge.bridgeOrientation.dir = _availableAxisDirections[
                EditorGUILayout.Popup(
                    "Direction",
                    Array.IndexOf(_availableAxisDirections, selectedAxisDirection),
                    _availableAxisDirections.Select(dir => dir.ToString()).ToArray()
                )
            ];

            GUILayout.Space(20);
            GUILayout.EndVertical();
        }

        DrawDefaultInspector();

        if (GUI.changed)
        {
            _targetBridge.CreateBridge();
        }
    }

    private void OnSceneGUI()
    {
        if (!_targetBridge) return;
        
        if (GameManager.current.bridgesDebugDrawStartSphere)
        {
            if (!_targetBridge.bridgeFirstEndObjectWithBlock) return;

            Vector3 pos = _targetBridge.bridgeFirstEndObjectWithBlock.transform.position;
            Handles.color = Color.magenta;
            Handles.SphereHandleCap(0, pos, Quaternion.identity, 0.5f, EventType.Repaint);
            
            Handles.Label(pos + Vector3.up , "Bridge Start", bridgeStartLabelStyle);
        }
    }
}
#endif