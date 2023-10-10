using UnityEngine;
using UnityEngine.UI;

public class UIShop : UIMonoBehaviour<UIShop>
{
    [SerializeField] Button _closeBtn;
    [SerializeField] UIShopItem[] items;

    protected override void OnStart()
    {
        _closeBtn.onClick.AddListener(ClickedCloseButton);
    }

    void ClickedCloseButton()
    {
        SetActive(false);
        UIMain.Instance.SetActive(true);
    }

    public override void SetActive(bool active)
    {
        base.SetActive(active);

        if (active)
        {
            for (int i = 0; i < items.Length; i++)
            {
                items[i].SetDisabled(!Player.Instance.CanMakeBuilding(items[i].Id));
            }
        }
    }
}
