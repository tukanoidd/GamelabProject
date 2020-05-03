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
        
        public override bool Equals(object obj)
        {
            if (obj is MapBlockData) return Equals((MapBlockData) obj);
            else return base.Equals(obj);
        }

        public bool Equals(MapBlockData mapBlockData)
        {
            if (mapBlockData == null) return false;
            return mapLoc.Equals(mapBlockData.mapLoc) && 
                   worldLoc == mapBlockData.worldLoc &&
                   block == mapBlockData.block;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + mapLoc.GetHashCode();
            hash = hash * 23 + worldLoc.GetHashCode();
            hash = hash * 23 + block.GetHashCode();

            return hash;
        }
    }
}