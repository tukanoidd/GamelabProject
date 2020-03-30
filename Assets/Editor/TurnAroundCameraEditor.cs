using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.TerrainAPI;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(TurnAroundCamera))]
public class TurnAroundCameraEditor : Editor
{
    private TurnAroundCamera _targetCamera;
    private float _circleRadius;
    private Vector3 _circleCenterPos;
    private Dictionary<Vector3, int> _snappingPoints;
    private int _selDeg = 0;
    private string[] _degOptions = new string[8] {"0", "45", "90", "135", "180", "225", "270", "315"};
    private int _degToSnap = 0;

    private bool _circleLock = true;

    private void OnEnable()
    {
        _targetCamera = target as TurnAroundCamera;
        if (_targetCamera)
        {
            _targetCamera.CreateTargetToLookAt();
            CalcCircle();
        }
    }

    private void CalcCircle()
    {
        if (_targetCamera.targetToLookAt)
        {
            Vector3 cameraPos = _targetCamera.transform.position;
            Vector3 targetPos = _targetCamera.targetToLookAt.transform.position;
            _circleCenterPos = new Vector3(targetPos.x, cameraPos.y, targetPos.z);
            _circleRadius = Vector2.Distance(new Vector2(targetPos.x, targetPos.z),
                new Vector2(cameraPos.x, cameraPos.z));

            _snappingPoints = new Dictionary<Vector3, int>();

            for (int i = 0; i < 360; i += 45)
            {
                _snappingPoints.Add(new Vector3(
                    _circleCenterPos.x + (_circleRadius * Mathf.Cos(i * Mathf.Deg2Rad)),
                    _circleCenterPos.y,
                    _circleCenterPos.z + (_circleRadius * Mathf.Sin(i * Mathf.Deg2Rad))
                ), i);
            }
        }
    }

    private void OnSceneGUI()
    {
        if (_targetCamera && _targetCamera.targetToLookAt)
        {
            Handles.color = Color.white;
            Handles.DrawWireDisc(_circleCenterPos, Vector3.up, _circleRadius);
            _targetCamera.transform.LookAt(_targetCamera.targetToLookAt.transform);

            Handles.color = Color.red;
            foreach (KeyValuePair<Vector3, int> snapPt in _snappingPoints)
            {
                Handles.SphereHandleCap(0, snapPt.Key, Quaternion.identity, 0.5f, EventType.Repaint);

                GUIStyle labelStyle = new GUIStyle();
                labelStyle.alignment = TextAnchor.MiddleCenter;
                labelStyle.normal.textColor = Color.green;
                Handles.Label(snapPt.Key + Vector3.up, snapPt.Value.ToString() + " deg", labelStyle);
            }
        }
    }

    public override void OnInspectorGUI()
    {
        if (_targetCamera)
        {
            _circleLock = GUILayout.Toggle(_circleLock, "Lock the circle");

            if (_circleLock)
            {
                if (GUILayout.Button("Put In Circle")) PutInCircle();

                _selDeg = EditorGUILayout.Popup("Snap to Degree", _selDeg, _degOptions);
                if (int.TryParse(_degOptions[_selDeg], out _degToSnap))
                {
                    if (GUILayout.Button("Snap To Degree")) SnapToDegree();
                }
            }
            else
            {
                if (GUILayout.Button("Set New Circle")) CalcCircle();
            }
        }

        base.OnInspectorGUI();
    }

    private void SnapToDegree()
    {
        if (_snappingPoints.Any())
            _targetCamera.transform.position = _snappingPoints.First(snapPt => snapPt.Value == _degToSnap).Key;
    }

    private void PutInCircle()
    {
        Vector3 cameraPos = _targetCamera.transform.position;
        cameraPos.y = _circleCenterPos.y;

        Vector3 offsetFromCenter = cameraPos - _circleCenterPos;

        _targetCamera.transform.position = _circleCenterPos + (offsetFromCenter.normalized * _circleRadius);
    }
}