using UnityEngine;

public class BuildGrid : MonoBehaviour
{
    Vector3 GetStartPosition(int x, int y)
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

    public bool IsWorldPositionIsOnPlane(Vector3 pos, Building b)
    {
        pos = transform.InverseTransformPoint(pos);
        Rect rect = new(b.X, b.Y, b.Columns, b.Rows);

        return rect.Contains(new Vector2(pos.x, pos.z));
    }
}
