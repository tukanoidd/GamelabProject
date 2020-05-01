using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DataTypes
{
    /// <summary>
    /// Keeps data of a block in map's cell
    /// </summary>
    public class MapBlockData
    {
        public MapLocation mapLoc;
        public Vector3 worldLoc;
        public Block block;

        /// <summary>
        /// Constructor for MapBlockData struct
        /// </summary>
        /// <param name="mapLocation">Locations on the map</param>
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
        /// <param name="block">MapBlockData that we need to find nearest MapBlockData to</param>
        /// <param name="blockDatasInTheMap">Other MapBlockDatas that are added to the map</param>
        /// <returns>Nearest MapBlockData if could find one, otherwise null</returns>
        public static MapBlockData GetNearestBlockData(Block block, MapBlockData[] blockDatasInTheMap)
        {
            MapBlockData[] viableBlockDatas = blockDatasInTheMap.Where(bD => bD.block != block).ToArray();
            if (viableBlockDatas.Length < 0) return null;

            return viableBlockDatas.OrderBy(bD => Vector3.Distance(bD.worldLoc, block.transform.position)).ToArray()[0];
        }
    }
}