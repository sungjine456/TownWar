using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AlertManager : SingletonDontDestroy<AlertManager>
{
    [SerializeField] Alert _prefab;
    [SerializeField] RectTransform _parent;

    Canvas _canvas;
    GameObjectPool<Alert> _alertPool;
    RectTransform _alertRect;
    List<Alert> _alerts;

    protected override void OnAwake()
    {
        _alerts = new List<Alert>();
        SceneManager.activeSceneChanged += ChangedActiveScene;
    }

    void ChangedActiveScene(Scene current, Scene next)
    {
        _canvas = FindFirstObjectByType<Canvas>();
        _alertRect = Instantiate(_parent);
        _alertRect.transform.SetParent(_canvas.transform, false);
        _alertPool = new GameObjectPool<Alert>(5, () =>
        {
            Alert a = Instantiate(_prefab, _alertRect);
            a.gameObject.SetActive(false);
            return a;
        });
    }

    public void Error(string message)
    {
        Alert alert = _alertPool.Get();
        alert.Initialize(message, new Color32(255, 105, 105, 255));

        _alerts.Add(alert);
    }

    public void Remove(Alert alert)
    {
        _alerts.Remove(alert);
        alert.gameObject.SetActive(false);
        _alertPool.Set(alert);
    }
}
