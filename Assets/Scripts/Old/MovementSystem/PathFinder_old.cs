/*using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AStarPathFinding;
using UnityEngine;
using UnityEngine.Rendering.UI;

public struct Coords
{
    public MapCoords mapCoords;
    public Vector3? blockCoords;

    public Coords(MapCoords mapCoords, Vector3? blockCoords)
    {
        this.mapCoords = mapCoords;
        this.blockCoords = blockCoords;
    }
}

namespace AStarPathFinding
{
    public class Location
    {
        public Coords coords;
        public int f = 0;
        public int g = 0;
        public int h = 0;
        public Location parent = null;

        public Location(Coords coords)
        {
            this.coords = coords;
        }
    }

    public class PahtfindAlgo
    {
        public static Location FindShortestPath(Coords startBlock, Coords targetBlock, MapBlockData[,] map)
        {
            Location current = null;

            if (startBlock.blockCoords.HasValue && targetBlock.blockCoords.HasValue)
            {
                Location start = new Location(new Coords(startBlock.mapCoords, startBlock.blockCoords.Value));
                Location target = new Location(new Coords(targetBlock.mapCoords, targetBlock.blockCoords.Value));

                List<Location> openList = new List<Location>();
                List<Location> closedList = new List<Location>();

                int g = 0;

                openList.Add(start);

                while (openList.Any())
                {
                    // Get the location with the lowest F score
                    int lowest = openList.Min(l => l.f);
                    current = openList.First(l => l.f == lowest);

                    // Add the current location to the closed list
                    closedList.Add(current);

                    // Remove it from the open list
                    openList.Remove(current);

                    // If we added the destination to the closed list, we've found a path
                    if (closedList.FirstOrDefault(l => l.coords.mapCoords.Equals(target.coords.mapCoords)) !=
                        null) break;

                    List<Location> adjBlocks =
                        CheckIfActuallyConnected(GetWalkableAdjacentBlocks(current.coords, map), current, map);

                    MapCoords currMapCoords = current.coords.mapCoords;
                    Block currBlock = map[currMapCoords.x, currMapCoords.z].block;

                    g = current.g + 1;

                    foreach (Location adjBlock in adjBlocks)
                    {
                        // If this adjacent block is already in the closed list, ignore
                        if (closedList.FirstOrDefault(l =>
                            l.coords.mapCoords.Equals(adjBlock.coords.mapCoords)) != null)
                            continue;

                        // If it's not in the open list...
                        if (openList.FirstOrDefault(l => l.coords.mapCoords.Equals(adjBlock.coords.mapCoords)) == null)
                        {
                            // Compute its score, set the parent
                            adjBlock.g = g;
                            adjBlock.h = ComputeHScore(adjBlock.coords, target.coords);
                            adjBlock.f = adjBlock.g + adjBlock.h;
                            adjBlock.parent = current;

                            // And add it to the open list
                            openList.Insert(0, adjBlock);
                        }
                        else
                        {
                            // Test if using the current G score makes the adjacent block's F
                            // score lower, if yes, update the parent because it means it's a better path
                            if (g + adjBlock.h < adjBlock.f)
                            {
                                adjBlock.g = g;
                                adjBlock.f = adjBlock.g + adjBlock.h;
                                adjBlock.parent = current;
                            }
                        }
                    }
                }
            }

            return current;
        }

        static List<Location> CheckIfActuallyConnected(List<Location> blocks, Location currentBlock,
            MapBlockData[,] map)
        {
            MapCoords curMapCoords = currentBlock.coords.mapCoords;
            MapBlockData blockData = map[curMapCoords.x, curMapCoords.z];

            if (blockData != null)
            {
                Block block = blockData.block;
                if (block)
                {
                    return blocks.Where(adjBlock => block.connectionPoints.Any(conPoint =>
                        conPoint.connection && conPoint.connection.parentBlock ==
                        map[adjBlock.coords.mapCoords.x, adjBlock.coords.mapCoords.z].block)).ToList();
                }   
            }

            return new List<Location>();
        }

        static List<Location> GetWalkableAdjacentBlocks(Coords coords, MapBlockData[,] map)
        {
            if (coords.blockCoords != null)
            {
                int x = coords.mapCoords.x;
                int z = coords.mapCoords.z;
                Vector3 blockPos = coords.blockCoords.Value;

                int fDimL = map.GetLength(0);
                int sDimL = map.GetLength(1);

                int checkX = x;
                int checkZ = z - 1;

                List<Location> proposedLocations = new List<Location>();

                if (CheckIfViableAdj(checkX, checkZ, map, fDimL, sDimL))
                {
                    proposedLocations.Add(new Location(new Coords(new MapCoords(checkX, checkZ),
                        map[checkX, checkZ].blockPos)));
                }

                checkZ = z + 1;

                if (CheckIfViableAdj(checkX, checkZ, map, fDimL, sDimL))
                {
                    proposedLocations.Add(new Location(new Coords(new MapCoords(checkX, checkZ),
                        map[checkX, checkZ].blockPos)));
                }

                checkX = x - 1;
                checkZ = z;

                if (CheckIfViableAdj(checkX, checkZ, map, fDimL, sDimL))
                {
                    proposedLocations.Add(new Location(new Coords(new MapCoords(checkX, checkZ),
                        map[checkX, checkZ].blockPos)));
                }

                checkX = x + 1;

                if (CheckIfViableAdj(checkX, checkZ, map, fDimL, sDimL))
                {
                    proposedLocations.Add(new Location(new Coords(new MapCoords(checkX, checkZ),
                        map[checkX, checkZ].blockPos)));
                }

                return proposedLocations;
            }
            else return new List<Location>();
        }

        static bool CheckIfViableAdj(int x, int z, MapBlockData[,] map, int fDimL, int sDimL)
        {
            return x >= 0 && z >= 0 && x < fDimL &&
                   z < sDimL && map[x, z] != null &&
                   map[x, z].block != null &&
                   map[x, z].block.isWalkable;
        }

        static int ComputeHScore(Coords coords, Coords targetCoords)
        {
            int x = coords.mapCoords.x;
            int z = coords.mapCoords.z;

            int targetX = targetCoords.mapCoords.x;
            int targetZ = targetCoords.mapCoords.z;

            return Math.Abs(targetX - x) + Math.Abs(targetZ - z);
        }
    }
}

public class PathFinder_old : MonoBehaviour
{
    [SerializeField] private float checkBlockUnderMaxDistance = 3f;

    [NonSerialized] public MapData mapData;

    [NonSerialized] public Coords? currBlockCoords;

    private const int CostToMove = 1;

    private bool _isCalc = false;

    private void Update()
    {
        GetBlockPosition();
    }

    private void GetBlockPosition()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, checkBlockUnderMaxDistance,
            LayerMask.NameToLayer("Map")))
        {
            Block hitBlock = hit.transform.GetComponent<Block>();
            if (hitBlock)
            {
                MapBlockData mapBlockData = hitBlock.thisBlocksMapData;
                if (mapBlockData != null)
                {
                    if (mapBlockData.mapCoords.HasValue)
                        currBlockCoords = new Coords(mapBlockData.mapCoords.Value, mapBlockData.blockPos);
                    else currBlockCoords = null;
                }
                else currBlockCoords = null;
            }
            else currBlockCoords = null;
        }
        else currBlockCoords = null;
    }

    public void GetMovementInstructions(MapBlockData blockData, Player player)
    {
        if (_isCalc) return;

        _isCalc = true;

        if (currBlockCoords.HasValue && blockData.mapCoords.HasValue && blockData.block != null &&
            mapData != null && mapData.map != null && player)
        {
            if (currBlockCoords.Value.mapCoords.Equals(blockData.mapCoords)) goto SkipMove;
            
            Block startBlock = null;
            
            MapBlockData startBlockData =
                mapData.map[currBlockCoords.Value.mapCoords.x, currBlockCoords.Value.mapCoords.z];
            if (startBlockData != null) startBlock = startBlockData.block;

            Location startingBlock = PahtfindAlgo.FindShortestPath(
                new Coords(blockData.mapCoords.Value, blockData.blockPos),
                currBlockCoords.Value,
                mapData.map
            );

            Block startBlockAlgo = null;
            
            if (startingBlock != null)
            {
                MapBlockData start2BlockData =
                    mapData.map[startingBlock.coords.mapCoords.x, startingBlock.coords.mapCoords.z];
                if (startBlockData != null) startBlockAlgo = start2BlockData.block;
            }

            if (startBlock && startBlockAlgo)
            {
                if (startBlock == startBlockAlgo) player.MovePath(startingBlock);
            }
        }
        
        SkipMove:

        _isCalc = false;
    }
}*/