using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITrain : SingletonMonoBehaviour<UITrain>
{
    [SerializeField] GameObject _elements;
    [SerializeField] Button _closeBtn;
    [SerializeField] Button _armyTab;
    [SerializeField] Button _trainTab;
    [SerializeField] RectTransform _armyPanel;
    [SerializeField] RectTransform _trainPanel;

    List<(Button, RectTransform)> _btnList;

    protected override void OnAwake()
    {
        SetStatus(false);

        _btnList = new()
        {
            (_armyTab, _armyPanel), (_trainTab, _trainPanel)
        };
    }

    void OnEnable()
    {
        ChangeTab(_trainTab);
    }

    protected override void OnStart()
    {
        _closeBtn.onClick.AddListener(ClickedClose);
        _armyTab.onClick.AddListener(() => ChangeTab(_armyTab));
        _trainTab.onClick.AddListener(() => ChangeTab(_trainTab));
    }

    void ClickedClose()
    {
        SetStatus(false);
        UIMain.Instance.SetStatus(true);
    }

    void ChangeTab(Button target)
    {
        for (int i = 0; i < _btnList.Count; i++)
        {
            if (target == _btnList[i].Item1)
                Change(target, _btnList[i].Item2, true);
            else
                Change(_btnList[i].Item1, _btnList[i].Item2, false);
        }
        
        void Change(Button targetBtn, RectTransform targetPanel, bool isSelect)
        {
            Utils.ChangeButtonColor(targetBtn, isSelect ? Color.white : Color.gray);
            targetPanel.gameObject.SetActive(isSelect);
        }
    }

    public void SetStatus(bool status)
    {
        _elements.SetActive(status);
    }
}
