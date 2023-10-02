using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIMain : UIMonoBehaviour<UIMain>
{
    [SerializeField] TextMeshProUGUI _goldText;
    [SerializeField] Button _shopBtn;

    public bool IsActive { get; private set; }

    protected override void OnStart()
    {
        _shopBtn.onClick.AddListener(ClickedShopButton);
    }

    void ClickedShopButton()
    {
        SetActive(false);
        UIShop.Instance.SetActive(true);
        UIBuild.Instance.Cancel();
    }

    public override void SetActive(bool active)
    {
        base.SetActive(active);
        IsActive = active;
    }
}
