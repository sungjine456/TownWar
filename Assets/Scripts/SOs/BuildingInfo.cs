using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/BuildingInfo", order = 1)]
public class BuildingInfo : ScriptableObject
{
    public List<Data.BuildingToBuild> buildingDataList;

    public Data.BuildingToBuild GetBuildingData(Data.BuildingId buildingId)
    {
        for (int i = 0; i < buildingDataList.Count; i++)
        {
            var data = buildingDataList[i];

            if (data.buildingId == buildingId)
                return data;
        }

        return null;
    }
}
