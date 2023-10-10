using System;
using System.Runtime.CompilerServices;

public sealed class FastPriorityQueue 
{
    readonly Cell[] _nodes;

    public int Count { get; private set; }

    public FastPriorityQueue(int maxNodes) 
    {
        Count = 0;
        _nodes = new Cell[maxNodes + 1];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void CascadeUp(Cell node) 
    {
        int parent;

        if (node.QueueIndex > 1) 
        {
            parent = node.QueueIndex >> 1;
            var parentNode = _nodes[parent];

            if (HasHigherOrEqualPriority(parentNode, node))
                return;

            _nodes[node.QueueIndex] = parentNode;
            parentNode.QueueIndex = node.QueueIndex;

            node.QueueIndex = parent;
        } 
        else
            return;

        while (parent > 1) 
        {
            parent >>= 1;
            var parentNode = _nodes[parent];

            if (HasHigherOrEqualPriority(parentNode, node))
                break;

            _nodes[node.QueueIndex] = parentNode;
            parentNode.QueueIndex = node.QueueIndex;
            node.QueueIndex = parent;
        }

        _nodes[node.QueueIndex] = node;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void CascadeDown(Cell node) 
    {
        var finalQueueIndex = node.QueueIndex;
        var childLeftIndex = 2 * finalQueueIndex;

        if (childLeftIndex > Count)
            return;

        var childRightIndex = childLeftIndex + 1;
        var childLeft = _nodes[childLeftIndex];

        if (HasHigherPriority(childLeft, node)) 
        {
            if (childRightIndex > Count) 
            {
                node.QueueIndex = childLeftIndex;
                childLeft.QueueIndex = finalQueueIndex;
                _nodes[finalQueueIndex] = childLeft;
                _nodes[childLeftIndex] = node;
                return;
            }

            var childRight = _nodes[childRightIndex];
            if (HasHigherPriority(childLeft, childRight)) 
            {
                childLeft.QueueIndex = finalQueueIndex;
                _nodes[finalQueueIndex] = childLeft;
                finalQueueIndex = childLeftIndex;
            } 
            else 
            {
                childRight.QueueIndex = finalQueueIndex;
                _nodes[finalQueueIndex] = childRight;
                finalQueueIndex = childRightIndex;
            }
        }
        else if (childRightIndex > Count) 
            return;
        else 
        {
            var childRight = _nodes[childRightIndex];
            if (HasHigherPriority(childRight, node)) 
            {
                childRight.QueueIndex = finalQueueIndex;
                _nodes[finalQueueIndex] = childRight;
                finalQueueIndex = childRightIndex;
            }
            else 
                return;
        }

        while (true) 
        {
            childLeftIndex = 2 * finalQueueIndex;

            if (childLeftIndex > Count) 
            {
                node.QueueIndex = finalQueueIndex;
                _nodes[finalQueueIndex] = node;
                break;
            }

            childRightIndex = childLeftIndex + 1;
            childLeft = _nodes[childLeftIndex];

            if (HasHigherPriority(childLeft, node))
            {
                if (childRightIndex > Count) 
                {
                    node.QueueIndex = childLeftIndex;
                    childLeft.QueueIndex = finalQueueIndex;
                    _nodes[finalQueueIndex] = childLeft;
                    _nodes[childLeftIndex] = node;
                    break;
                }

                var childRight = _nodes[childRightIndex];

                if (HasHigherPriority(childLeft, childRight)) 
                {
                    childLeft.QueueIndex = finalQueueIndex;
                    _nodes[finalQueueIndex] = childLeft;
                    finalQueueIndex = childLeftIndex;
                } 
                else 
                {
                    childRight.QueueIndex = finalQueueIndex;
                    _nodes[finalQueueIndex] = childRight;
                    finalQueueIndex = childRightIndex;
                }
            }
            else if (childRightIndex > Count) 
            {
                node.QueueIndex = finalQueueIndex;
                _nodes[finalQueueIndex] = node;

                break;
            } 
            else
            {
                var childRight = _nodes[childRightIndex];

                if (HasHigherPriority(childRight, node)) 
                {
                    childRight.QueueIndex = finalQueueIndex;
                    _nodes[finalQueueIndex] = childRight;
                    finalQueueIndex = childRightIndex;
                }
                else 
                {
                    node.QueueIndex = finalQueueIndex;
                    _nodes[finalQueueIndex] = node;
                    break;
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool HasHigherPriority(Cell higher, Cell lower) => higher.F < lower.F;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool HasHigherOrEqualPriority(Cell higher, Cell lower) => higher.F <= lower.F;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void OnNodeUpdated(Cell node) 
    {
        var parentIndex = node.QueueIndex >> 1;

        if (parentIndex > 0 && HasHigherPriority(node, _nodes[parentIndex]))
            CascadeUp(node);
        else
            CascadeDown(node); 
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        Array.Clear(_nodes, 1, Count);
        Count = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(Cell node) => _nodes[node.QueueIndex] == node;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Enqueue(Cell node, double priority)
    {
        node.F = priority;
        Count++;
        _nodes[Count] = node;
        node.QueueIndex = Count;
        CascadeUp(node);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Cell Dequeue()
    {
        var returnMe = _nodes[1];

        if (Count == 1)
        {
            _nodes[1] = null;
            Count = 0;

            return returnMe;
        }

        var formerLastNode = _nodes[Count];
        _nodes[1] = formerLastNode;
        formerLastNode.QueueIndex = 1;
        _nodes[Count] = null;
        Count--;

        CascadeDown(formerLastNode);

        return returnMe;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UpdatePriority(Cell node, double priority)
    {
        node.F = priority;
        OnNodeUpdated(node);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Remove(Cell node)
    {
        if (node.QueueIndex == Count)
        {
            _nodes[Count] = null;
            Count--;

            return;
        }

        var formerLastNode = _nodes[Count];
        _nodes[node.QueueIndex] = formerLastNode;
        formerLastNode.QueueIndex = node.QueueIndex;
        _nodes[Count] = null;
        Count--;

        OnNodeUpdated(formerLastNode);
    }
}
