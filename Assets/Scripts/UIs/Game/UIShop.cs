using UnityEngine;
using UnityEngine.UI;

public class UIShop : SingletonMonoBehaviour<UIShop>
{
    [SerializeField] GameObject _elements;
    [SerializeField] Button _closeBtn;

    protected override void OnAwake()
    {
        _elements.SetActive(false);
    }

    protected override void OnStart()
    {
        _closeBtn.onClick.AddListener(Close);
    }

    void Close()
    {
        SetStatus(false);
        UIMain.Instance.SetStatus(true);
    }

    public void SetStatus(bool status)
    {
        _elements.SetActive(status);
    }
}
