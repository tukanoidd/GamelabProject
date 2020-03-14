using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public struct MapCoords
{
    public int x;
    public int z;

    public MapCoords(int x, int z)
    {
        this.x = x;
        this.z = z;
    }
}

public class MapBlockData
{
    public Block block;
    public Transform blockTransform;
    public Vector3 blockPos;
    public Quaternion blockRot;

    public MapBlockData(Block block)
    {
        this.block = block;
        this.block.thisBlocksMapData = this;
        blockTransform = block.transform;
        blockPos = blockTransform.position;
        blockRot = blockTransform.rotation;
    }
}

public class MapData
{
    public int length;
    public int height;

    public int minCoordX;
    public int maxCoordX;

    public int minCoordZ;
    public int maxCoordZ;

    public int gridSizeX;
    public int gridSizeZ;

    public KeyValuePair<MapCoords, MapBlockData>[,] map;

    private List<MapBlockData> _discardedBlockDatas;

    public MapData(int minX, int maxX, int minZ, int maxZ, BlockSize size)
    {
        minCoordX = minX;
        maxCoordX = maxX;

        minCoordZ = minZ;
        maxCoordZ = maxZ;

        gridSizeX = (int) size.xSize;
        gridSizeZ = (int) size.zSize;

        length = (maxCoordX - minCoordX) / gridSizeX + 1;
        height = (maxCoordZ - minCoordZ) / gridSizeZ + 1;

        map = new KeyValuePair<MapCoords, MapBlockData>[length, height];

        _discardedBlockDatas = new List<MapBlockData>();
    }

    public void AddBlocksToMap(MapBlockData[] blockDatas)
    {;
        foreach (MapBlockData blockData in blockDatas)
        {
            if (!_discardedBlockDatas.Contains(blockData))
            {
                // Get first block with connection in column (if any)
                // And discard blocks that have no purpose for pathfinding
                MapBlockData blockOnTheMap = GetViableBlockInColumn(blockData, blockDatas);

                if (blockOnTheMap != null)
                {
                    AddBlockToMap(blockOnTheMap);

                    // Cycle through viable block's connections to find other blocks and add them to the map if any
                    AddBlocksFromConnections(blockOnTheMap);
                }
            }
        }
    }

    private void AddBlocksFromConnections(MapBlockData block)
    {
        ConnectionPoint[] viableConnections = GetViableConnections(block);

        foreach (ConnectionPoint conPoint in viableConnections)
        {
            AddBlockToMapFromConnection(block, conPoint);
        }
    }

    private void AddBlockToMapFromConnection(MapBlockData blockData, ConnectionPoint conPoint)
    {
        if (conPoint.hasConnection && conPoint.connection)
        {
            if (conPoint.connection.parentBlock)
            {
                if (conPoint.isConnectedNearby && conPoint.connection.parentBlock.thisBlocksMapData != null)
                {
                    AddBlockToMap(conPoint.connection.parentBlock.thisBlocksMapData);
                }
                else if (conPoint.hasCustomConnection && conPoint.connection.parentBlock.thisBlocksMapData != null)
                {
                    AddCustomlyConnectedBlock(blockData, conPoint, conPoint.connection.parentBlock.thisBlocksMapData);
                }
            }
        }
    }

    private void AddCustomlyConnectedBlock(MapBlockData blockConnectedWith,
        ConnectionPoint conPointTargetBlockConnectedWith,
        MapBlockData targetBlockData)
    {
        MapCoords customMapCoords =
            GetCustomlyConnectedBlockMapCoords(blockConnectedWith, conPointTargetBlockConnectedWith);

        AddBlockWithCoords(targetBlockData, customMapCoords);
    }

    private MapCoords GetCustomlyConnectedBlockMapCoords(MapBlockData blockConnectedWith,
        ConnectionPoint conPointTargetBlockConnectedWith)
    {
        switch (conPointTargetBlockConnectedWith.posDir)
        {
            case ConnectionPoint.PosDir.UpForward:
                return GetBlockMapCoords(blockConnectedWith.blockPos +
                                         blockConnectedWith.blockTransform.forward * gridSizeZ);
            case ConnectionPoint.PosDir.UpBackward:
                return GetBlockMapCoords(blockConnectedWith.blockPos +
                                         -blockConnectedWith.blockTransform.forward * gridSizeZ);
            case ConnectionPoint.PosDir.UpRight:
                return GetBlockMapCoords(blockConnectedWith.blockPos +
                                         blockConnectedWith.blockTransform.right * gridSizeX);
            case ConnectionPoint.PosDir.UpLeft:
                return GetBlockMapCoords(blockConnectedWith.blockPos +
                                         -blockConnectedWith.blockTransform.right * gridSizeX);
            default: return new MapCoords(0, 0);
        }
    }

    private void AddBlockToMap(MapBlockData block)
    {
        MapCoords mapCoords = GetBlockMapCoords(block.blockPos);

        AddBlockWithCoords(block, mapCoords);
    }

    private void AddBlockWithCoords(MapBlockData block, MapCoords mapCoords)
    {
        try
        {
            map[mapCoords.x, mapCoords.z] = new KeyValuePair<MapCoords, MapBlockData>(mapCoords, block);
            _discardedBlockDatas.Add(block);
        }
        catch (Exception e)
        {
            Debug.Log("Couldn't add block to PathFinding map. Error: " + e);
        }
    }

