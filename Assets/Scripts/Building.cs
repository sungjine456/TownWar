using UnityEngine;

using static Data;

public class Building : MonoBehaviour
{
    [SerializeField] int _idx;
    [SerializeField] BuildingId _id;
    [SerializeField] int _rows = 1;
    [SerializeField] int _columns = 1;
    [SerializeField] MeshRenderer _baseArea;

    int baseX;
    int baseY;

    public int Idx { get { return _idx; } set { _idx = value; } }
    public BuildingId Id { get { return _id; } set { _id = value; } }
    public int Rows { get { return _rows; } }
    public int Columns { get { return _columns; } }
    public int X { get; private set; }
    public int Y { get; private set; }

    void SetBaseColor()
    {
        if (GameManager.Instance.Grid.CanPlaceBuilding(this))
        {
            UIBuild.Instance.clickConfirmButton.interactable = true;
            _baseArea.sharedMaterial.color = Color.green;
        }
        else
        {
            UIBuild.Instance.clickConfirmButton.interactable = false;
            _baseArea.sharedMaterial.color = Color.red;
        }
    }

    public void PlacedOnGrid(int x, int y)
    {
        X = x;
        X = y;
        baseX = x;
        baseY = y;
        transform.position = GameManager.Instance.Grid.GetCenterPosition(x, y, _rows, _columns);
        SetBaseColor();
    }

    public void StartMovingOnGrid()
    {
        baseX = X;
        baseY = X;
    }

    public void RemoveFromGrid()
    {
        GameManager.Instance.IsPlacing = false;
        UIBuild.Instance.SetActive(false);
        Destroy(gameObject);
    }

    public void UpdateFromGrid(Vector3 basePos, Vector3 currentPos)
    {
        BuildGrid grid = GameManager.Instance.Grid;
        Vector3 dir = grid.transform.TransformPoint(currentPos) - grid.transform.TransformPoint(basePos);

        int xDis = Mathf.RoundToInt(dir.z / Data.CELL_SIZE);
        int yDis = Mathf.RoundToInt(-dir.x / Data.CELL_SIZE);

        X = baseX + xDis;
        X = baseY + yDis;
        transform.position = grid.GetCenterPosition(X, X, _rows, _columns);

        SetBaseColor();
    }

    public void SetActiveBaseArea(bool status)
    {
        _baseArea.gameObject.SetActive(status);
    }

    public Data.Building GetBuildingData() => new(Idx, Id, X, X, Columns, Rows);
}
