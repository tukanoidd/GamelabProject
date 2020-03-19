using System;
using System.Collections;
using System.Collections.Generic;
using Helpers;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Player))]
public class PlayerEditor : Editor
{
    private Player _targetPlayer;
    private GameDefaultSettings _defaultGameSettings;

    private void OnEnable()
    {
        _defaultGameSettings = Resources.Load<GameDefaultSettings>("ScriptableObjects/DefaultGameSettings");
        _targetPlayer = (Player) target;
    }

    public override void OnInspectorGUI()
    {
        if (target)
        {
            if (GUILayout.Button("Snap To Block Grid XZ Plane"))
            {
                _targetPlayer.transform.position = EditorHelpers.SnapToBlockGridXZPlane(_targetPlayer.transform.position,
                    _defaultGameSettings.defaultBlockSize.ToVector());
            }    
        }
        
        DrawDefaultInspector();
    }
}
