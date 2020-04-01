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

    public Vector2 ToVector2() => new Vector2(x, z);
    public Vector3 ToVector3() => new Vector3(x, 0, z);
}

public class MapBlockData
{
    public Block block;
    public Transform blockTransform;
    public Vector3 blockPos;
    public Quaternion blockRot;
    public MapCoords? mapCoords;

    public MapBlockData(Block block)
    {
        this.block = block;
        this.block.thisBlocksMapData = this;
        blockTransform = block.transform;
        blockPos = blockTransform.position;
        blockRot = blockTransform.rotation;
        mapCoords = null;
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

    public MapBlockData[,] map;

    private HashSet<MapBlockData> _discardedBlockDatas;

    public MapData(int minX, int maxX, int minZ, int maxZ, BlockSize size)
    {
        minCoordX = minX;
        maxCoordX = maxX;

        minCoordZ = minZ;
        maxCoordZ = maxZ;

        gridSizeX = (int) size.xSize;
        gridSizeZ = (int) size.zSize;

        length = ((maxCoordX - minCoordX) / gridSizeX) * 5;
        height = ((maxCoordZ - minCoordZ) / gridSizeZ) * 5;

        map = new MapBlockData[height, length];

        _discardedBlockDatas = new HashSet<MapBlockData>();
    }

    public void AddBlocksToMap(MapBlockData[] blockDatas)
    {
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
            if (!conPoint.connection.parentBlock && conPoint.connection.transform.parent)
            {
                conPoint.connection.parentBlock = conPoint.connection.transform.parent.GetComponent<Block>();
            }

            if (!conPoint.parentBlock && conPoint.transform.parent)
            {
                conPoint.parentBlock = conPoint.transform.parent.GetComponent<Block>();
            }

            if (conPoint.connection.parentBlock)
            {
                if (conPoint.connection.parentBlock.thisBlocksMapData != null)
                {
                    if (_discardedBlockDatas.Contains(conPoint.connection.parentBlock.thisBlocksMapData)) return;

                    AddCustomlyConnectedBlock(blockData, conPoint,
                        conPoint.connection.parentBlock.thisBlocksMapData);
                }
            }
        }
    }

    private void AddCustomlyConnectedBlock(MapBlockData blockConnectedWith,
        ConnectionPoint conPointTargetBlockConnectedWith,
        MapBlockData targetBlockData)
    {
        MapCoords customMapCoords =
            GetCustomlyConnectedBlockMapCoords(blockConnectedWith, conPointTargetBlockConnectedWith,
                blockConnectedWith.mapCoords);

        AddBlockWithCoords(targetBlockData, customMapCoords);
    }

    private MapCoords GetCustomlyConnectedBlockMapCoords(MapBlockData blockConnectedWith,
        ConnectionPoint conPointTargetBlockConnectedWith, MapCoords? blockConnectedWithMapCoords)
    {
        ConnectionPoint.PosDir posDir = conPointTargetBlockConnectedWith.posDir;

        if (blockConnectedWithMapCoords != null)
        {
            MapCoords coords = blockConnectedWithMapCoords.Value;

            switch (posDir)
            {
                case ConnectionPoint.PosDir.UpBackward: return new MapCoords(coords.x, coords.z + 1);
                case ConnectionPoint.PosDir.UpForward: return new MapCoords(coords.x, coords.z - 1);
                case ConnectionPoint.PosDir.UpLeft: return new MapCoords(coords.x - 1, coords.z);
                case ConnectionPoint.PosDir.UpRight: return new MapCoords(coords.x + 1, coords.z);
                default: return new MapCoords(0, 0);
            }
        }
        else
        {
            if (posDir == ConnectionPoint.PosDir.UpBackward || posDir == ConnectionPoint.PosDir.UpForward ||
                posDir == ConnectionPoint.PosDir.UpLeft ||
                posDir == ConnectionPoint.PosDir.UpRight)
            {
                Vector3 offset = conPointTargetBlockConnectedWith.offsetFromParentBlock;
                return GetBlockMapCoords(blockConnectedWith.blockPos +
                                         new Vector3(offset.x * gridSizeX, 0, offset.z * gridSizeZ));
            }
            else return new MapCoords(0, 0);
        }
    }

