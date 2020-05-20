#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class BlockEditorMenuEditor
{
    [MenuItem("MapBuilder/Blocks/All Blocks/Is Walkable Points/All/Check If Walkable")]
    private static void AllBlocksAllWalkablePointsCheckIfWalkable()
    {
        IsWalkablePointsCheckIfWalkable(GetAllBlocks());
    }

    [MenuItem("MapBuilder/Blocks/All Blocks/Is Walkable Points/All/Set Active and Set Parent Block")]
    private static void AllBlocksAllWalkablePointsSetActiveSetParentBlock()
    {
        IsWalkablePointsSetActiveAndSetParentBlock(GetAllBlocks());
    }

    private static void IsWalkablePointsSetActiveAndSetParentBlock(Block[] blocks)
    {
        foreach (Block block in blocks)
        {
            block.IsWalkablePointsSetActiveAndSetParentBlock();
        }
    }
    
    private static void IsWalkablePointsCheckIfWalkable(Block[] blocks)
    {
        foreach (Block block in blocks)
        {
            block.IsWalkablePointsCheckIfWalkable();
        }
    }

    private static Block[] GetAllBlocks() => GameObject.FindObjectsOfType<Block>();
}
#endif