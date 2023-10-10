using System.Collections;
using UnityEngine;

public class ResourceBuilding : GameBuilding
{
    UIButton _collectButton;
    bool _workingCollect;

    void Start()
    {
        if (!GameManager.Instance.IsBattling)
            StartCoroutine(CollectReources());
    }

    protected override void Update()
    {
        base.Update();

        if (!GameManager.Instance.IsBattling)
        {
            if (!IsConstructing)
                AdjustCollectUI();

            if (!IsConstructing && !_workingCollect)
                StartCoroutine(CollectReources());
            else if (IsConstructing && _workingCollect)
                StopCoroutine(CollectReources());
        }
    }

    IEnumerator CollectReources()
    {
        _workingCollect = true;

        while (true)
        {
            if (!GameCameraCtrl.Instance.IsPlacingBuilding || IsConstructing)
            {
                if (data.storage > data.capacity)
                    data.storage = data.capacity;
                else
                    data.storage += (data.speed / 3600f);
            }

            yield return YieldInstructionCache.WaitForSecondsRealtime(1f);
        }
    }

    void AdjustCollectUI()
    {
        if (!GameCameraCtrl.Instance.IsPlacingBuilding || !IsConstructing)
        {
            if (!_collectButton && data.storage >= Data.minGoldCollect)
            {
                switch (BuildingId)
                {
                    case Data.BuildingId.goldMine:
                        _collectButton = UICollectPoolManager.Instance.GetGold();
                        _collectButton._button.onClick.AddListener(Collect);
                        break;
                    case Data.BuildingId.elixirMine:
                        _collectButton = UICollectPoolManager.Instance.GetElixir();
                        _collectButton._button.onClick.AddListener(Collect);
                        break;
                }
            }

            if (_collectButton)
            {
                Vector3 end = UIMain.Instance.Grid.GetEndPosition(this);
                Vector3 planDownLeft = GameCameraCtrl.Instance._planDownLeft;
                Vector3 planTopRight = GameCameraCtrl.Instance._planTopRight;

                float w = planTopRight.x - planDownLeft.x;
                float h = planTopRight.z - planDownLeft.z;

                float endW = end.x - planDownLeft.x;
                float endH = end.z - planDownLeft.z;

                Vector2 screenPoint = new(endW / w * Screen.width, endH / h * Screen.height);
                _collectButton._rect.anchoredPosition = screenPoint;
            }
        }
    }

    public void Collect()
    {
        int remainedResource = 0;
        int resource = (int)data.storage;

        switch (BuildingId)
        {
            case Data.BuildingId.goldMine:
                remainedResource = UIMain.Instance.CollectGold(resource);
                break;
            case Data.BuildingId.elixirMine:
                remainedResource = UIMain.Instance.CollectElixir(resource);
                break;
        }

        if (remainedResource < Data.minGoldCollect)
        {
            switch (BuildingId)
            {
                case Data.BuildingId.goldMine:
                    UICollectPoolManager.Instance.RemoveGold(_collectButton);
                    break;
                case Data.BuildingId.elixirMine:
                    UICollectPoolManager.Instance.RemoveElixir(_collectButton);
                    break;
            }
            
            data.storage = remainedResource;
        }
        else if (remainedResource != resource)
            data.storage = remainedResource;
        else
            AlertManager.Instance.Error("자원이 가득 찼습니다.");
    }
}
