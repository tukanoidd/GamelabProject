using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Experimental.TerrainAPI;
using UnityEngine;

[CustomEditor(typeof(TurnAroundCamera))]
public class TurnAroundCameraEditor : Editor
{
    private TurnAroundCamera _targetCamera;
    private float _circleRadius;
    private Vector3 _circleCenterPos;

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
            _circleRadius = Vector2.Distance(new Vector2(targetPos.x, targetPos.z), new Vector2(cameraPos.x, cameraPos.z));
        }
    }

    private void OnSceneGUI()
    {
        if (_targetCamera && _targetCamera.targetToLookAt)
        {
            Handles.DrawWireDisc(_circleCenterPos, Vector3.up, _circleRadius);
            _targetCamera.transform.LookAt(_targetCamera.targetToLookAt.transform);
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
            }
            else
            {
                if (GUILayout.Button("Set New Circle")) CalcCircle();;
            }   
        }
        
        base.OnInspectorGUI();
    }

    private void PutInCircle()
    {
        Vector3 cameraPos = _targetCamera.transform.position;
        cameraPos.y = _circleCenterPos.y;
        
        Vector3 offsetFromCenter = cameraPos - _circleCenterPos;

        _targetCamera.transform.position = _circleCenterPos + (offsetFromCenter.normalized * _circleRadius);

    }
}
