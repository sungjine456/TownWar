using System.Collections.Generic;
using UnityEngine;

public class BattleManager : SingletonMonoBehaviour<BattleManager>
{
    public static int LoadedNumberOfMap = -1;

    [SerializeField] UIBattleMain _main;
    [SerializeField] BattleMap _maps;
    [SerializeField] BuildingInfo _buildingInfo;
    [SerializeField] BuildGrid _grid;
    [SerializeField] BattleFieldUnit[] _unitPrefabs;

    BattleMapData[] _datas;

    public BuildGrid Grid => _grid;

    protected override void OnStart()
    {
        if (LoadedNumberOfMap >= 1)
        {
            List<Data.Building> buildings = new();
            _datas = _maps.GetMapData(LoadedNumberOfMap);

            for (int i = 0; i < _datas.Length; i++)
            {
                Data.Building data = new(_buildingInfo.GetBuildingData(_datas[i].id, _datas[i].level))
                {
                    id = i,
                    x = _datas[i].x,
                    y = _datas[i].y
                };

                buildings.Add(data);
            }

            UIBattleMain.Instance.Initialize(buildings);
        }
        else
            print("맵 정보를 불러올 수 없습니다.");
    }

    public BattleFieldUnit GetUnitPrefab(Data.UnitId id)
    {
        for (int i = 0; i < _unitPrefabs.Length; i++)
        {
            if (_unitPrefabs[i].Id == id)
                return _unitPrefabs[i];
        }

        return null;
    }
}
