using System.Collections;
using UnityEngine;

public class ResourceBuilding : Building
{
    UIButton collectButton;

    void Awake()
    {
        switch (Id)
        {
            case Data.BuildingId.GoldMine:
                collectButton = Instantiate(UIMain.Instance._collectGold, UIMain.Instance._buttonsParent);
                collectButton.button.onClick.AddListener(Collect);
                break;
        }
    }

    void Start()
    {
        StartCoroutine(CollectReources());
    }

    void Update()
    {
        AdjustUI();
    }

    IEnumerator CollectReources()
    {
        while (true)
        {
            if (Storage > Capacity)
                Storage = Capacity;
            else
                Storage += (Speed / 360f);

            yield return new WaitForSecondsRealtime(1.0f);
        }
    }

    void AdjustUI()
    {
        switch (Id)
        {
            case Data.BuildingId.GoldMine:
                collectButton.gameObject.SetActive(Storage >= Data.MIN_COLLECT_RESOUCES);
                break;
        }

        if (collectButton.gameObject.activeSelf)
        {
            Vector3 end = GameManager.Instance.Grid.GetEndPosition(this);
            Vector3 planDownLeft = GameManager.Instance.Camera.planDownLeft;
            Vector3 planTopRight = GameManager.Instance.Camera.planTopRight;

            float w = planTopRight.x - planDownLeft.x;
            float h = planTopRight.z - planDownLeft.z;

            float endW = end.x - planDownLeft.x;
            float endH = end.z - planDownLeft.z;

            Vector2 screenPoint = new(endW / w * Screen.width, endH / h * Screen.height);
            collectButton.rect.anchoredPosition = screenPoint;
        }
    }

    public void Collect()
    {
        collectButton.gameObject.SetActive(false);
        int gold = (int)Storage;
        Storage -= gold;
        Player.Instance.CollectGold(gold);
    }
}
