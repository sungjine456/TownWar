using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIBuilding : MonoBehaviour
{
    [SerializeField] Button _btn;
    [SerializeField] Data.BuildingId _buildingId;
    [SerializeField] TextMeshProUGUI _requiredResourceText;
    [SerializeField] Image _type;

    void Start()
    {
        _btn.onClick.AddListener(Clicked);

        switch (_buildingId)
        {
            case Data.BuildingId.goldMine:
            case Data.BuildingId.goldStorage:
            case Data.BuildingId.armyCamp:
            case Data.BuildingId.barracks:
                _type.color = Color.yellow;
                _requiredResourceText.text = BuildingController.Instance.GetRequiredElixir(_buildingId, 1).ToString();
                break;
            case Data.BuildingId.elixirMine:
            case Data.BuildingId.elixirStorage:
            case Data.BuildingId.wall:
            case Data.BuildingId.tower:
                _type.color = new(255, 109, 221, 255);
                _requiredResourceText.text = BuildingController.Instance.GetRequiredGold(_buildingId, 1).ToString();
                break;
            case Data.BuildingId.buildersHut:
                _type.color = Color.green;
                _requiredResourceText.text = BuildingController.Instance.GetRequiredGems(_buildingId, 1).ToString();
                break;
        }
    }

    void Clicked()
    {
        if (BuildingController.Instance.CanBuild())
        {
            GameBuilding prefab = GameManager.Instance.GetBuildingPrefab(_buildingId);

            if (prefab)
            {
                UIShop.Instance.SetStatus(false);
                UIMain.Instance.SetStatus(true);

                GameCameraCtrl.Instance.IsPlacingBuilding = true;

                GameBuilding building = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                building.Initialize(true);
                building.SetPosition(20, 20);
                building.AdjustBaseColor();
                building.CurrentLevel = 1;

                UIBuild.Instance.Target = building;
                UIBuild.Instance.SetStatus(true);
            }
        }
    }
}
