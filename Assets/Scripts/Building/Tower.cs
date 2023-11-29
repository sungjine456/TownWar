using UnityEngine;

public class Tower : GameBuilding
{
    [SerializeField] GameObject _attackPosition;

    void Awake()
    {
        BuildingId = Data.BuildingId.tower;
    }

    public void SetTarget(Vector3 targetPos)
    {
        LaunchedObj t = LaunchedObjPoolManager.Instance.GetTower();
        t.Initialized(LaunchedObjType.tower, _attackPosition.transform.position, targetPos, 3f);
    }
}
