using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIMain : UIMonoBehaviour<UIMain>
{
    [SerializeField] TextMeshProUGUI _goldText;
    [SerializeField] Button _shopBtn;
    [SerializeField] Building[] buildingPrefabs;

    public bool IsActive { get; private set; }

    protected override void OnStart()
    {
        _shopBtn.onClick.AddListener(ClickedShopButton);
    }

    void ClickedShopButton()
    {
        SetActive(false);
        UIShop.Instance.SetActive(true);
    }

    public override void SetActive(bool active)
    {
        base.SetActive(active);
        IsActive = active;
    }
}
