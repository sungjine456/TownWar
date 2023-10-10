public struct BattleVector2Int
{
    public int _x;
    public int _y;

    public BattleVector2Int(Vector2Int v)
    {
        _x = v._x;
        _y = v._y;
    }

    public BattleVector2Int(int x, int y)
    {
        _x = x;
        _y = y;
    }
}
