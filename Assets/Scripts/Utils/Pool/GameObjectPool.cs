using System;
using System.Collections.Generic;

public class GameObjectPool<T>
{
    readonly Queue<T> _queue;
    readonly int _count;
    readonly Func<T> _createFunc;

    public GameObjectPool(int count, Func<T> createFunc)
    {
        _count = count;
        _createFunc = createFunc;
        _queue = new(count);
        Allocate();
    }

    void Allocate()
    {
        for (int i = 0; i < _count; i++)
        {
            _queue.Enqueue(_createFunc());
        }
    }

    public T Get()
    {
        if (_queue.Count > 0)
            return _queue.Dequeue();

        return _createFunc();
    }

    public void Set(T t) => _queue.Enqueue(t);
}
