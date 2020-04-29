#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using DataTypes;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(Block))]
public class BlockEditor : Editor
{
    private Block _targetBlock;
    private Transform _blockTransform;
    
    private Tool _lastTool = Tool.None;

    private bool _expandIsWalkablePointsDictionary = false;

    private void OnEnable()
    {
        _lastTool = Tools.current;

        GameObject selectedGameObject = Selection.activeGameObject;
        if (selectedGameObject)
        {
            _targetBlock = selectedGameObject.GetComponent<Block>();
            if (_targetBlock) _blockTransform = _targetBlock.transform;
        }
    }

    public override void OnInspectorGUI()
    {
        if (_targetBlock)
        {
            if (_targetBlock.isWalkablePoints.Count > 0)
            {
                _expandIsWalkablePointsDictionary =
                    EditorGUILayout.Foldout(_expandIsWalkablePointsDictionary, "Is Walkable Points");
                if (_expandIsWalkablePointsDictionary)
                {
                    foreach (KeyValuePair<GravitationalPlane, IsWalkablePoint> point in _targetBlock.isWalkablePoints)
                    {
                        GUILayout.BeginHorizontal();
                        
                        GUILayout.Label("GravitationalPlane");
                        GUILayout.Label("IsWalkablePoint");
                        
                        GUILayout.EndHorizontal();
                    
                        GUILayout.BeginHorizontal();
                        GUILayout.BeginVertical();
                        GUILayout.Label(point.Key.plane.ToString());
                        GUILayout.Label(point.Key.planeSide.ToString());
                        GUILayout.EndVertical();
                    
                        GUILayout.BeginVertical();
                        GUILayout.Label(point.Value.name);
                        GUILayout.Label(point.Value.isWalkable.ToString());
                        GUILayout.Label(point.Value.parentBlock ? point.Value.parentBlock.name : "None");
                        GUILayout.Label("GravitationalPlane");
                        GUILayout.BeginVertical();
                        GUILayout.Label("Plane " + point.Value.gravitationalPlane.plane);
                        GUILayout.Label("PlaneSide " + point.Value.gravitationalPlane.planeSide);
                        GUILayout.EndVertical();
                        GUILayout.EndVertical();
                        
                        GUILayout.EndHorizontal();
                    }
                }   
            }   
        }
        
        DrawDefaultInspector();
    }

    private void OnSceneGUI()
    {
        if (!_targetBlock && !_blockTransform.parent && !_targetBlock.mapPartBuilderParent) return;

        if (_targetBlock.mapPartBuilderParent)
        {
            if (Tools.current == Tool.Move) Tools.current = Tool.None;

            Vector3 newPos = Handles.PositionHandle(_blockTransform.position, _blockTransform.rotation);
            Vector3 snap = Handles.SnapValue(newPos, Block.size.ToVector());

            if (_blockTransform.position != snap)
            {
                if (HelperMethods.CheckInGrid(snap))
                {
                    if (!HelperMethods.CheckBlockInPosition(snap))
                    {
                        GameObject newBlock = Instantiate(_targetBlock.mapPartBuilderParent.blockPrefab, snap,
                            _blockTransform.rotation, _blockTransform.parent);
                        String[] newName = _targetBlock.name.Split(' ');
                        newBlock.name = newName[0] + " " + FindObjectsOfType<Block>().Length;
                        newBlock.GetComponent<Block>().mapPartBuilderParent = _targetBlock.mapPartBuilderParent;

                        Selection.activeGameObject = newBlock;
                    }
                }
            }
        }
    }

    private void OnDisable()
    {
        Tools.current = _lastTool;
    }
}
#endif