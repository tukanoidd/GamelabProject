/*using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Helpers;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(MapPartBuilder))]
public class MapPartBuilderEditor : Editor
{
    private MapPartBuilder _mapPartBuilder;
    private GameDefaultSettings _defaultGameSettings;

    private void OnEnable()
    {
        _mapPartBuilder = (MapPartBuilder) target;
        _defaultGameSettings = Resources.Load<GameDefaultSettings>("ScriptableObjects/DefaultGameSettings");

        if (_mapPartBuilder)
        {
            if (!_mapPartBuilder.blockPrefab)
            {
                _mapPartBuilder.blockPrefab = Resources.Load<GameObject>("Prefabs/BuildingBlockPrefab");
            }
        } 
    }

    public override void OnInspectorGUI()
    {
        if (_mapPartBuilder)
        {
            if (GUILayout.Button("Snap To 1x1 Grid"))
            {
                EditorHelpers.SnapToGrid(_mapPartBuilder.transform);
            }

            if (GUILayout.Button("Snap To Block Sized Grid"))
            {
                _mapPartBuilder.transform.position = EditorHelpers.SnapToBlockGrid(_mapPartBuilder.transform.position,
                    _defaultGameSettings.defaultBlockSize.ToVector());
            }

            if (GUILayout.Button("CreateStartingBlock"))
            {
                _mapPartBuilder.CreateStartingBlock();
            }
        }

        DrawDefaultInspector();
    }
}
#endif*/