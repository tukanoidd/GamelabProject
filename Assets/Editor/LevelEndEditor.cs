#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using Plane = DataTypes.Plane;

[CustomEditor(typeof(LevelEnd))]
public class LevelEndEditor : Editor
{
    private LevelEnd _targetLevelEnd;

    private void OnEnable()
    {
        _targetLevelEnd = (LevelEnd) target;
    }

    public override void OnInspectorGUI()
    {
        if (_targetLevelEnd)
        {
            if (GUILayout.Button("Snap To BLock Grid XZ Plane"))
            {
                _targetLevelEnd.transform.position =
                    HelperMethods.SnapToBlockGridPlane(_targetLevelEnd.transform.position, Plane.XZ);
            }
            
            if (GUILayout.Button("Snap To BLock Grid XY Plane"))
            {
                _targetLevelEnd.transform.position =
                    HelperMethods.SnapToBlockGridPlane(_targetLevelEnd.transform.position, Plane.XY);
            }
            
            if (GUILayout.Button("Snap To BLock Grid YZ Plane"))
            {
                _targetLevelEnd.transform.position =
                    HelperMethods.SnapToBlockGridPlane(_targetLevelEnd.transform.position, Plane.YZ);
            }
        }

        DrawDefaultInspector();
    }
}
#endif