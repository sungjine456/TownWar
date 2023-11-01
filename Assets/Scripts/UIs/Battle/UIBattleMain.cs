using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

public class UIBattleMain : SingletonMonoBehaviour<UIBattleMain>
{
    class BuildingOnGrid
    {
        public int id;
        public int index = -1;
        public Building building;
        public UIBar healthBar;
    }

    [SerializeField] GameObject _elements;
    [SerializeField] TextMeshProUGUI _goldText;
    [SerializeField] TextMeshProUGUI _elixirText;
    [SerializeField] TextMeshProUGUI _plunderGoldText;
    [SerializeField] TextMeshProUGUI _plunderElixirText;
    [SerializeField] TextMeshProUGUI _timeText;
    [SerializeField] Button _closeBtn;
    [SerializeField] TextMeshProUGUI _damage;
    [SerializeField] Image[] _stars;
    [SerializeField] UIBar _healthBarPrefab;
    [SerializeField] RectTransform _healthBarGrid;
    [SerializeField] FieldUnit[] _battleUnits;

    int _selectedUnitIndex = -1;

    Battle _battle;

    readonly List<FieldUnit> unitsOnGrid = new();
    readonly List<BuildingOnGrid> buildingsOnGrid = new();
    readonly List<BattleBuilding> battleBuildings = new();

    protected override void OnAwake()
    {
        _closeBtn.onClick.AddListener(Close);
    }

    protected override void OnStart()
    {
        _goldText.text = GameManager.Instance.MyPlayer.Gold.ToString();
        _elixirText.text = GameManager.Instance.MyPlayer.Elixir.ToString();
        
        SoundManager.Instance.PlayBGM(SoundManager.BgmClip.waitBattle);
    }

    void Update()
    {
        if (_battle._isStart)
        {
            TimeSpan span = _battle._startTime.AddSeconds(180) - DateTime.Now;

            if (span.TotalSeconds > 0.01)
                _timeText.text = span.ToString(@"m\분\ ss\초");
            else
                Close();
        }

        if (buildingsOnGrid.Count == 0 || (unitsOnGrid.Count == 0 && UIBattleUnits.Instance.IsEmpty()))
            Close();

        _battle.ExecuteFrame();
        UpdateUnits();
    }

    void Close()
    {
        _battle.End(int.Parse(_plunderGoldText.text), int.Parse(_plunderElixirText.text));
    }

    void AddStar()
    {
        for (int i = 0; i < _stars.Length; i++)
        {
            if (_stars[i].color != Color.yellow)
            {
                _stars[i].color = Color.yellow;
                break;
            }
        }
    }

    void PlunderGold(int gold)
    {
        GameManager.Instance.MyPlayer.CollectGold(gold);
        _goldText.text = GameManager.Instance.MyPlayer.Gold.ToString();
        _plunderGoldText.text = (int.Parse(_plunderGoldText.text) - gold).ToString();
    }

    void PlunderElixir(int elixir)
    {
        GameManager.Instance.MyPlayer.CollectElixir(elixir);
        _elixirText.text = GameManager.Instance.MyPlayer.Elixir.ToString();
        _plunderElixirText.text = (int.Parse(_plunderElixirText.text) - elixir).ToString();
    }

    void UpdateUnits()
    {
        for (int i = 0; i < unitsOnGrid.Count; i++)
        {
            if (_battle._units[unitsOnGrid[i].Index]._health > 0)
            {
                Vector3 position = new(_battle._units[unitsOnGrid[i].Index]._position._x, 0, _battle._units[unitsOnGrid[i].Index]._position._y);

                unitsOnGrid[i].transform.localPosition = position;

                if (_battle._units[unitsOnGrid[i].Index]._health < _battle._units[unitsOnGrid[i].Index]._data.health)
                {
                    unitsOnGrid[i]._healthBar.gameObject.SetActive(true);
                    unitsOnGrid[i]._healthBar.FillAmount(_battle._units[unitsOnGrid[i].Index]._health / _battle._units[unitsOnGrid[i].Index]._data.health);
                    unitsOnGrid[i]._healthBar._rect.anchoredPosition = GetUnitBarPosition(unitsOnGrid[i].transform.position);
                }
            }
        }
    }

