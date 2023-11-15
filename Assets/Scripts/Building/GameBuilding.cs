using System;
using UnityEngine;

public class GameBuilding : Building
{
    UIBar _bar;
    int _baseX;
    int _baseY;
    bool _isConstrucing;
    Data.BuildingToBuild _nextBuilding;
    
    public bool IsBuilding { get; set; }
    public bool IsConstructing
    {
        get => _isConstrucing;
        set
        {
            _isConstrucing = value;

            if (value)
            {
                Player.Instance.UpdateBuildingIsConstructing(id, true);
                _bar = UIBarPoolManager.Instance.Get();
            }
            else if (_bar)
            {
                Player.Instance.UpdateBuildingIsConstructing(id, false);
                UIBarPoolManager.Instance.Remove(_bar);
            }
        }
    }

    public new virtual void Initialize(bool isStatusBaseArea = false) => base.Initialize(isStatusBaseArea);

    public new void Initialize(Data.Building data)
    {
        Initialize();
        SetData(data);
    }

    protected virtual void Update()
    {
        AdjustConstructingUI();
    }

    void AdjustConstructingUI()
    {
        if (IsConstructing)
        {
            TimeSpan span = data.ConstructTime - DateTime.Now;
            
            if (span.TotalSeconds > 0.01)
            {
                _bar.AdjustUI(BuildTime, span);

                Vector3 end = UIMain.Instance.Grid.GetEndPosition(this);
                Vector3 planDownLeft = GameCameraCtrl.Instance._planDownLeft;
                Vector3 planTopRight = GameCameraCtrl.Instance._planTopRight;

                float w = planTopRight.x - planDownLeft.x;
                float h = planTopRight.z - planDownLeft.z;

                float endW = end.x - planDownLeft.x;
                float endH = end.z - planDownLeft.z;

                Vector2 screenPoint = new(endW / w * Screen.width, endH / h * Screen.height);
                _bar._rect.anchoredPosition = screenPoint;

                Player.Instance.UpdateBuildingConstructTime(data.id, BuildTime - (float)span.TotalSeconds);
            }
            else
                InstantUpgrade();
        }
    }

    public void AdjustBaseColor()
    {
        if (UIMain.Instance.Grid.CanPlaceBuilding(this))
        {
            UIBuild.Instance._clickConfirmButton.interactable = true;
            baseArea.sharedMaterial.color = Color.green;
        }
        else
        {
            UIBuild.Instance._clickConfirmButton.interactable = false;
            baseArea.sharedMaterial.color = Color.red;
        }
    }

    public void SavePosition()
    {
        SetPosition(CurrentX, CurrentY);
        AdjustBaseColor();
        Player.Instance.UpdateBuildingPosition(id, CurrentX, CurrentY);
    }

    public override void SetPosition(int x, int y)
    {
        base.SetPosition(x, y);

        _baseX = x;
        _baseY = y;
    }

    public void ResetPosition()
    {
        CurrentX = _baseX;
        CurrentY = _baseY;
    }

    public void StartMovingOnGrid()
    {
        _baseX = CurrentX;
        _baseY = CurrentY;
    }

    public void UpdateFromGrid(Vector3 basePos, Vector3 currentPos)
    {
        GameBuildGrid grid = UIMain.Instance.Grid;
        Vector3 dir = grid.transform.TransformPoint(currentPos) - grid.transform.TransformPoint(basePos);

        int xDis = Mathf.RoundToInt(dir.z / Data.CELL_SIZE);
        int yDis = Mathf.RoundToInt(-dir.x / Data.CELL_SIZE);

        CurrentX = _baseX + xDis;
        CurrentY = _baseY + yDis;
        transform.position = grid.GetCenterPosition(CurrentX, CurrentY, rows, columns);

        AdjustBaseColor();
    }

    public void Upgrade(Data.BuildingToBuild next)
    {
        _nextBuilding = next;
        data.SetConstructTime(_nextBuilding.buildTime);
        Player.Instance.UpdateBuildingConstructTime(data.id, 0);

        IsConstructing = true;
        UIBuildingOptions.Instance.SetStatus(true);
        UIMain.Instance.UpdateBuilder();
    }

    public void Upgrade(Data.BuildingToBuild next, float constructedTime)
    {
        _nextBuilding = next;
        data.SetConstructTime(next.buildTime, constructedTime);

        IsConstructing = true;
        UIBuildingOptions.Instance.SetStatus(true);
        UIMain.Instance.UpdateBuilder();
    }

    public void InstantUpgrade()
    {
        if (_nextBuilding != null)
        {
            switch (_nextBuilding.buildingId)
            {
                case Data.BuildingId.townHall:
                    GameManager.Instance.AddMaxGold(_nextBuilding.capacity - Capacity);
                    GameManager.Instance.AddMaxElixir(_nextBuilding.capacity - Capacity);
                    break;
                case Data.BuildingId.goldStorage:
                    GameManager.Instance.AddMaxGold(_nextBuilding.capacity - Capacity);
                    break;
                case Data.BuildingId.elixirStorage:
                    GameManager.Instance.AddMaxElixir(_nextBuilding.capacity - Capacity);
                    break;
            }

            if (_nextBuilding.buildingId == Data.BuildingId.townHall)
                BuildingController.Instance.HallLevel = _nextBuilding.level;

            data.SetData(_nextBuilding);

            for (int i = 0; i < levels.Length; i++)
            {
                if (levels[i].level != data.level)
                    levels[i].mesh.SetActive(false);
                else
                    levels[i].mesh.SetActive(true);
            }

            Player.Instance.UpdateBuilding(Id, _nextBuilding);

            _nextBuilding = null;
        }

        if (IsBuilding)
        {
            switch (BuildingId)
            {
                case Data.BuildingId.townHall:
                    GameManager.Instance.AddMaxGold(Capacity);
                    GameManager.Instance.AddMaxElixir(Capacity);
                    break;
                case Data.BuildingId.goldStorage:
                    GameManager.Instance.AddMaxGold(Capacity);
                    break;
                case Data.BuildingId.elixirStorage:
                    GameManager.Instance.AddMaxElixir(Capacity);
                    break;
            }

            IsBuilding = false;
            Player.Instance.UpdateBuildingIsBuilding(id, false);
        }
        
        IsConstructing = false;
        UIMain.Instance.UpdateBuilder();
        BuildingController.Instance.DeselectBuilding();
    }

    public bool CanUpgrade()
    {
        return !IsConstructing 
            && Data.BuildingId.buildersHut != buildingId 
            && BuildingController.Instance.HasNextLevelBuildingInfo();
    }
}
