using System;

public struct BattleVector2
{
    public float _x;
    public float _y;

    public BattleVector2(Vector2Int v)
    {
        _x = v._x;
        _y = v._y;
    }

    public BattleVector2(float x, float y)
    {
        _x = x;
        _y = y;
    }

    public static BattleVector2 LerpUnclamped(BattleVector2 a, BattleVector2 b, float t)
    {
        return new(a._x + (b._x - a._x) * t, a._y + (b._y - a._y) * t);
    }

    public static float Distance(BattleVector2 a, BattleVector2 b)
    {
        float diff_x = a._x - b._x;
        float diff_y = a._y - b._y;

        return (float)Math.Sqrt(diff_x * diff_x + diff_y * diff_y);
    }

    public static float Distance(BattleVector2Int a, BattleVector2Int b)
    {
        return Distance(new BattleVector2(a._x, a._y), new BattleVector2(b._x, b._y));
    }

    public static BattleVector2 LerpStatic(BattleVector2 source, BattleVector2 target, float deltaTime, float speed)
    {
        if ((source._x == target._x && source._y == target._y) || speed <= 0)
            return source;

        float distance = Distance(source, target);
        float t = speed * deltaTime;

        if (t > distance)
            t = distance;

        return LerpUnclamped(source, target, distance == 0 ? 1 : t / distance);
    }

    public override string ToString() => $"[{_x}, {_y}]";
}