    Vector2 GetUnitBarPosition(Vector3 position)
    {
        Vector3 planDownLeft = BattleCameraCtrl.Instance._planDownLeft;
        Vector3 planTopRight = BattleCameraCtrl.Instance._planTopRight;

        float w = planTopRight.x - planDownLeft.x;
        float h = planTopRight.z - planDownLeft.z;

        float endW = position.x - planDownLeft.x;
        float endH = position.z - planDownLeft.z;

        return new(endW / w * Screen.width, endH / h * Screen.height);
    }

    public void Initialize(List<Data.Building> buildings)
    {
        int gold = 0;
        int elixir = 0;

        for (int i = 0; i < buildings.Count; i++)
        {
            battleBuildings.Add(new(buildings[i]));

            switch (buildings[i].buildingId)
            {
                case Data.BuildingId.townHall:
                    gold += buildings[i].capacity;
                    elixir += buildings[i].capacity;
                    break;
                case Data.BuildingId.goldMine:
                case Data.BuildingId.goldStorage:
                    gold += buildings[i].capacity;
                    break;
                case Data.BuildingId.elixirMine:
                case Data.BuildingId.elixirStorage:
                    elixir += buildings[i].capacity;
                    break;
            }
        }

        _plunderGoldText.text = gold.ToString();
        _plunderElixirText.text = elixir.ToString();

        for (int i = 0; i < battleBuildings.Count; i++)
        {
            battleBuildings[i]._attackCallback = BuildingAttackCallBack;
            battleBuildings[i]._destroyCallback = BuildingDestroyedCallBack;
            battleBuildings[i]._damageCallback = BuildingDamageCallBack;

            Building prefab = GameManager.Instance.GetBuildingPrefab(battleBuildings[i]._building.buildingId);

            if (prefab)
            {
                var building = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                building.Initialize(battleBuildings[i]._building);

                var healthBar = Instantiate(_healthBarPrefab, _healthBarGrid);
                healthBar.gameObject.SetActive(false);

                BuildingOnGrid buildingGrid = new()
                {
                    building = building,
                    healthBar = healthBar,
                    id = battleBuildings[i]._building.id,
                    index = i
                };

                buildingsOnGrid.Add(buildingGrid);
            }
        }

        _battle = new(battleBuildings, gold, elixir);
    }

    public void PlaceUnit(int x, int y)
    {
        for (int i = 0; i < UIBattleUnits.Instance._units.Count; i++)
        {
            if (UIBattleUnits.Instance._units[i]._id == UIBattleUnits.Instance._target._id)
            {
                _selectedUnitIndex = i;
                break;
            }
        }

        if (_selectedUnitIndex >= 0 && UIBattleUnits.Instance._units[_selectedUnitIndex].Count > 0 && _battle.CanAddUnit(x, y))
        {
            var u = UIBattleUnits.Instance._target.Pop();

            if (u != null)
            {
                if (UIBattleUnits.Instance._units[_selectedUnitIndex].Count <= 0)
                    _selectedUnitIndex = -1;

                GameManager.Instance.MyPlayer.RemoveUnit(u.id, Data.UnitStatus.army);

                _battle.AddUnit(u, x, y, UnitSpawnCallBack, UnitAttackCallBack, UnitDiedCallBack, UnitDamageCallBack);
            }
        }
    }

    public void UnitSpawnCallBack(int index)
    {
        for (int i = 0; i < _battle._units.Count; i++)
        {
            if (_battle._units[i]._index == index)
            {
                FieldUnit prefab = BattleManager.Instance.GetUnitPrefab(UIBattleUnits.Instance._target._id);

                if (prefab)
                {
                    FieldUnit unit = Instantiate(prefab, BattleManager.Instance.Grid.transform);
                    unit.transform.localPosition = new(_battle._units[i]._position._x, 0, _battle._units[i]._position._y);
                    unit.Initialize(i, _battle._units[i]._data);
                    unit._healthBar = Instantiate(_healthBarPrefab, _healthBarGrid);
                    unit._healthBar.gameObject.SetActive(false);

                    unitsOnGrid.Add(unit);
                }

                break;
            }
        }
    }

