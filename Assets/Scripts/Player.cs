using UnityEngine;

public class Player : SingletonMonoBehaviour<Player>
{
    [SerializeField] BuildingInfo _info;

    Data.Player _data;
    int _buildingIdx;

    protected override void OnStart()
    {
        if (!Load())
        {
            _data = new Data.Player { gold = 1000 };

            Save();
        }

        UIMain.Instance.SetGold(_data.gold);

        for (int i = 0; i < _data.buildings.Count; i++)
        {
            Building prefab = GameManager.Instance.GetBuildingPrefab(_data.buildings[i].buildingId);

            if (prefab)
            {
                Building b = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                b.Idx = _data.buildings[i].idx;
                b.Initialized(_info.GetBuildingData(_data.buildings[i].buildingId), _data.buildings[i].x, _data.buildings[i].y);
                b.SetActiveBaseArea(false);

                if (b.Idx > _buildingIdx)
                    _buildingIdx = b.Idx + 1;

                GameManager.Instance.Grid.AddBuilding(b);
            }
        }
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

        _data = JsonUtility.FromJson<Data.Player>(PlayerPrefs.GetString("PLAYER_DATA"));

        return true;
    }

    public int NextBuildingIdx() => _buildingIdx++;

    public void UpdateBuilding(Building building)
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

    public void SaveBuilding(Building building)
    {
        if (_data.gold >= building.RequiredGold)
        {
            _data.buildings.Add(building.GetSaveBuildingData());
            _data.gold -= building.RequiredGold;

            UIMain.Instance.SetGold(_data.gold);

            Save();
        }
    }
}
