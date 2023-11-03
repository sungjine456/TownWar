using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public delegate void SpawnCallBack(int index);
public delegate void IndexCallback(int index);
public delegate void FloatCallback(int index, float value);
public delegate void AttackUnitCallback(int index, BattleUnit target);
public delegate void AttackBuildingCallback(int index, BattleBuilding target);

public class Battle
{
    public List<BattleBuilding> _buildings;
    public List<BattleUnit> _units = new();
    int _unitIndex;
    BattleGrid _grid;
    BattleGrid _unlimitedGrid;
    Search _search;
    Search _unlimitedSearch;
    List<Tile> _blockedTiles = new();
    readonly List<Projectile> _projectiles = new();
    public int _allCount;
    public bool _isStart;
    public DateTime _startTime;
    readonly int _maxPlunderableGold;
    readonly int _maxPlunderableElixir;

    public Battle(List<BattleBuilding> buildings, int maxPlunderableGold, int maxPlunderableElixir)
    {
        _maxPlunderableGold = maxPlunderableGold;
        _maxPlunderableElixir = maxPlunderableElixir;
        _buildings = buildings;
        _grid = new(Data.GRID_SIZE, Data.GRID_SIZE);
        _unlimitedGrid = new(Data.GRID_SIZE, Data.GRID_SIZE);
        _search = new(_grid);
        _unlimitedSearch = new(_unlimitedGrid);

        for (int i = 0; i < buildings.Count; i++)
        {
            if (buildings[i]._building.buildingId != Data.BuildingId.wall)
                _allCount++;
        }

        for (int i = 0; i < buildings.Count; i++)
        {
            int startX = buildings[i]._building.x;
            int endX = buildings[i]._building.x + buildings[i]._building.columns;
            int startY = buildings[i]._building.y;
            int endY = buildings[i]._building.y + buildings[i]._building.rows;

            if (buildings[i]._building.buildingId != Data.BuildingId.wall && buildings[i]._building.columns > 1 && buildings[i]._building.rows > 1)
            {
                startX++;
                startY++;
                endX--;
                endY--;

                if (endX <= startX || endY <= startY)
                    continue;
            }

            for (int x = startX; x < endX; x++)
            {
                for (int y = startY; y < endY; y++)
                {
                    _grid[x, y].Blocked = true;
                    _blockedTiles.Add(new(buildings[i]._building.buildingId, new(x, y), i));
                }
            }
        }
    }

    void HandleBuilding(int index)
    {
        if (_buildings[index]._target >= 0)
        {
            if (_units[_buildings[index]._target]._health <= 0 || !IsUnitInRange(_buildings[index]._target, index))
                _buildings[index]._target = -1;
            else
            {
                _buildings[index]._attackTimer += Time.deltaTime;
                int attacksCount = (int)Math.Floor(_buildings[index]._attackTimer / _buildings[index]._building.speed);

                if (attacksCount > 0)
                {
                    _buildings[index]._attackTimer -= (attacksCount * _buildings[index]._building.speed);

                    for (int i = 1; i <= attacksCount; i++)
                    {
                        if (_buildings[index]._building.radius > 0 && _buildings[index]._building.rangedSpeed > 0)
                        {
                            float distance = BattleVector2.Distance(_units[_buildings[index]._target]._position, _buildings[index]._worldCenterPosition);
                            
                            Projectile projectile = new()
                            {
                                _type = TargetType.unit,
                                _target = _buildings[index]._target,
                                _timer = distance / _buildings[index]._building.rangedSpeed,
                                _damage = _buildings[index]._building.damage
                            };
                            _projectiles.Add(projectile);
                        }
                        else
                            _units[_buildings[index]._target].TakeDamage(_buildings[index]._building.damage);

                        if (_buildings[index]._attackCallback != null)
                            _buildings[index]._attackCallback.Invoke(_buildings[index]._building.id, _units[_buildings[index]._target]);
                    }
                }
            }
        }
        else if (FindTargetForBuilding(index))
            HandleBuilding(index);
    }

