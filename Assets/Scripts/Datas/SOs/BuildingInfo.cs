using System;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingInfoData", menuName = "ScriptableObjects/BuildingInfo", order = 1)]
public class BuildingInfo : ScriptableObject
{
    [SerializeField] SerializableDictionaryBase<Data.BuildingId, BuildingArray> buildingDataList;

    public Data.BuildingToBuild GetBuildingData(Data.BuildingId buildingId, int level)
    {
        for (int i = 0; i < buildingDataList[buildingId].building.Length; i++)
        {
            if (buildingDataList[buildingId].building[i].level == level)
                return buildingDataList[buildingId].building[i];
        }

        return null;
    }

    [Serializable]
    public class BuildingArray
    {
        public Data.BuildingToBuild[] building;
    }
}
