using System.Collections.Generic;

using UnityEngine;

using static Data;

public class ArmyCamp : GameBuilding
{
    [SerializeField] GameObject[] _waypoints;
    [SerializeField] FieldUnit[] _unitPrefabs;

    int _index;
    List<FieldUnit> _armyUnits;

    void Awake()
    {
        BuildingId = BuildingId.armyCamp;
        _armyUnits = new();
    }

    protected override void Update()
    {
        base.Update();
        
        for (int i = 0; i < _armyUnits.Count; i++)
        {
            if (_armyUnits[i].CurrentState == AniMotion.Idle)
            {
                _armyUnits[i]._lastPosition = _waypoints[Random.Range(0, _waypoints.Length)].transform.position + new Vector3(0, 0.1f, 0);
            }
        }
    }

    public void AddUnit(Unit unitData)
    {
        FieldUnit prefab = null;

        for (int i = 0; i < _unitPrefabs.Length; i++)
        {
            if (_unitPrefabs[i].Id == unitData.id)
                prefab = _unitPrefabs[i];
        }

        if (prefab)
        {
            var pos = _waypoints[Random.Range(0, _waypoints.Length)].transform.position + new Vector3(0, 0.1f, 0);
            FieldUnit unit = Instantiate(prefab, pos, Quaternion.identity);
            unit.Initialize(_index++, unitData);
            _armyUnits.Add(unit);
        }
    }

    public void RemoveUnit(UnitId id)
    {
        for (int i = 0; i < _armyUnits.Count; i++)
        {
            if (_armyUnits[i].Id == id)
            {
                if (!GameManager.Instance.IsBattling)
                    Destroy(_armyUnits[i].gameObject);
                
                _armyUnits.RemoveAt(i);
                return;
            }
        }
    }
}