    bool FindTargetForBuilding(int index)
    {
        for (int i = 0; i < _units.Count; i++)
        {
            if (_units[i]._health <= 0)
                continue;

            if (IsUnitInRange(i, index))
            {
                _buildings[index]._attackTimer = 0;
                _buildings[index]._target = i;

                return true;
            }
        }

        return false;
    }

    bool IsUnitInRange(int unitIndex, int buildingIndex)
    {
        float distance = BattleVector2.Distance(_buildings[buildingIndex]._worldCenterPosition, _units[unitIndex]._position);

        return distance <= _buildings[buildingIndex]._building.radius && 
            !(_buildings[buildingIndex]._building.blindRange > 0 && distance <= _buildings[buildingIndex]._building.blindRange);
    }

    void HandleUnit(int index, double deltaTime)
    {
        if (_units[index]._path != null)
        {
            if (_units[index]._target < 0 || (_units[index]._target >= 0 && _buildings[_units[index]._target]._health <= 0))
            {
                _units[index]._path = null;
                _units[index]._target = -1;
            }
            else
            {
                double remainedTime = _units[index]._pathTime - _units[index]._pathTraveledTime;

                if (remainedTime >= deltaTime)
                {
                    _units[index]._pathTraveledTime += deltaTime;
                    deltaTime = 0;
                }
                else
                {
                    _units[index]._pathTraveledTime = _units[index]._pathTime;
                    deltaTime -= remainedTime;
                }

                _units[index]._position = GetPathPosition(_units[index]._path._points, (float)(_units[index]._pathTraveledTime / _units[index]._pathTime));

                if (_units[index]._data.attackRange > 0 && IsBuildingInRange(index, _units[index]._target))
                    _units[index]._path = null;
                else
                {
                    BattleVector2 targetPosition = GridToWorldPosition(new(_units[index]._path.LastCell().Location._x, _units[index]._path.LastCell().Location._y));
                    float distance = BattleVector2.Distance(_units[index]._position, targetPosition);

                    if (distance <= Data.CELL_SIZE * 0.05f)
                    {
                        _units[index]._position = targetPosition;
                        _units[index]._path = null;
                    }
                }
            }
        }

        if (_units[index]._target >= 0)
        {
            if (_buildings[_units[index]._target]._health > 0)
            {
                if(_buildings[_units[index]._target]._building.buildingId == Data.BuildingId.wall && _units[index]._mainTarget >= 0 && _buildings[_units[index]._mainTarget]._health <= 0)
                    _units[index]._target = -1;
                else
                {
                    if (_units[index]._path == null)
                    {
                        _units[index]._attackTimer += deltaTime;

                        if (_units[index]._attackTimer >= _units[index]._data.attackSpeed)
                        {
                            float distance = BattleVector2.Distance(_units[index]._position, _buildings[_units[index]._target]._worldCenterPosition);

                            if (_units[index]._data.attackRange > 0 && _units[index]._data.rangedSpeed > 0)
                            {
                                Projectile projectile = new()
                                {
                                    _type = TargetType.building,
                                    _target = _units[index]._target,
                                    _timer = distance / _units[index]._data.rangedSpeed,
                                    _damage = _units[index]._data.damage
                                };
                                _projectiles.Add(projectile);
                            }
                            else
                                _buildings[_units[index]._target].TakeDamage(_units[index]._data.damage, ref _grid, ref _blockedTiles);

                            _units[index]._attackTimer -= _units[index]._data.attackSpeed;

                            if (_units[index]._attackCallback != null)
                                _units[index]._attackCallback.Invoke(index, _buildings[_units[index]._target]);
                        }
                    }
                }
            }
            else
                _units[index]._target = -1;
        }

        if (_units[index]._target < 0)
        {
            FindTargets(index);

            if (deltaTime > 0 && _units[index]._target >= 0)
                HandleUnit(index, deltaTime);
        }
    }

