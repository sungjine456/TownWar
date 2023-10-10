using System.Collections.Generic;
using UnityEngine;

public class TrainingController : SingletonMonoBehaviour<TrainingController>
{
    const int TRAIN_UNIT_WIDTH = 300;

    [SerializeField] UITrainingUnit _trainingPrefab;
    [SerializeField] UITrainedUnit _trainedPrefab;
    [SerializeField] RectTransform _trainingGrid;
    [SerializeField] RectTransform _trainedGrid;
    [SerializeField] RectTransform _armyGrid;
    public UnitInfo _unitInfo;

    LinkedList<UITrainedUnit> _trainedUnits;
    LinkedList<UITrainingUnit> _trainingUnits;
    List<UITrainedUnit> _armyUnits; // 동일한 유닛일 경우 하나로 묶어 보여주기 때문에 재배치할 필요가 없으므로 관리하기 용이하게 List를 사용한다.

    protected override void OnAwake()
    {
        _trainedUnits = new();
        _trainingUnits = new();
        _armyUnits = new();
    }

    protected override void OnStart()
    {
        SettingUnits(Data.UnitStatus.army);
        SettingUnits(Data.UnitStatus.trained);
        SettingUnits(Data.UnitStatus.training);
    }

    void Update()
    {
        if (_trainingUnits.Count > 0)
        {
            if (_trainingUnits.First.Value._unit.trainTime <= 0 || _trainingUnits.First.Value._unit.trainedTime >= _trainingUnits.First.Value._unit.trainTime)
            {
                var target = _trainingUnits.First.Value;

                if (GameManager.Instance.MyPlayer.CanAddUnit(target._unit.numberOfPopulation))
                    AddUnit(target._unit);
                else
                {
                    var t = Instantiate(_trainedPrefab, _trainedGrid);
                    t.Initialize(target._unit);
                    _trainedUnits.AddLast(t);

                    GameManager.Instance.MyPlayer.AddUnit(target._unit, Data.UnitStatus.trained);

                    Rearrange(_trainedUnits, _trainedGrid);
                }

                ReduceFirstUnit(_trainingUnits);
            }
            else
            {
                _trainingUnits.First.Value._unit.trainedTime += Time.deltaTime;
                _trainingUnits.First.Value._bar.fillAmount = _trainingUnits.First.Value._unit.trainedTime / _trainingUnits.First.Value._unit.trainTime;
            }
        }
    }

    void SettingUnits(Data.UnitStatus status)
    {
        Dictionary<Data.UnitId, (Data.Unit, int)> dic = new();

        foreach (var u in GameManager.Instance.MyPlayer.GetUnits(status))
        {
            if (dic.ContainsKey(u.id))
                dic[u.id] = (u, dic[u.id].Item2 + 1);
            else
                dic.Add(u.id, (u, 1));
        }

        foreach (var t in dic)
        {
            if (status == Data.UnitStatus.army)
            {
                var unit = Instantiate(_trainedPrefab, _armyGrid);
                unit.Initialize(t.Value.Item1, t.Value.Item2);
                unit.SetReady();
                _armyUnits.Add(unit);
            }
            else if (status == Data.UnitStatus.trained)
            {
                var unit = Instantiate(_trainedPrefab, _trainedGrid);
                unit.Initialize(t.Value.Item1, t.Value.Item2);
                _trainedUnits.AddLast(unit);
            }
            else
            {
                var unit = Instantiate(_trainingPrefab, _trainingGrid);
                unit.Initialize(t.Value.Item1, t.Value.Item2);
                _trainingUnits.AddLast(unit);
            }
        }

        if (status == Data.UnitStatus.trained)
            SwitchUnitState();
        else if (status == Data.UnitStatus.training)
            Rearrange(_trainingUnits, _trainingGrid);
    }

