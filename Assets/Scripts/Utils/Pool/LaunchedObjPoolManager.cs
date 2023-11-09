using System.Collections.Generic;

using UnityEngine;

public class LaunchedObjPoolManager : SingletonMonoBehaviour<LaunchedObjPoolManager>
{
    public enum Type
    {
        arrow,
        tower
    }

    [SerializeField] LaunchedObj _arrowPrefab;
    [SerializeField] LaunchedObj _towerPrefab;

    Dictionary<Type, GameObjectPool<LaunchedObj>> _pools;

    protected override void OnAwake()
    {
        _pools = new();

        _pools.Add(Type.arrow, new(10, () =>
        {
            var o = Instantiate(_arrowPrefab);
            o.gameObject.SetActive(false);
            return o;
        }));

        _pools.Add(Type.tower, new(10, () =>
        {
            var o = Instantiate(_towerPrefab);
            o.gameObject.SetActive(false);
            return o;
        }));
    }

    LaunchedObj Get(Type type)
    {
        var o = _pools[type].Get();
        o.gameObject.SetActive(true);

        return o;
    }

    public void Remove(Type type, LaunchedObj o)
    {
        o.gameObject.SetActive(false);
        _pools[type].Set(o);
    }

    public LaunchedObj GetArrow() => Get(Type.arrow);
    public LaunchedObj GetTower() => Get(Type.tower);

    public void RemoveArrow(LaunchedObj o) => Remove(Type.arrow, o);
    public void RemoveTower(LaunchedObj o) => Remove(Type.tower, o);
}
