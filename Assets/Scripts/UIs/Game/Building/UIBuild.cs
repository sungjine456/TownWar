using UnityEngine;
using UnityEngine.UI;

public class UIBuild : SingletonMonoBehaviour<UIBuild>
{
    [SerializeField] GameObject _elements;
    [SerializeField] BuildingInfo _buildingInfo;

    public RectTransform _buttonConfirm;
    public RectTransform _buttonCancel;
    
    [HideInInspector] public Button _clickConfirmButton;

    public GameBuilding Target { get; set; }

    protected override void OnAwake()
    {
        _clickConfirmButton = _buttonConfirm.gameObject.GetComponent<Button>();
    }

    protected override void OnStart()
    {
        _buttonConfirm.gameObject.GetComponent<Button>().onClick.AddListener(Confirm);
        _buttonCancel.gameObject.GetComponent<Button>().onClick.AddListener(Cancel);

        _buttonConfirm.anchorMin = Vector3.zero;
        _buttonConfirm.anchorMax = Vector3.zero;
        _buttonCancel.anchorMin = Vector3.zero;
        _buttonCancel.anchorMax = Vector3.zero;
    }

    void Update()
    {
        if (Target != null && GameCameraCtrl.Instance.IsPlacingBuilding)
        {
            Vector3 end = UIMain.Instance.Grid.GetEndPosition(Target);

            Vector3 planDownLeft = GameCameraCtrl.Instance._planDownLeft;
            Vector3 planTopRight = GameCameraCtrl.Instance._planTopRight;

            float w = planTopRight.x - planDownLeft.x;
            float h = planTopRight.z - planDownLeft.z;

            float endW = end.x - planDownLeft.x;
            float endH = end.z - planDownLeft.z;

            Vector2 screenPoint = new Vector2(endW / w * Screen.width, endH / h * Screen.height);

            Vector2 confirmPos = screenPoint;
            confirmPos.x += _buttonConfirm.rect.width + 10f;
            _buttonConfirm.anchoredPosition = confirmPos;

            Vector2 cancelPos = screenPoint;
            cancelPos.x -= _buttonCancel.rect.width - 10f;
            _buttonCancel.anchoredPosition = cancelPos;
        }
    }

    void Confirm()
    {
        if (Target != null && UIMain.Instance.Grid.CanPlaceBuilding(Target))
        {
            var data = _buildingInfo.GetBuildingData(Target.BuildingId, Target.CurrentLevel);
            BuildingController.Instance.BuildBuilding(data, Target.CurrentX, Target.CurrentY);
            Cancel();
        }
    }

    public void Cancel()
    {
        if (Target != null)
        {
            GameCameraCtrl.Instance.IsPlacingBuilding = false;
            Destroy(Target.gameObject);
            Target = null;
            SetStatus(false);
        }
    }

    public void SetStatus(bool status)
    {
        _elements.SetActive(status);
    }
}