    void ListUnitTargets(int index)
    {
        _units[index]._targets.Clear();

        for (int i = 0; i < _buildings.Count; i++)
        { 
            if(_buildings[i]._health <= 0 || _buildings[i]._building.buildingId == Data.BuildingId.wall)
                continue;

            float distance = BattleVector2.Distance(_buildings[i]._worldCenterPosition, _units[index]._position);

            _units[index]._targets.Add(i, distance);
        }
    }

    void FindTargets(int index)
    {
        ListUnitTargets(index);

        Dictionary<int, float> temp = _units[index]._targets;

        if (temp.Count > 0)
            AssignTarget(index, ref temp);
    }

    void AssignTarget(int index, ref Dictionary<int, float> targets)
    {
        float min = float.MaxValue;
        int r = 0;

        foreach (var t in targets)
        {
            if (t.Value < min)
            {
                min = t.Value;
                r = t.Key;
            }
        }

        var path = GetPathToBuilding(r, index);

        if (path.Item1 >= 0)
            _units[index].AssignTarget(path.Item1, path.Item2);
    }

    (int, Path) GetPathToBuilding(int buildingIndex, int unitIndex)
    {
        if (_buildings[buildingIndex]._building.buildingId == Data.BuildingId.wall)
            return (-1, null);

        BattleVector2Int unitGridPosition = WorldToGridPosition(_units[unitIndex]._position);

        List<int> columns = new();
        List<int> rows = new();

        int startX = _buildings[buildingIndex]._building.x;
        int endX = _buildings[buildingIndex]._building.x + _buildings[buildingIndex]._building.columns - 1;
        int startY = _buildings[buildingIndex]._building.y;
        int endY = _buildings[buildingIndex]._building.y + _buildings[buildingIndex]._building.rows - 1;

        if (_buildings[buildingIndex]._building.buildingId == Data.BuildingId.wall)
        {
            startX--;
            startY--;
            endX++;
            endY++;
        }

        columns.Add(startX);
        columns.Add(endX);
        rows.Add(startY);
        rows.Add(endY);

        List<Path> tiles = new();

        int closest = -1;
        float distance = 99999;
        int blocks = 999;

        for (int x = 0; x < columns.Count; x++)
        {
            for (int y = 0; y < rows.Count; y++)
            {
                if (x >= 0 && y >= 0 && x < Data.GRID_SIZE && y < Data.GRID_SIZE)
                {
                    Path path1 = new();
                    Path path2 = new();
                    path1.Create(ref _search, new(columns[x], rows[y]), unitGridPosition);
                    path2.Create(ref _unlimitedSearch, new(columns[x], rows[y]), unitGridPosition);

                    if (path1._points != null && path1._points.Count > 0)
                    {
                        path1._length = GetPathLength(path1._points);
                        int lengthToBlocks = (int)Math.Floor(path1._length / (Data.battleTilesWorthOfOneWall * Data.CELL_SIZE));

                        if (path1._length < distance && lengthToBlocks <= blocks)
                        {
                            closest = tiles.Count;
                            distance = path1._length;
                            blocks = lengthToBlocks;
                        }

                        tiles.Add(path1);
                    }

                    if (path2._points != null && path2._points.Count > 0)
                    {
                        path2._length = GetPathLength(path2._points);

                        for (int i = 0; i < path2._points.Count; i++)
                        {
                            for (int j = 0; j < _blockedTiles.Count; j++)
                            {
                                if (_blockedTiles[j]._position._x == path2._points[i].Location._x && _blockedTiles[j]._position._y == path2._points[i].Location._y)
                                {
                                    if (_blockedTiles[j]._id == Data.BuildingId.wall && _buildings[_blockedTiles[j]._index]._health > 0)
                                        path2._blocks.Add(_blockedTiles[j]);

                                    break;
                                }
                            }
                        }

                        if (path2._length < distance && path2._blocks.Count <= blocks)
                        {
                            closest = tiles.Count;
                            distance = path1._length;
                            blocks = path2._blocks.Count;
                        }

                        tiles.Add(path2);
                    }
                }
            }
        }

        tiles[closest]._points.Reverse();

        if (tiles[closest]._blocks.Count > 0)
        {
            for (int i = 0; i < _units.Count; i++)
            {
                if(_units[i]._health <= 0 || i != unitIndex || _units[i]._target < 0 || _units[i]._mainTarget != buildingIndex || _units[i]._mainTarget < 0 || _buildings[_units[i]._mainTarget]._building.buildingId != Data.BuildingId.wall || _buildings[_units[i]._mainTarget]._health <= 0)
                    continue;

                BattleVector2Int pos = WorldToGridPosition(_units[i]._position);
                List<Cell> points = _search.FindToList(new(pos._x, pos._y), new(unitGridPosition._x, unitGridPosition._y));

                if (!Path.IsValid(ref points, new(pos._x, pos._y), new(unitGridPosition._x, unitGridPosition._y)))
                    continue;

                Vector2Int end = _units[i]._path.LastCell().Location;
                Path path = new();

                if (path.Create(ref _search, pos, new(end)))
                {
                    _units[unitIndex]._mainTarget = buildingIndex;
                    path._blocks = _units[i]._path._blocks;
                    path._length = GetPathLength(path._points);

                    return (_units[i]._target, path);
                }
            }

            Tile last = tiles[closest].LastTile();

            for (int i = tiles[closest]._points.Count - 1; i >= 0; i--)
            {
                int x = tiles[closest]._points[i].Location._x;
                int y = tiles[closest]._points[i].Location._y;
                tiles[closest]._points.RemoveAt(i);

                if (x == last._position._x && y == last._position._y)
                    break;
            }

            _units[unitIndex]._mainTarget = buildingIndex;

            return (last._index, tiles[closest]);
        }
        else
            return (buildingIndex, tiles[closest]);
    }

