#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapBuilder))]
public class MapBuilderEditor : Editor
{
    private MapBuilder _mapBuilder;

    private void OnEnable()
    {
        _mapBuilder = (MapBuilder) target;
    }

    public override void OnInspectorGUI()
    {
        if (!_mapBuilder) return;

        if (GUILayout.Button("Center Position"))
        {
            HelperMethods.CenterPosition(_mapBuilder.transform);
        }

        if (GUILayout.Button("Add Map Part Builder"))
        {
            Selection.activeGameObject = _mapBuilder.AddMapPartBuilder();
        }

        DrawDefaultInspector();
    }
}
#endif