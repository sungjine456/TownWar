using System.Collections.Generic;
using UnityEngine;

public class BattleManager : SingletonMonoBehaviour<BattleManager>
{
    public static int LoadedNumberOfMap = -1;

    [SerializeField] UIBattleMain main;
    [SerializeField] BattleMap maps;
    [SerializeField] BuildingInfo buildingInfo;
    [SerializeField] BuildGrid grid;
    [SerializeField] BattleFieldUnit[] unitPrefabs;

    BattleMapData[] datas;

    public BuildGrid Grid => grid;

    protected override void OnStart()
    {
        if (LoadedNumberOfMap >= 1)
        {
            List<Data.Building> buildings = new();
            datas = maps.GetMapData(LoadedNumberOfMap);

            for (int i = 0; i < datas.Length; i++)
            {
                var d = buildingInfo.GetBuildingData(datas[i].id, datas[i].level);
                Data.Building data = new(d)
                {
                    id = i,
                    x = datas[i].x,
                    y = datas[i].y
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
        for (int i = 0; i < unitPrefabs.Length; i++)
        {
            if (unitPrefabs[i].id == id)
                return unitPrefabs[i];
        }

        return null;
    }
}
