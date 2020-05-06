using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using UnityEngine;

namespace DataTypes
{
    public class MapData
    {
        private readonly int _length;
        private readonly int _height;

        public MapData(int l, int h)
        {
            _length = l;
            _height = h;
        }

        private MapLocation _center;

        public Dictionary<GravitationalPlane, HashSet<MapBlockData>[,]> maps =
            new Dictionary<GravitationalPlane, HashSet<MapBlockData>[,]>();

        private HashSet<MapBlockData> _blockDatasInMap;

        private Dictionary<BlockConnection, HashSet<KeyValuePair<MapLocation, MapLocation>>> _blockConnectionsInMap =
            new Dictionary<BlockConnection, HashSet<KeyValuePair<MapLocation, MapLocation>>>();

        public void CreateMap(Block[] blocks, GravitationalPlane gravitationalPlane)
        {
            HashSet<MapBlockData>[,] map = new HashSet<MapBlockData>[_height, _length];
            _center = new MapLocation(_height / 2, _length / 2);

            Block[] viableBlocks = blocks
                .Where(block => block.isWalkablePoints[gravitationalPlane].isWalkable).ToArray();

            if (viableBlocks.Length < 1) return;

            AddBlocksToMap(ref map, viableBlocks, gravitationalPlane);
            //map = ShrinkMap(map);

            maps[gravitationalPlane] = map;
        }

        private void AddBlocksToMap(ref HashSet<MapBlockData>[,] map, Block[] viableBlocks,
            GravitationalPlane gravitationalPlane)
        {
            _blockDatasInMap = new HashSet<MapBlockData>();

            AddBlockToMap(ref map, viableBlocks, viableBlocks[0], gravitationalPlane, true);

            for (int i = 1; i < viableBlocks.Length; i++)
            {
                if (_blockDatasInMap.All(blockData => blockData.block != viableBlocks[i]))
                    AddBlockToMap(ref map, viableBlocks, viableBlocks[i], gravitationalPlane);
            }
        }

        private void AddBlockToMap(ref HashSet<MapBlockData>[,] map, Block[] viableBlocks, Block block,
            GravitationalPlane gravitationalPlane, bool first = false)
        {
            if (first)
            {
                AddBlockToMapWithCoords(ref map, viableBlocks, block, new MapLocation(_height / 2, _length / 2),
                    gravitationalPlane);
            }
            else
            {
                MapLocation location = new MapLocation(0, 0), nearLocation = new MapLocation(0, 0);

                MapBlockData nearBlockData =
                    MapBlockData.GetNearestBlockData(block, _blockDatasInMap.ToArray());

                Vector3 blockWorldLoc = block.transform.position;

                if (nearBlockData == null) return;

                switch (gravitationalPlane.plane)
                {
                    case Plane.XY:
                        location.row = (int) blockWorldLoc.y;
                        location.col = (int) blockWorldLoc.x;

                        nearLocation.row = (int) nearBlockData.worldLoc.y;
                        nearLocation.col = (int) nearBlockData.worldLoc.x;
                        break;
                    case Plane.XZ:
                        location.row = (int) blockWorldLoc.z;
                        location.col = (int) blockWorldLoc.x;

                        nearLocation.row = (int) nearBlockData.worldLoc.z;
                        nearLocation.col = (int) nearBlockData.worldLoc.x;
                        break;
                    case Plane.YZ:
                        location.row = (int) blockWorldLoc.y;
                        location.col = (int) blockWorldLoc.z;

                        nearLocation.row = (int) nearBlockData.worldLoc.y;
                        nearLocation.col = (int) nearBlockData.worldLoc.z;
                        break;
                }

                MapLocation offset = location - nearLocation;

                AddBlockToMapWithCoords(ref map, viableBlocks, block, nearBlockData.mapLoc + offset,
                    gravitationalPlane);
            }
        }

