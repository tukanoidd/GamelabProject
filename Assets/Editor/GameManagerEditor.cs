#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    private GameManager _targetGameManager;

    private void OnEnable()
    {
        _targetGameManager = (GameManager) target;
    }

    public override void OnInspectorGUI()
    {
        if (_targetGameManager)
        {
            if (GUILayout.Button("Set Current Game Manager"))
            {
                GameManager.current = _targetGameManager;
            }   
        }
        
        DrawDefaultInspector();
    }
}
#endif