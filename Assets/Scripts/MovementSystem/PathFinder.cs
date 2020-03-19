using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    struct Coords
    {
        public MapCoords mapCoords;
        public Vector3? blockCoords;

        public Coords(MapCoords mapCoords, Vector3? blockCoords)
        {
            this.mapCoords = mapCoords;
            this.blockCoords = blockCoords;
        }
    }

    class ShortestPathStep
    {
        public Coords coords;
        public int gScore;
        public int hScore;
        public ShortestPathStep parent;

        public ShortestPathStep(MapCoords pos, Vector3? realPos = null)
        {
            coords.mapCoords = pos;
            coords.blockCoords = realPos.HasValue ? realPos.Value : Vector3.zero;
            gScore = 0;
            hScore = 0;
            parent = null;
        }

        public int FScore => gScore + hScore;

        bool IsEqual(ShortestPathStep other) => coords.mapCoords.Equals(other.coords.mapCoords);
    }

    [SerializeField] private float checkBlockUnderMaxDistance = 3f;

    [NonSerialized] public MapData mapData;

    private Coords? _coords;

    private List<ShortestPathStep> _openSteps;
    private List<ShortestPathStep> _closedSteps;

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
                        _coords = new Coords(mapBlockData.mapCoords.Value, mapBlockData.blockPos);
                    else _coords = null;
                }
                else _coords = null;
            }
            else _coords = null;
        }
        else _coords = null;
    }

    public void MoveToward(MapBlockData target)
    {
        if (_isCalc) return;

        _isCalc = true;
        
        if (!_coords.HasValue) return;
        if (!target.mapCoords.HasValue) return;

        MapCoords targetPos = target.mapCoords.Value;

        if (targetPos.Equals(_coords.Value.mapCoords)) return;
        if (!target.block.isWalkable) return;

        bool pathFound = false;
        _openSteps = new List<ShortestPathStep>();
        _closedSteps = new List<ShortestPathStep>();

        _openSteps.Add(new ShortestPathStep(_coords.Value.mapCoords, _coords.Value.blockCoords));

        do
        {
            ShortestPathStep currentStep = _openSteps[0];
            _closedSteps.Add(currentStep);

            _openSteps.RemoveAt(0);

            if (currentStep.coords.mapCoords.Equals(targetPos))
            {
                pathFound = true;
                ShortestPathStep tmpStep = currentStep;
                Debug.Log("path found: ");
                do
                {
                    Debug.Log(tmpStep.coords.mapCoords.ToVector2());
                    tmpStep = tmpStep.parent;
                } while (tmpStep != null);

                _openSteps = null;
                _closedSteps = null;
                break;
            }

            MapCoords[] adjSteps = WalkableAdjacentBlockCoordForBlockCoord(currentStep.coords.mapCoords);
            foreach (MapCoords v in adjSteps)
            {
                ShortestPathStep step = new ShortestPathStep(v, GetBlockPosFromMapCoords(v));

                if (_closedSteps.Contains(step)) continue;

                int index = _openSteps.IndexOf(step);

                if (_openSteps.ElementAtOrDefault(index) == null)
                {
                    step.parent = currentStep;

                    step.gScore = currentStep.gScore + CostToMove;
                    step.hScore = ComputeHScoreFromCoord(step.coords.mapCoords, targetPos);

                    InsertInOpenSteps(step);
                }
                else
                {
                    step = _openSteps[index];

                    if (currentStep.gScore + CostToMove < step.gScore)
                    {
                        step.gScore = currentStep.gScore + CostToMove;
                        _openSteps.RemoveAt(index);
                        InsertInOpenSteps(step);
                    }
                }
            }
        } while (_openSteps.Any());

        if (!pathFound) Debug.Log("No Path");

        _isCalc = false;
    }

    private void InsertInOpenSteps(ShortestPathStep step)
    {
        int stepFScore = step.FScore;
        int count = _openSteps.Count();
        int i = 0;

        for (; i < count; i++)
        {
            if (stepFScore <= _openSteps[i].FScore) break;
        }

        _openSteps.Insert(i, step);
    }

    private int ComputeHScoreFromCoord(MapCoords fromCoord, MapCoords toCoord) =>
        Mathf.Abs(toCoord.x - fromCoord.x) + Mathf.Abs(toCoord.z - fromCoord.z);

    private MapCoords[] WalkableAdjacentBlockCoordForBlockCoord(MapCoords blockCoord)
    {
        MapCoords[] tmp = new MapCoords[4];

        if (mapData != null)
        {
            // Top
            MapCoords p = new MapCoords(blockCoord.x - 1, blockCoord.z);
            if (IsValidBlock(p)) tmp[0] = p;

            // Right
            p = new MapCoords(blockCoord.x, blockCoord.z + 1);
            if (IsValidBlock(p)) tmp[1] = p;

            // Bottom
            p = new MapCoords(blockCoord.x + 1, blockCoord.z);
            if (IsValidBlock(p)) tmp[2] = p;

            // Left
            p = new MapCoords(blockCoord.x, blockCoord.z - 1);
            if (IsValidBlock(p)) tmp[3] = p;
        }

        return tmp;
    }

    private bool IsValidBlock(MapCoords p)
    {
        if (mapData.height > p.x && p.x >= 0 && mapData.length > p.z && p.z >= 0)
        {
            MapBlockData blockDataOnMap = mapData.map[p.x, p.z];
            if (blockDataOnMap != null)
            {
                Block blockOnMap = blockDataOnMap.block;
                if (blockOnMap) return blockOnMap.isWalkable;
                else return false;
            }
            else return false;
        }
        else return false;
    }

    private Vector3? GetBlockPosFromMapCoords(MapCoords p)
    {
        if (IsValidBlock(p)) return mapData.map[p.x, p.z].blockPos;
        else return null;
    }
}