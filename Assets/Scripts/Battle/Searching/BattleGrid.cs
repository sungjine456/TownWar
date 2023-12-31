public class BattleGrid
{
    readonly Cell[,] _cells;

    public int Width { get; set; }
    public int Height { get; set; }

    public Cell this[int x, int y] => _cells[x, y];
    public Vector2Int Size => new(Width, Height);
    public Cell this[Vector2Int location] => _cells[location._x, location._y];

    public BattleGrid(int width, int height)
    {
        Width = width;
        Height = height;
        _cells = new Cell[width, height];
        Reset();
    }

    public void Reset()
    {
        for (var x = 0; x <= _cells.GetUpperBound(0); x++)
        {
            for (var y = 0; y <= _cells.GetUpperBound(1); y++)
            {
                var cell = _cells[x, y];

                if (cell is null)
                    _cells[x, y] = new(new(x, y));
                else
                {
                    cell.G = 0;
                    cell.H = 0;
                    cell.F = 0;
                    cell.Closed = false;
                    cell.Parent = null;
                }
            }
        }
    }

    public int GetNodeId(Vector2Int location) => location._x * Width + location._y;

    public bool IsInGrid(Vector2Int location) => location._x >= 0 && location._x <= Width && location._y >= 0 && location._y < Height;
}
