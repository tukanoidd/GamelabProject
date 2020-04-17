using System;
using System.Collections.Generic;
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
        public int coord1;
        public int coord2;

        /// <summary>
        /// Constructor for MapLocation struct
        /// </summary>
        /// <param name="coord1">First coordinate</param>
        /// <param name="coord2">Secind coordinate</param>
        public MapLocation(int coord1, int coord2)
        {
            this.coord1 = coord1;
            this.coord2 = coord2;
        }
    }

    /// <summary>
    /// Struct that stores information about connected block
    /// </summary>
    public struct BlockConnection
    {
        public KeyValuePair<Block, Block> connectedBlocks;
        public Plane plane;
        public Dictionary<ConnectionPoint, ConnectionPoint> connectionPoints;
        public List<Vector3> customCameraPositions;
        public bool isNear;

        /// <summary>
        /// Constructor for BlockConnection struct
        /// </summary>
        /// <param name="connectedBlocks">Blocks that are connected</param>
        /// <param name="plane">Plane of their connection</param>
        /// <param name="connectionPoints">Connection points blocks are connected with</param>
        /// <param name="customCameraPositions">Camera positions that allow this connection to be viable</param>>
        /// <param name="isNear">If the connection is between near blocks</param>
        public BlockConnection(KeyValuePair<Block, Block> connectedBlocks, Plane plane, Dictionary<ConnectionPoint, ConnectionPoint> connectionPoints, List<Vector3> customCameraPositions, bool isNear)
        {
            this.connectedBlocks = connectedBlocks;
            this.plane = plane;
            this.connectionPoints = connectionPoints;
            this.customCameraPositions = customCameraPositions;
            this.isNear = isNear;
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
        public MapBlockData(MapLocation mapLocation, Vector3 worldLocation, List<BlockConnection> blockConnections, Block block)
        {
            mapLoc = mapLocation;
            worldLoc = worldLocation;
            this.block = block;
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
    }
    //----------------Structs----------------\\

    //----------------Classes----------------\\
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