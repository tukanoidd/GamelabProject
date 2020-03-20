﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Source: https://github.com/mortennobel/UnityUtils/blob/master/Assets/UnityUtils/ThirdParty/Priority Queue/IFixedSizePriorityQueue.cs
internal interface IFixedSizePriorityQueue<TItem, in TPriority> : IPriorityQueue<TItem, TPriority>
    where TPriority : IComparable<TPriority>
{
    /// <summary>
    /// Resize the queue so it can accept more nodes.
    /// All currently enqueued nodes are remain.
    /// Attempting to decrease the queue size to a size too small
    /// to hold the existing nodes results in undefined behavior
    /// </summary>
    void Resize(int maxNodes);

    /// <summary>
    /// Returns the maximum number of items that can be enqueued at once in this queue.
    /// Once you hit this number (ie. once Count == MaxSize),
    /// attempting to enqueue another item will cause undefined behavior.
    /// </summary>
    int MaxSize { get; }
}
