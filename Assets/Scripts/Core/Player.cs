using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Data;

public class Player : MonoBehaviour
{
    [SerializeField] BuildingInfo _buildingInfo;
    [SerializeField] UnitInfo _unitInfo;
    [SerializeField] Data.Player _data;

    LinkedList<Unit> _armyUnits;
    LinkedList<Unit> _trainedUnits;
    LinkedList<Unit> _trainingUnits;

    public int Gold { get { return _data.gold; } }
    public int Elixir { get { return _data.elixir; } }
    public int Gems { get { return _data.gems; } }

    void Start()
    {
        Load();

        StartCoroutine(nameof(Coroutine_UpdateTime));

        DontDestroyOnLoad(gameObject);
    }

    IEnumerator Coroutine_UpdateTime()
    {
        while(true)
        {
            PlayerPrefs.SetString("PLAYER_LASTTIME", JsonUtility.ToJson((JsonDateTime)DateTime.Now));
            PlayerPrefs.Save();

            yield return YieldInstructionCache.WaitForSecondsRealtime(0.5f);
        }
    }

    void FirstStartSettins()
    {
        _data = new(){ gold = 100000, elixir = 100000, gems = 5000, lastPlayTime = DateTime.Now };

        var buildData = _buildingInfo.GetBuildingData(BuildingId.townHall, 1);
        Data.Building building = new(0, BuildingId.townHall, 1, 25, 25, 4, 4);
        building.SetData(buildData);
        _data.buildings.Add(building.GetPlayerBuilding());

        buildData = _buildingInfo.GetBuildingData(BuildingId.buildersHut, 1);
        building = new(1, BuildingId.buildersHut, 1, 25, 20, 2, 2);
        building.SetData(buildData);
        _data.buildings.Add(building.GetPlayerBuilding());

        Save();
    }

    void Save()
    {
        int i = 0;
        PlayerUnit[] units = new PlayerUnit[_armyUnits.Count];
        
        foreach (var u in _armyUnits)
        {
            units[i++] = u.GetPlayerUnit();
        }
        _data.armyUnits = units;
        i = 0;
        units = new PlayerUnit[_trainedUnits.Count];

        foreach (var u in _trainedUnits)
        {
            units[i++] = u.GetPlayerUnit();
        }
        _data.trainedUnits = units;
        i = 0;
        units = new PlayerUnit[_trainingUnits.Count];

        foreach (var u in _trainingUnits)
        {
            units[i++] = u.GetPlayerUnit();
        }
        _data.trainingUnits = units;

        PlayerPrefs.SetString("PLAYER_DATA", JsonUtility.ToJson(_data));
        PlayerPrefs.SetString("PLAYER_LASTTIME", JsonUtility.ToJson((JsonDateTime)_data.lastPlayTime));
        PlayerPrefs.Save();
    }

    void Load()
    {
        _armyUnits = new();
        _trainedUnits = new();
        _trainingUnits = new();

        if (!PlayerPrefs.HasKey("PLAYER_DATA"))
            FirstStartSettins();
        else
        {
            _data = JsonUtility.FromJson<Data.Player>(PlayerPrefs.GetString("PLAYER_DATA"));
            _data.lastPlayTime = JsonUtility.FromJson<JsonDateTime>(PlayerPrefs.GetString("PLAYER_LASTTIME"));

            for (int i = 0; i < _data.armyUnits.Length; i++)
            {
                Unit unit = new(_unitInfo.GetUnitData(_data.armyUnits[i].id, _data.armyUnits[i].level));
                _armyUnits.AddLast(unit);
            }

            for (int i = 0; i < _data.trainedUnits.Length; i++)
            {
                Unit unit = new(_unitInfo.GetUnitData(_data.trainedUnits[i].id, _data.trainedUnits[i].level));
                _trainedUnits.AddLast(unit);
            }

            for (int i = 0; i < _data.trainingUnits.Length; i++)
            {
                Unit unit = new(_unitInfo.GetUnitData(_data.trainingUnits[i].id, _data.trainingUnits[i].level))
                {
                    trainedTime = _data.trainingUnits[i].trainedTime
                };
                _trainingUnits.AddLast(unit);
            }
        }

        TimeSpan span = DateTime.Now - _data.lastPlayTime;
        float remain = (float)span.TotalSeconds;
        int armyPopulations = GetPopulations(_armyUnits);
        int trainedPopulations = GetPopulations(_trainedUnits);
        int maxUnit = MaxHaveUnit();
        int maxTrainedUnit = MaxTrainingUnit();

        while (_trainingUnits.Count > 0)
        {
            if (remain >= _trainingUnits.First.Value.trainTime - _trainingUnits.First.Value.trainedTime)
            {
                _trainingUnits.First.Value.trainedTime = 0f;

                if (_trainingUnits.First.Value.numberOfPopulation + armyPopulations <= maxUnit)
                {
                    _armyUnits.AddLast(_trainingUnits.First.Value);
                    armyPopulations += _trainingUnits.First.Value.numberOfPopulation;
                }
                else if (_trainingUnits.First.Value.numberOfPopulation + trainedPopulations <= maxTrainedUnit)
                {
                    _trainedUnits.AddLast(_trainingUnits.First.Value);
                    trainedPopulations += _trainingUnits.First.Value.numberOfPopulation;
                }
                else
                    break;

                _trainingUnits.RemoveFirst();
                
                if (_trainingUnits.Count > 0)
                    remain -= _trainingUnits.First.Value.trainTime - _trainingUnits.First.Value.trainedTime;
            }
            else
            {
                if (_trainingUnits.Count > 0)
                    _trainingUnits.First.Value.trainedTime = remain;

                break;
            }
        }

        if (_data.buildings.Count > 0)
        {
            for (int i = 0; i < _data.buildings.Count; i++)
            {
                Data.Building building = new(_buildingInfo.GetBuildingData(_data.buildings[i].buildingId, _data.buildings[i].level))
                {
                    id = _data.buildings[i].id,
                    x = _data.buildings[i].x,
                    y = _data.buildings[i].y
                };

                switch (building.buildingId)
                {
                    case BuildingId.goldMine:
                    case BuildingId.elixirMine:
                        remain = (float)span.TotalSeconds;

                        if (building.storage < building.capacity)
                        {
                            building.storage += (building.speed / 3600f * remain);

                            if (building.storage > building.capacity)
                                building.storage = building.capacity;
                        }
                        break;
                }

                UIMain.Instance.Grid.AddBuilding(building);
            }
        }

        UIMain.Instance.UpdateBuilder();
    }

