using UnityEngine;

using static Data;

public class BuildManager : SingletonMonoBehaviour<BuildManager>
{
    int _baseX;
    int _baseY;

    Building _currentTarget;

    public Building CurrentTarget { get { return _currentTarget; } private set { _currentTarget = value; } }
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

        UIBuild.Instance.SetActive(true);
    }

    public void StartMovingOnGrid()
    {
        if (HasTarget())
        {
            _baseX = CurrentTarget.X;
            _baseY = CurrentTarget.Y;

            IsMoveingBuilding = true;
        }
    }

    public void PlacedOnGrid(int x, int y)
    {
        if (HasTarget())
        {
            _baseX = x;
            _baseY = y;
            CurrentTarget.SetPosition(x, y);
        }
    }

    public void UpdateFromGrid(Vector3 basePos, Vector3 currentPos)
    {
        if (HasTarget())
        {
            BuildGrid grid = GameManager.Instance.Grid;
            Vector3 dir = grid.transform.TransformPoint(currentPos) - grid.transform.TransformPoint(basePos);

            int xDis = Mathf.RoundToInt(dir.z / CELL_SIZE);
            int yDis = Mathf.RoundToInt(-dir.x / CELL_SIZE);

            CurrentTarget.SetPosition(_baseX + xDis, _baseY + yDis);
        }
    }

    public void RemoveFromGrid()
    {
        if (HasTarget())
        {
            GameManager.Instance.IsPlacing = false;
            Destroy(CurrentTarget.gameObject);
            CurrentTarget = null;
        }
    }

    public bool HasTarget() => CurrentTarget != null;

    public bool EmptyTarget() => !HasTarget();
}
