using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Helpers;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(MapBuilder))]
public class MapBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapBuilder mapBuilder = (MapBuilder) target;

        if (GUILayout.Button("Center Position"))
        {
            EditorHelpers.CenterPosition(mapBuilder.transform);
        }
        
        if (GUILayout.Button("Add Map Part Builder"))
        {
            mapBuilder.AddMapPartBuilder();
        }
        
    }
}
#endif