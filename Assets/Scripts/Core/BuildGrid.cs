using UnityEngine;

public class BuildGrid : MonoBehaviour
{
    public virtual void AddBuilding(Data.Building buildingData, bool nowConstruct = false) { }

    public Vector3 GetStartPosition(int x, int y)
    {
        Vector3 position = transform.position;
        position += (x * Data.CELL_SIZE * transform.right) + (y * Data.CELL_SIZE * transform.forward);
        return position;
    }

    public Vector3 GetCenterPosition(int x, int y, int rows, int columns)
    {
        Vector3 position = GetStartPosition(x, y);
        position += (columns * Data.CELL_SIZE * transform.right / 2f) + (rows * Data.CELL_SIZE * transform.forward / 2f);
        return position;
    }

    public Vector3 GetEndPosition(int x, int y, int rows, int columns)
    {
        Vector3 position = GetStartPosition(x, y);
        position += (columns * Data.CELL_SIZE * transform.right) + (rows * Data.CELL_SIZE * transform.forward);
        return position;
    }

    public Vector3 GetEndPosition(Building building)
    {
        return GetEndPosition(building.CurrentX, building.CurrentY, building.Columns, building.Rows);
    }

    public bool IsWorldPositionOnPlane(Vector3 pos, Building building)
    {
        return IsWorldPositionOnPlane(pos, building.CurrentX, building.CurrentY, building.Rows, building.Columns);
    }

    public bool IsWorldPositionOnPlane(Vector3 pos, int x, int y, int rows, int columns)
    {
        pos = transform.InverseTransformPoint(pos);
        Rect rect = new(x, y, columns, rows);
        return rect.Contains(new Vector2(pos.x, pos.z));
    }
}
