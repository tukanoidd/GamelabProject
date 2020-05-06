using System.Collections.Generic;

namespace DataTypes
{
    /// <summary>
    /// Stores the path in PathFinder object
    /// </summary>
    public class PathFindingLocation
    {
        public MapBlockData mapBlockData;
        public MapLocation mapLoc;
        public BlockConnection connection;
        
        public PathFindingLocation parent;

        public int f = 0;
        public int g = 0;
        public int h = 0;

        public PathFindingLocation(MapBlockData mapBlockData, MapLocation mapLoc)
        {
            this.mapBlockData = mapBlockData;
            this.mapLoc = mapLoc;
            parent = null;
            connection = null;
        }
        
        public PathFindingLocation(MapBlockData mapBlockData, MapLocation mapLoc, BlockConnection connection)
        {
            this.mapBlockData = mapBlockData;
            this.mapLoc = mapLoc;
            parent = null;
            connection = null;
        }
    }
}