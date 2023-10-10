using UnityEngine;
using UnityEngine.UI;

public class UIBuilding : MonoBehaviour
{
    [SerializeField] Button _btn;
    [SerializeField] Data.BuildingId _buildingId;

    void Start()
    {
        _btn.onClick.AddListener(Clicked);
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
                building.Initialize();
                building.SetPosition(20, 20);
                building.AdjustBaseColor();
                building.CurrentLevel = 1;

                UIBuild.Instance.Target = building;
                UIBuild.Instance.SetStatus(true);
            }
        }
    }
}