    private void AddBlockToMap(MapBlockData block)
    {
        AddBlockWithCoords(block, new MapCoords(height / 2, length / 2));
    }

    private void AddBlockWithCoords(MapBlockData block, MapCoords mapCoords)
    {
        if (!block.block.isWalkable) return;

        try
        {
            block.mapCoords = mapCoords;
            map[mapCoords.x, mapCoords.z] = block;
            _discardedBlockDatas.Add(block);

            // Cycle through viable block's connections to find other blocks and add them to the map if any
            AddBlocksFromConnections(block);
        }
        catch (Exception e)
        {
            Debug.Log(mapCoords.ToVector2());
            Debug.Log(map.GetLength(0) + " " + map.GetLength(1));
            Debug.Log("Couldn't add block to PathFinding map. Error: " + e);
        }
    }

    private MapCoords GetBlockMapCoords(Vector3 blockPos) => new MapCoords(
        (int) ((blockPos.x - minCoordX) / gridSizeX),
        (int) ((blockPos.z - minCoordZ) / gridSizeZ)
    );

    private MapBlockData GetViableBlockInColumn(MapBlockData blockDataToCheck, MapBlockData[] blockDatas)
    {
        MapBlockData[] blocksToCheck = blockDatas.Where(blockData =>
                !_discardedBlockDatas.Contains(blockData) &&
                Math.Abs(blockData.blockPos.x - blockDataToCheck.blockPos.x) < 0.2f &&
                Math.Abs(blockData.blockPos.z - blockDataToCheck.blockPos.z) < 0.2f && blockData.block.isWalkable)
            .OrderBy(block => block.blockPos.y)
            .ToArray();

        foreach (MapBlockData blockData in blocksToCheck)
        {
            if (CheckBlockForViableConnections(blockData)) return blockData;
            _discardedBlockDatas.Add(blockData);
        }

        return null;
    }

    private bool CheckBlockForViableConnections(MapBlockData mapBlockData) =>
        GetViableConnections(mapBlockData).Length > 0;

    private ConnectionPoint[] GetViableConnections(MapBlockData mapBlockData)
    {
        if (mapBlockData.block.connectionPoints.Count == 0) mapBlockData.block.UpdateConnectionPoints();

        return mapBlockData.block.connectionPoints
            .Where(conPoint =>
                conPoint.posDir != ConnectionPoint.PosDir.WIP && conPoint.hasConnection && conPoint.connection
            ).ToArray();
    }

    public void ShrinkMap()
    {
        MapCoords minCoords = new MapCoords(), maxCoords = new MapCoords();

        int? minI = null;
        int minJ = map.GetLength(1);
        // Getting coordinates of the minBounds of the map
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if (map[i, j] != null)
                {
                    if (map[i, j].block)
                    {
                        if (!minI.HasValue) minI = i;
                        if (j < minJ) minJ = j;
                        break;
                    }
                }
            }
        }
        
        minCoords = new MapCoords(minI ?? 0, minJ);

        int? maxI = null;
        int maxJ = 0;
        
        // Getting coordinates of the mapBounds of the map
        for (int i = map.GetLength(0) - 1; i >= 0; i--)
        {
            for (int j = map.GetLength(1) - 1; j >= 0; j--)
            {
                if (map[i, j] != null)
                {
                    if (map[i, j].block)
                    {
                        if (!maxI.HasValue) maxI = i;
                        if (j > maxJ) maxJ = j;
                        break;
                    }
                }
            }
        }
        
        maxCoords = new MapCoords(maxI ?? map.GetLength(0), maxJ);

        int fDimL = Mathf.Abs(maxCoords.x - minCoords.x);
        int sDimL = Mathf.Abs(maxCoords.z - minCoords.z);

        MapBlockData[,] newMap = new MapBlockData[fDimL + 1, sDimL + 1];

        for (int i = 0; i < newMap.GetLength(0); i++)
        {
            for (int j = 0; j < newMap.GetLength(1); j++)
            {
                newMap[i, j] = map[i + minCoords.x, j + minCoords.z];
                if (newMap[i, j] != null)
                {
                    if (newMap[i, j].block) newMap[i, j].mapCoords = new MapCoords(i, j);
                }
            }
        }

        map = newMap;
    }
}