    private MapCoords GetBlockMapCoords(Vector3 blockPos) => new MapCoords(
        (int) ((blockPos.x - minCoordX) / gridSizeX),
        (int) ((blockPos.z - minCoordZ) / gridSizeZ)
    );

    private MapBlockData GetViableBlockInColumn(MapBlockData blockDataToCheck, MapBlockData[] blockDatas)
    {
        if (CheckBlockForViableConnections(blockDataToCheck)) return blockDataToCheck;
        _discardedBlockDatas.Add(blockDataToCheck);

        MapBlockData[] blocksToCheck = blockDatas.Where(blockData =>
            !_discardedBlockDatas.Contains(blockData) && blockData.Equals(blockDataToCheck)).ToArray();

        foreach (MapBlockData blockData in blocksToCheck)
        {
            if (CheckBlockForViableConnections(blockData)) return blockData;
            _discardedBlockDatas.Add(blockData);
        }

        return null;
    }

    private bool CheckBlockForViableConnections(MapBlockData mapBlockData) =>
        GetViableConnections(mapBlockData).Length > 0;

    private ConnectionPoint[] GetViableConnections(MapBlockData mapBlockData) => mapBlockData.block.connectionPoints
        .Where(conPoint =>
            conPoint.posDir != ConnectionPoint.PosDir.WIP && conPoint.hasConnection && conPoint.connection).ToArray();
}

public class MapBuilder : MonoBehaviour
{
    private GameDefaultSettings _defaultGameSettings;
    private BlockSize _defaultBlockSize;

    private MapData _pathFindingMap;

    private MapBlockData[] _blockDatas;

    private GameObject _mapShow;

    public bool dataExists
    {
        get { return _blockDatas != null; }
    }

    void Awake()
    {
        GetNeededData();
    }

    public void GetNeededData()
    {
        _defaultGameSettings = Resources.Load<GameDefaultSettings>("ScriptableObjects/DefaultGameSettings");
        _defaultBlockSize = _defaultGameSettings.defaultBlockSize;
        _blockDatas = FindObjectsOfType<Block>().Select(block => new MapBlockData(block)).ToArray();
    }

    void Start()
    {
        if (Application.isPlaying)
        {
            GenerateMap();
        }
    }

    private Vector2 GetMinBlockXZ(int[] xCoords, int[] zCoords) => new Vector2(
        Mathf.Min(xCoords),
        Mathf.Min(zCoords)
    );

    private Vector2 GetMaxBlockXZ(int[] xCoords, int[] zCoords) => new Vector2(
        Mathf.Max(xCoords),
        Mathf.Max(zCoords)
    );

    public void GenerateAndShow()
    {
        GenerateMap();
        ShowMap();
    }

    public void GenerateMap()
    {
        if (_mapShow)
        {
            foreach (Transform block in _mapShow.transform)
            {
                Destroy(block.gameObject);
            }
        }
        
        if (_blockDatas != null)
        {
            int[] xCoords = _blockDatas.Select(blockData => (int) blockData.blockPos.x).ToArray();
            int[] zCoords = _blockDatas.Select(blockData => (int) blockData.blockPos.z).ToArray();

            Vector2 minXZCoords = GetMinBlockXZ(xCoords, zCoords);
            Vector2 maxXZCoords = GetMaxBlockXZ(xCoords, zCoords);

            Debug.Log("Creating map");
            _pathFindingMap = new MapData((int) minXZCoords.x, (int) maxXZCoords.x, (int) minXZCoords.y,
                (int) maxXZCoords.y, _defaultBlockSize);

            _pathFindingMap.AddBlocksToMap(_blockDatas);   
        }
    }

    public void ShowMap()
    {
        if (_pathFindingMap?.map != null && _pathFindingMap.map.Length > 0)
        {
            if (_mapShow)
            {
                if (_mapShow.transform.childCount == 0) AddBlocksToMapRepresentation(_mapShow.transform.position);
                _mapShow.SetActive(true);
            }
            else CreateMapRepresentation();   
        }
    }

    private void CreateMapRepresentation()
    {
        _mapShow = new GameObject("MapToShow");
        _mapShow.transform.position = Vector3.up * 20;

        AddBlocksToMapRepresentation(_mapShow.transform.position);
    }

    private void AddBlocksToMapRepresentation(Vector3 mapShowPos)
    {
        KeyValuePair<MapCoords, MapBlockData>[,] map = _pathFindingMap.map;
        int gridSizeX = _pathFindingMap.gridSizeX;
        int gridSizeZ = _pathFindingMap.gridSizeZ;

        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                MapCoords blockCoords = map[i, j].Key;
                MapBlockData blockData = map[i, j].Value;

                if (blockData != null)
                {
                    Debug.Log("create");
                    Instantiate(blockData.block.gameObject,
                        new Vector3(blockCoords.x * gridSizeX, mapShowPos.y, blockCoords.z * gridSizeZ), blockData.blockRot,
                        _mapShow.transform);   
                }
            }
        }
    }
    
    public void HideMap()
    {
        if (_mapShow) _mapShow.SetActive(false);
    }
}