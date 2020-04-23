#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(Block))]
public class BlockEditor : Editor
{
    private Block _targetBlock;
    private Transform _blockTransform;
    
    private Tool _lastTool = Tool.None;

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