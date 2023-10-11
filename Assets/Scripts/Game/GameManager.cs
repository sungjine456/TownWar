using UnityEngine;

public class GameManager : SingletonDontDestroy<GameManager>
{
    [SerializeField] GameBuilding[] _buildingPrefabs;
    [SerializeField] Player _myPlayer;

    public Player MyPlayer => _myPlayer;
    public BuildGrid Grid => IsBattling ? BattleManager.Instance.Grid : UIMain.Instance.Grid;
    public bool IsBattling => BattleManager.Instance != null;

    public GameBuilding GetBuildingPrefab(Data.BuildingId buildingId)
    {
        for (int i = 0; i < _buildingPrefabs.Length; i++)
        {
            if (_buildingPrefabs[i].BuildingId == buildingId)
                return _buildingPrefabs[i];
        }

        return null;
    }

    public int GetInstantTimeRequiredGems(int remainedSeconds)
    {
        if (remainedSeconds > 0)
        {
            if (remainedSeconds <= 60)
                return 1;
            else if (remainedSeconds <= 60 * 60)
                return (int)(0.00537f * (remainedSeconds - 60)) + 1;
            else if (remainedSeconds <= 60 * 60 * 24)
                return (int)(0.00266f * (remainedSeconds - (60 * 60))) + 20;
            else
                return (int)(0.00143f * (remainedSeconds - (60 * 60 * 24))) + 260;
        }

        return 0;
    }
}
