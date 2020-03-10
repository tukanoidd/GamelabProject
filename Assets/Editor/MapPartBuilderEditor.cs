using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Helpers;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(MapPartBuilder))]
public class MapPartBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapPartBuilder mapPartBuilder = (MapPartBuilder) target;

        if (GUILayout.Button("Snap To Grid"))
        {
            EditorHelpers.SnapToGrid(mapPartBuilder.transform);
        }

        if (GUILayout.Button("CreateStartingBlock"))
        {
            mapPartBuilder.CreateStartingBlock();
        }

        DrawDefaultInspector();
    }
}
#endif