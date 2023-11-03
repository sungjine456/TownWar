using UnityEngine;

public class Cannon : GameBuilding
{
    [SerializeField] GameObject _attackPosition;

    void Awake()
    {
        BuildingId = Data.BuildingId.cannon;
    }

    public void SetTarget(Vector3 targetPos)
    {
        transform.rotation = Quaternion.LookRotation(targetPos - transform.position);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y - 45, transform.eulerAngles.z);

        LaunchedObj bullet = LaunchedObjPoolManager.Instance.GetBullet();
        bullet.Initialized(LaunchedObjPoolManager.Type.bullet, _attackPosition.transform.position, targetPos, 1.44f);
    }
}
