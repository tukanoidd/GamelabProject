using System;
using System.Collections.Generic;
using System.Linq;
using DataTypes;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    public List<PathFindingLocation> FindShortestPath(Block startBlock, Block targetBlock, GravitationalPlane gravitationalPlane)
    {
        HashSet<MapBlockData>[,] map = GameManager.current.mapBuilder.pathFindingMapsData.maps[gravitationalPlane];
        
        if (map == null) return new List<PathFindingLocation>();

        GameManager.current.cameraLockedMovement = true;
        GameManager.current.playerLockedMovement = true;

        List<List<PathFindingLocation>> pathVariations = new List<List<PathFindingLocation>>();

        List<MapBlockData> startblockDatas = startBlock.mapBlockDatas[gravitationalPlane].ToList();
        List<MapBlockData> targetBlockDatas = targetBlock.mapBlockDatas[gravitationalPlane].ToList();

        PathFindingLocation current, start, target;

        List<PathFindingLocation> openList, closedList;

        BlockConnection connection;

        int lowest;

        foreach (MapBlockData startBlockData in startblockDatas)
        {
            start = new PathFindingLocation(startBlockData, startBlockData.mapLoc);

            foreach (MapBlockData targetBlockData in targetBlockDatas)
            {
                current = null;

                target = new PathFindingLocation(targetBlockData, targetBlockData.mapLoc);

                openList = new List<PathFindingLocation>();
                closedList = new List<PathFindingLocation>();

                int g = 0;

                openList.Add(start);

                while (openList.Any())
                {
                    // Get the location with the lowest F score
                    lowest = openList.Min(l => l.f);
                    current = openList.First(l => l.f == lowest);

                    // Add the current location to the closed list
                    closedList.Add(current);

                    // Remove it from the open list
                    openList.Remove(current);

                    // If we added the destination to the closed list, we've found the path
                    if (closedList.Any(l => l.mapLoc.Equals(target.mapLoc)))
                    {
                        AddPossiblePath(ref pathVariations, current);
                        break;
                    }


                    IEnumerable<HashSet<MapBlockData>> adjBlocks = GetAdjacentBlocks(current.mapLoc, map)
                        .Select(mapLoc => map[mapLoc.row, mapLoc.col]);

                    List<List<PathFindingLocation>> viableAdjacentBlocks = new List<List<PathFindingLocation>>();
                    foreach (HashSet<MapBlockData> blockDatas in adjBlocks)
                    {
                        Dictionary<MapBlockData, BlockConnection> checkBlockDatas = new Dictionary<MapBlockData, BlockConnection>();
                        foreach (MapBlockData blockData in blockDatas)
                        {
                            if (GameManager.current.ConnectionExists(blockData.block, current.mapBlockData.block,
                                gravitationalPlane, out connection))
                            {
                                checkBlockDatas.Add(blockData, connection);
                            }
                        }
                        if (checkBlockDatas.Count > 0)
                        {
                            viableAdjacentBlocks.Add(checkBlockDatas
                                .Select(blockDataConnection => new PathFindingLocation(blockDataConnection.Key, blockDataConnection.Key.mapLoc, blockDataConnection.Value)).ToList());
                        }
                    }

                    g = current.g + 1;

                    foreach (List<PathFindingLocation> adjBlocksLocations in viableAdjacentBlocks)
                    {
                        foreach (PathFindingLocation adjBlockLocation in adjBlocksLocations)
                        {
                            // If this adjacent block is already in the closed list, ignore
                            if (closedList.Any(l => l.mapLoc.Equals(adjBlockLocation.mapLoc))) continue;
                            
                            // If it's not in the open list ...
                            if (!openList.Any(l => l.mapLoc.Equals(adjBlockLocation.mapLoc)))
                            {
                                // Compute its score, set the parent
                                adjBlockLocation.g = g;
                                adjBlockLocation.h = ComputeHScore(adjBlockLocation.mapLoc, target.mapLoc);
                                adjBlockLocation.f = adjBlockLocation.g + adjBlockLocation.h;
                                adjBlockLocation.parent = current;
                                current.connection = adjBlockLocation.connection;
                                
                                // And add it to the open list
                                openList.Insert(0, adjBlockLocation);
                            }
                            else
                            {
                                // Test if using the current G score makes the adjacent block's F
                                // score lower, if yes, update the parent because it means it's a better path
                                if (g + adjBlockLocation.h < adjBlockLocation.f)
                                {
                                    adjBlockLocation.g = g;
                                    adjBlockLocation.f = adjBlockLocation.g + adjBlockLocation.h;
                                    adjBlockLocation.parent = current;
                                    current.connection = adjBlockLocation.connection;
                                }
                            }
                        }
                    }
                }
            }
        }
        // If no paths variations are presented, return empty list
        if (pathVariations.Count == 0) return new List<PathFindingLocation>();
        // Other wise return the shortest path
        return pathVariations.OrderBy(pathVariant => pathVariant.Count).ToList()[0];
    }

    private void AddPossiblePath(ref List<List<PathFindingLocation>> pathVariations, PathFindingLocation current)
    {
        List<PathFindingLocation> pathVariant = new List<PathFindingLocation>();

        while (current.parent != null)
        {
            pathVariant.Add(current);
            current = current.parent;
        }

        pathVariant.Reverse();
        pathVariations.Add(pathVariant);
    }

    private List<MapLocation> GetAdjacentBlocks(MapLocation currentMapLocation, HashSet<MapBlockData>[,] map)
    {
        int row = currentMapLocation.row;
        int col = currentMapLocation.col;

        int rows = map.GetLength(0);
        int cols = map.GetLength(1);

        List<MapLocation> proposedLocations = new List<MapLocation>();

        AddViableAdjacent(ref proposedLocations, row, col - 1, map, rows, cols);
        AddViableAdjacent(ref proposedLocations, row, col + 1, map, rows, cols);
        AddViableAdjacent(ref proposedLocations, row - 1, col, map, rows, cols);
        AddViableAdjacent(ref proposedLocations, row + 1, col, map, rows, cols);

        return proposedLocations;
    }

    private void AddViableAdjacent(ref List<MapLocation> proposedLocations, int checkRow, int checkCol,
        HashSet<MapBlockData>[,] map, int rows, int cols)
    {
        if (CheckIfViableAdj(checkRow, checkCol, ref map, rows, cols))
        {
            proposedLocations.Add(new MapLocation(checkRow, checkCol));
        }
    }

    private bool CheckIfViableAdj(int checkRow, int checkCol, ref HashSet<MapBlockData>[,] map, int rows, int cols)
    {
        return checkRow >= 0 && checkCol >= 0 &&
               checkRow < rows && checkCol < cols &&
               map[checkRow, checkCol] != null;
    }
    
    private int ComputeHScore(MapLocation mapLoc, MapLocation targetMapLoc)
    {
        int row = mapLoc.row;
        int col = mapLoc.col;

        int targetRow = targetMapLoc.row;
        int targetCol = targetMapLoc.col;

        return Math.Abs(targetRow - row) + Math.Abs(targetCol - col);
    }
}