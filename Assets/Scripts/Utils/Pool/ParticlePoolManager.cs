using UnityEngine;

public class ParticlePoolManager : SingletonMonoBehaviour<ParticlePoolManager>
{
    [SerializeField] ParticleSystem _psPrefab;
    [SerializeField] RectTransform _parent;

    GameObjectPool<ParticleSystem> _psPool;

    protected override void OnStart()
    {
        _psPool = new(3, () =>
        {
            var obj = Instantiate(_psPrefab, _parent);
            var main = obj.main;
            obj.gameObject.SetActive(false);
            return obj;
        });
    }

    public ParticleSystem Get(Color color)
    {
        var ps = _psPool.Get();
        var main = ps.main;
        main.startColor = color;
        ps.gameObject.SetActive(true);

        return ps;
    }

    public void Remove(ParticleSystem ps)
    {
        ps.gameObject.SetActive(false);
        _psPool.Set(ps);
    }
}
