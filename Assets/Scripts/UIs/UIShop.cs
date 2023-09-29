using UnityEngine;
using UnityEngine.UI;

public class UIShop : UIMonoBehaviour<UIShop>
{
    [SerializeField] Button _closeBtn;

    protected override void OnStart()
    {
        _closeBtn.onClick.AddListener(ClickedCloseButton);
    }

    void ClickedCloseButton()
    {
        SetActive(false);
        UIMain.Instance.SetActive(true);
    }
}
