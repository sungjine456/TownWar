using System;
using UnityEngine;

public class BuildingController : SingletonMonoBehaviour<BuildingController>
{
    const int MAX_BUILDERS_HUT = 5;

    [SerializeField] BuildingInfo _buildingInfo;
    [SerializeField] BuildingLimit _buildingLimit;

    int _hallLevel;
    int _index;
    readonly int[] _needfulGemsForBuilderHut = new int[] { 0, 250, 500, 1000, 2000 };

    public GameBuilding SelectedBuilding { get; private set; }
    public int HallLevel { get { return _hallLevel; } set { _hallLevel = value; } }

    protected override void OnStart()
    {
        _index = UIMain.Instance.Grid._buildings.Count;

        for (int i = 0; i < UIMain.Instance.Grid._buildings.Count; i++)
        {
            if (UIMain.Instance.Grid._buildings[i].BuildingId == Data.BuildingId.townHall)
                _hallLevel = UIMain.Instance.Grid._buildings[i].data.level;
        }
    }

    bool CheckMaxBuilder()
    {
        int count = CountBuildBuildingHut();

        if (count < MAX_BUILDERS_HUT)
            return false;

        print("장인기지는 5개 이상 지을 수 없습니다.");
        return true;
    }

    int CountBuilding(Data.BuildingId id)
    {
        int count = 0;

        for (int i = 0; i < UIMain.Instance.Grid._buildings.Count; i++)
        {
            if (UIMain.Instance.Grid._buildings[i].BuildingId == id)
                count++;
        }

        return count;
    }

    int CountBuildingsConstruction()
    {
        int count = 0;

        for (int i = 0; i < UIMain.Instance.Grid._buildings.Count; i++)
        {
            if (UIMain.Instance.Grid._buildings[i].IsConstructing)
                count++;
        }

        return count;
    }

    public bool HasNextLevelBuildingInfo(GameBuilding target = null)
    {
        return GetNextLevelBuildingInfo(target) != null;
    }

    public Data.BuildingToBuild GetNextLevelBuildingInfo(GameBuilding target = null)
    {
        if (target == null)
            target = SelectedBuilding;

        return _buildingInfo.GetBuildingData(target.BuildingId, target.CurrentLevel + 1);
    }

    public int GetRequiredGold(Data.BuildingId id, int level)
    {
        return _buildingInfo.GetBuildingData(id, level).requiredGold;
    }

    public int GetRequiredElixir(Data.BuildingId id, int level)
    {
        return _buildingInfo.GetBuildingData(id, level).requiredElixir;
    }

    public int GetRequiredGems(Data.BuildingId id, int level)
    {
        if (id == Data.BuildingId.buildersHut)
            return _needfulGemsForBuilderHut[CountBuildBuildingHut()];

        return _buildingInfo.GetBuildingData(id, level).requiredGems;
    }

    public bool CanBuild()
    {
        if (CountBuildBuildingHut() > CountBuildingsConstruction())
            return true;

        print("장인기지가 꽉 찼습니다.");
        return false;
    }

    public void UpgradeBuilding()
    {
        var nextInfo = GetNextLevelBuildingInfo();

        if (nextInfo is not null && CanBuild())
        {
            if (SelectedBuilding.CurrentLevel < _buildingLimit.GetBuildingLimitLevel(_hallLevel, SelectedBuilding.BuildingId))
            {
                if (Player.Instance.ConsumeResources(nextInfo.requiredGold, nextInfo.requiredElixir, nextInfo.requiredGems))
                    SelectedBuilding.Upgrade(nextInfo);
            }
            else
                print("회관에 따른 최대 레벨은 넘길 수 없습니다.");
        }
        else if (nextInfo is null)
            print("다음 레벨이 없습니다.");
    }

    public void BuildBuilding(Data.BuildingToBuild buildingData, int x, int y)
    {
        if (CanBuild())
        {
            if (buildingData.buildingId == Data.BuildingId.buildersHut && CheckMaxBuilder())
                return;

            if (CountBuilding(buildingData.buildingId) < _buildingLimit.GetBuildingLimitCount(_hallLevel, buildingData.buildingId))
            {
                Data.Building building = new(_index++, buildingData.buildingId, 0, x, y, buildingData.columns, buildingData.rows);
                int gems = buildingData.requiredGems;

                if (buildingData.buildingId == Data.BuildingId.buildersHut)
                    gems = _needfulGemsForBuilderHut[CountBuildBuildingHut()];

                if (Player.Instance.ConsumeResources(buildingData.requiredGold, buildingData.requiredElixir, gems))
                {
                    building.SetData(buildingData);

                    Player.Instance.AddBuilding(building);
                    UIMain.Instance.Grid.BuildBuilding(building);
                    UIMain.Instance.UpdateBuilder();
                }
            }
            else
                print("회관에 따른 최대 건설 수를 넘을 수 없습니다.");
        }
    }

    public void InstantUpgradeBuilding()
    {
        var requiredGems = GameManager.Instance.GetInstantTimeRequiredGems(SelectedBuilding.BuildTime);

        if (Player.Instance.ConsumeResources(0, 0, requiredGems))
            SelectedBuilding.InstantUpgrade();
        else
            print("Gem이 부족합니다.");
    }

    public void SelectBuilding(GameBuilding building)
    {
        if (SelectedBuilding != null && SelectedBuilding != building)
            DeselectBuilding();

        SelectedBuilding = building;
        SelectedBuilding.StatusBaseArea(true);
        UIBuildingOptions.Instance.SetStatus(true);
    }

    public void DeselectBuilding()
    {
        UIBuildingOptions.Instance.SetStatus(false);

        if (SelectedBuilding != null)
        {
            SelectedBuilding.StatusBaseArea(false);

            if (UIMain.Instance.Grid.CanPlaceBuilding(SelectedBuilding))
                SelectedBuilding.SavePosition();
            else
                SelectedBuilding.ResetPosition();

            SelectedBuilding = null;
        }

        GameCameraCtrl.Instance.IsReplacingBuilding = false;
    }

    public int CountBuildBuildingHut()
    {
        int count = 0;

        for (int i = 0; i < UIMain.Instance.Grid._buildings.Count; i++)
        {
            if (UIMain.Instance.Grid._buildings[i].BuildingId == Data.BuildingId.buildersHut)
                count++;
        }

        return count;
    }
}
