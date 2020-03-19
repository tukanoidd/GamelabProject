using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class MapBuilder : MonoBehaviour
{
    private GameDefaultSettings _defaultGameSettings;

    private KeyValuePair<Vector2, Block>[,] _pathFindingMap;

    private Block[] _blocks;

    void Awake()
    {
        _defaultGameSettings = Resources.Load<GameDefaultSettings>("ScriptableObjects/DefaultGameSettings");
    }

    void Start()
    {
        GeneratePathfindingMap();
        ShowMap();
    }

    private void ShowMap()
    {
        Vector3 offset = Vector3.one * 20;
        
        for (int i = 0; i < _pathFindingMap.GetLength(0); i++)
        {
            for (int j = 0; j < _pathFindingMap.GetLength(1); j++)
            {
                Block block = _pathFindingMap[i, j].Value;
                if (block)
                {
                    Instantiate(block.gameObject, block.transform.position + offset, block.transform.rotation);   
                }
            }
        }
    }

    private void GeneratePathfindingMap()
    {
        _blocks = FindObjectsOfType<Block>().Where(block => block.isWalkable).ToArray();
        int[] blocksXCoords = _blocks.Select(block => (int) block.transform.position.x).ToArray();
        int[] blocksZCoords = _blocks.Select(block => (int) block.transform.position.z).ToArray();

        Vector2 blocksMinXZCoords = GetMinVec2Pos(blocksXCoords, blocksZCoords);
        Vector2 blocksMaxXZCoords = GetMaxVec2Pos(blocksXCoords, blocksZCoords);

        int mapLength =
            (int) ((blocksMaxXZCoords.x - blocksMinXZCoords.x) / _defaultGameSettings.defaultBlockSize.xSize) + 1;
        int mapHeight =
            (int) ((blocksMaxXZCoords.y - blocksMinXZCoords.y) / _defaultGameSettings.defaultBlockSize.zSize) + 1;

        _pathFindingMap = new KeyValuePair<Vector2, Block>[mapLength, mapHeight];

        List<Block> discardedBlocks = new List<Block>();

        foreach (Block block in _blocks)
        {
            if (!discardedBlocks.Contains(block))
            {
                AddBlockToMap(block, ref discardedBlocks, blocksMinXZCoords);
            }
        }
    }

    private void AddBlockToMap(Block block, ref List<Block> discardedBlocks, Vector2 minXZCoords)
    {
        Block blockOnTheMap = DiscardBlocksBelow(block, ref discardedBlocks);

        try
        {
            Vector2 mapXZCoord = GetMapXZCoord(blockOnTheMap, minXZCoords);
            _pathFindingMap[(int) mapXZCoord.x, (int) mapXZCoord.y] =
                new KeyValuePair<Vector2, Block>(new Vector2(mapXZCoord.x, mapXZCoord.y), blockOnTheMap);
            discardedBlocks.Add(blockOnTheMap);
        }
        catch (Exception e)
        {
            Debug.Log("Couldn't add block to map. Error: " + e);
        }

        ConnectionPoint[] conPoints = blockOnTheMap.connectionPoints
            .Where(conPoint =>
                conPoint.posDir != ConnectionPoint.PosDir.WIP && conPoint.hasConnection && conPoint.connection)
            .ToArray();

        foreach (ConnectionPoint conPoint in conPoints)
        {
            AddNewBlockFromConnection(conPoint, blockOnTheMap, ref discardedBlocks, minXZCoords);
        }
    }

    private Vector2 GetMapXZCoord(Block block, Vector2 minXZCoords)
    {
        Vector3 blockPos = block.transform.position;
        return new Vector2((int) ((blockPos.x - minXZCoords.x) / _defaultGameSettings.defaultBlockSize.xSize),
            (int) ((blockPos.z - minXZCoords.y) / _defaultGameSettings.defaultBlockSize.zSize));
    }

    private void AddNewBlockFromConnection(ConnectionPoint conPoint, Block block, ref List<Block> discardedBlocks, Vector2 minXZCoords)
    {
        Transform targetConParent = conPoint.connection.transform.parent;
        if (targetConParent)
        {
            Block targetConBlock = targetConParent.GetComponent<Block>();
            if (targetConBlock && !discardedBlocks.Contains(targetConBlock))
            {
                AddBlockToMap(targetConBlock, ref discardedBlocks, minXZCoords);
            }
        }
    }

    private Block DiscardBlocksBelow(Block block, ref List<Block> discardedBlocks)
    {
        Vector3 blockPos = block.transform.position;
        Vector2 blockXZPos = new Vector2(blockPos.x, blockPos.z);

        Block[] blocksSameXZ = _blocks.Where(bl =>
                new Vector2(bl.transform.position.x, bl.transform.position.z) == blockXZPos)
            .OrderByDescending(bl => bl.transform.position.y).ToArray();

        for (int i = 1; i < blocksSameXZ.Length; i++) discardedBlocks.Add(blocksSameXZ[i]);

        return blocksSameXZ[0];
    }

    private Vector2 GetMinVec2Pos(int[] coords1, int[] coords2)
    {
        return new Vector2(
            Mathf.Min(coords1),
            Mathf.Min(coords2)
        );
    }

    private Vector2 GetMaxVec2Pos(int[] coords1, int[] coords2)
    {
        return new Vector2(
            Mathf.Max(coords1),
            Mathf.Max(coords2)
        );
    }

    public MapPartBuilder AddMapPartBuilder()
    {
        GameObject newMapPartBuilder = new GameObject("MapPartBuilder");
        MapPartBuilder newMapPartBuilderComponent = newMapPartBuilder.AddComponent<MapPartBuilder>();
        newMapPartBuilder.transform.parent = transform;

        return newMapPartBuilderComponent;
    }
}