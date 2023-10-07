using UnityEngine;

using static Data;

public class BuildManager : SingletonMonoBehaviour<BuildManager>
{
    public Building CurrentTarget { get; private set; }

    public void CreateBuilding(BuildingId id)
    {
        var prefab = GameManager.Instance.GetBuildingPrefab(id);

        if (prefab != null)
        {
            CurrentTarget = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            CurrentTarget.SetActiveBaseArea(true);
            CurrentTarget.PlacedOnGrid(20, 20);
            GameManager.Instance.IsPlacing = true;
        }

        UIBuild.Instance.SetActive(true);
    }

    public void AddFromGrid()
    {
        if (HasTarget())
        {
            GameManager.Instance.IsPlacing = false;
            CurrentTarget.Idx = Player.Instance.NextBuildingIdx();
            CurrentTarget.SetActiveBaseArea(false);
            GameManager.Instance.Grid.AddBuilding(CurrentTarget);
            Player.Instance.SaveBuilding(CurrentTarget);
            CurrentTarget = null;
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
