using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class UIBuildingOptions : SingletonMonoBehaviour<UIBuildingOptions>
{
    [SerializeField] GameObject _elements;
    [SerializeField] RectTransform _infoPanel;
    [SerializeField] RectTransform _upgradePanel;
    [SerializeField] RectTransform _instantPanel;
    [SerializeField] RectTransform _trainPanel;
    [SerializeField] Button _infoBtn;
    [SerializeField] Button _upgradeBtn;
    [SerializeField] Button _instantBtn;
    [SerializeField] Button _trainBtn;
    [SerializeField] TextMeshProUGUI _instantResourceText;

    int requiredGems;

    protected override void OnAwake()
    {
        SetStatus(false);

        _infoBtn.onClick.AddListener(ClickedInfoBtn);
        _upgradeBtn.onClick.AddListener(ClickedUpgradeBtn);
        _instantBtn.onClick.AddListener(ClickedInstantBtn);
        _trainBtn.onClick.AddListener(ClickedTrainBtn);
    }

    void ClickedInfoBtn()
    {
        // TODO: 정보 보여주기
    }

    void ClickedUpgradeBtn()
    {
        var buildingData = BuildingController.Instance.GetNextLevelBuildingInfo();

        if (buildingData is not null)
            UIBuildingUpgrade.Instance.Open(buildingData);
        else
            AlertManager.Instance.Error("다음 레벨이 없습니다.");
    }

    void ClickedInstantBtn()
    {
        if (Player.Instance.ConsumeResources(0, 0, requiredGems))
        {
            BuildingController.Instance.InstantUpgradeBuilding();
        }
        else
            AlertManager.Instance.Error("Gem이 부족합니다.");
    }

    void ClickedTrainBtn()
    {
        UITrain.Instance.SetStatus(true);
    }

    public void SetStatus(bool status)
    {
        if (status && BuildingController.Instance.SelectedBuilding != null)
        {
            _infoPanel.gameObject.SetActive(true);
            _upgradePanel.gameObject.SetActive(BuildingController.Instance.SelectedBuilding.CanUpgrade());
            _instantPanel.gameObject.SetActive(BuildingController.Instance.SelectedBuilding.IsConstructing);
            _trainPanel.gameObject.SetActive(
                !BuildingController.Instance.SelectedBuilding.IsConstructing && 
                (BuildingController.Instance.SelectedBuilding.BuildingId == Data.BuildingId.barracks || 
                BuildingController.Instance.SelectedBuilding.BuildingId == Data.BuildingId.armyCamp));

            requiredGems = GameManager.Instance.GetInstantTimeRequiredGems(BuildingController.Instance.SelectedBuilding.BuildTime);
            _instantResourceText.text = requiredGems.ToString();

            if (requiredGems > Player.Instance.Gems)
            {
                Utils.ChangeButtonColor(_instantBtn, Color.gray);
                _instantResourceText.color = Color.red;
            }
        }

        _elements.SetActive(status);
    }
}
