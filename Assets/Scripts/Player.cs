using UnityEngine;

using static Data;

public class Player : MonoBehaviour
{
    PlayerData data;

    void Start()
    {
        if (!Load())
        {
            data = new PlayerData { gold = 1000 };

            Save();
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

        data = JsonUtility.FromJson<PlayerData>(PlayerPrefs.GetString("PLAYER_DATA"));

        return true;
    }
}