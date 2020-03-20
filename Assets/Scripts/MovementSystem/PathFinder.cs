using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// General logic for pathfinding was made thanks to this
// https://blog.nobel-joergensen.com/2011/02/26/a-path-finding-algorithm-in-unity/ blog post

// Interface for a shortest path problem
public interface IShortestPath<TState, TAction>
{
    /**
     * Should return an estimate of a shortest distance.
     * The estimate must be admissible (never overestimate)
     */
    float Heuristic(TState fromLocation, TState toLocation);

    // Return the legal moves from a state
    List<TAction> Expand(TState position);

    // Return the actual cost between two adjacent locations
    float ActualCost(TState fromLocation, TAction action);

    // Returns the new state after an action has been applied
    TState ApplyAction(TState location, Action action);
}

public class ShortestPathGraphSearch<TState, TAction>
{
    class SearchNode<TState, TAction> : IComparable<SearchNode<TState, TAction>>
    {
        public SearchNode<TState, TAction> parent;
        public TState state;
        public TAction action;
        public float g; // cost
        public float f; // estimate

        public SearchNode(SearchNode<TState, TAction> parent, float g, float f, TState state, TAction action)
        {
            this.parent = parent;
            this.g = g;
            this.f = f;
            this.state = state;
            this.action = action;
        }

        // Reverse sort order (smallest numbers first)
        public int CompareTo(SearchNode<TState, TAction> other)
        {
            return other.f.CompareTo(f);
        }

        public override string ToString()
        {
            return "SN {f:" + f + ", state: " + state + " action: " + action + "}";
        }
    }

    private IShortestPath<TState, TAction> info;

    public ShortestPathGraphSearch(IShortestPath<TState, TAction> info)
    {
        this.info = info;
    }

    /*public List<TAction> GetShortestPath(TState fromState, TState toState)
    {
        PriorityQueue<float, SearchNode<TState, TAction>>
            frontier = new PriorityQueue<float, SearchNode<TState, TAction>>();
        
        HashSet<TState> exploredSet = new HashSet<TState>();
        
        Dictionary<TState, SearchNode<TState, TAction>> frontierMap = new Dictionary<TState, SearchNode<TState, TAction>>();
        
        SearchNode<TState, TAction> startNode = new SearchNode<TState, TAction>(null, 0, 0, fromState, default(TAction));
        
        frontier.Enqueue(startNode, 0);
        frontierMap.Add(fromState, startNode);
        
        while (true)
        {
            if (frontier.IsEmpty) return null;
            
            SearchNode<TState, TAction> node = frontier.Dequeue();
            
            if (node.state.Equals(toState)) return BuildSolution(node);
            
            exploredSet.Add(node.state);
            
            // expand node and add to frontier
            foreach (TAction action in info.Expand(node.state))
            {
                TState child = info.ApplyAction(node.state, action);
                
                SearchNode<TState, TAction> frontierNode = null;
                
                bool isNodeInFrontier = frontierMap.TryGetValue(child, out frontierNode);
                
                if (!exploredSet.Contains(child) && !isNodeInFrontier)
                {
                    SearchNode<TState, TAction> searchNode = CreateSearchNode(node, action, child, toState);
                    frontier.Enqueue(searchNode, searchNode.f);
                    exploredSet.Add(child);
                }
                else if (isNodeInFrontier)
                {
                    SearchNode<TState, TAction> searchNode = CreateSearchNode(node, action, child, toState);
                    
                    if (frontierNode.f > searchNode.f) frontier.Replace(frontierNode, frontierNode.f, searchNode.f);
                }
            }
        }
    }*/

    private SearchNode<TState, TAction> CreateSearchNode(SearchNode<TState, TAction> node, TAction action, TState child,
        TState toState)
    {
        float cost = info.ActualCost(node.state, action);
        float heuristic = info.Heuristic(child, toState);
        return new SearchNode<TState, TAction>(node, node.g + cost, node.g + cost + heuristic, child, action);
    }

    private List<TAction> BuildSolution(SearchNode<TState, TAction> seachNode)
    {
        List<TAction> list = new List<TAction>();
        while (seachNode != null)
        {
            if ((seachNode.action != null) && (!seachNode.action.Equals(default(TAction))))
            {
                list.Insert(0, seachNode.action);
            }

            seachNode = seachNode.parent;
        }

        return list;
    }
}

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

    [SerializeField] private float checkBlockUnderMaxDistance = 3f;

    [NonSerialized] public MapData mapData;

    private Coords? _coords;

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
}