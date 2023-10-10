using System;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool<T> where T : MonoBehaviour
{
    readonly Queue<T> _queue;
    readonly Func<T> _createFunc;

    public GameObjectPool(int count, Func<T> createFunc)
    {
        _createFunc = createFunc;
        _queue = new Queue<T>(count);
        Allocate();
    }

    void Allocate()
    {
        for (int i = 0; i < _queue.Count; i++)
            _queue.Enqueue(_createFunc());
    }

    public T Get()
    {
        if (_queue.Count > 0)
            return _queue.Dequeue();

        return _createFunc();
    }

    public void Set(T t)
    {
        _queue.Enqueue(t);
    }
}
