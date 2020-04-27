#if UNITY_EDITOR
using System;
using System.Linq;
using DataTypes;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(BoxCollider))]
public class TpTriggerEditor : Editor
{
    private BoxCollider[] _tpTriggers;

    private void OnEnable()
    {
        _tpTriggers = FindObjectsOfType<BoxCollider>().Where(boxCollider => boxCollider.GetComponent<ConnectionPoint>() != null).ToArray();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }

    private void OnSceneGUI()
    {
        switch (GameManager.current.tpTriggerDebugDrawMode)
        {
            case TpTriggerDebugDrawMode.None: break;
            case TpTriggerDebugDrawMode.Outline: break;
            case TpTriggerDebugDrawMode.DimensionLines: break;
        }
    }
}
#endif