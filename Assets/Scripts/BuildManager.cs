using UnityEngine;

using static Data;

public class BuildManager : SingletonMonoBehaviour<BuildManager>
{
    [SerializeField] BuildingInfo _info;

    public Building CurrentTarget { get; private set; }

    public void CreateBuilding(BuildingId id)
    {
        var prefab = GameManager.Instance.GetBuildingPrefab(id);

        if (prefab != null)
        {
            CurrentTarget = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            CurrentTarget.Initialized(_info.GetBuildingData(id), 20, 20);
            GameManager.Instance.IsPlacing = true;
        }

        UIBuild.Instance.SetActive(true);
    }

    public bool AddFromGrid()
    {
        if (HasTarget())
        {
            GameManager.Instance.IsPlacing = false;

            if (CurrentTarget.RequiredGold <= Player.Instance.Gold)
            {
                CurrentTarget.Idx = Player.Instance.NextBuildingIdx();
                CurrentTarget.SetActiveBaseArea(false);
                GameManager.Instance.Grid.AddBuilding(CurrentTarget);
                Player.Instance.SaveBuilding(CurrentTarget);
                CurrentTarget = null;
                return true;
            }
            else
                AlertManager.Instance.Error("자원이 부족합니다.");
        }

        return false;
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
