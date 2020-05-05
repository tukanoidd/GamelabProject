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
        
        public PathFindingLocation parent;

        public int f = 0;
        public int g = 0;
        public int h = 0;

        /// <summary>
        /// Constructor for PathFindingLocation class
        /// </summary>
        /// <param name="mapBlockData">Data of block in the map's cell</param>
        public PathFindingLocation(MapBlockData mapBlockData, MapLocation mapLoc)
        {
            this.mapBlockData = mapBlockData;
            this.mapLoc = mapLoc;
            parent = null;
        }
    }
}