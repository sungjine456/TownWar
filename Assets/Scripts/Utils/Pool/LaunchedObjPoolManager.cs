using System.Collections.Generic;

using UnityEngine;

public enum LaunchedObjType
{
    arrow,
    tower
}

public class LaunchedObjPoolManager : SingletonMonoBehaviour<LaunchedObjPoolManager>
{
    [SerializeField] LaunchedObj _arrowPrefab;
    [SerializeField] LaunchedObj _towerPrefab;

    Dictionary<LaunchedObjType, GameObjectPool<LaunchedObj>> _pools;

    protected override void OnAwake()
    {
        _pools = new();

        _pools.Add(LaunchedObjType.arrow, new(10, () =>
        {
            var o = Instantiate(_arrowPrefab);
            o.gameObject.SetActive(false);
            return o;
        }));

        _pools.Add(LaunchedObjType.tower, new(10, () =>
        {
            var o = Instantiate(_towerPrefab);
            o.gameObject.SetActive(false);
            return o;
        }));
    }

    LaunchedObj Get(LaunchedObjType type)
    {
        var o = _pools[type].Get();
        o.gameObject.SetActive(true);

        return o;
    }

    public void Remove(LaunchedObjType type, LaunchedObj o)
    {
        o.gameObject.SetActive(false);
        _pools[type].Set(o);
    }

    public LaunchedObj GetArrow() => Get(LaunchedObjType.arrow);
    public LaunchedObj GetTower() => Get(LaunchedObjType.tower);

    public void RemoveArrow(LaunchedObj o) => Remove(LaunchedObjType.arrow, o);
    public void RemoveTower(LaunchedObj o) => Remove(LaunchedObjType.tower, o);
}
