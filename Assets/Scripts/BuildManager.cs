using UnityEngine;

using static Data;

public class BuildManager : SingletonMonoBehaviour<BuildManager>
{
    int _baseX;
    int _baseY;

    public Building CurrentTarget;
    public bool IsMoveingBuilding { get; set; }

    public void CreateBuilding(BuildingId id)
    {
        var prefab = GameManager.Instance.GetBuildingPrefab(id);

        if (prefab != null)
        {
            CurrentTarget = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            PlacedOnGrid(20, 20);
            GameManager.Instance.IsPlacing = true;
        }
    }

    public void StartMovingOnGrid()
    {
        _baseX = CurrentTarget.X;
        _baseY = CurrentTarget.Y;

        IsMoveingBuilding = true;
    }

    public void PlacedOnGrid(int x, int y)
    {
        _baseX = x;
        _baseY = y;
        CurrentTarget.SetPosition(x, y);
    }

    public void UpdateFromGrid(Vector3 basePos, Vector3 currentPos)
    {
        BuildGrid grid = GameManager.Instance.Grid;
        Vector3 dir = grid.transform.TransformPoint(currentPos) - grid.transform.TransformPoint(basePos);

        int xDis = Mathf.RoundToInt(-dir.z / CELL_SIZE);
        int yDis = Mathf.RoundToInt(dir.x / CELL_SIZE);

        CurrentTarget.SetPosition(_baseX + xDis, _baseY + yDis);
    }
}
