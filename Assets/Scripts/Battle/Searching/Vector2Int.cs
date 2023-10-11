public struct Vector2Int
{
    public int _x;
    public int _y;

    public Vector2Int(int x, int y) 
    {
        _x = x;
        _y = y;
    }

    public override string ToString() => $"[{_x}, {_y}]";
    public static bool operator ==(Vector2Int lhs, Vector2Int rhs) => lhs.Equals(rhs);
    public static bool operator !=(Vector2Int lhs, Vector2Int rhs) => !lhs.Equals(rhs);

    public override bool Equals(object obj)
    {
        if (obj is Vector2Int o)
            return o._x == _x && o._y == _y;
        else
            return false;
    }

    public override int GetHashCode()
    {
        return System.HashCode.Combine(_x, _y);
    }
}
