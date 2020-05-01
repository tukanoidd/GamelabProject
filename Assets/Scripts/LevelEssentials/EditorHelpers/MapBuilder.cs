using System.Collections.Generic;
using System.Linq;
using DataTypes;
using UnityEngine;
using Plane = DataTypes.Plane;

public class MapBuilder : MonoBehaviour
{
    //---------Public and Private Visible In Inspector---------\\
    [SerializeField] private List<GravitationalPlane> gravitationalPlanes = new List<GravitationalPlane>();

    public bool PathFindingMapsDataExists => _pathFindingMapsData != null;
    public bool MapRepresentationExists => _mapRepresentation != null;

    public Plane[] AvailablePlanes => _pathFindingMapsData.maps.Select(map => map.Key.plane).ToArray();

    public PlaneSide[] AvailablePlaneSides(Plane availablePlane) =>
        _pathFindingMapsData.maps.Where(map => map.Key.plane == availablePlane).Select(map => map.Key.planeSide).ToArray();
    //---------Public and Private Visible In Inspector---------\\

    //--------Private and Public Invisible In Inspector--------\\
    private MapData _pathFindingMapsData;

    private Block[] _blocks;

    private GameObject _mapRepresentation;

#if UNITY_EDITOR
    [HideInInspector] public GravitationalPlane showMap;
#endif
    //--------Private and Public Invisible In Inspector--------\\

    private void Start()
    {
        if (Application.isPlaying)
        {
            _blocks = FindObjectsOfType<Block>().Where(block => block.transform.parent.CompareTag("MapBuild")).ToArray();
            GenerateMaps();
        }
    }

    private void GenerateMaps()
    {
        _pathFindingMapsData = new MapData();

        if (_blocks.Length > 0)
        {
            foreach (GravitationalPlane gravitationalPlane in gravitationalPlanes)
            {
                _pathFindingMapsData.CreateMap(_blocks, gravitationalPlane);
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
        if (showMap == null || _pathFindingMapsData.maps[showMap].Length <= 0) return;

        _mapRepresentation = new GameObject("Map Represenation " + showMap.plane + " " + showMap.planeSide);
        _mapRepresentation.transform.position = Vector3.up * 50;

        HashSet<MapBlockData>[,] mapToShow = _pathFindingMapsData.maps[showMap];

        foreach (HashSet<MapBlockData> blockDatas in mapToShow)
        {
            if (blockDatas == null) continue;
            
            int i = 0;
            foreach (MapBlockData blockData in blockDatas)
            {
                GameObject blockRepresenation = Instantiate(blockData.block.gameObject, _mapRepresentation.transform, true);

                switch (showMap.plane)
                {
                    case Plane.XY:
                        blockRepresenation.transform.localPosition = new Vector3(
                            blockData.mapLoc.col,
                            blockData.mapLoc.row,
                            i * Block.size.z * 2 * GravitationalPlane.PlaneSideToInt(showMap.planeSide)
                        );
                        break;
                    case Plane.XZ:
                        blockRepresenation.transform.localPosition = new Vector3(
                            blockData.mapLoc.col,
                            i * Block.size.y * 2 * GravitationalPlane.PlaneSideToInt(showMap.planeSide),
                            blockData.mapLoc.row
                        );
                        break;
                    case Plane.YZ:
                        blockRepresenation.transform.localPosition = new Vector3(
                            blockData.mapLoc.row,
                            i * Block.size.y * 2 * GravitationalPlane.PlaneSideToInt(showMap.planeSide),
                            blockData.mapLoc.col
                        );
                        break;
                }
            }
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