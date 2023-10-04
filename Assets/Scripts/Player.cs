using UnityEngine;

public class Player : SingletonMonoBehaviour<Player>
{
    Data.Player data;

    protected override void OnStart()
    {

        if (!Load())
        {
            data = new Data.Player { gold = 1000 };

            Save();
        }

        for (int i = 0; i < data.buildings.Count; i++)
        {
            Building prefab = GameManager.Instance.GetBuildingPrefab(data.buildings[i].buildingId);

            if (prefab)
            {
                Building b = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                b.SetPosition(data.buildings[i].x, data.buildings[i].y);
                GameManager.Instance.Grid.AddBuilding(b);
            }
        }
    }

    void Save()
    {
        var json = JsonUtility.ToJson(data);

        PlayerPrefs.SetString("PLAYER_DATA", json);
        PlayerPrefs.Save();
    }

    bool Load()
    {
        if (!PlayerPrefs.HasKey("PLAYER_DATA"))
            return false;

        data = JsonUtility.FromJson<Data.Player>(PlayerPrefs.GetString("PLAYER_DATA"));

        return true;
    }

    public void SaveBuilding(Building building)
    {
        data.buildings.Add(building.GetBuildingData());
        Save();
    }
}
