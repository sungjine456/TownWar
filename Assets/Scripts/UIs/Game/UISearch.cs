using UnityEngine;
using UnityEngine.UI;

public class UISearch : SingletonMonoBehaviour<UISearch>
{
    [SerializeField] GameObject _elements;
    [SerializeField] Button _closeBtn;

    protected override void OnAwake()
    {
        _closeBtn.onClick.AddListener(Close);
        SetStatus(false);
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
