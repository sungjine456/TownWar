using UnityEngine;
using UnityEngine.UI;

public class UIBuildingOptions : SingletonMonoBehaviour<UIBuildingOptions>
{
    [SerializeField] GameObject _elements;
    [SerializeField] RectTransform _infoPanel;
    [SerializeField] RectTransform _upgradePanel;
    [SerializeField] RectTransform _instantPanel;
    [SerializeField] RectTransform _trainPanel;
    public Button _infoBtn;
    public Button _upgradeBtn;
    public Button _instantBtn;
    public Button _trainBtn;

    protected override void OnAwake()
    {
        SetStatus(false);
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
        }

        _elements.SetActive(status);
    }
}
