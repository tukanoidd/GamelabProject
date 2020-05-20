using System;
using System.Collections.Generic;
using System.Linq;
using DataTypes;
using UnityEngine;
using Plane = DataTypes.Plane;

public class MapBuilder : MonoBehaviour
{
    //---------Public and Private Visible In Inspector---------\\
    [SerializeField] private int mapHeight = 100;
    [SerializeField] private int mapLength = 100;
    
    [SerializeField] private List<GravitationalPlane> gravitationalPlanes = new List<GravitationalPlane>();
    [SerializeField] private float mapRepresentationUpOffset = 10;
    
    public MapData pathFindingMapsData;

    public bool PathFindingMapsDataExists => pathFindingMapsData != null;
    public bool MapRepresentationExists => _mapRepresentation != null;

    public Plane[] AvailablePlanes => pathFindingMapsData.maps.Select(map => map.Key.plane).ToArray();

    public PlaneSide[] AvailablePlaneSides(Plane availablePlane) =>
        pathFindingMapsData.maps.Where(map => map.Key.plane == availablePlane).Select(map => map.Key.planeSide)
            .ToArray();
    //---------Public and Private Visible In Inspector---------\\

    //--------Private and Public Invisible In Inspector--------\\

    private Block[] _blocks;

    private GameObject _mapRepresentation;

#if UNITY_EDITOR
    [HideInInspector] public GravitationalPlane showMapGravitationalPlane;
#endif
    //--------Private and Public Invisible In Inspector--------\\

    private void Start()
    {
        if (Application.isPlaying)
        {
            _blocks = FindObjectsOfType<Block>().Where(block => block.transform.parent.CompareTag("MapBuild"))
                .ToArray();
            GenerateMaps();
        }
    }

    private void GenerateMaps()
    {
        pathFindingMapsData = new MapData(mapLength, mapHeight);

        if (_blocks.Length > 0)
        {
            foreach (GravitationalPlane gravitationalPlane in gravitationalPlanes)
            {
                pathFindingMapsData.CreateMap(_blocks, gravitationalPlane);
            }
        }
    }

    public GameObject AddMapPartBuilder()
    {
        GameObject newMapPartBuilder = new GameObject("MapPartBuilder " +
                                                      (FindObjectsOfType<MapPartBuilder>().Length + 1));
        newMapPartBuilder.tag = "MapBuild";
        newMapPartBuilder.AddComponent<MapPartBuilder>();
        newMapPartBuilder.transform.SetParent(transform);

        return newMapPartBuilder;
    }

#if UNITY_EDITOR
    public void ShowMap()
    {
        if (showMapGravitationalPlane == null ||
            pathFindingMapsData.maps[showMapGravitationalPlane].Length <= 0) return;

        _mapRepresentation = new GameObject("Map Represenation " + showMapGravitationalPlane.plane + " " +
                                            showMapGravitationalPlane.planeSide);
        _mapRepresentation.transform.position = Vector3.up * mapRepresentationUpOffset;

        HashSet<MapBlockData>[,] mapToShow = pathFindingMapsData.maps[showMapGravitationalPlane];

        int rows = mapToShow.GetLength(0);
        int cols = mapToShow.GetLength(1);

        foreach (HashSet<MapBlockData> blockDatas in mapToShow)
        {
            if (blockDatas == null) continue;

            int i = 0;
            
            foreach (MapBlockData blockData in blockDatas)
            {
                GameObject blockRepresenation =
                    Instantiate(blockData.block.gameObject, _mapRepresentation.transform, true);

                int row = blockData.mapLoc.row - rows / 2;
                int col = blockData.mapLoc.col - cols / 2;
                
                blockRepresenation.transform.localPosition = DebugMapRepresentationBlockLocalPosition(
                    showMapGravitationalPlane.plane,
                    col,
                    row,
                    i
                );

                i++;
            }
        }
    }

    private Vector3 DebugMapRepresentationBlockLocalPosition(Plane plane, int col, int row, int i)
    {
        switch (plane)
        {
            case Plane.XY:
                return new Vector3(
                    col,
                    row,
                    i * Block.size.z * 2 *
                    GravitationalPlane.PlaneSideToInt(showMapGravitationalPlane.planeSide)
                );
            case Plane.XZ:
                return new Vector3(
                    col,
                    i * Block.size.y * 2 *
                    GravitationalPlane.PlaneSideToInt(showMapGravitationalPlane.planeSide),
                    row
                );
            case Plane.YZ:
                return new Vector3(
                    i * Block.size.x * 2 *
                    GravitationalPlane.PlaneSideToInt(showMapGravitationalPlane.planeSide),
                    row,
                    col
                );
            default: return Vector3.zero;
        }
    }

    public void RemoveMap()
    {
        if (_mapRepresentation)
        {
            foreach (Transform blockRepresentation in _mapRepresentation.transform)
            {
                DestroyImmediate(blockRepresentation.gameObject);
            }

            DestroyImmediate(_mapRepresentation);
        }
    }
#endif
}