using UnityEngine;

using static Data;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [SerializeField] Building[] _buildingPrefabs;
    [SerializeField] BuildGrid _grid;

    public bool IsPlacing { get; set; }
    public BuildGrid Grid => _grid;

    public Building GetBuildingPrefab(BuildingId id)
    {
        for (int i = 0; i < _buildingPrefabs.Length; i++)
            if (_buildingPrefabs[i].Id == id)
                return _buildingPrefabs[i];

        return null;
    }
}
