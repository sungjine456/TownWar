using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIBuildingUpgrade : SingletonMonoBehaviour<UIBuildingUpgrade>
{
    [SerializeField] GameObject _elements;
    [SerializeField] Button _closeBtn;
    public  Button _upgradeBtn;
    [SerializeField] TextMeshProUGUI _reqGold;
    [SerializeField] TextMeshProUGUI _reqElixir;
    [SerializeField] TextMeshProUGUI _reqGems;
    [SerializeField] TextMeshProUGUI _reqTime;

    protected override void OnAwake()
    {
        SetStatus(false);
    }

    protected override void OnStart()
    {
        _closeBtn.onClick.AddListener(Close);
        _upgradeBtn.onClick.AddListener(Upgrade);
    }

    void Upgrade()
    {
        BuildingController.Instance.UpgradeBuilding();

        Close();
    }

    void Close()
    {
        SetStatus(false);

        UIMain.Instance.SetStatus(true);
    }

    void SetStatus(bool status)
    {
        _elements.SetActive(status);
    }

    public void Open(Data.BuildingToBuild data)
    {
        SetStatus(true);

        _reqGold.text = data.requiredGold.ToString();
        _reqElixir.text = data.requiredElixir.ToString();
        _reqGems.text = data.requiredGems.ToString();
        _reqTime.text = data.buildTime.ToString();
    }
}
