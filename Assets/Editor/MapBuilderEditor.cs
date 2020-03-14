using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Helpers;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(MapBuilder))]
public class MapBuilderEditor : Editor
{
    private MapBuilder _mapBuilder;
    private bool _showMap = false;
    
    private void OnEnable()
    {
        _mapBuilder = (MapBuilder) target;
    }

    public override void OnInspectorGUI()
    {
        if (!_mapBuilder) return;
        
        if (GUILayout.Button("Center Position"))
        {
            EditorHelpers.CenterPosition(_mapBuilder.transform);
        }
        
        if (GUILayout.Button("Add Map Part Builder"))
        {
            Selection.activeGameObject = AddMapPartBuilder(_mapBuilder).gameObject;
        }

        if (GUILayout.Button("Get Needed Data"))
        {
            _mapBuilder.GetNeededData();
        }

        if (_mapBuilder.dataExists)
        {
            if (GUILayout.Button("Generate Map"))
            {
                _mapBuilder.GenerateMap();
            }   
        }

        bool newVal = GUILayout.Toggle(_showMap, "Show Map");
        if (newVal != _showMap)
        {
            _showMap = newVal;
            if (_showMap) _mapBuilder.ShowMap();
            else _mapBuilder.HideMap();
        }
    }

    private MapPartBuilder AddMapPartBuilder(MapBuilder mapBuilder)
    {
        GameObject newMapPartBuilder = new GameObject("MapPartBuilder");
        MapPartBuilder newMapPartBuilderComponent = newMapPartBuilder.AddComponent<MapPartBuilder>();
        newMapPartBuilder.transform.parent = mapBuilder.transform;

        return newMapPartBuilderComponent;
    }
}
#endif