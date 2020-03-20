using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// SimplePriorityQueue Implementation from
// https://github.com/mortennobel/UnityUtils/blob/master/Assets/UnityUtils/ThirdParty/Priority Queue/SimplePriorityQueue.cs

/// <summary>
/// A simplified priority queue implementation. Is stable, auto-resizes, and thread-safe,
/// at the cost of being slightly slower than other possible implementations
/// </summary>
/// <typeparam name="TItem">The type of enqueue</typeparam>
/// <typeparam name="TPriority">
/// The priority-type to use for nodes. Must extend IComparable&lt;TPriority&gt;
/// </typeparam>
public class PriorityQueue<TItem, TPriority> : IPriorityQueue<TItem, TPriority>
    where TPriority : IComparable<TPriority>
{
    private class SimpleNode : GenericPriorityQueueNode<TPriority>
    {
        public TItem Data { get; private set; }

        public SimpleNode(TItem data) => Data = data;
    }

    private const int InitialQueueSize = 10;
    private readonly GenericPriorityQueue<SimpleNode, TPriority> _queue;

    public PriorityQueue()
    {
        _queue = new GenericPriorityQueue<SimpleNode, TPriority>(InitialQueueSize);
    }

    /// <summary>
    /// Given anf item of type T, returns the existing SimpleNode in the queue
    /// </summary>
    /// <param name="item">Item to return the node of</param>
    /// <returns>Appropriate to 'item' SimpleNode</returns>
    private SimpleNode GetExistingNode(TItem item)
    {
        EqualityComparer<TItem> comparer = EqualityComparer<TItem>.Default;

        foreach (SimpleNode node in _queue)
        {
            if (comparer.Equals(node.Data, item)) return node;
        }

        throw new InvalidOperationException("Item cannot be found in queue: " + item);
    }

    /// <summary>
    /// Return the number of nodes in the queue
    /// </summary>
    public int Count
    {
        get
        {
            lock (_queue)
            {
                return _queue.Count;
            }
        }
    }

    /// <summary>
    /// Returns the head of the queue, without removing it (use Dequeue() for that).
    /// Throws an exception when the queue is empty.
    /// O(1)
    /// </summary>
    public TItem First
    {
        get
        {
            lock (_queue)
            {
                if (_queue.Count <= 0) throw new InvalidOperationException("Cannot call .First on an empty queue");

                SimpleNode first = _queue.First;
                return first != null ? first.Data : default(TItem);
            }
        }
    }

    /// <summary>
    /// Removes every node from the queue
    /// O(n)
    /// </summary>
    public void Clear()
    {
        lock (_queue)
        {
            _queue.Clear();
        }
    }

    /// <summary>
    /// Returns whether the given item is in the queue.
    /// O(n)
    /// </summary>
    /// <param name="item">Item to look for</param>
    /// <returns>If node is in the queue</returns>
    public bool Contains(TItem item)
    {
        lock (_queue)
        {
            EqualityComparer<TItem> comparer = EqualityComparer<TItem>.Default;

            foreach (SimpleNode node in _queue)
            {
                if (comparer.Equals(node.Data, item)) return true;
            }

            return false;
        }
    }

    /// <summary>
    /// Removes the head of the queue
    /// (node with minimum priority; ties are broken by order of insertion),
    /// and returns it.
    /// If queue is empty, throws an exception
    /// O(log n)
    /// </summary>
    /// <returns>Head of the queue</returns>
    public TItem Dequeue()
    {
        lock (_queue)
        {
            if (_queue.Count <= 0) throw new InvalidOperationException("Cannot call Dequeue() on an empty queue");

            SimpleNode node = _queue.Dequeue();
            return node.Data;
        }
    }

    /// <summary>
    /// Enqueue a node to the priority queue. Lower values are placed in front.
    /// Ties are broken by first-in-first-out.
    /// This queue automatically resizes itself, so there's no concern of the queue becoming 'full'.
    /// Duplicates are allowed.
    /// O(log n) 
    /// </summary>
    /// <param name="item">Item to enqueue</param>
    /// <param name="priority">Priority of the node no enqueue</param>
    public void Enqueue(TItem item, TPriority priority)
    {
        lock (_queue)
        {
            SimpleNode node = new SimpleNode(item);

            if (_queue.Count == _queue.MaxSize) _queue.Resize(_queue.MaxSize * 2 + 1);

            _queue.Enqueue(node, priority);
        }
    }

    /// <summary>
    /// Removes an item from queue. The item does not need to be head of the queue.
    /// If item is not in the queue, an exception is thrown. If unsure, check Contains() first.
    /// If multiple copies of the item are enqueued, only the first one is removed.
    /// O(n)
    /// </summary>
    /// <param name="item">Item to remove from queue</param>
    public void Remove(TItem item)
    {
        lock (_queue)
        {
            try
            {
                _queue.Remove(GetExistingNode(item));
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("Cannot call Remove() on a node which is not enqueued: "
                                                    + item,
                    ex
                );
            }
        }
    }

    /// <summary>
    /// Call this method to change the priority of an item.
    /// Calling this method on an item not in queue will throw an exception.
    /// If the item is enqueued multiple times, only the first one will be updated.
    /// (If your requirements are complex enough that you need to enqueue the same item multiple times <i>and</i>
    /// be able to update all of them, please wrap your items in the wrapper class so they can be distinguished).
    /// O(n)
    /// </summary>
    /// <param name="item">Item to update priority of</param>
    /// <param name="priority">Priority to set to the item</param>
    public void UpdatePriority(TItem item, TPriority priority)
    {
        lock (_queue)
        {
            try
            {
                SimpleNode updateMe = GetExistingNode(item);
                _queue.UpdatePriority(updateMe, priority);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException(
                    "Cannot call UpdatePriority() on a node which is not enqueued: " + item, ex);
            }
        }
    }

    public IEnumerator<TItem> GetEnumerator()
    {
        List<TItem> queueData = new List<TItem>();

        lock (_queue)
        {
            //Copy separate list because we dont want to 'yield return' inside a lock
            foreach (SimpleNode node in _queue)
            {
                queueData.Add(node.Data);
            }
        }

        return queueData.GetEnumerator();
    }
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool IsValidQueue()
    {
        lock (_queue)
        {
            return _queue.IsValidQueue();
        }
    }
}

/// <summary>
/// A simplified priority queue implementation. Is stable, auto-resizes, and thread-safe, at the cost of being slightly
/// slower than other possible priority queue implementations
/// This class is kept here for backwards compatibility. 
/// </summary>
/// <typeparam name="TItem">The type to enqueue</typeparam>
//public class PriorityQueue<TItem> : PriorityQueue<TItem, float> { }