using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DataTypes
{
    //-----------------Enums-----------------\\
    /// <summary>
    /// Enum that indicates world plane
    /// </summary>
    public enum Plane
    {
        XY,
        XZ,
        YZ
    }

    /// <summary>
    /// Enum that indicates side of a world plane
    /// </summary>
    public enum PlaneSide
    {
        PlaneNormalPositive,
        PlaneNormalNegative,
        PlaneNormalZero
    }

    /// <summary>
    /// Enum that indicates direction along the axis
    /// </summary>
    public enum AxisDirection
    {
        Positive,
        Negative,
        Zero
    }

    /// <summary>
    /// Enum that indicated the name of the axis
    /// </summary>
    public enum Axis
    {
        X,
        Y,
        Z
    }
    //-----------------Enums-----------------\\

    //----------------Structs----------------\\
    /// <summary>
    /// Struct for storing block location on the map 
    /// </summary>
    public struct MapLocation
    {
        public int row;
        public int col;

        /// <summary>
        /// Constructor for MapLocation struct
        /// </summary>
        /// <param name="row">First coordinate</param>
        /// <param name="col">Secind coordinate</param>
        public MapLocation(int row, int col)
        {
            this.row = row;
            this.col = col;
        }

        public Vector2 ToVector2() => new Vector2(row, col);

        public static MapLocation operator -(MapLocation mL1, MapLocation mL2)
        {
            return new MapLocation(mL1.row - mL2.row, mL1.col - mL2.col);
        }

        public static MapLocation operator +(MapLocation mL1, MapLocation mL2)
        {
            return new MapLocation(mL1.row + mL2.row, mL1.col + mL2.col);
        }
    }

    /// <summary>
    /// Struct that keeps data of a block in map's cell
    /// </summary>
    public struct MapBlockData
    {
        public MapLocation mapLoc;
        public Vector3 worldLoc;
        public Block block;

        /// <summary>
        /// Constructor for MapBlockData struct
        /// </summary>
        /// <param name="mapLocation">Location on the map</param>
        /// <param name="worldLocation">Location in the scene</param>
        /// <param name="block">Block that is stored in this map cell</param>
        public MapBlockData(MapLocation mapLocation, Vector3 worldLocation, Block block)
        {
            mapLoc = mapLocation;
            worldLoc = worldLocation;
            this.block = block;
        }

        /// <summary>
        /// Find nearest blockData in the map
        /// </summary>
        /// <param name="blockData">MapBlockData that we need to find nearest MapBlockData to</param>
        /// <param name="blockDatasInTheMap">Other MapBlockDatas that are added to the map</param>
        /// <returns>Nearest MapBlockData if could find one, otherwise null</returns>
        public static MapBlockData? GetNearestBlockData(MapBlockData blockData, MapBlockData[] blockDatasInTheMap)
        {
            MapBlockData[] viableBlockDatas = blockDatasInTheMap.Where(bD => bD.block != blockData.block).ToArray();
            if (viableBlockDatas.Length < 0) return null;

            return viableBlockDatas.OrderBy(bD => Vector3.Distance(bD.worldLoc, blockData.worldLoc)).ToArray()[0];
        }
    }

    /// <summary>
    /// Struct that indicates which plane character should move on based on gravity direction
    /// </summary>
    public struct GravitationalPlane
    {
        public Plane plane;
        public PlaneSide planeSide;

        /// <summary>
        /// Constructor for GravitationalPlane struct
        /// </summary>
        /// <param name="plane">Which plane character moves on</param>
        /// <param name="planeSide">Which side of a plane character moves on</param>
        public GravitationalPlane(Plane plane, PlaneSide planeSide)
        {
            this.plane = plane;
            this.planeSide = planeSide;
        }

        public static PlaneSide GetPlaneSide(AxisDirection normal)
        {
            if (normal == AxisDirection.Positive) return PlaneSide.PlaneNormalPositive;
            if (normal == AxisDirection.Negative) return PlaneSide.PlaneNormalNegative;
            return PlaneSide.PlaneNormalZero;
        }

        public static PlaneSide GetPlaneSide(float normal)
        {
            if (normal > 0) return PlaneSide.PlaneNormalPositive;
            if (normal < 0) return PlaneSide.PlaneNormalNegative;
            return PlaneSide.PlaneNormalZero;
        }

        public static bool operator ==(GravitationalPlane gV1, GravitationalPlane gV2)
        {
            return gV1.plane == gV2.plane && gV1.planeSide == gV2.planeSide;
        }

        public static bool operator !=(GravitationalPlane gV1, GravitationalPlane gV2)
        {
            return !(gV1 == gV2);
        }
    }

    /// <summary>
    /// Struct for indicating movement direction
    /// </summary>
    public struct MovementDirection
    {
        public AxisDirection xAxis;
        public AxisDirection yAxis;
        public AxisDirection zAxis;

        /// <summary>
        /// Constructor for MovementDirectionStruct struct
        /// </summary>
        /// <param name="xAxisDirection">Direction along x axis</param>
        /// <param name="yAxisDirection">Direction along y axis</param>
        /// <param name="zAxisDirection">Direction along z axis</param>
        public MovementDirection(AxisDirection xAxisDirection, AxisDirection yAxisDirection,
            AxisDirection zAxisDirection)
        {
            xAxis = xAxisDirection;
            yAxis = yAxisDirection;
            zAxis = zAxisDirection;
        }
    }

    /// <summary>
    /// Struct for indication of a constraint along different axis for player movement
    /// </summary>
    public struct MovementAxisConstraints
    {
        public bool x;
        public bool y;
        public bool z;

        /// <summary>
        /// Constructor for MovementAxisConstraint struct
        /// </summary>
        /// <param name="xAxisConstrained">Is movement constrained on x Axis</param>
        /// <param name="yAxisConstrained">Is movement constrained on y Axis</param>
        /// <param name="zAxisConstrained">Is movement constrained on z Axis</param>
        public MovementAxisConstraints(bool xAxisConstrained, bool yAxisConstrained, bool zAxisConstrained)
        {
            x = xAxisConstrained;
            y = yAxisConstrained;
            z = zAxisConstrained;
        }
    }

    /// <summary>
    /// Struct for saving block size in 3 dimensions
    /// </summary>
    [Serializable]
    public struct BlockSize
    {
        [Range(1, 5)] public int x;
        [Range(1, 5)] public int y;
        [Range(1, 5)] public int z;

        /// <summary>
        /// Constructor for BlockSize struct
        /// </summary>
        /// <param name="xSize">Size of the block on x axis</param>
        /// <param name="ySize">Size of the block on y axis</param>
        /// <param name="zSize">Size of the block on z axis</param>
        public BlockSize(int xSize, int ySize, int zSize)
        {
            x = xSize;
            y = ySize;
            z = zSize;
        }

        public Vector3 ToVector()
        {
            return new Vector3(x, y, z);
        }

        public static BlockSize operator /(Vector3 v1, BlockSize v2) => new BlockSize(
            (int) (v1.x / v2.x),
            (int) (v1.y / v2.y),
            (int) (v1.z / v2.z)
        );

        public static BlockSize operator /(BlockSize v1, Vector3 v2) => new BlockSize(
            (int) (v1.x / v2.x),
            (int) (v1.y / v2.y),
            (int) (v1.z / v2.z)
        );

        public static BlockSize operator /(BlockSize v1, BlockSize v2) => new BlockSize(
            (int) (v1.x / v2.x),
            (int) (v1.y / v2.y),
            (int) (v1.z / v2.z)
        );
    }

    /// <summary>
    /// Struct for showing position and direction of the object on another object's axis
    /// </summary>
    public struct AxisPositionDirection
    {
        public Axis axis;
        public AxisDirection dir;

        /// <summary>
        /// Constructor for AxisPositionDirection struct
        /// </summary>
        /// <param name="axis">Axis name</param>
        /// <param name="axisDirection">Axis direction</param>
        public AxisPositionDirection(Axis axis, AxisDirection axisDirection)
        {
            this.axis = axis;
            dir = axisDirection;
        }

        public static AxisDirection GetDirection(float val)
        {
            if (val > 0) return AxisDirection.Positive;
            if (Math.Abs(val) < 0.05f) return AxisDirection.Zero;
            return AxisDirection.Negative;
        }

        public static int NormalizeDirection(AxisDirection dir)
        {
            switch (dir)
            {
                case AxisDirection.Positive:
                    return 1;
                case AxisDirection.Negative:
                    return -1;
                default:
                    return 0;
            }
        }
    }
    //----------------Structs----------------\\

    //----------------Classes----------------\\
    /// <summary>
    /// Struct that stores information about connected block
    /// </summary>
    public class BlockConnection
    {
        public HashSet<Block> connectedBlocks;
        public GravitationalPlane gravitationalPlane;
        public HashSet<ConnectionPoint> connectionPoints;
        public SortedSet<Vector3> customCameraPositions;
        public bool isNear;

        /// <summary>
        /// Constructor for BlockConnection struct
        /// </summary>
        /// <param name="connectedBlocks">Blocks that are connected</param>
        /// <param name="gravitationalPlane">Plane of their connection</param>
        /// <param name="connectionPoints">Connection points blocks are connected with</param>
        /// <param name="customCameraPositions">Camera positions that allow this connection to be viable</param>>
        /// <param name="isNear">If the connection is between near blocks</param>
        public BlockConnection(HashSet<Block> connectedBlocks, GravitationalPlane gravitationalPlane,
            HashSet<ConnectionPoint> connectionPoints, SortedSet<Vector3> customCameraPositions,
            bool isNear)
        {
            this.connectedBlocks = connectedBlocks;
            this.gravitationalPlane = gravitationalPlane;
            this.connectionPoints = connectionPoints;
            this.customCameraPositions = customCameraPositions;
            this.isNear = isNear;
        }
    }
    
    public class MapData
    {
        private const int _length = 100;
        private const int _height = 100;

        private MapLocation _center;

        public Dictionary<GravitationalPlane, HashSet<MapBlockData>[,]> maps =
            new Dictionary<GravitationalPlane, HashSet<MapBlockData>[,]>();

        private HashSet<MapBlockData> _blockDatasInMap = new HashSet<MapBlockData>();

        public void CreateMap(MapBlockData[] blockDatas, GravitationalPlane gravitationalPlane)
        {
            HashSet<MapBlockData>[,] map = new HashSet<MapBlockData>[_height, _length];
            _center = new MapLocation(_height / 2, _length / 2);

            MapBlockData[] viableBlockDatas = blockDatas
                .Where(blockData => blockData.block.isWalkablePoints[gravitationalPlane].isWalkable).ToArray();

            if (viableBlockDatas.Length < 1) return;

            AddBlocksToMap(ref map, viableBlockDatas, gravitationalPlane);
            map = ShrinkMap(map);

            maps[gravitationalPlane] = map;
        }

        private void AddBlocksToMap(ref HashSet<MapBlockData>[,] map, MapBlockData[] blockDatas,
            GravitationalPlane gravitationalPlane)
        {
            AddBlockToMap(ref map, blockDatas[0], gravitationalPlane, true);
        }

        private void AddBlockToMap(ref HashSet<MapBlockData>[,] map, MapBlockData blockData,
            GravitationalPlane gravitationalPlane, bool first = false)
        {
            if (first)
            {
                AddBlockToMapWithCoords(ref map, blockData, new MapLocation(_height / 2, _length / 2),
                    gravitationalPlane);
            }
            else
            {
                MapLocation location = new MapLocation(0, 0), nearLocation = new MapLocation(0, 0);

                MapBlockData? checkNearBlockData =
                    MapBlockData.GetNearestBlockData(blockData, _blockDatasInMap.ToArray());
                if (!checkNearBlockData.HasValue) return;

                MapBlockData nearBlockData = checkNearBlockData.Value;

                switch (gravitationalPlane.plane)
                {
                    case Plane.XY:
                        location.row = (int) blockData.worldLoc.y;
                        location.col = (int) blockData.worldLoc.x;

                        nearLocation.row = (int) nearBlockData.worldLoc.y;
                        nearLocation.col = (int) nearBlockData.worldLoc.x;
                        break;
                    case Plane.XZ:
                        location.row = (int) blockData.worldLoc.z;
                        location.col = (int) blockData.worldLoc.x;

                        nearLocation.row = (int) nearBlockData.worldLoc.z;
                        nearLocation.col = (int) nearBlockData.worldLoc.x;
                        break;
                    case Plane.YZ:
                        location.row = (int) blockData.worldLoc.y;
                        location.col = (int) blockData.worldLoc.z;

                        nearLocation.row = (int) nearBlockData.worldLoc.y;
                        nearLocation.col = (int) nearBlockData.worldLoc.z;
                        break;
                }

                MapLocation offset = location - nearLocation;

                AddBlockToMapWithCoords(ref map, blockData, nearBlockData.mapLoc + offset, gravitationalPlane);
            }
        }

        private void AddBlockToMapWithCoords(ref HashSet<MapBlockData>[,] map, MapBlockData blockData,
            MapLocation mapLocation,
            GravitationalPlane gravitationalPlane)
        {
            try
            {
                map[mapLocation.row, mapLocation.col].Add(blockData);
                blockData.mapLoc = mapLocation;
                _blockDatasInMap.Add(blockData);

                // Cycle through viable block's connections to find other blocks and add them to the map if any
                AddBlocksFromConnections(ref map, blockData, gravitationalPlane);
            }
            catch (Exception e)
            {
                Debug.Log("------------------------------------");
                Debug.Log(mapLocation.ToVector2());
                Debug.Log(map.GetLength(0) + " " + map.GetLength(1));
                Debug.Log("Couldn't add block to PathFinding map. Error: " + e);
                Debug.Log("------------------------------------\n");
            }
        }

        private void AddBlocksFromConnections(ref HashSet<MapBlockData>[,] map, MapBlockData blockData,
            GravitationalPlane gravitationalPlane)
        {
            ConnectionPoint[] viableConnectionPoints = blockData.block.connectionPoints
                .Where(connectionPoint => connectionPoint.posDirs.Key.Contains(gravitationalPlane)).ToArray();
            List<BlockConnection> viableConnections =
                GetViableConnections(blockData, gravitationalPlane, viableConnectionPoints);

            foreach (BlockConnection blockConnection in viableConnections)
            {
                AddBlockToMapFromConnection(ref map, blockConnection, blockData, gravitationalPlane);
            }
        }

        private List<BlockConnection> GetViableConnections(MapBlockData blockData,
            GravitationalPlane gravitationalPlane, ConnectionPoint[] viableConnectionPoints)
        {
            HashSet<BlockConnection> viableConnections = new HashSet<BlockConnection>();

            foreach (ConnectionPoint viableConnectionPoint in viableConnectionPoints)
            {
                foreach (BlockConnection blockConnection in GameManager.current.blockConnections)
                {
                    if (blockConnection.gravitationalPlane != gravitationalPlane) continue;
                    if (!blockConnection.connectionPoints.Contains(viableConnectionPoint)) continue;

                    if (blockConnection.connectedBlocks.Contains(blockData.block))
                    {
                        if (!_blockDatasInMap.Contains(blockData)) viableConnections.Add(blockConnection);   
                    }
                }
            }

            return viableConnections.ToList();
        }

        private void AddBlockToMapFromConnection(ref HashSet<MapBlockData>[,] map, BlockConnection blockConnection,
            MapBlockData blockData, GravitationalPlane gravitationalPlane)
        {
            AddBlockToMapWithCoords(
                ref map,
                blockData,
                GetCustomlyConnectedBlockMapLocation(blockData, blockConnection, gravitationalPlane),
                gravitationalPlane
            );
        }

        private MapLocation GetCustomlyConnectedBlockMapLocation(MapBlockData blockData,
            BlockConnection blockConnection, GravitationalPlane gravitationalPlane)
        {
            MapLocation newMapLocation = blockData.mapLoc;

            ConnectionPoint connectionPointFrom =
                blockConnection.connectionPoints.First(
                    connectionPoint => connectionPoint.parentBlock == blockData.block);

            AxisDirection xDir = connectionPointFrom.posDirs.Value.First(dir => dir.axis == Axis.X).dir;
            AxisDirection yDir = connectionPointFrom.posDirs.Value.First(dir => dir.axis == Axis.Y).dir;
            AxisDirection zDir = connectionPointFrom.posDirs.Value.First(dir => dir.axis == Axis.Z).dir;

            if (gravitationalPlane.plane == Plane.XY)
            {
                newMapLocation.row += AxisPositionDirection.NormalizeDirection(yDir);
                newMapLocation.col += AxisPositionDirection.NormalizeDirection(xDir);
            }
            else if (gravitationalPlane.plane == Plane.XZ)
            {
                newMapLocation.row += AxisPositionDirection.NormalizeDirection(zDir);
                newMapLocation.col += AxisPositionDirection.NormalizeDirection(xDir);
            }
            else if (gravitationalPlane.plane == Plane.YZ)
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

    /// <summary>
    /// Class that is used in saving the path in PathFinder object
    /// </summary>
    public class PathFindingLocation
    {
        public List<MapBlockData> mapBlockDatas;
        public PathFindingLocation parent;

        /// <summary>
        /// Constructor for PathFindingLocation class
        /// </summary>
        /// <param name="mapBlockDatas">Datas of blocks in the map's cell</param>
        public PathFindingLocation(List<MapBlockData> mapBlockDatas)
        {
            this.mapBlockDatas = mapBlockDatas;
            parent = null;
        }
    }

    //----------------Classes----------------\\
}