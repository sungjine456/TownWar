using UnityEngine;

using static Data;

public class Building : MonoBehaviour
{
    [SerializeField] BuildingId _id;
    [SerializeField] int _rows;
    [SerializeField] int _columns;
    [SerializeField] MeshRenderer _baseArea;

    public BuildingId Id { get { return _id; } }
    public int Rows { get { return _rows; } }
    public int Columns { get { return _columns; } }
    public int X { get; private set; }
    public int Y { get; private set; }

    void SetBaseColor()
    {
        if (GameManager.Instance.Grid.CanPlaceBuilding(this))
            _baseArea.sharedMaterial.color = Color.green;
        else
            _baseArea.sharedMaterial.color = Color.red;
    }

    public void SetPosition(int x, int y)
    {
        X = x;
        Y = y;
        transform.position = GameManager.Instance.Grid.GetCenterPosition(x, y, Rows, Columns);
        SetBaseColor();
    }
}