        private void AddBlockToMapWithCoords(ref HashSet<MapBlockData>[,] map, Block[] viableBlocks, Block block,
            MapLocation newMapLocation,
            GravitationalPlane gravitationalPlane, BlockConnection blockConnection = null,
            MapLocation connectionFromBlockLocation = null)
        {
            try
            {
                if (map[newMapLocation.row, newMapLocation.col] == null)
                    map[newMapLocation.row, newMapLocation.col] = new HashSet<MapBlockData>();

                MapBlockData newMapBlockData = new MapBlockData(newMapLocation, block.transform.position, block);

                map[newMapLocation.row, newMapLocation.col].Add(newMapBlockData);
                
                if (!block.mapBlockDatas.ContainsKey(gravitationalPlane)) block.mapBlockDatas[gravitationalPlane] = new HashSet<MapBlockData>();
                block.mapBlockDatas[gravitationalPlane].Add(newMapBlockData);

                if (blockConnection != null && connectionFromBlockLocation != null)
                {
                    if (!_blockConnectionsInMap.ContainsKey(blockConnection))
                    {
                        _blockConnectionsInMap[blockConnection] = new HashSet<KeyValuePair<MapLocation, MapLocation>>
                        {
                            new KeyValuePair<MapLocation, MapLocation>(newMapLocation,
                                connectionFromBlockLocation)
                        };
                    }
                    else
                    {
                        KeyValuePair<MapLocation, MapLocation> blockConnectionLocations =
                            new KeyValuePair<MapLocation, MapLocation>(newMapLocation, connectionFromBlockLocation);

                        if (_blockConnectionsInMap[blockConnection].All(mapLocs =>
                            !HelperMethods.KeyValuePairsEqualBothWays(mapLocs, blockConnectionLocations)
                        )) _blockConnectionsInMap[blockConnection].Add(blockConnectionLocations);
                    }
                }
                
                _blockDatasInMap.Add(newMapBlockData);

                // Cycle through viable block's connections to find other blocks and add them to the map if any
                AddBlocksFromConnections(ref map, viableBlocks, newMapBlockData, gravitationalPlane);
            }
            catch (Exception e)
            {
                Debug.Log("------------------------------------");
                Debug.Log(newMapLocation.ToVector2());
                Debug.Log(map.GetLength(0) + " " + map.GetLength(1));
                Debug.Log("Couldn't add block to PathFinding map. Error: " + e);
                Debug.Log("------------------------------------\n");
            }
        }

        private void AddBlocksFromConnections(ref HashSet<MapBlockData>[,] map, Block[] viableBlocks,
            MapBlockData blockData,
            GravitationalPlane gravitationalPlane)
        {
            ConnectionPoint[] viableConnectionPoints = blockData.block.connectionPoints
                .Where(connectionPoint => connectionPoint.posDirs.gravitationalPlanes.Contains(gravitationalPlane))
                .ToArray();

            Dictionary<KeyValuePair<Block, BlockConnection>, KeyValuePair<MapLocation, MapLocation>> blocksToAdd =
                GetViableBlocksToAddFromConnections(ref map, viableBlocks, viableConnectionPoints, blockData,
                    gravitationalPlane);

            foreach (KeyValuePair<KeyValuePair<Block, BlockConnection>, KeyValuePair<MapLocation, MapLocation>>
                blockToAdd in blocksToAdd)
            {
                AddBlockToMapWithCoords(
                    ref map,
                    viableBlocks,
                    blockToAdd.Key.Key, // blockToAdd
                    blockToAdd.Value.Key, // mapLocationTo
                    gravitationalPlane,
                    blockToAdd.Key.Value, // blockConnection
                    blockToAdd.Value.Value // mapLocationFrom
                );
            }
        }

        private Dictionary<KeyValuePair<Block, BlockConnection>, KeyValuePair<MapLocation, MapLocation>>
            GetViableBlocksToAddFromConnections(
                ref HashSet<MapBlockData>[,] map, Block[] viableBlocks, ConnectionPoint[] viableConnectionPoints,
                MapBlockData blockDataFrom,
                GravitationalPlane gravitationalPlane)
        {
            Dictionary<KeyValuePair<Block, BlockConnection>, KeyValuePair<MapLocation, MapLocation>> blocksToAdd =
                new Dictionary<KeyValuePair<Block, BlockConnection>, KeyValuePair<MapLocation, MapLocation>>();

            foreach (ConnectionPoint viableConnectionPoint in viableConnectionPoints)
            {
                foreach (BlockConnection blockConnection in GameManager.current.blockConnections)
                {
                    if (!blockConnection.gravitationalPlane.Equals(gravitationalPlane)) continue;
                    if (!blockConnection.connectionPoints.Contains(viableConnectionPoint)) continue;
                    if (!blockConnection.connectedBlocks.Contains(blockDataFrom.block)) continue;

                    KeyValuePair<Block, BlockConnection> blockToAdd = new KeyValuePair<Block, BlockConnection>(
                        blockConnection.connectedBlocks.First(block => block != blockDataFrom.block),
                        blockConnection
                    );

                    if (!viableBlocks.Contains(blockToAdd.Key)) continue;

                    KeyValuePair<MapLocation, MapLocation> mapLocationsToFrom =
                        new KeyValuePair<MapLocation, MapLocation>(
                            GetMapLocationFromConnection(blockDataFrom, blockConnection, gravitationalPlane.plane),
                            blockDataFrom.mapLoc
                        );

                    if (_blockConnectionsInMap.ContainsKey(blockConnection))
                    {
                        if (_blockConnectionsInMap[blockConnection].Any(mapLocs =>
                                HelperMethods.KeyValuePairsEqualBothWays(mapLocs, mapLocationsToFrom)
                            )
                        ) continue;

                        blocksToAdd.Add(blockToAdd, mapLocationsToFrom);
                    }
                    else blocksToAdd.Add(blockToAdd, mapLocationsToFrom);
                }
            }

            return blocksToAdd;
        }

