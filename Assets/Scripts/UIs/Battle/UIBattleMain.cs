using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using TMPro;

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
    int _plunderedGold;
    int _plunderedElixir;

    Battle _battle;

    readonly List<FieldUnit> unitsOnGrid = new();
    readonly List<BuildingOnGrid> buildingsOnGrid = new();
    readonly List<BattleBuilding> battleBuildings = new();

    protected override void OnAwake()
    {
        _closeBtn.onClick.AddListener(End);
    }

    protected override void OnStart()
    {
        _goldText.text = GameManager.Instance.MyPlayer.Gold.ToString();
        _elixirText.text = GameManager.Instance.MyPlayer.Elixir.ToString();
        
        SoundManager.Instance.PlayBGM(SoundManager.BgmClip.waitBattle);
    }

    void Update()
    {
        if (_battle.IsStart)
        {
            TimeSpan span = _battle.StartTime.AddSeconds(180) - DateTime.Now;

            if (span.TotalSeconds > 0.01)
                _timeText.text = span.ToString(@"m\분\ ss\초");
            else
                End();
        }

        if (buildingsOnGrid.Count == 0 || (unitsOnGrid.Count == 0 && UIBattleUnits.Instance.IsEmpty()))
            End();

        _battle.ExecuteFrame();
        UpdateUnits();
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
        var p = GameManager.Instance.MyPlayer;
        var g = GameManager.Instance.MaxGold;

        if (g >= p.Gold + gold)
        {
            GameManager.Instance.MyPlayer.CollectGold(gold);
            _plunderedGold += gold;
        }
        else if (g > p.Gold && g < p.Gold + gold)
        {
            GameManager.Instance.MyPlayer.CollectGold(g - p.Gold);
            _plunderedGold += g - p.Gold;
        }

        _goldText.text = GameManager.Instance.MyPlayer.Gold.ToString();
        _plunderGoldText.text = (int.Parse(_plunderGoldText.text) - gold).ToString();
    }

    void PlunderElixir(int elixir)
    {
        var p = GameManager.Instance.MyPlayer;
        var e = GameManager.Instance.MaxElixir;

        if (e >= p.Elixir + elixir)
        {
            GameManager.Instance.MyPlayer.CollectElixir(elixir);
            _plunderedElixir += elixir;
        }
        else if (e > p.Elixir && e < p.Elixir + elixir)
        {
            GameManager.Instance.MyPlayer.CollectGold(e - p.Elixir);
            _plunderedElixir += e - p.Elixir;
        }

        _elixirText.text = GameManager.Instance.MyPlayer.Elixir.ToString();
        _plunderElixirText.text = (int.Parse(_plunderElixirText.text) - elixir).ToString();
    }

    void UpdateUnits()
    {
        for (int i = 0; i < unitsOnGrid.Count; i++)
        {
            if (_battle.units[unitsOnGrid[i].Index].Health > 0)
            {
                Vector3 position = new(_battle.units[unitsOnGrid[i].Index].position._x, 0, _battle.units[unitsOnGrid[i].Index].position._y);

                unitsOnGrid[i].transform.localPosition = position;

                if (_battle.units[unitsOnGrid[i].Index].Health < _battle.units[unitsOnGrid[i].Index].Data.health)
                {
                    unitsOnGrid[i]._healthBar.gameObject.SetActive(true);
                    unitsOnGrid[i]._healthBar.FillAmount(_battle.units[unitsOnGrid[i].Index].Health / _battle.units[unitsOnGrid[i].Index].Data.health);
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
            battleBuildings[i].attackCallback = BuildingAttackCallBack;
            battleBuildings[i].destroyCallback = BuildingDestroyedCallBack;
            battleBuildings[i].damageCallback = BuildingDamageCallBack;

            Building prefab = GameManager.Instance.GetBuildingPrefab(battleBuildings[i].Building.buildingId);

            if (prefab)
            {
                var building = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                building.Initialize(battleBuildings[i].Building);

                var healthBar = Instantiate(_healthBarPrefab, _healthBarGrid);
                healthBar.gameObject.SetActive(false);

                BuildingOnGrid buildingGrid = new()
                {
                    building = building,
                    healthBar = healthBar,
                    id = battleBuildings[i].Building.id,
                    index = i
                };

                buildingsOnGrid.Add(buildingGrid);
            }
        }

        _battle = new(battleBuildings);
    }

    public void PlaceUnit(int x, int y)
    {
        for (int i = 0; i < UIBattleUnits.Instance.CountOfUnits(); i++)
        {
            if (UIBattleUnits.Instance.GetUnit(i)._id == UIBattleUnits.Instance._target._id)
            {
                _selectedUnitIndex = i;
                break;
            }
        }

        if (_selectedUnitIndex >= 0 && UIBattleUnits.Instance.GetUnit(_selectedUnitIndex).Count > 0 && _battle.CanAddUnit(x, y))
        {
            var u = UIBattleUnits.Instance._target.Pop();

            if (u is not null)
            {
                if (UIBattleUnits.Instance.GetUnit(_selectedUnitIndex).Count <= 0)
                    _selectedUnitIndex = -1;

                GameManager.Instance.MyPlayer.RemoveUnit(u.id, Data.UnitStatus.army);

                _battle.AddUnit(u, x, y, UnitSpawnCallBack, UnitAttackCallBack, UnitDiedCallBack, UnitDamageCallBack);
            }
        }
    }

    public void UnitSpawnCallBack(int index)
    {
        for (int i = 0; i < _battle.units.Count; i++)
        {
            if (_battle.units[i].Index == index)
            {
                FieldUnit prefab = BattleManager.Instance.GetUnitPrefab(UIBattleUnits.Instance._target._id);

                if (prefab)
                {
                    FieldUnit unit = Instantiate(prefab, BattleManager.Instance.Grid.transform);
                    unit.transform.localPosition = new(_battle.units[i].position._x, 0, _battle.units[i].position._y);
                    unit.Initialize(i, _battle.units[i].Data);
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
                unitsOnGrid[i].SetState(FieldUnitState.Death);
                Destroy(unitsOnGrid[i]._healthBar.gameObject);
                unitsOnGrid.RemoveAt(i);
                break;
            }
        }
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

    public void UnitDamageCallBack(int id, float damage) { }

    public void BuildingAttackCallBack(int id, BattleUnit target)
    {
        for (int i = 0; i < buildingsOnGrid.Count; i++)
        {
            if (buildingsOnGrid[i].id == id && buildingsOnGrid[i].building is Tower t)
                if (GetPosOfUnit(target.Index).HasValue)
                    t.SetTarget(GetPosOfUnit(target.Index).Value);
        }
    }

    public void BuildingDamageCallBack(BattleBuilding building)
    {
        for (int i = 0; i < buildingsOnGrid.Count; i++)
        {
            if (buildingsOnGrid[i].id == building.Building.id && building.Health < building.Building.health)
            {
                buildingsOnGrid[i].healthBar.gameObject.SetActive(true);
                buildingsOnGrid[i].healthBar.FillAmount(_battle.buildings[buildingsOnGrid[i].index].Health / _battle.buildings[buildingsOnGrid[i].index].Building.health);
                buildingsOnGrid[i].healthBar._rect.anchoredPosition = GetUnitBarPosition(BattleManager.Instance.Grid.GetEndPosition(buildingsOnGrid[i].building));
            }
        }
    }

    public void BuildingDestroyedCallBack(int id)
    {
        var beforePercent = GetPercent();

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

        if (GetBuildingCountForPercent() < _battle.AllCount)
        {
            var percent = GetPercent();

            _damage.text = percent.ToString();

            if ((beforePercent < 50 && percent >= 50) || (beforePercent < 100 && percent >= 100))
                AddStar();
        }
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

    public Vector3? GetPosOfUnit(int id)
    {
        for (int i = 0; i < unitsOnGrid.Count; i++)
        {
            if (unitsOnGrid[i].Index == id)
                return unitsOnGrid[i].transform.position;
        }

        return null;
    }

    public int GetPercent()
    {
        return Mathf.RoundToInt((_battle.AllCount - Instance.GetBuildingCountForPercent()) * 100f / _battle.AllCount);
    }

    public void End()
    {
        if (_battle.IsStart)
        {
            _elements.SetActive(false);

            UIBattleUnits.Instance.End();
            UIBattleResult.Instance.SetResult(_plunderedGold, _plunderedElixir, GetPercent(), Instance.GetStarsCount());
        }
        else
            SceneManager.LoadScene("Game");
    }
}
