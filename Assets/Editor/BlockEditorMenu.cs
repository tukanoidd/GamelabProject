using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BlockEditorMenu
{
    [MenuItem("MapBuilder/Blocks/All/Create Points")]
    private static void AllBlocksCreatePoints()
    {
        BlocksCreatePoints(GameObject.FindObjectsOfType<Block>());
    }
    
    [MenuItem("MapBuilder/Blocks/Selected/Create Points")]
    private static void SelectedBlocksCreatePoints()
    {
        BlocksCreatePoints(GetSelectedBlocks()); 
    }
    
    [MenuItem("MapBuilder/Blocks/All/Reset Points")]
    private static void AllBlocksResetPoints()
    {
        BlocksCreatePoints(GameObject.FindObjectsOfType<Block>());
    }
    
    [MenuItem("MapBuilder/Blocks/Selected/Reset Points")]
    private static void SelectedBlocksResetPoints()
    {
        BlocksCreatePoints(GetSelectedBlocks()); 
    }

    private static Block[] GetSelectedBlocks() => Selection.gameObjects.Where(obj => obj.GetComponent<Block>() != null)
            .Select(obj => obj.GetComponent<Block>()).ToArray();

    private static void BlocksCreatePoints(Block[] blocks)
    {
        foreach (Block block in blocks)
        {
            block.pointsReset = true;
            block.CreatePoints();
        }
    }
}
