using System;

using UnityEngine;

using RotaryHeart.Lib.SerializableDictionary;

[CreateAssetMenu(fileName = "BattleMapData", menuName = "ScriptableObjects/BattleMap", order = 1)]
public class BattleMap : ScriptableObject
{
    /*
     * 키 값인 int형 정보는 맵의 번호를 나타내기 위한 정보이다.
     * 키 값은 1 부터 순차적으로 오르는 정수값이다.
     */
    [SerializeField] SerializableDictionaryBase<int, MapArray> battleMapList;

    public BattleMapData[] GetMapData(int number)
    {
        return battleMapList.ContainsKey(number) ? battleMapList[number].buildings : new BattleMapData[0];
    }

    [Serializable]
    public class MapArray
    {
        public BattleMapData[] buildings;
    }
}
