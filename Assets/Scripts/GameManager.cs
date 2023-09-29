using UnityEngine;

using static Data;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [SerializeField] Building[] buildingPrefabs;

    public Building GetBuildingPrefab(BuildingId id)
    {
        for (int i = 0; i < buildingPrefabs.Length; i++)
            if (buildingPrefabs[i].Id == id)
                return buildingPrefabs[i];

        return null;
    }
}
