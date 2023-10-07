using UnityEngine;

using static Data;

public class Building : MonoBehaviour
{
    [SerializeField] BuildingId _id;
    [SerializeField] MeshRenderer _baseArea;

    int _baseX;
    int _baseY;

    public int Idx { get; set; }
    public BuildingId Id { get { return _id; } set { _id = value; } }
    public int Rows { get; private set; }
    public int Columns { get; private set; }
    public int X { get; private set; }
    public int Y { get; private set; }
    public int RequiredGold { get; set; }

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

    public void Initialized(BuildingToBuild data, int x, int y)
    {
        Rows = data.rows;
        Columns = data.columns;
        RequiredGold = data.requiredGold;
        SetActiveBaseArea(true);
        PlacedOnGrid(x, y);
    }

    public void PlacedOnGrid(int x, int y)
    {
        X = x;
        Y = y;
        _baseX = x;
        _baseY = y;
        transform.position = GameManager.Instance.Grid.GetCenterPosition(x, y, Rows, Columns);
        SetBaseColor();
    }

    public void StartMovingOnGrid()
    {
        _baseX = X;
        _baseY = Y;
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

        int xDis = Mathf.RoundToInt(dir.z / CELL_SIZE);
        int yDis = Mathf.RoundToInt(-dir.x / CELL_SIZE);

        X = _baseX + xDis;
        Y = _baseY + yDis;
        transform.position = grid.GetCenterPosition(X, Y, Rows, Columns);

        SetBaseColor();
    }

    public void SetActiveBaseArea(bool status)
    {
        _baseArea.gameObject.SetActive(status);
    }

    public BuildingToSave GetSaveBuildingData() => new(Idx, Id, X, Y);

    public override bool Equals(object other)
    {
        if (other is Building b)
            return b.Idx == Idx;

        return false;
    }

    public override int GetHashCode() => base.GetHashCode();
}
