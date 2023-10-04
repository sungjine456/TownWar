using UnityEngine;
using UnityEngine.UI;

public class UIBuild : UIMonoBehaviour<UIBuild>
{
    public RectTransform confirmButton;
    public RectTransform cancelButton;

    protected override void OnStart()
    {
        base.OnStart();

        confirmButton.gameObject.GetComponent<Button>().onClick.AddListener(Confirm);
        cancelButton.gameObject.GetComponent<Button>().onClick.AddListener(Cancel);
    }

    void Update()
    {
        if (BuildManager.Instance.HasTarget() && GameManager.Instance.IsPlacing)
        {
            Vector3 end = GameManager.Instance.Grid.GetEndPosition(BuildManager.Instance.CurrentTarget);

            Vector3 planDownLeft = GameManager.Instance.Camera.CameraScreenPositionToPlanePosition(Vector2.zero);
            Vector3 planTopRight = GameManager.Instance.Camera.CameraScreenPositionToPlanePosition(new Vector2(Screen.width, Screen.height));

            float w = planTopRight.x - planDownLeft.x;
            float h = planTopRight.z - planDownLeft.z;

            float endW = end.x - planDownLeft.x;
            float endH = end.z - planDownLeft.z;

            Vector2 screenPoint = new(endW / w * Screen.width, endH / h * Screen.height);

            Vector2 confirmPos = screenPoint;
            confirmPos.x += confirmButton.rect.width + 10f;
            confirmButton.anchoredPosition = confirmPos;

            Vector2 cancelPos = screenPoint;
            cancelPos.x -= cancelButton.rect.width - 10f;
            cancelButton.anchoredPosition = cancelPos;
        }
    }

    void Confirm()
    {
        Building building = BuildManager.Instance.CurrentTarget;

        if (GameManager.Instance.Grid.CanPlaceBuilding(building))
        {
            BuildManager.Instance.AddFromGrid();
            SetActive(false);
        }
    }

    public void Cancel()
    {
        if (BuildManager.Instance.HasTarget())
        {
            BuildManager.Instance.RemoveFromGrid();
            SetActive(false);
        }
    }
}