public class MapBuilder : MonoBehaviour
{
    public bool showOnPlay = false;

    private GameDefaultSettings _defaultGameSettings;
    private BlockSize _defaultBlockSize;

    private MapData _pathFindingMap;

    private MapBlockData[] _blockDatas;

    private GameObject _mapShow;

    public bool dataExists
    {
        get { return _blockDatas != null; }
    }

    public void GetNeededData()
    {
        _defaultGameSettings = Resources.Load<GameDefaultSettings>("ScriptableObjects/DefaultGameSettings");
        _defaultBlockSize = _defaultGameSettings.defaultBlockSize;
        _blockDatas = FindObjectsOfType<Block>().Where(block => block.transform.parent.CompareTag("MapBuild"))
            .Select(block => new MapBlockData(block)).ToArray();
    }

    void Start()
    {
        if (Application.isPlaying)
        {
            GenerateMap();

            if (showOnPlay) ShowMap();
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

    public void GenerateMap()
    {
        GetNeededData();

        if (_mapShow)
        {
            if (Application.isEditor) DestroyImmediate(_mapShow);
            else if (Application.isPlaying) Destroy(_mapShow);
        }
        else
        {
            GameObject mapShowDup = GameObject.FindWithTag("MapRepresentation");
            if (mapShowDup)
            {
                if (Application.isEditor) DestroyImmediate(mapShowDup);
                else if (Application.isPlaying) Destroy(mapShowDup);
            }
        }

        if (_blockDatas != null)
        {
            int[] xCoords = _blockDatas.Select(blockData => (int) blockData.blockPos.x).ToArray();
            int[] zCoords = _blockDatas.Select(blockData => (int) blockData.blockPos.z).ToArray();

            Vector2 minXZCoords = GetMinBlockXZ(xCoords, zCoords);
            Vector2 maxXZCoords = GetMaxBlockXZ(xCoords, zCoords);

            _pathFindingMap = new MapData((int) minXZCoords.x, (int) maxXZCoords.x, (int) minXZCoords.y,
                (int) maxXZCoords.y, _defaultBlockSize);

            _pathFindingMap.AddBlocksToMap(_blockDatas);
            
            // Shrink map to proper size
            //_pathFindingMap.ShrinkMap();

            if (FindObjectOfType<PathFinder>()) FindObjectOfType<PathFinder>().mapData = _pathFindingMap;
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
        _mapShow.tag = "MapRepresentation";
        _mapShow.transform.position = Vector3.up * 20;

        AddBlocksToMapRepresentation(_mapShow.transform.position);
    }

    private void AddBlocksToMapRepresentation(Vector3 mapShowPos)
    {
        MapBlockData[,] map = _pathFindingMap.map;
        int gridSizeX = _pathFindingMap.gridSizeX;
        int gridSizeZ = _pathFindingMap.gridSizeZ;

        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                MapBlockData blockData = map[i, j];

                if (blockData?.mapCoords != null)
                {
                    MapCoords blockCoords = blockData.mapCoords.Value;

                    Instantiate(
                        blockData.block.gameObject,
                        new Vector3(blockCoords.x * gridSizeX, mapShowPos.y, blockCoords.z * gridSizeZ),
                        blockData.blockRot,
                        _mapShow.transform
                    );
                }
            }
        }
    }

    public void HideMap()
    {
        if (_mapShow) _mapShow.SetActive(false);
    }
}