    public void UnitDiedCallBack(int index)
    {
        for (int i = 0; i < unitsOnGrid.Count; i++)
        {
            if (unitsOnGrid[i].Index == index)
            {
                Destroy(unitsOnGrid[i]._healthBar.gameObject);
                Destroy(unitsOnGrid[i].gameObject);
                unitsOnGrid.RemoveAt(i);
                break;
            }
        }
        print("Unit killed.");
    }

    public void UnitAttackCallBack(int index, BattleBuilding target)
    {
        for (int i = 0; i < unitsOnGrid.Count; i++)
        {
            if (unitsOnGrid[i].Index == index)
            {
                unitsOnGrid[i].SetState(FieldUnitState.Attack);
                unitsOnGrid[i].SetTarget(target);
            }
        }
    }

    public void UnitDamageCallBack(int id, float damage)
    {
        print("Unit took damage: " + damage);
    }

    public void BuildingAttackCallBack(int id, BattleVector2 target) { }

    public void BuildingDamageCallBack(BattleBuilding building)
    {
        for (int i = 0; i < buildingsOnGrid.Count; i++)
        {
            if (buildingsOnGrid[i].id == building._building.id && building._health < building._building.health)
            {
                buildingsOnGrid[i].healthBar.gameObject.SetActive(true);
                buildingsOnGrid[i].healthBar.FillAmount(_battle._buildings[buildingsOnGrid[i].index]._health / _battle._buildings[buildingsOnGrid[i].index]._building.health);
                buildingsOnGrid[i].healthBar._rect.anchoredPosition = GetUnitBarPosition(BattleManager.Instance.Grid.GetEndPosition(buildingsOnGrid[i].building));
            }
        }
    }

    public void BuildingDestroyedCallBack(int id)
    {
        var beforePercent = _battle.GetPercent();

        for (int i = 0; i < buildingsOnGrid.Count; i++)
        {
            if (buildingsOnGrid[i].id == id)
            {
                switch (buildingsOnGrid[i].building.BuildingId)
                {
                    case Data.BuildingId.townHall:
                        PlunderGold(buildingsOnGrid[i].building.Capacity);
                        PlunderElixir(buildingsOnGrid[i].building.Capacity);
                        AddStar();
                        break;
                    case Data.BuildingId.goldMine:
                    case Data.BuildingId.goldStorage:
                        PlunderGold(buildingsOnGrid[i].building.Capacity);
                        break;
                    case Data.BuildingId.elixirMine:
                    case Data.BuildingId.elixirStorage:
                        PlunderElixir(buildingsOnGrid[i].building.Capacity);
                        break;
                }

                Destroy(buildingsOnGrid[i].healthBar.gameObject);
                Destroy(buildingsOnGrid[i].building.gameObject);
                buildingsOnGrid.RemoveAt(i);
                break;
            }
        }

        if (GetBuildingCountForPercent() < _battle._allCount)
        {
            var percent = _battle.GetPercent();

            _damage.text = percent.ToString();

            if (beforePercent < 50 && percent >= 50)
                AddStar();
            else if (beforePercent < 100 && percent >= 100)
                AddStar();
        }
    }

    public void End()
    {
        _elements.SetActive(false);
    }

    public int GetBuildingCountForPercent()
    {
        int count = 0;

        for (int i = 0; i < buildingsOnGrid.Count; i++)
        {
            if (buildingsOnGrid[i].building.BuildingId != Data.BuildingId.wall)
                count++;
        }

        return count;
    }

    public int GetStarsCount()
    {
        int count = 0;

        for (int i = 0; i < _stars.Length; i++)
        {
            if (_stars[i].color == Color.yellow)
                count++;
        }

        return count;
    }

    public Vector3? GetPosOfBuilding(int id)
    {
        for (int i = 0; i < buildingsOnGrid.Count; i++)
        {
            if (buildingsOnGrid[i].id == id)
                return buildingsOnGrid[i].building.transform.position;
        }

        return null;
    }
}
