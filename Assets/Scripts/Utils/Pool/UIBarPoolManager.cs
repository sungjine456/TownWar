using UnityEngine;

public class UIBarPoolManager : SingletonMonoBehaviour<UIBarPoolManager>
{
    [SerializeField] UIBar _barPrefab;
    [SerializeField] RectTransform _parent;

    GameObjectPool<UIBar> _barPool;

    protected override void OnStart()
    {
        _barPool = new(5, () =>
        {
            var obj = Instantiate(_barPrefab, _parent);
            obj.gameObject.SetActive(false);
            return obj;
        });
    }

    public UIBar Get()
    {
        var bar = _barPool.Get();
        bar.gameObject.SetActive(true);

        return bar;
    }

    public void Remove(UIBar bar)
    {
        bar.gameObject.SetActive(false);
        _barPool.Set(bar);
    }
}
