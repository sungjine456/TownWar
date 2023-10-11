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

    public Data.BuildingToBuild GetNextLevelBuildingInfo()
    {
        return _buildingInfo.GetBuildingData(SelectedBuilding.BuildingId, SelectedBuilding.CurrentLevel + 1);
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

        if (nextInfo != null && CanBuild())
        {
            if (SelectedBuilding.CurrentLevel < _buildingLimit.GetBuildingLimitLevel(_hallLevel, SelectedBuilding.BuildingId))
            {
                if (GameManager.Instance.MyPlayer.ConsumeResources(nextInfo.requiredGold, nextInfo.requiredElixir, nextInfo.requiredGems))
                {
                    switch (nextInfo.buildingId)
                    {
                        case Data.BuildingId.townHall:
                            UIMain.Instance.AddMaxGold(nextInfo.capacity - SelectedBuilding.Capacity);
                            UIMain.Instance.AddMaxElixir(nextInfo.capacity - SelectedBuilding.Capacity);
                            break;
                        case Data.BuildingId.goldStorage:
                            UIMain.Instance.AddMaxGold(nextInfo.capacity - SelectedBuilding.Capacity);
                            break;
                        case Data.BuildingId.elixirStorage:
                            UIMain.Instance.AddMaxElixir(nextInfo.capacity - SelectedBuilding.Capacity);
                            break;
                    }

                    SelectedBuilding.Upgrade(nextInfo);

                    if (nextInfo.buildingId == Data.BuildingId.townHall)
                        _hallLevel = nextInfo.level;

                    GameManager.Instance.MyPlayer.UpdateBuilding(SelectedBuilding.Id, nextInfo);
                    UIMain.Instance.UpdateBuilder();
                }
            }
            else
                print("회관에 따른 최대 레벨은 넘길 수 없습니다.");
        }
        else
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
                Data.Building building = new(_index++, buildingData.buildingId, buildingData.level, x, y, buildingData.columns, buildingData.rows);
                int gold = buildingData.requiredGold;
                int elixir = buildingData.requiredElixir;
                int gems = buildingData.requiredGems;

                if (buildingData.buildingId == Data.BuildingId.buildersHut)
                    gems = _needfulGemsForBuilderHut[CountBuildBuildingHut()];

                if (GameManager.Instance.MyPlayer.ConsumeResources(gold, elixir, gems))
                {
                    building.SetData(buildingData);

                    GameManager.Instance.MyPlayer.AddBuilding(building);
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

        if (GameManager.Instance.MyPlayer.ConsumeResources(0, 0, requiredGems))
        {
            SelectedBuilding.IsConstructing = false;
            UIBuildingOptions.Instance.SetStatus(true);
            UIMain.Instance.UpdateBuilder();
        }
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
