using System;
using System.Collections.Generic;

public class Search 
{
    readonly BattleGrid _grid;
    readonly FastPriorityQueue _open;

    public Search(BattleGrid grid) 
    {
        _grid = grid;
        _open = new(grid.Size._x * grid.Size._y);
    }

    double Heuristic(Cell cell, Cell goal) 
    {
        var dX = Math.Abs(cell.Location._x - goal.Location._x);
        var dY = Math.Abs(cell.Location._y - goal.Location._y);

        return dX + dY + (Math.Sqrt(2) - 2) * Math.Min(dX, dY);
    }

    public void Reset() 
    {
        _grid.Reset();
        _open.Clear();
    }

    public List<Cell> FindToList(Vector2Int start, Vector2Int goal)
    {
        var cells = Find(start, goal);
        List<Cell> result = new(cells.Length);
        
        for (int i = 0; i < cells.Length; i++)
        {
            result.Add(cells[i]);
        }

        return result;
    }

    public Cell[] Find(Vector2Int start, Vector2Int goal) 
    {
        if (!_grid.IsInGrid(start) || !_grid.IsInGrid(goal))
            return null;

        Reset();
        Cell startCell = _grid[start];
        Cell goalCell = _grid[goal];
        _open.Enqueue(startCell, 0);
        var bounds = _grid.Size;
        Cell node = null;

        while (_open.Count > 0) 
        {
            node = _open.Dequeue();
            node.Closed = true;
            var cBlock = false;
            var g = node.G + 1;

            if (goalCell.Location == node.Location) 
                break;

            Vector2Int proposed = new(0, 0);

            for (var i = 0; i < PathingConstants._directions.Length; i++) 
            {
                var direction = PathingConstants._directions[i];
                proposed._x = node.Location._x + direction._x;
                proposed._y = node.Location._y + direction._y;

                if (proposed._x < 0 || proposed._x >= bounds._x || proposed._y < 0 || proposed._y >= bounds._y)
                    continue;

                Cell neighbour = _grid[proposed];

                if (neighbour.Blocked) 
                {
                    if (i < 4)
                        cBlock = true;
                    
                    continue;
                }

                if (i >= 4 && cBlock)
                    continue;

                if (_grid[neighbour.Location].Closed)
                    continue;

                if (!_open.Contains(neighbour)) 
                {
                    neighbour.G = g;
                    neighbour.H = Heuristic(neighbour, node);
                    neighbour.Parent = node;

                    _open.Enqueue(neighbour, neighbour.G + neighbour.H);
                }
                else if (g + neighbour.H < neighbour.F) 
                {
                    neighbour.G = g;
                    neighbour.F = neighbour.G + neighbour.H;
                    neighbour.Parent = node;
                }
            }
        }

        Stack<Cell> path = new();

        while (node != null) 
        {
            path.Push(node);
            node = node.Parent;
        }

        return path.ToArray();
    }
}
