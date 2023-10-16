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
                _bar = UIBarPoolManager.Instance.Get();
            else if (_bar)
                UIBarPoolManager.Instance.Remove(_bar);
        }
    }

    public new virtual void Initialize() => base.Initialize();

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
        GameManager.Instance.MyPlayer.UpdateBuildingPosition(id, CurrentX, CurrentY);
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

        for (int i = 0; i < levels.Length; i++)
        {
            if (levels[i].level != data.level)
                levels[i].mesh.SetActive(false);
            else
                levels[i].mesh.SetActive(true);
        }

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
                    UIMain.Instance.AddMaxGold(_nextBuilding.capacity - Capacity);
                    UIMain.Instance.AddMaxElixir(_nextBuilding.capacity - Capacity);
                    break;
                case Data.BuildingId.goldStorage:
                    UIMain.Instance.AddMaxGold(_nextBuilding.capacity - Capacity);
                    break;
                case Data.BuildingId.elixirStorage:
                    UIMain.Instance.AddMaxElixir(_nextBuilding.capacity - Capacity);
                    break;
            }

            if (_nextBuilding.buildingId == Data.BuildingId.townHall)
                BuildingController.Instance.HallLevel = _nextBuilding.level;

            data.SetData(_nextBuilding);

            GameManager.Instance.MyPlayer.UpdateBuilding(Id, _nextBuilding);

            _nextBuilding = null;
        }

        if (IsBuilding)
        {
            switch (BuildingId)
            {
                case Data.BuildingId.townHall:
                    UIMain.Instance.AddMaxGold(Capacity);
                    UIMain.Instance.AddMaxElixir(Capacity);
                    break;
                case Data.BuildingId.goldStorage:
                    UIMain.Instance.AddMaxGold(Capacity);
                    break;
                case Data.BuildingId.elixirStorage:
                    UIMain.Instance.AddMaxElixir(Capacity);
                    break;
            }

            IsBuilding = false;
        }
        
        IsConstructing = false;
        UIBuildingOptions.Instance.SetStatus(false);
        UIMain.Instance.UpdateBuilder();
    }

    public bool CanUpgrade()
    {
        return !IsConstructing 
            && Data.BuildingId.buildersHut != buildingId 
            && BuildingController.Instance.GetNextLevelBuildingInfo() != null;
    }
}
