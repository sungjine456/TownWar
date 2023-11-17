using System.Collections;

using UnityEngine;

public class ResourceBuilding : GameBuilding
{
    UIButton _collectButton;
    bool _workingCollect;

    readonly WaitForSecondsRealtime _wfsr = new(1f);

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

                Player.Instance.UpdateResourceBuildingStorage(Id, data.storage);
            }

            switch (BuildingId)
            {
                case Data.BuildingId.goldMine:
                    if (_collectButton && _collectButton.gameObject.activeSelf)
                        _collectButton.SetActive(!UIMain.Instance.IsFullGold());
                    break;
                case Data.BuildingId.elixirMine:
                    if (_collectButton && _collectButton.gameObject.activeSelf)
                        _collectButton.SetActive(!UIMain.Instance.IsFullElixir());
                    break;
            }

            yield return _wfsr;
        }
    }

    void AdjustCollectUI()
    {
        if (!GameCameraCtrl.Instance.IsPlacingBuilding || !IsConstructing)
        {
            if (data.storage >= Data.minResourceCollect)
            {
                if (!_collectButton)
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
                else if (!_collectButton.gameObject.activeSelf)
                    _collectButton.gameObject.SetActive(true);
            }

            if (_collectButton && _collectButton.gameObject.activeSelf)
                _collectButton._rect.anchoredPosition = CalculateChildPosition();
        }
    }

    public void Collect()
    {
        int remainedResource = 0;
        int resource = (int)data.storage;
        ParticleSystem ps;

        switch (BuildingId)
        {
            case Data.BuildingId.goldMine:
                remainedResource = UIMain.Instance.CollectGold(resource);
                break;
            case Data.BuildingId.elixirMine:
                remainedResource = UIMain.Instance.CollectElixir(resource);
                break;
        }

        if (remainedResource < Data.minResourceCollect)
        {
            switch (BuildingId)
            {
                case Data.BuildingId.goldMine:
                    UICollectPoolManager.Instance.RemoveGold(_collectButton);
                    ps = ParticlePoolManager.Instance.Get(Color.yellow);
                    ps.transform.position = new(transform.position.x, 1.8f, transform.position.z);
                    ps.Play();
                    break;
                case Data.BuildingId.elixirMine:
                    UICollectPoolManager.Instance.RemoveElixir(_collectButton);
                    ps = ParticlePoolManager.Instance.Get(new(1, 0.427f, 0.867f));
                    ps.transform.position = new(transform.position.x, 1.8f, transform.position.z);
                    ps.Play();
                    break;
            }

            data.storage = remainedResource;
        }
        else if (remainedResource != resource)
        {
            switch (BuildingId)
            {
                case Data.BuildingId.goldMine:
                    ps = ParticlePoolManager.Instance.Get(Color.yellow);
                    ps.transform.position = new(transform.position.x, 1.8f, transform.position.z);
                    ps.Play();
                    break;
                case Data.BuildingId.elixirMine:
                    ps = ParticlePoolManager.Instance.Get(new(1, 0.427f, 0.867f));
                    ps.transform.position = new(transform.position.x, 1.8f, transform.position.z);
                    ps.Play();
                    break;
            }

            data.storage = remainedResource;
        }
        else
            AlertManager.Instance.Error("자원이 가득 찼습니다.");
    }
}
