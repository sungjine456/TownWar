using System.Collections.Generic;

using UnityEngine;

using static Data;

public class UICollectPoolManager : SingletonMonoBehaviour<UICollectPoolManager>
{
    [SerializeField] UIButton _goldPrefab;
    [SerializeField] UIButton _elixirPrefab;
    [SerializeField] RectTransform _parent;

    Dictionary<ResourceType, GameObjectPool<UIButton>> _collectPool;

    protected override void OnStart()
    {
        _collectPool = new()
        {
            {
                ResourceType.gold,
                new(3, () =>
                {
                    var obj = Instantiate(_goldPrefab, _parent);
                    obj.gameObject.SetActive(false);
                    return obj;
                })
            },
            {
                ResourceType.elixir,
                new(3, () =>
                {
                    var obj = Instantiate(_elixirPrefab, _parent);
                    obj.gameObject.SetActive(false);
                    return obj;
                })
            }
        };
    }

    UIButton Get(ResourceType type)
    {
        var collect = _collectPool[type].Get();
        collect.gameObject.SetActive(true);

        return collect;
    }

    void Remove(ResourceType type, UIButton collect)
    {
        collect.gameObject.SetActive(false);
        _collectPool[type].Set(collect);
    }

    public UIButton GetGold() => Get(ResourceType.gold);

    public UIButton GetElixir() => Get(ResourceType.elixir);

    public void RemoveGold(UIButton collect) => Remove(ResourceType.gold, collect);

    public void RemoveElixir(UIButton collect) => Remove(ResourceType.elixir, collect);
}