    int GetPopulations(LinkedList<Unit> list)
    {
        int p = 0;

        foreach (var u in list)
        {
            p += u.numberOfPopulation;
        }

        return p;
    }

    int MaxHaveUnit()
    {
        int count = 0;

        for (int i = 0; i < _data.buildings.Count; i++)
        {
            if (_data.buildings[i].buildingId == BuildingId.armyCamp)
                count += _data.buildings[i].capacity;
        }

        return count;
    }

    Data.PlayerBuilding FindBuilding(int id)
    {
        for (int i = 0; i < _data.buildings.Count; i++)
        {
            if (_data.buildings[i].id == id)
                return _data.buildings[i];
        }

        return null;
    }

    public int MaxTrainingUnit() => MaxHaveUnit() * 2;

    public bool ConsumeResources(int gold, int elixir, int gems)
    {
        if (CheckResources(gold, elixir, gems))
        {
            _data.gold -= gold;
            _data.elixir -= elixir;
            _data.gems -= gems;

            UIMain.Instance.SyncResourcesData();

            Save();

            return true;
        }

        return false;
    }

    public void AddBuilding(Data.Building building)
    {
        _data.buildings.Add(building.GetPlayerBuilding());
        UIMain.Instance.Grid.AddBuilding(building, true);
        Save();
    }

    public void UpdateBuilding(int id, BuildingToBuild building)
    {
        var b = FindBuilding(id);
        
        if (b != null)
        {
            b.SetData(building);

            Save();
        }
    }

    public void UpdateBuildingPosition(int id, int x, int y)
    {
        var b = FindBuilding(id);

        if (b != null)
        {
            b.x = x;
            b.y = y;

            Save();
        }
    }

    public void CollectGold(int gold)
    {
        _data.gold += gold;

        Save();
    }

    public void CollectElixir(int elixir)
    {
        _data.elixir += elixir;

        Save();
    }

    public bool CheckResources(int gold, int elixir, int gems)
    {
        if (_data.gold >= gold && _data.elixir >= elixir && _data.gems >= gems)
            return true;
        else
        {
            AlertManager.Instance.Error("필요한 자원이 부족합니다.");
            return false;
        }
    }

    public bool CanAddUnit(int population)
    {
        return population + GetPopulations(_armyUnits) <= MaxHaveUnit();
    }

    public void AddUnit(Unit unit, UnitStatus status)
    {
        switch (status)
        {
            case UnitStatus.army:
                _armyUnits.AddLast(unit);
                break;
            case UnitStatus.trained:
                _trainedUnits.AddLast(unit);
                break;
            case UnitStatus.training:
                _trainingUnits.AddLast(unit);
                break;
        }
        
        Save();
    }

    public LinkedList<Unit> GetUnits(UnitStatus status)
    {
        if (status == UnitStatus.army)
            return _armyUnits;
        else if (status == UnitStatus.trained)
            return _trainedUnits;
        else
            return _trainingUnits;
    }

    public void RemoveUnit(UnitId id, UnitStatus status)
    {
        foreach (var u in GetUnits(status))
        {
            if (u.id == id)
            {
                GetUnits(status).Remove(u);
                break;
            }   
        }

        Save();
    }
}
