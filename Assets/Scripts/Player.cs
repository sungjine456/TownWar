using UnityEngine;

using static Data;

using PlayerData = Data.Player;

public class Player : SingletonMonoBehaviour<Player>
{
    [SerializeField] BuildingInfo _info;

    PlayerData _data;
    int _buildingIdx;
    int _maxGold;
    int _goldStorageCount;
    float _gold;

    public float Gold 
    { 
        get => _gold;
        private set 
        {
            _gold = value;
            UIMain.Instance.SetGold((int)value, _maxGold);
            ShareResources();
        } 
    }

    protected override void OnStart()
    {
        Building prefab;
        float gold = 0;
        Building b;

        if (!Load())
        {
            _data = new PlayerData();

            prefab = GameManager.Instance.GetBuildingPrefab(BuildingId.TownHall);

            if (prefab)
            {
                b = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                b.Idx = NextBuildingIdx();
                b.Initialized(_info.GetBuildingData(BuildingId.TownHall), 20, 20);
                b.Storage = 1000;
                b.SetActiveBaseArea(false);

                GameManager.Instance.Grid.AddBuilding(b);
                _data.buildings.Add(b.GetSaveBuildingData());
            }

            Save();
        }

        for (int i = 0; i < _data.buildings.Count; i++)
        {
            prefab = GameManager.Instance.GetBuildingPrefab(_data.buildings[i].buildingId);

            if (prefab)
            {
                b = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                b.Initialized(_data.buildings[i], _info.GetBuildingData(_data.buildings[i].buildingId));
                b.SetActiveBaseArea(false);

                switch (_data.buildings[i].buildingId)
                {
                    case BuildingId.TownHall:
                        gold += b.Storage;
                        _maxGold += b.Capacity;
                        break;
                    case BuildingId.GoldStorage:
                        _goldStorageCount++;
                        gold += b.Storage;
                        _maxGold += b.Capacity;
                        break;
                }

                if (b.Idx > _buildingIdx)
                    _buildingIdx = b.Idx + 1;

                GameManager.Instance.Grid.AddBuilding(b);
            }
        }

        Gold = gold;
    }

    void Save()
    {
        var json = JsonUtility.ToJson(_data);

        PlayerPrefs.SetString("PLAYER_DATA", json);
        PlayerPrefs.Save();
    }

    bool Load()
    {
        if (!PlayerPrefs.HasKey("PLAYER_DATA"))
            return false;

        _data = JsonUtility.FromJson<PlayerData>(PlayerPrefs.GetString("PLAYER_DATA"));

        return true;
    }

    void ShareResources()
    {
        BuildingToSave hall = null;
        int hallCapacity = _info.GetBuildingData(BuildingId.TownHall).capacity;

        for (int i = 0; i < _data.buildings.Count; i++)
        {
            if (_data.buildings[i].buildingId == BuildingId.TownHall)
                hall = _data.buildings[i];
        }

        float g = Gold;
        float hallGold = g / (_goldStorageCount + 1) > hallCapacity ? hallCapacity : g / (_goldStorageCount + 1);

        hall.storage = hallGold;
        g -= hallGold;

        for (int i = 0; i < _data.buildings.Count; i++)
        {
            if (_data.buildings[i].buildingId == BuildingId.GoldStorage)
                _data.buildings[i].storage = g / _goldStorageCount;
        }
    }

    public int NextBuildingIdx() => _buildingIdx++;

    public void UpdateBuildingPosition(Building building)
    {
        for (int i = 0; i < _data.buildings.Count; i++)
        {
            if (_data.buildings[i].idx == building.Idx)
            {
                _data.buildings[i].x = building.X;
                _data.buildings[i].y = building.Y;
            }
        }

        Save();
    }

    public void UpdateBuildingResources(Building building)
    {
        for (int i = 0; i < _data.buildings.Count; i++)
        {
            if (_data.buildings[i].idx == building.Idx)
                _data.buildings[i].storage = building.Storage;
        }

        Save();
    }

    public void SaveBuilding(Building building)
    {
        if (Gold >= building.RequiredGold)
        {
            _data.buildings.Add(building.GetSaveBuildingData());

            if (building.Id == BuildingId.GoldStorage)
            {
                _maxGold += building.Capacity;
                _goldStorageCount++;
            }

            Gold -= building.RequiredGold;

            Save();
        }
    }

    public void CollectGold(int gold)
    {
        Gold += gold;
        Save();
    }
}
