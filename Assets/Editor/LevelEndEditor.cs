using System;
using System.Collections;
using System.Collections.Generic;
using Helpers;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelEnd))]
public class LevelEndEditor : Editor
{
    private LevelEnd _targetLevelEnd;
    private GameDefaultSettings _defaultGameSettings;

    private void OnEnable()
    {
        _defaultGameSettings = Resources.Load<GameDefaultSettings>("ScriptableObjects/DefaultGameSettings");
        _targetLevelEnd = (LevelEnd) target;
    }

    public override void OnInspectorGUI()
    {
        if (_targetLevelEnd)
        {
            if (GUILayout.Button("Snap To Block Grid XZ Plane") && _defaultGameSettings)
            {
                _targetLevelEnd.transform.position = EditorHelpers.SnapToBlockGridXZPlane(_targetLevelEnd.transform.position,
                    _defaultGameSettings.defaultBlockSize.ToVector());
            }    
        }

        DrawDefaultInspector();
    }
}
