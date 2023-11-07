using System.Collections.Generic;

public class Path
{
    public BattleVector2Int start;
    public BattleVector2Int end;
    public List<Cell> points;
    public float length;
    public List<Tile> blocks;

    public Path()
    {
        points = null;
        blocks = new();
    }

    public bool Create(Search search, BattleVector2Int start, BattleVector2Int end)
    {
        var target = search.Find(new(start._x, start._y), new(end._x, end._y));

        if (target == null)
            return false;

        points = new();

        for (int i = 0; i < target.Length; i++)
        {
            points.Add(target[i]);
        }
        
        if (!IsValid(ref points, new(start._x, start._y), new(end._x, end._y)))
        {
            points = null;

            return false;
        }
        else
        {
            this.start._x = start._x;
            this.start._y = start._y;
            this.end._x = end._x;
            this.end._y = end._y;

            return true;
        }
    }

    public Cell LastCell() => points[^1];

    public Tile LastTile() => blocks[^1];

    public static bool IsValid(ref List<Cell> points, Vector2Int start, Vector2Int end)
    {
        return !(points.Count == 0 || !points[^1].Location.Equals(end) || !points[0].Location.Equals(start));
    }
}