    int AllPopulation()
    {
        int population = 0;

        for (int i = 0; i < _armyUnits.Count; i++)
        {
            population += _armyUnits[i]._unit.numberOfPopulation * _armyUnits[i].Count;
        }
        foreach (var u in _trainingUnits)
        {
            population += u._unit.numberOfPopulation * u.Count;
        }
        foreach (var u in _trainedUnits)
        {
            population += u._unit.numberOfPopulation * u.Count;
        }

        return population;
    }

    void Rearrange<T>(LinkedList<T> list, RectTransform grid) where T : TrainUnit
    {
        if (list.Count > 0)
        {
            var node = list.First;

            while (node.Next != null)
            {
                if (node.Value._unit.id == node.Next.Value._unit.id)
                {
                    node.Value.Count += node.Next.Value.Count;
                    Destroy(node.Next.Value.gameObject);
                    list.Remove(node.Next);
                }
                else
                    node = node.Next;
            }
        }

        ResizeGrid(grid, list.Count);
    }

    void ResizeGrid(RectTransform grid, int count)
    {
        grid.sizeDelta = new Vector2(TRAIN_UNIT_WIDTH * count, 0);
    }

    void AddUnit(Data.Unit unit)
    {
        bool notFind = true;

        for (int i = 0; i < _armyUnits.Count; i++)
        {
            if (_armyUnits[i]._unit.id == unit.id)
            {
                notFind = false;
                _armyUnits[i].Count++;
                break;
            }
        }

        if (notFind)
        {
            var t = Instantiate(_trainedPrefab, _armyGrid);
            t.Initialize(unit);
            t.SetReady();
            _armyUnits.Add(t);
        }

        GameManager.Instance.MyPlayer.AddUnit(unit, Data.UnitStatus.army);
    }

    void ReduceFirstUnit<T>(LinkedList<T> list) where T : TrainUnit
    {
        var target = list.First.Value;
        bool isTraining = target is UITrainingUnit;
        var status = isTraining ? Data.UnitStatus.training : Data.UnitStatus.trained;

        if (target.Count <= 1)
        {
            list.RemoveFirst();

            Rearrange(_trainedUnits, _trainedGrid);
            Destroy(target.gameObject);
        }
        else
        {
            target.Count -= 1;
            target._unit.trainedTime = 0f;
        }

        GameManager.Instance.MyPlayer.RemoveUnit(target._unit.id, status);
    }

    public void TrainUnit(Data.UnitToTrain data)
    {
        if (AllPopulation() + data.numberOfPopulation <= GameManager.Instance.MyPlayer.MaxTrainingUnit())
        {
            var last = _trainingUnits.Last;
            Data.Unit unit = new(data);

            if (last != null && last.Value._unit.id == data.id)
                last.Value.Count += 1;
            else
            {
                var train = Instantiate(_trainingPrefab, _trainingGrid);
                train.Initialize(unit);
                _trainingUnits.AddLast(train);

                ResizeGrid(_trainingGrid, _trainingUnits.Count);
            }

            GameManager.Instance.MyPlayer.AddUnit(unit, Data.UnitStatus.training);
        }
        else
            print("집합소의 공간이 부족합니다.");
    }

    public void RemoveTrainingUnit(UITrainingUnit unit)
    {
        _trainingUnits.Remove(unit);

        Rearrange(_trainingUnits, _trainingGrid);
    }

    public void RemoveTrainedUnit(UITrainedUnit unit, bool isReady)
    {
        if (isReady)
        {
            for (int i = 0; i < _armyUnits.Count; i++)
            {
                if (_armyUnits[i]._unit.id == unit._unit.id)
                {
                    _armyUnits.RemoveAt(i);
                    break;
                }
            }
        }
        else
        {
            _trainedUnits.Remove(unit);

            Rearrange(_trainedUnits, _trainedGrid);
        }
    }

    public void SwitchUnitState()
    {
        while (_trainedUnits.Count > 0)
        {
            var target = _trainedUnits.First.Value;

            if (GameManager.Instance.MyPlayer.CanAddUnit(target._unit.numberOfPopulation))
            {
                AddUnit(target._unit);
                ReduceFirstUnit(_trainedUnits);
            }
            else
                break;
        }
    }
}
