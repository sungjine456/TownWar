using System;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitInfoData", menuName = "ScriptableObjects/UnitInfo", order = 1)]
public class UnitInfo : ScriptableObject
{
    [SerializeField] SerializableDictionaryBase<Data.UnitId, UnitArray> unitDataList;

    public Data.UnitToTrain GetUnitData(Data.UnitId id, int level)
    {
        for (int i = 0; i < unitDataList[id].units.Length; i++)
        {
            if (unitDataList[id].units[i].level == level)
                return unitDataList[id].units[i];
        }

        return null;
    }

    [Serializable]
    public class UnitArray
    {
        public Data.UnitToTrain[] units;
    }
}
