using System.Collections.Generic;
using UnityEngine;

using static Data;

public class BuildGrid : MonoBehaviour
{
    List<Building> _buildings;

    public List<Building> Buildings { get { return _buildings; } }

    void Awake()
    {
        _buildings = new List<Building>();
    }

    public void AddBuilding(Building building)
    {
        _buildings.Add(building);
    }

    public bool CanPlaceBuilding(Building building)
    {
        if (building.X < 0 || building.Y < 0 || building.X + building.Columns > GRID_SIZE || building.Y + building.Rows > GRID_SIZE)
            return false;

        for (int i = 0; i < _buildings.Count; i++)
        {
            if (_buildings[i] != building)
            {
                Rect rect1 = new(_buildings[i].X, _buildings[i].Y, _buildings[i].Columns, _buildings[i].Rows);
                Rect rect2 = new(building.X, building.Y, building.Columns, building.Rows);

                if (rect2.Overlaps(rect1))
                    return false;
            }
        }

        return true;
    }

    Vector3 GetStartPosition(int x, int y)
    {
        Vector3 position = transform.position;

        position += (x * CELL_SIZE * transform.right) + (y * CELL_SIZE * transform.forward);

        return position;
    }

    public Vector3 GetCenterPosition(int x, int y, int rows, int columns)
    {
        Vector3 position = GetStartPosition(x, y);

        position += (columns * CELL_SIZE * transform.right / 2f) + (rows * CELL_SIZE * transform.forward / 2f);

        return position;
    }

    public Vector3 GetEndPosition(int x, int y, int rows, int columns)
    {
        Vector3 position = GetStartPosition(x, y);

        position += (columns * CELL_SIZE * transform.right) + (rows * CELL_SIZE * transform.forward);

        return position;
    }

    public Vector3 GetEndPosition(Building building)
    {
        return GetEndPosition(building.X, building.Y, building.Columns, building.Rows);
    }

    public bool IsWorldPositionIsOnPlane(Vector3 pos, Building b)
    {
        pos = transform.InverseTransformPoint(pos);
        Rect rect = new(b.X, b.Y, b.Columns, b.Rows);

        return rect.Contains(new Vector2(pos.x, pos.z));
    }
}
