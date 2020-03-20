using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;

//Source: https://github.com/mortennobel/UnityUtils/blob/master/Assets/UnityUtils/ThirdParty/Priority Queue/GenericPriorityQueueNode.cs

public class GenericPriorityQueueNode<TPriority>
{
    /// <summary>
    /// The Priority to insert this node at.
    /// Must be set BEFORE adding a node to the queue (ideally just once, in the node's constructor).
    /// Should not be manually edited once the node has been enqueued - use queue.UpdatePriority() instead
    /// </summary>
    public TPriority Priority { get; protected internal set; }

    /// <summary>
    /// Represents the current position in the queue
    /// </summary>
    public int QueueIndex { get; internal set; }

    /// <summary>
    /// Represents the order the node was inserted in
    /// </summary>
    public long InsertionIndex { get; internal set; }
}


//Source: https://github.com/mortennobel/UnityUtils/blob/master/Assets/UnityUtils/ThirdParty/Priority Queue/GenericPriorityQueue.cs

/// <typeparam name="TItem">
/// The values are in queue. Must extend the GenericQueueNode class
/// </typeparam>
/// <typeparam name="TPriority">
/// The priority-type. Must extend IComparable&lt;TPriority&gt;
/// </typeparam>
public class GenericPriorityQueue<TItem, TPriority> : IFixedSizePriorityQueue<TItem, TPriority>
    where TItem : GenericPriorityQueueNode<TPriority>
    where TPriority : IComparable<TPriority>
{
    private int _numNodes;
    private TItem[] _nodes;
    private long _numNodesEverEnqueued;

    /// <summary>
    /// Instantiate a new Priority Queue
    /// </summary>
    /// <param name="maxNodes">
    /// The max nodes ever allowed to be enqueued (going over this will cause undefined behavior) 
    /// </param>
    public GenericPriorityQueue(int maxNodes)
    {
#if DEBUG
        if (maxNodes <= 0) throw new InvalidOperationException("New queue cannot be smaller than 1");
#endif

        _numNodes = 0;
        _nodes = new TItem[maxNodes + 1];
        _numNodesEverEnqueued = 0;
    }

    /// <summary>
    /// Returns the number of nodes in the queue.
    /// O(1)
    /// </summary>
    public int Count => _numNodes;

    /// <summary>
    /// Return the maximum number of items that can be enqueued at once in this queue.
    /// Once you hit this number (ie. once Count == MaxSize),
    /// attempting to enqueue another item will cause undefined behavior.
    /// O(1)
    /// </summary>
    public int MaxSize => _nodes.Length - 1;
    
    /// <summary>
    /// Removes every node from the queue.
    /// O(n) (So, don't do this often)
    /// </summary>
#if NET_VERSION_4_5
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public void Clear()
    {
        Array.Clear(_nodes, 1, _numNodes);
        _numNodes = 0;
    }

    /// <summary>
    /// Returns whether the given node is in the queue.
    /// O(1)
    /// </summary>
    /// <param name="node">Node to look for</param>
    /// <returns></returns>
#if NET_VERSION_4_5
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public bool Contains(TItem node)
    {
#if DEBUG
        if (node == null) throw new ArgumentNullException("node");

        if (node.QueueIndex < 0 || node.QueueIndex >= _nodes.Length)
            throw new InvalidOperationException(
                "node.QueueIndex has been corrupted. Did you change it manually? Or add this node to another queue?"
            );
#endif

        return _nodes[node.QueueIndex] == node;
    }

    /// <summary>
    /// Enqueue a node to the priority queue.
    /// Lower values are placed in front.
    /// Ties are broken by first-in-first-out.
    /// If the queue is full, the result is undefined.
    /// O(log n)
    /// </summary>
    /// <param name="node">Node to enqueue</param>
    /// <param name="priority">Priority of the node</param>
#if NET_VERSION_4_5
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public void Enqueue(TItem node, TPriority priority)
    {
#if DEBUG
        if (node == null) throw new ArgumentException("node");
        if (_numNodes >= _nodes.Length - 1)
            throw new InvalidOperationException("Queue is full - node cannot be added: " + node);
        if (Contains(node)) throw new InvalidOperationException("Node is already enqueued: " + node);
#endif

        node.Priority = priority;
        _numNodes++;
        _nodes[_numNodes] = node;
        node.QueueIndex = _numNodes;
        node.InsertionIndex = _numNodesEverEnqueued++;
        CascadeUp(_nodes[_numNodes]);
    }

    /// <summary>
    /// Swap two nodes in queue
    /// </summary>
    /// <param name="node1">First node</param>
    /// <param name="node2">Second node</param>
#if NET_VERSION_4_5
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    private void Swap(TItem node1, TItem node2)
    {
        //Swap the nodes
        _nodes[node1.QueueIndex] = node2;
        _nodes[node2.QueueIndex] = node1;

        //Swap their indices
        int temp = node1.QueueIndex;
        node1.QueueIndex = node2.QueueIndex;
        node2.QueueIndex = temp;
    }

    private void CascadeUp(TItem node)
    {
        // aka Heapify-up
        int parent = node.QueueIndex / 2;
        while (parent >= 1)
        {
            TItem parentNode = _nodes[parent];
            if (HasHigherPriority(parentNode, node)) break;

            // Node has lower priority value, so move it up the heap
            Swap(node, parentNode);

            parent = node.QueueIndex / 2;
        }
    }

#if NET_VERSION_4_5
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    private void CascadeDown(TItem node)
    {
        // aka Heapify-down
        TItem newParent;
        int finalQueueIndex = node.QueueIndex;

        while (true)
        {
            newParent = node;
            int childLeftIndex = 2 * finalQueueIndex;

            // Check if left-child is higher priority that the current node
            if (childLeftIndex > _numNodes)
            {
                node.QueueIndex = finalQueueIndex;
                _nodes[finalQueueIndex] = node;
                break;
            }

            TItem childLeft = _nodes[childLeftIndex];
            if (HasHigherPriority(childLeft, newParent)) newParent = childLeft;

            // Check if right-child is higher priority that either the current node or the left child
            int childRightIndex = childLeftIndex + 1;
            if (childRightIndex <= _numNodes)
            {
                TItem childRight = _nodes[childRightIndex];
                if (HasHigherPriority(childRight, newParent)) newParent = childRight;
            }

            // If either of the child has higher (smaller) priority, swap and continue cascading
            if (newParent != node)
            {
                // Move the new parent to its new index.
                // Node will be moved once, at the end.
                // Doing it this way is one less assignment operation than calling Swap()
                _nodes[finalQueueIndex] = newParent;

                int temp = newParent.QueueIndex;
                newParent.QueueIndex = finalQueueIndex;
                finalQueueIndex = temp;
            }
            else
            {
                node.QueueIndex = finalQueueIndex;
                _nodes[finalQueueIndex] = node;
                break;
            }
        }
    }

    /// <summary>
    /// Function used to see if 'higher' node has higher priority than 'lower' node
    /// Note that calling HasHigherPriority(node, node) (ie. both arguments are the same node) will return false 
    /// </summary>
    /// <param name="higher">Node we want to check if has higher priority</param>
    /// <param name="lower">Node that we are checking if 'higher' has higher priority than this one</param>
    /// <returns>true if 'higher' has higher priority than 'lower', false otherwise.</returns>
#if NET_VERSION_4_5
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    private bool HasHigherPriority(TItem higher, TItem lower)
    {
        int cmp = higher.Priority.CompareTo(lower.Priority);
        return cmp < 0 || (cmp == 0 && higher.InsertionIndex < lower.InsertionIndex);
    }

    /// <summary>
    /// Removes the head of the queue
    /// (node with minimum priority; ties are broken by order of insertion),
    /// and returns it.
    /// If queue is empty, result is undefined.
    /// O(log n)
    /// </summary>
    /// <returns>Head of the queue</returns>
    public TItem Dequeue()
    {
#if DEBUG
        if (_numNodes <= 0) throw new InvalidOperationException("Cannot call Dequeue() on an empty queue");
        if (!IsValidQueue())
            throw new InvalidOperationException(
                "Queue has been corrupted (Did you update a node priority manually instead of calling UpdatePriority()? " +
                "Or add the same node to two different queues?)"
            );
#endif

        TItem returnMe = _nodes[1];
        Remove(returnMe);

        return returnMe;
    }

    /// <summary>
    /// Resize the queue so it can accept more nodes. All currently enqueued nodes are remain.
    /// Attempting to decrease the queue size to a size too small to hold the
    /// existing nodes results in undefined behavior
    /// O(n)
    /// </summary>
    /// <param name="maxNodes">New size of the queue</param>
    public void Resize(int maxNodes)
    {
#if DEBUG
        if (maxNodes <= 0) throw new InvalidOperationException("Queue size cannot be smaller than 1");
        if (maxNodes < _numNodes)
            throw new InvalidOperationException("Called Resize(" + maxNodes + "), but current queue contains " +
                                                _numNodes + " nodes");

#endif

        TItem[] newArray = new TItem[maxNodes + 1];
        int highestIndexToCopy = Math.Min(maxNodes, _numNodes);

        for (int i = 1; i <= highestIndexToCopy; i++) newArray[i] = _nodes[i];

        _nodes = newArray;
    }

    public TItem First
    {
        get
        {
#if DEBUG
            if (_numNodes <= 0) throw new InvalidOperationException("Cannot call .First on an empty queue");
#endif

            return _nodes[1];
        }
    }

    /// <summary>
    /// This method must be called on a node every time its priority changes while it is in the queue.
    /// <b>Forgetting to call this method will result in corrupted queue!</b>
    /// Calling this method on a node not in queue results in undefined behavior
    /// O(log n) 
    /// </summary>
    /// <param name="node">Node we need to update priority of</param>
    /// <param name="priority">Priority to be set to node</param>
#if NET_VERSION_4_5
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public void UpdatePriority(TItem node, TPriority priority)
    {
#if DEBUG
        if (node == null) throw new ArgumentException("node");
        if (!Contains(node))
            throw new InvalidOperationException("Cannot call UpdatePriority() on a node which is not queued: " + node);
#endif

        node.Priority = priority;
        OnNodeUpdated(node);
    }

    private void OnNodeUpdated(TItem node)
    {
        // Bubble the updated node up or down as appropriate
        int parentIndex = node.QueueIndex / 2;
        TItem parentNode = _nodes[parentIndex];

        if (parentIndex > 0 && HasHigherPriority(node, parentNode)) CascadeUp(node);
        else
        {
            // Note that CascadeDown will be called if parentNode == node (that is, node is the root)
            CascadeDown(node);
        }
    }

    /// <summary>
    /// Removes a node from the queue. The node does not need to be the head of the queue.
    /// If the node is not in the queue, the result in undefined. If unsure, check Contains() first
    /// O(log n)
    /// </summary>
    /// <param name="node"></param>
    public void Remove(TItem node)
    {
#if DEBUG
        if (node == null) throw new ArgumentException("node");
        if (!Contains(node))
            throw new InvalidOperationException("Cannot call Remove() on a node which is not queued: " + node);
#endif

        // If the node is already the last node, we can remove it immediately
        if (node.QueueIndex == _numNodes)
        {
            _nodes[_numNodes] = null;
            _numNodes--;
            return;
        }

        // Swap the node with the last one
        TItem formerLastNode = _nodes[_numNodes];
        Swap(node, formerLastNode);
        _nodes[_numNodes] = null;
        _numNodes--;

        // Now bubble formerLastNode (which is no longer the last node) up or down as appropriate
        OnNodeUpdated(formerLastNode);
    }

    public IEnumerator<TItem> GetEnumerator()
    {
        for (int i = 0; i <= _numNodes; i++) yield return _nodes[i];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// <b>Should not be called in production code</b>
    /// Checks to make sure the queue is still in a valid state. Used for testing/debugging queue.
    /// </summary>
    /// <returns>If queue is valid or not</returns>
    public bool IsValidQueue()
    {
        for (int i = 1; i < _nodes.Length; i++)
        {
            if (_nodes[i] != null)
            {
                int childLeftIndex = 2 * i;
                if (childLeftIndex < _nodes.Length && _nodes[childLeftIndex] != null &&
                    HasHigherPriority(_nodes[childLeftIndex], _nodes[i])) return false;

                int childRightIndex = childLeftIndex + 1;
                if (childRightIndex < _nodes.Length && _nodes[childRightIndex] != null &&
                    HasHigherPriority(_nodes[childRightIndex], _nodes[i])) return false;
            }
        }

        return true;
    }
}