        private MapLocation GetMapLocationFromConnection(MapBlockData blockDataFrom, BlockConnection blockConnection,
            Plane plane)
        {
            MapLocation newMapLocation = new MapLocation(blockDataFrom.mapLoc.row, blockDataFrom.mapLoc.col);

            ConnectionPoint connectionPointFrom = blockConnection.connectionPoints.First(
                connectionPoint => connectionPoint.parentBlock == blockDataFrom.block
            );

            AxisDirection xDir = connectionPointFrom.posDirs.axisPositionDirections.First(dir => dir.axis == Axis.X)
                .dir;
            AxisDirection yDir = connectionPointFrom.posDirs.axisPositionDirections.First(dir => dir.axis == Axis.Y)
                .dir;
            AxisDirection zDir = connectionPointFrom.posDirs.axisPositionDirections.First(dir => dir.axis == Axis.Z)
                .dir;

            if (plane == Plane.XY)
            {
                newMapLocation.row += AxisPositionDirection.NormalizeDirection(yDir);
                newMapLocation.col += AxisPositionDirection.NormalizeDirection(xDir);
            }
            else if (plane == Plane.XZ)
            {
                newMapLocation.row += AxisPositionDirection.NormalizeDirection(zDir);
                newMapLocation.col += AxisPositionDirection.NormalizeDirection(xDir);
            }
            else if (plane == Plane.YZ)
            {
                newMapLocation.row += AxisPositionDirection.NormalizeDirection(yDir);
                newMapLocation.col += AxisPositionDirection.NormalizeDirection(zDir);
            }

            return newMapLocation;
        }

        private HashSet<MapBlockData>[,] ShrinkMap(HashSet<MapBlockData>[,] map)
        {
            MapLocation minCoords, maxCoords;

            int minRow = map.GetLength(0);
            int minCol = map.GetLength(1);

            for (int row = 0; row < map.GetLength(0); row++)
            {
                for (int col = 0; col < map.GetLength(1); col++)
                {
                    if (map[row, col] != null && map[row, col].Count > 0)
                    {
                        minRow = row;
                        minCol = col;
                        goto FoundMin;
                    }
                }
            }

            FoundMin:
            minCoords = new MapLocation(minRow, minCol);

            int maxRow = 0, maxCol = 0;

            for (int row = map.GetLength(0) - 1; row >= 0; row--)
            {
                for (int col = map.GetLength(1) - 1; col >= 0; col--)
                {
                    if (map[row, col] != null && map[row, col].Count > 0)
                    {
                        maxRow = row;
                        maxCol = col;
                        goto FoundMax;
                    }
                }
            }

            FoundMax:
            maxCoords = new MapLocation(maxRow, maxCol);

            int rows = Mathf.Abs(maxCoords.row - minCoords.row);
            int cols = Mathf.Abs(maxCoords.col - minCoords.col);

            HashSet<MapBlockData>[,] newMap = new HashSet<MapBlockData>[rows + 1, cols + 1];

            for (int row = 0; row <= rows; row++)
            {
                for (int col = 0; col <= cols; col++)
                {
                    newMap[row, col] = map[row + minRow, col + minCol];
                    if (newMap[row, col] != null)
                    {
                        newMap[row, col].Select(blockData => blockData = new MapBlockData(
                            new MapLocation(row, col),
                            blockData.worldLoc,
                            blockData.block
                        ));
                    }
                }
            }

            return newMap;
        }
    }
}