    bool IsBuildingInRange(int unitIndex, int buildingIndex)
    {
        for (int x = _buildings[buildingIndex]._building.x; x < _buildings[buildingIndex]._building.x + _buildings[buildingIndex]._building.columns; x++)
        {
            for (int y = _buildings[buildingIndex]._building.y; y < _buildings[buildingIndex]._building.y + _buildings[buildingIndex]._building.columns; y++)
            {
                float distance = BattleVector2.Distance(GridToWorldPosition(new(x, y)), _units[unitIndex]._position);

                if(distance <= _units[unitIndex]._data.attackRange)
                    return true;
            }
        }

        return false;
    }

    float GetPathLength(IList<Cell> path, bool includeCellSize = true)
    {
        float length = 0;

        if(path != null && path.Count > 1)
        {
            for (int i = 1; i < path.Count; i++)
            {
                length += BattleVector2.Distance(new BattleVector2(path[i - 1].Location), new BattleVector2(path[i].Location));
            }
        }

        if (includeCellSize)
            length *= Data.CELL_SIZE;

        return length;
    }

    BattleVector2 GetPathPosition(IList<Cell> path, float t)
    {
        if(t < 0)
            t = 0;
        else if(t > 1)
            t = 1;

        float totalLength = GetPathLength(path);
        float length = 0;

        if (path != null && path.Count > 1)
        {
            for (int i = 1; i < path.Count; i++)
            {
                BattleVector2Int a = new(path[i - 1].Location._x, path[i - 1].Location._y);
                BattleVector2Int b = new(path[i].Location._x, path[i].Location._y);
                float l = BattleVector2.Distance(a, b) * Data.CELL_SIZE;
                float p = (length + l) / totalLength;

                if (p >= t)
                {
                    t = (t - (length / totalLength)) / (p - (length / totalLength));

                    return BattleVector2.LerpUnclamped(GridToWorldPosition(a), GridToWorldPosition(b), t);
                }

                length += l;
            }
        }

        return GridToWorldPosition(new(path[0].Location._x, path[0].Location._y));
    }

