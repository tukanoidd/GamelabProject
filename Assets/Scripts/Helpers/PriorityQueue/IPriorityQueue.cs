using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Source: https://github.com/mortennobel/UnityUtils/blob/master/Assets/UnityUtils/ThirdParty/Priority Queue/IPriorityQueue.cs

/// <summary>
/// The IPriorityQueue interface.  This is mainly here for purists, and in case I decide to add more implementations later.
/// For speed purposes, it is actually recommended that you *don't* access the priority queue through this interface, since the JIT can
/// (theoretically?) optimize method calls from concrete-types slightly better.
/// </summary>
public interface IPriorityQueue<TItem, in TPriority> : IEnumerable<TItem> where TPriority : IComparable<TPriority>
{
    /// <summary>
    /// Enqueue a not to the priority queue.
    /// Lower values are placed in front.
    /// Ties are broken by first-in-first-out 
    /// </summary>
    void Enqueue(TItem node, TPriority priority);

    /// <summary>
    /// Removes the head of the queue (node with minimum priority;
    /// ties are broken by order of insertion), and returns it
    /// </summary>
    TItem Dequeue();

    /// <summary>
    /// Removes every node from the queue 
    /// </summary>
    void Clear();

    /// <summary>
    /// Return whether the given node is in the queue 
    /// </summary>
    /// <param name="node">Node to search for</param>
    /// <returns>If the node inside the PriorityQueue or not</returns>
    bool Contains(TItem node);

    /// <summary>
    /// Removes a node from the queue. The node does not need to be the head of queue 
    /// </summary>
    /// <param name="node">Node to search for</param>
    void Remove(TItem node);

    /// <summary>
    /// Call this method to change the priority of the node 
    /// </summary>
    /// <param name="node">Node to search for</param>
    /// <param name="priority">Priority to set to the node</param>
    void UpdatePriority(TItem node, TPriority priority);

    /// <summary>
    /// Returns the head of the queue, without removing it (use Dequeue() for that) 
    /// </summary>
    TItem First { get; }

    /// <summary>
    /// Returns the number of the nodes in the queue
    /// </summary>
    int Count { get; }
}
