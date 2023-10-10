using UnityEngine;

[CreateAssetMenu(fileName = "BuildingLimitData", menuName = "ScriptableObjects/BuildingLimit", order = 1)]
public class BuildingLimit : ScriptableObject
{
    [SerializeField] Data.BuildingAvailability[] buildingAvailabilityList;

    Data.BuildingLimit GetBuildingLimit(int hallLevel, Data.BuildingId id)
    {
        for (int i = 0; i < buildingAvailabilityList.Length; i++)
        {
            if (buildingAvailabilityList[i].hallLevel == hallLevel)
            {
                for (int j = 0; j < buildingAvailabilityList[i].limits.Length; j++)
                {
                    if (buildingAvailabilityList[i].limits[j].id == id)
                        return buildingAvailabilityList[i].limits[j];
                }
            }
        }
        return null;
    }

    public int GetBuildingLimitCount(int hallLevel, Data.BuildingId id)
    {
        return GetBuildingLimit(hallLevel, id).count;
    }

    public int GetBuildingLimitLevel(int hallLevel, Data.BuildingId id)
    {
        return GetBuildingLimit(hallLevel, id).maxLevel;
    }
}