    BattleVector2 GridToWorldPosition(BattleVector2Int position)
    {
        return new(position._x * Data.CELL_SIZE + Data.CELL_SIZE / 2f, position._y * Data.CELL_SIZE + Data.CELL_SIZE / 2f);
    }

    BattleVector2Int WorldToGridPosition(BattleVector2 position)
    {
        return new((int)Math.Floor(position._x / Data.CELL_SIZE), (int)Math.Floor(position._y / Data.CELL_SIZE));
    }

    public bool CanAddUnit(int x, int y)
    {
        for (int i = 0; i < _buildings.Count; i++)
        {
            if (_buildings[i]._health <= 0)
                continue;

            int startX = _buildings[i]._building.x;
            int endX = _buildings[i]._building.x + _buildings[i]._building.columns;

            int startY = _buildings[i]._building.y;
            int endY = _buildings[i]._building.y + _buildings[i]._building.rows;

            for (int x2 = startX; x2 < endX; x2++)
            {
                for (int y2 = startY; y2 < endY; y2++)
                {
                    if (x == x2 && y == y2)
                    {
                        AlertManager.Instance.Error("건물 위에는 유닛을 배치할 수 없습니다.");
                        return false;
                    }
                }
            }
        }

        return true;
    }

    public void AddUnit(Data.Unit unit, int x, int y, SpawnCallBack spawnCallback, AttackBuildingCallback attackCallback, IndexCallback dieCallback, FloatCallback damageCallback)
    {
        BattleUnit battleUnit = new(unit)
        {
            _index = _unitIndex++,
            _attackCallback = attackCallback,
            _dieCallback = dieCallback,
            _damageCallback = damageCallback,
            _health = unit.health,
            _position = GridToWorldPosition(new(x, y))
        };

        if (!_isStart)
        {
            _isStart = true;
            _startTime = DateTime.Now;

            SoundManager.Instance.PlayBGM(SoundManager.BgmClip.battle);
        }

        _units.Insert(_units.Count, battleUnit);

        spawnCallback.Invoke(battleUnit._index);
    }

    public void ExecuteFrame()
    {
        if (_isStart)
        {
            for (int i = 0; i < _buildings.Count; i++)
            {
                if (_buildings[i]._building.damage > 0 && _buildings[i]._health > 0)
                    HandleBuilding(i);
            }

            for (int i = 0; i < _units.Count; i++)
            {
                if (_units[i]._health > 0)
                    HandleUnit(i, Time.deltaTime);
            }

            if (_projectiles.Count > 0)
            {
                for (int i = _projectiles.Count - 1; i >= 0; i--)
                {
                    _projectiles[i]._timer -= Time.deltaTime;

                    if (_projectiles[i]._timer <= 0)
                    {
                        if (_projectiles[i]._type == TargetType.unit)
                            _units[_projectiles[i]._target].TakeDamage(_projectiles[i]._damage);
                        else
                            _buildings[_projectiles[i]._target].TakeDamage(_projectiles[i]._damage, ref _grid, ref _blockedTiles);

                        _projectiles.RemoveAt(i);
                    }
                }
            }
        }
    }

    public int GetPercent()
    {
        return Mathf.RoundToInt((_allCount - UIBattleMain.Instance.GetBuildingCountForPercent()) * 100f / _allCount);
    }

    public void End(int remainingGold, int remainingElixir)
    {
        if (_isStart)
        {
            int gold = _maxPlunderableGold - remainingGold;
            int elixir = _maxPlunderableElixir - remainingElixir;

            UIBattleUnits.Instance.End();
            UIBattleMain.Instance.End();
            UIBattleResult.Instance.SetResult(gold, elixir, GetPercent(), UIBattleMain.Instance.GetStarsCount());
        }
        else
            SceneManager.LoadScene("Game");
    }
}
