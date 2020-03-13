using System;
using UnityEngine;
using Helpers;
using HandleUtility = UnityEngine.ProBuilder.HandleUtility;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(Block))]
public class BuildingBlockEditor : Editor
{
    private Block _targetBlock;
    private Transform _blockTransform;
    private GameDefaultSettings _defaultGameSettings;

    private void OnEnable()
    {
        EditorHelpers.ToolsHidden = true;

        GameObject selectedGameObject = Selection.activeGameObject;
        if (selectedGameObject)
        {
            _targetBlock = selectedGameObject.GetComponent<Block>();
            if (_targetBlock) _blockTransform = _targetBlock.transform;   
        }
        
        _defaultGameSettings = Resources.Load<GameDefaultSettings>("ScriptableObjects/DefaultGameSettings");
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }

    private void OnSceneGUI()
    {
        if (!_targetBlock) return;
        if (!_blockTransform.parent) return;

        if (_blockTransform.parent.name.Contains("MapPartBuilder"))
        {
            Vector3 newPos = Handles.PositionHandle(_blockTransform.position, _blockTransform.rotation);
            Vector3 snap = Handles.SnapValue(newPos, _defaultGameSettings.defaultBlockSize.ToVector());

            if (_blockTransform.position != snap)
            {
                if (BlockHelpers.CheckIfPosInBlockGrid(snap, _defaultGameSettings.defaultBlockSize))
                {
                    GameObject newBlock = Instantiate(_targetBlock.gameObject, snap, _blockTransform.rotation, _blockTransform.parent);

                    ConnectionPoint[] conPoints = newBlock.GetComponentsInChildren<ConnectionPoint>();
                    foreach (ConnectionPoint conPoint in conPoints)
                    {
                        conPoint.CheckForNearbyConnectionPoint();
                    }
                        
                    Selection.activeGameObject = newBlock;
                }   
            }
        }
    }

    private void OnDisable()
    {
        EditorHelpers.ToolsHidden = false;
    }
}
#endif