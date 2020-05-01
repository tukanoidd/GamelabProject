using System.Collections.Generic;

namespace DataTypes
{
    /// <summary>
    /// Stores the path in PathFinder object
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
}