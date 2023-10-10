using System.Collections.Generic;

public class Path
{
    public BattleVector2Int _start;
    public BattleVector2Int _end;
    public List<Cell> _points;
    public float _length;
    public List<Tile> _blocks;

    public Path()
    {
        _length = 0;
        _points = null;
        _blocks = new();
    }

    public bool Create(ref Search search, BattleVector2Int start, BattleVector2Int end)
    {
        var target = search.Find(new(start._x, start._y), new(end._x, end._y));

        if (target == null)
            return false;

        _points = new();

        for (int i = 0; i < target.Length; i++)
        {
            _points.Add(target[i]);
        }
        
        if (!IsValid(ref _points, new(start._x, start._y), new(end._x, end._y)))
        {
            _points = null;

            return false;
        }
        else
        {
            _start._x = start._x;
            _start._y = start._y;
            _end._x = end._x;
            _end._y = end._y;

            return true;
        }
    }

    public Cell LastCell()
    {
        return _points[^1];
    }

    public Tile LastTile()
    {
        return _blocks[^1];
    }

    public static bool IsValid(ref List<Cell> points, Vector2Int start, Vector2Int end)
    {
        return !(points.Count == 0 || !points[^1].Location.Equals(end) || !points[0].Location.Equals(start));
    }
}
