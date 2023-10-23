using UnityEngine;

public class ArrowPoolManager : SingletonMonoBehaviour<ArrowPoolManager>
{
    [SerializeField] Arrow _arrowPrefab;

    GameObjectPool<Arrow> _pool;

    protected override void OnAwake()
    {
        _pool = new(10, () =>
        {
            var o = Instantiate(_arrowPrefab);
            o.gameObject.SetActive(false);
            return o;
        });
    }

    public Arrow Get()
    {
        var arrow = _pool.Get();
        arrow.gameObject.SetActive(true);

        return arrow;
    }

    public void Remove(Arrow arrow)
    {
        arrow.gameObject.SetActive(false);
        _pool.Set(arrow);
    }
}
