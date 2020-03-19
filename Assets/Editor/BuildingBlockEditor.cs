using System;
using UnityEngine;
using Helpers;
using HandleUtility = UnityEngine.ProBuilder.HandleUtility;
#if UNITY_EDITOR
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(Block))]
public class BuildingBlockEditor : Editor
{
    private Block _targetBlock;
    private Transform _blockTransform;
    private GameDefaultSettings _defaultGameSettings;
    private int[] _sizes;

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

        _defaultGameSettings = Resources.Load<GameDefaultSettings>("ScriptableObjects/DefaultGameSettings");
        BlockSize size = _defaultGameSettings.defaultBlockSize;
        _sizes = new[] {(int) size.xSize, (int) size.ySize, (int) size.zSize};
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
            if (Tools.current == Tool.Move) Tools.current = Tool.None;
            
            Vector3 newPos = Handles.PositionHandle(_blockTransform.position, _blockTransform.rotation);
            Vector3 snap = Handles.SnapValue(newPos, _defaultGameSettings.defaultBlockSize.ToVector());

            if (_blockTransform.position != snap)
            {
                if (BlockHelpers.CheckIfPosInBlockGrid(snap, _defaultGameSettings.defaultBlockSize))
                {
                    if (!CheckIfBlockInPosition(snap))
                    {
                        GameObject newBlock = Instantiate(_targetBlock.gameObject, snap, _blockTransform.rotation,
                            _blockTransform.parent);
                        String[] newName = _targetBlock.name.Split(' ');
                        newBlock.name = newName[0] + " " + (FindObjectsOfType<Block>().Length);

                        Selection.activeGameObject = newBlock;
                    }
                }
            }
        }
    }

    private bool CheckIfBlockInPosition(Vector3 pos)
    {
        Block[] blocks = FindObjectsOfType<Block>();

        if (_sizes.Length == 3)
        {
            foreach (Block block in blocks)
            {
                if (Vector3.Distance(block.transform.position, pos) <=
                    Mathf.Min(_sizes[0], _sizes[1], _sizes[2]) / 2) return true;
            }

            return false;
        }

        return false;
    }

    private void OnDisable()
    {
        Tools.current = _lastTool;
    }
}
#endif