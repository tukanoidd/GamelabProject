using System;
using System.Collections.Generic;
using System.Linq;
using DataTypes;
using UnityEngine;

public class MapBuilder : MonoBehaviour
{
    //---------Public and Private Visible In Inspector---------\\
    [SerializeField] private List<GravitationalPlane> gravitationalPlanes = new List<GravitationalPlane>();
    //---------Public and Private Visible In Inspector---------\\

    //--------Private and Public Invisible In Inspector--------\\
    private MapData _pathFindingMapsData;

    private MapBlockData[] _blockDatas;
    //--------Private and Public Invisible In Inspector--------\\

    private void Start()
    {
        if (Application.isPlaying)
        {
            _blockDatas = FindObjectsOfType<Block>().Where(block => block.transform.parent.CompareTag("MapBuild"))
                .Select(block => new MapBlockData(new MapLocation(0, 0), block.transform.position, block)).ToArray();
            GenerateMap();
        }
    }

    private void GenerateMap()
    {
        _pathFindingMapsData = new MapData();
        
        if (_blockDatas.Length > 0)
        {
            foreach (GravitationalPlane gravitationalPlane in gravitationalPlanes)
            {
                _pathFindingMapsData.CreateMap(_blockDatas, gravitationalPlane);
            }
        }
    }
}