#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class BlockEditorMenuEditor
{
    [MenuItem("MapBuilder/Blocks/All Blocks/Connection Points/Set Parent Block")]
    private static void AllBlocksSetParentBlockToConnectionPoints()
    {
        SetParentBlocks(GetAllBlocks());
    }

    private static void SetParentBlocks(Block[] blocks)
    {
        foreach (Block block in blocks)
        {
            block.SetParentBlockToConnectionPoints();
        }
    }

    private static Block[] GetAllBlocks() => GameObject.FindObjectsOfType<Block>();
}
#endif