#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TurnAroundCamera))]
public class TurnAroundCameraEditor : Editor
{
    private TurnAroundCamera _targetCamera;
    private Transform _targetCamTransform;
    private float _circleRadius;
    private Vector3 _circleCenterPos;

    private Tool _lastTool = Tool.None;

    private bool _circleLock = true;
    private bool _snapToCircle = false;

    private GUIStyle _labelStyle = new GUIStyle();

    private void OnEnable()
    {
        _lastTool = Tools.current;
        
        _targetCamera = (TurnAroundCamera) target;
        if (_targetCamera != null)
        {
            _targetCamera.CreateTargetToLookAt();
            _targetCamTransform = _targetCamera.transform;
            
            CalcCircle();
        }

        _labelStyle.alignment = TextAnchor.MiddleCenter;
    }

    private void CalcCircle()
    {
        if (_targetCamera.targetToLookAt && !_targetCamera.circleCalc)
        {
            Vector3 cameraPos = _targetCamTransform.position;
            Vector3 targetPos = _targetCamera.targetToLookAt.transform.position;
            _circleCenterPos = new Vector3(targetPos.x, cameraPos.y, targetPos.z);
            _circleRadius = Vector2.Distance(new Vector2(targetPos.x, targetPos.z),
                new Vector2(cameraPos.x, cameraPos.z));

            _targetCamera.snappingPoints = new Dictionary<Vector3, int>();

            for (int i = 0; i < 360; i += 45)
            {
                _targetCamera.snappingPoints.Add(new Vector3(
                    _circleCenterPos.x + (_circleRadius * Mathf.Cos(i * Mathf.Deg2Rad)),
                    _circleCenterPos.y,
                    _circleCenterPos.z + (_circleRadius * Mathf.Sin(i * Mathf.Deg2Rad))
                ), i);
            }

            _targetCamera.circleCalc = true;
        }

        Repaint();
    }

    private void OnSceneGUI()
    {
        if (_targetCamera != null && _targetCamera.targetToLookAt)
        {
            _targetCamTransform.LookAt(_targetCamera.targetToLookAt.transform);

            Handles.color = Color.white;
            Handles.DrawWireDisc(_circleCenterPos, Vector3.up, _circleRadius);
            
            Handles.color = Color.red;
            foreach (KeyValuePair<Vector3, int> snapPt in _targetCamera.snappingPoints)
            {
                Handles.SphereHandleCap(0, snapPt.Key, Quaternion.identity, 0.5f, EventType.Repaint);

                _labelStyle.normal.textColor = Color.green;
                Handles.Label(snapPt.Key + Vector3.up, snapPt.Value + " deg", _labelStyle);
            }
            
            Handles.color = Color.magenta;
            Handles.SphereHandleCap(
                0,
                _targetCamera.targetToLookAt.transform.position,
                Quaternion.identity,
                0.5f,
                EventType.Repaint
            );

            if (_snapToCircle)
            {
                if (Tools.current == Tool.Move) Tools.current = Tool.None;

                Vector3 newPos =
                    Handles.PositionHandle(_targetCamTransform.position, Quaternion.identity);

                if (_targetCamTransform.position != newPos)
                {
                    _targetCamTransform.position = newPos;
                    PutInCircle();
                }
            }
            else Tools.current = _lastTool;
        }
    }

    public override void OnInspectorGUI()
    {
        if (_targetCamera)
        {
            _circleLock = GUILayout.Toggle(_circleLock, "Lock the circle");

            if (_circleLock)
            {
                _snapToCircle = GUILayout.Toggle(_snapToCircle, "Snap To Circle On Moving");
                if (GUILayout.Button("Put In Circle")) PutInCircle();

                _targetCamera.selDeg =
                    EditorGUILayout.Popup("Snap to Degree", _targetCamera.selDeg, _targetCamera.degOptions);
                if (int.TryParse(_targetCamera.degOptions[_targetCamera.selDeg], out _targetCamera.degToSnap))
                {
                    if (GUILayout.Button("Snap To Degree")) SnapToDegree();
                }
            }
            else
            {
                if (_snapToCircle) _snapToCircle = false;
                
                if (GUILayout.Button("Set New Circle"))
                {
                    _targetCamera.circleCalc = false;
                    CalcCircle();
                }
            }
        }

        DrawDefaultInspector();
    }

    private void SnapToDegree()
    {
        if (_targetCamera.snappingPoints.Any())
            _targetCamTransform.position = _targetCamera.snappingPoints
                .First(snapPt => snapPt.Value == _targetCamera.degToSnap).Key;
    }

    private void PutInCircle()
    {
        Vector3 cameraPos = _targetCamTransform.position;
        cameraPos.y = _circleCenterPos.y;

        Vector3 offsetFromCenter = cameraPos - _circleCenterPos;

        _targetCamTransform.position = _circleCenterPos + (offsetFromCenter.normalized * _circleRadius);
    }

    private void OnDisable()
    {
        Tools.current = _lastTool;
    }
}
#endif