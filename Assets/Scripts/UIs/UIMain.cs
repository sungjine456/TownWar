using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIMain : SingletonMonoBehaviour<UIMain>
{
    [SerializeField] TextMeshProUGUI _goldText;
    [SerializeField] Button _shopBtn;

    protected override void OnStart()
    {
        _shopBtn.onClick.AddListener(ClickedShopButton);
    }

    void ClickedShopButton()
    {

    }
}
