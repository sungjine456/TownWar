using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIMain : SingletonMonoBehaviour<UIMain>
{
    [SerializeField] TextMeshProUGUI _goldText;
    [SerializeField] Button _shopBtn;
    [SerializeField] Building[] buildingPrefabs;

    protected override void OnStart()
    {
        _shopBtn.onClick.AddListener(ClickedShopButton);
    }

    void ClickedShopButton()
    {

    }
}
