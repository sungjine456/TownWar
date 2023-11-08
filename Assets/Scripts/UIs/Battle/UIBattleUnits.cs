using System.Collections.Generic;
using UnityEngine;

public class UIBattleUnits : SingletonMonoBehaviour<UIBattleUnits>
{
    [SerializeField] GameObject _elements;
    [SerializeField] RectTransform _parent;
    [SerializeField] UIBattleUnit _unitPrefab;
    [SerializeField] UnitInfo _unitInfo;

    [HideInInspector] public UIBattleUnit _target;
    
    List<UIBattleUnit> _units;

    protected override void OnAwake()
    {
        base.OnAwake();

        _units = new();
    }

    protected override void OnStart()
    {
        base.OnStart();

        Dictionary<(Data.UnitId, int), Queue<Data.Unit>> group = new();

        foreach (var u in GameManager.Instance.MyPlayer.GetUnits(Data.UnitStatus.army))
        {
            if (group.ContainsKey((u.id, u.level)))
                group[(u.id, u.level)].Enqueue(u);
            else
            {
                Queue<Data.Unit> list = new();
                list.Enqueue(u);
                group.Add((u.id, u.level), list);
            }
        }

        foreach (var g in group)
        {
            UIBattleUnit u = Instantiate(_unitPrefab, _parent);
            u.Initialized(g.Value, g.Key.Item2);
            _units.Add(u);
        }
    }

    public Data.UnitToTrain GetUnitInfo(Data.UnitId id, int level)
    {
        return _unitInfo.GetUnitData(id, level);
    }

    public void SelectUnit(Data.UnitId id)
    {
        if (_target is null || _target?._id != id)
        {
            for (int i = 0; i < _units.Count; i++)
            {
                if (_units[i]._id == id && _units[i].Count > 0)
                {
                    _target = _units[i];
                    _target.Active(true);
                }
                else
                    _units[i].Active(false);
            }
        }
    }

    public void End()
    {
        _elements.SetActive(false);
    }

    public bool IsEmpty()
    {
        for (int i = 0; i < _units.Count; i++)
        {
            if (_units[i].Count > 0)
                return false;
        }

        return true;
    }

    public int CountOfUnits() => _units.Count;

    public UIBattleUnit GetUnit(int index) => _units[index];
}
