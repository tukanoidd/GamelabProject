using System;
using System.Collections.Generic;
using System.Linq;
using DataTypes;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    /// <summary>
    /// Function to find a shortest path in from starting block to target block.
    /// Modified code from https://gigi.nullneuron.net/gigilabs/a-pathfinding-example-in-c/
    /// and https://greenday96.blogspot.com/2018/10/c-4-aa-star-simple-4-way-algorithm.html 
    /// </summary>
    /// <param name="startBlock">Starting block</param>
    /// <param name="targetBlock">Block to find path to</param>
    /// <param name="gravitationalPlane">Gravitational plane where player is standing</param>
    /// <returns>A path</returns>
    public List<PathFindingLocation> FindShortestPath(Block startBlock, Block targetBlock,
        GravitationalPlane gravitationalPlane)
    {
        if (startBlock == targetBlock) return new List<PathFindingLocation>();

        HashSet<MapBlockData>[,] map = GameManager.current.mapBuilder.pathFindingMapsData.maps[gravitationalPlane];

        if (map == null) return new List<PathFindingLocation>();

        GameManager.current.cameraLockedMovement = true;
        GameManager.current.playerLockedMovement = true;

        List<List<PathFindingLocation>> pathVariations = new List<List<PathFindingLocation>>();

        List<MapBlockData> startblockDatas = startBlock.mapBlockDatas[gravitationalPlane].ToList();
        List<MapBlockData> targetBlockDatas = targetBlock.mapBlockDatas[gravitationalPlane].ToList();

        PathFindingLocation current, start, target;

        List<PathFindingLocation> openList, closedList;

        int lowest;

        foreach (MapBlockData startBlockData in startblockDatas)
        {
            start = new PathFindingLocation(startBlockData, startBlockData.mapLoc);

            foreach (MapBlockData targetBlockData in targetBlockDatas)
            {
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
                    if (closedList.FirstOrDefault(l =>
                        l.mapLoc.Equals(target.mapLoc) && l.mapBlockData.block == targetBlock && l.connection.Equals(current.connection)) != null)
                    {
                        AddPossiblePath(ref pathVariations, current, gravitationalPlane);
                        break;
                    }


                    IEnumerable<HashSet<MapBlockData>> adjBlocks = GetAdjacentBlocks(current.mapLoc, map)
                        .Select(mapLoc => map[mapLoc.row, mapLoc.col]);

                    List<List<PathFindingLocation>> viableAdjacentBlocks = new List<List<PathFindingLocation>>();
                    foreach (HashSet<MapBlockData> blockDatas in adjBlocks)
                    {
                        Dictionary<MapBlockData, BlockConnection> checkBlockDatas =
                            new Dictionary<MapBlockData, BlockConnection>();
                        foreach (MapBlockData blockData in blockDatas)
                        {
                            BlockConnection checkConnection = GameManager.current.FindConnection(blockData.block,
                                current.mapBlockData.block,
                                gravitationalPlane);
                            if (checkConnection != null)
                            {
                                checkBlockDatas.Add(blockData, checkConnection);
                            }
                        }

                        if (checkBlockDatas.Count > 0)
                        {
                            viableAdjacentBlocks.Add(checkBlockDatas
                                .Select(blockDataConnection => new PathFindingLocation(blockDataConnection.Key,
                                    blockDataConnection.Key.mapLoc, blockDataConnection.Value)).ToList());
                        }
                    }

                    g = current.g + 1;

                    foreach (List<PathFindingLocation> adjBlocksLocations in viableAdjacentBlocks)
                    {
                        foreach (PathFindingLocation adjBlockLocation in adjBlocksLocations)
                        {
                            // If this adjacent block is already in the closed list, ignore
                            if (closedList.FirstOrDefault(l =>
                                l.mapLoc.Equals(adjBlockLocation.mapLoc) &&
                                l.mapBlockData.block == adjBlockLocation.mapBlockData.block &&
                                l.connection.Equals(adjBlockLocation.connection)) != null)
                                continue;

                            // If it's not in the open list ...
                            if (openList.FirstOrDefault(l =>
                                l.mapLoc.Equals(adjBlockLocation.mapLoc) &&
                                l.mapBlockData.block == adjBlockLocation.mapBlockData.block &&
                                l.connection.Equals(adjBlockLocation.connection)) == null)
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
        
        pathVariations = pathVariations.Where(pathVar =>
                pathVar.Any(loc => loc.mapBlockData.block == targetBlock) && pathVar.Count() > 1)
            .ToList();

        // If no paths variations are presented, return empty list
        if (pathVariations.Count == 0) return new List<PathFindingLocation>();

        // Other wise return the shortest path
        return pathVariations.OrderByDescending(pathVariant => pathVariant.Count(loc => loc.connection.isNear))
            .ToList()[0];
    }

    private void AddPossiblePath(ref List<List<PathFindingLocation>> pathVariations, PathFindingLocation current,
        GravitationalPlane gravitationalPlane)
    {
        List<PathFindingLocation> pathVariant = new List<PathFindingLocation>();

        while (current != null)
        {
            pathVariant.Add(current);
            current = current.parent;
        }

        pathVariant.Reverse();
        UpdatePathConnections(ref pathVariant, gravitationalPlane);

        pathVariations.Add(pathVariant);
    }

    private void UpdatePathConnections(ref List<PathFindingLocation> pathVariant, GravitationalPlane gravitationalPlane)
    {
        int pathLen = pathVariant.Count();

        for (int i = 0; i < pathVariant.Count - 1; i++)
        {
            pathVariant[i].connection = GameManager.current.FindConnection(
                pathVariant[i].mapBlockData.block,
                pathVariant[i + 1].mapBlockData.block,
                gravitationalPlane
            );
        }

        if (pathLen > 1)
            pathVariant[pathLen - 1].connection = GameManager.current.FindConnection(
                pathVariant[pathLen - 1].mapBlockData.block,
                pathVariant[pathLen - 2].mapBlockData.block,
                gravitationalPlane
            );
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