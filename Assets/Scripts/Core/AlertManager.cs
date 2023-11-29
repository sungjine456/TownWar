using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class AlertManager : SingletonDontDestroy<AlertManager>
{
    [SerializeField] Alert _prefab;
    [SerializeField] RectTransform _parent;

    Canvas _canvas;
    GameObjectPool<Alert> _alertPool;
    RectTransform _alerts;
    List<Alert> _list;

    protected override void OnAwake()
    {
        _list = new();
        SceneManager.activeSceneChanged += ChangedActiveScene;
    }

    void ChangedActiveScene(Scene current, Scene next)
    {
        _canvas = FindFirstObjectByType<Canvas>();
        _alerts = Instantiate(_parent);
        _alerts.transform.SetParent(_canvas.transform, false);
        _alertPool = new(5, () =>
        {
            var obj = Instantiate(_prefab, _alerts);
            obj.gameObject.SetActive(false);
            return obj;
        });
    }

    public void Error(string message)
    {
        var alert = _alertPool.Get();
        alert.Initialize(message, Color.red);
        _list.Add(alert);
    }

    public void Remove(Alert alert)
    {
        _list.Remove(alert);
        alert.gameObject.SetActive(false);
        _alertPool.Set(alert);
    }
}
