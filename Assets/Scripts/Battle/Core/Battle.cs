using System;
using System.Collections.Generic;

using UnityEngine;

public delegate void SpawnCallBack(int index);
public delegate void IndexCallback(int index);
public delegate void FloatCallback(int index, float value);
public delegate void AttackUnitCallback(int index, BattleUnit target);
public delegate void AttackBuildingCallback(int index, BattleBuilding target);

public class Battle
{
    int _unitIndex;
    BattleGrid _grid;
    BattleGrid _unlimitedGrid;
    Search _search;
    Search _unlimitedSearch;
    List<Tile> _blockedTiles;
    readonly List<Projectile> _projectiles;

    public List<BattleBuilding> buildings;
    public List<BattleUnit> units;

    public int AllCount { get; private set; }
    public bool IsStart { get; private set; }
    public DateTime StartTime { get; private set; }

    public Battle(List<BattleBuilding> buildings)
    {
        _grid = new(Data.GRID_SIZE, Data.GRID_SIZE);
        _unlimitedGrid = new(Data.GRID_SIZE, Data.GRID_SIZE);
        _search = new(_grid);
        _unlimitedSearch = new(_unlimitedGrid);
        _blockedTiles = new();
        _projectiles = new();
        this.buildings = buildings;
        units = new();

        for (int i = 0; i < buildings.Count; i++)
        {
            if (buildings[i].Building.buildingId != Data.BuildingId.wall)
                AllCount++;

            int startX = buildings[i].Building.x;
            int endX = buildings[i].Building.x + buildings[i].Building.columns;
            int startY = buildings[i].Building.y;
            int endY = buildings[i].Building.y + buildings[i].Building.rows;

            if (buildings[i].Building.buildingId != Data.BuildingId.wall && buildings[i].Building.columns > 1 && buildings[i].Building.rows > 1)
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
                    _blockedTiles.Add(new(buildings[i].Building.buildingId, new(x, y), i));
                }
            }
        }
    }

    void HandleBuilding(int index)
    {
        if (buildings[index].target >= 0)
        {
            if (units[buildings[index].target].Health <= 0 || !IsUnitInRange(buildings[index].target, index))
                buildings[index].target = -1;
            else
            {
                buildings[index].attackTimer += Time.deltaTime;
                int attacksCount = (int)Math.Floor(buildings[index].attackTimer / buildings[index].Building.speed);

                if (attacksCount > 0)
                {
                    buildings[index].attackTimer -= (attacksCount * buildings[index].Building.speed);

                    for (int i = 1; i <= attacksCount; i++)
                    {
                        if (buildings[index].Building.radius > 0 && buildings[index].Building.rangedSpeed > 0)
                        {
                            float distance = BattleVector2.Distance(units[buildings[index].target].position, buildings[index].worldCenterPosition);
                            
                            Projectile projectile = new()
                            {
                                _type = TargetType.unit,
                                _target = buildings[index].target,
                                _timer = distance / buildings[index].Building.rangedSpeed,
                                _damage = buildings[index].Building.damage
                            };
                            _projectiles.Add(projectile);
                        }
                        else
                            units[buildings[index].target].TakeDamage(buildings[index].Building.damage);

                        buildings[index].attackCallback?.Invoke(buildings[index].Building.id, units[buildings[index].target]);
                    }
                }
            }
        }
        else if (FindTargetForBuilding(index))
            HandleBuilding(index);
    }

    bool FindTargetForBuilding(int index)
    {
        for (int i = 0; i < units.Count; i++)
        {
            if (units[i].Health <= 0)
                continue;

            if (IsUnitInRange(i, index))
            {
                buildings[index].attackTimer = 0;
                buildings[index].target = i;

                return true;
            }
        }

        return false;
    }

    bool IsUnitInRange(int unitIndex, int buildingIndex)
    {
        float distance = BattleVector2.Distance(buildings[buildingIndex].worldCenterPosition, units[unitIndex].position);

        return distance <= buildings[buildingIndex].Building.radius && 
            !(buildings[buildingIndex].Building.blindRange > 0 && distance <= buildings[buildingIndex].Building.blindRange);
    }

    void HandleUnit(int index, double deltaTime)
    {
        if (units[index].path is not null)
        {
            if (units[index].target < 0 || (units[index].target >= 0 && buildings[units[index].target].Health <= 0))
            {
                units[index].path = null;
                units[index].target = -1;
            }
            else
            {
                double remainedTime = units[index].pathTime - units[index].pathTraveledTime;

                if (remainedTime >= deltaTime)
                {
                    units[index].pathTraveledTime += deltaTime;
                    deltaTime = 0;
                }
                else
                {
                    units[index].pathTraveledTime = units[index].pathTime;
                    deltaTime -= remainedTime;
                }

                units[index].position = GetPathPosition(units[index].path.points, (float)(units[index].pathTraveledTime / units[index].pathTime));

                if (units[index].Data.attackRange > 0 && IsBuildingInRange(index, units[index].target))
                    units[index].path = null;
                else
                {
                    BattleVector2 targetPosition = GridToWorldPosition(new(units[index].path.LastCell().Location._x, units[index].path.LastCell().Location._y));
                    float distance = BattleVector2.Distance(units[index].position, targetPosition);

                    if (distance <= Data.CELL_SIZE * 0.05f)
                    {
                        units[index].position = targetPosition;
                        units[index].path = null;
                    }
                }
            }
        }

        if (units[index].target >= 0)
        {
            if (buildings[units[index].target].Health > 0)
            {
                if(buildings[units[index].target].Building.buildingId == Data.BuildingId.wall && units[index].mainTarget >= 0 && buildings[units[index].mainTarget].Health <= 0)
                    units[index].target = -1;
                else
                {
                    if (units[index].path is null)
                    {
                        units[index].attackTimer += deltaTime;

                        if (units[index].attackTimer >= units[index].Data.attackSpeed)
                        {
                            float distance = BattleVector2.Distance(units[index].position, buildings[units[index].target].worldCenterPosition);

                            if (units[index].Data.attackRange > 0 && units[index].Data.rangedSpeed > 0)
                            {
                                Projectile projectile = new()
                                {
                                    _type = TargetType.building,
                                    _target = units[index].target,
                                    _timer = distance / units[index].Data.rangedSpeed,
                                    _damage = units[index].Data.damage
                                };
                                _projectiles.Add(projectile);
                            }
                            else
                                buildings[units[index].target].TakeDamage(units[index].Data.damage, ref _grid, ref _blockedTiles);

                            units[index].attackTimer -= units[index].Data.attackSpeed;
                            units[index].attackCallback?.Invoke(index, buildings[units[index].target]);
                        }
                    }
                }
            }
            else
                units[index].target = -1;
        }

        if (units[index].target < 0)
        {
            FindTargets(index);

            if (deltaTime > 0 && units[index].target >= 0)
                HandleUnit(index, deltaTime);
        }
    }

    void ListUnitTargets(int index)
    {
        units[index].Targets.Clear();

        for (int i = 0; i < buildings.Count; i++)
        {
            if(buildings[i].Health <= 0 || buildings[i].Building.buildingId == Data.BuildingId.wall)
                continue;

            float distance = BattleVector2.Distance(buildings[i].worldCenterPosition, units[index].position);

            units[index].Targets.Add(i, distance);
        }
    }

    void FindTargets(int index)
    {
        ListUnitTargets(index);

        Dictionary<int, float> temp = units[index].Targets;

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
            units[index].AssignTarget(path.Item1, path.Item2);
    }

    (int, Path) GetPathToBuilding(int buildingIndex, int unitIndex)
    {
        if (buildings[buildingIndex].Building.buildingId == Data.BuildingId.wall)
            return (-1, null);

        BattleVector2Int unitGridPosition = WorldToGridPosition(units[unitIndex].position);

        List<int> columns = new();
        List<int> rows = new();

        int startX = buildings[buildingIndex].Building.x;
        int endX = buildings[buildingIndex].Building.x + buildings[buildingIndex].Building.columns - 1;
        int startY = buildings[buildingIndex].Building.y;
        int endY = buildings[buildingIndex].Building.y + buildings[buildingIndex].Building.rows - 1;

        if (buildings[buildingIndex].Building.buildingId == Data.BuildingId.wall)
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
                if (x < Data.GRID_SIZE && y < Data.GRID_SIZE)
                {
                    Path path1 = new();
                    Path path2 = new();
                    path1.Create(_search, new(columns[x], rows[y]), unitGridPosition);
                    path2.Create(_unlimitedSearch, new(columns[x], rows[y]), unitGridPosition);

                    if (path1.points?.Count > 0)
                    {
                        path1.length = GetPathLength(path1.points);
                        int lengthToBlocks = (int)Math.Floor(path1.length / (Data.battleTilesWorthOfOneWall * Data.CELL_SIZE));

                        if (path1.length < distance && lengthToBlocks <= blocks)
                        {
                            closest = tiles.Count;
                            distance = path1.length;
                            blocks = lengthToBlocks;
                        }

                        tiles.Add(path1);
                    }

                    if (path2.points?.Count > 0)
                    {
                        path2.length = GetPathLength(path2.points);

                        for (int i = 0; i < path2.points.Count; i++)
                        {
                            for (int j = 0; j < _blockedTiles.Count; j++)
                            {
                                if (_blockedTiles[j]._position._x == path2.points[i].Location._x && _blockedTiles[j]._position._y == path2.points[i].Location._y)
                                {
                                    if (_blockedTiles[j]._id == Data.BuildingId.wall && buildings[_blockedTiles[j]._index].Health > 0)
                                        path2.blocks.Add(_blockedTiles[j]);

                                    break;
                                }
                            }
                        }

                        if (path2.length < distance && path2.blocks.Count <= blocks)
                        {
                            closest = tiles.Count;
                            distance = path1.length;
                            blocks = path2.blocks.Count;
                        }

                        tiles.Add(path2);
                    }
                }
            }
        }

        tiles[closest].points.Reverse();

        if (tiles[closest].blocks.Count > 0)
        {
            for (int i = 0; i < units.Count; i++)
            {
                if(units[i].Health <= 0 || i != unitIndex || units[i].target < 0 || units[i].mainTarget != buildingIndex || units[i].mainTarget < 0 || buildings[units[i].mainTarget].Building.buildingId != Data.BuildingId.wall || buildings[units[i].mainTarget].Health <= 0)
                    continue;

                BattleVector2Int pos = WorldToGridPosition(units[i].position);
                List<Cell> points = _search.FindToList(new(pos._x, pos._y), new(unitGridPosition._x, unitGridPosition._y));

                if (!Path.IsValid(ref points, new(pos._x, pos._y), new(unitGridPosition._x, unitGridPosition._y)))
                    continue;

                Vector2Int end = units[i].path.LastCell().Location;
                Path path = new();

                if (path.Create(_search, pos, new(end)))
                {
                    units[unitIndex].mainTarget = buildingIndex;
                    path.blocks = units[i].path.blocks;
                    path.length = GetPathLength(path.points);

                    return (units[i].target, path);
                }
            }

            Tile last = tiles[closest].LastTile();

            for (int i = tiles[closest].points.Count - 1; i >= 0; i--)
            {
                int x = tiles[closest].points[i].Location._x;
                int y = tiles[closest].points[i].Location._y;

                tiles[closest].points.RemoveAt(i);

                if (x == last._position._x && y == last._position._y)
                    break;
            }

            units[unitIndex].mainTarget = buildingIndex;

            return (last._index, tiles[closest]);
        }
        else
            return (buildingIndex, tiles[closest]);
    }

    bool IsBuildingInRange(int unitIndex, int buildingIndex)
    {
        var b = buildings[buildingIndex].Building;

        for (int x = b.x; x < b.x + b.columns; x++)
        {
            for (int y = b.y; y < b.y + b.columns; y++)
            {
                float distance = BattleVector2.Distance(GridToWorldPosition(new(x, y)), units[unitIndex].position);

                if(distance <= units[unitIndex].Data.attackRange)
                    return true;
            }
        }

        return false;
    }

    float GetPathLength(IList<Cell> path, bool includeCellSize = true)
    {
        float length = 0;

        for (int i = 1; i < path?.Count; i++)
        {
            length += BattleVector2.Distance(new BattleVector2(path[i - 1].Location), new BattleVector2(path[i].Location));
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

        for (int i = 1; i < path?.Count; i++)
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
        for (int i = 0; i < buildings.Count; i++)
        {
            if (buildings[i].Health <= 0)
                continue;

            int startX = buildings[i].Building.x;
            int endX = buildings[i].Building.x + buildings[i].Building.columns;

            int startY = buildings[i].Building.y;
            int endY = buildings[i].Building.y + buildings[i].Building.rows;

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
        BattleUnit battleUnit = new(_unitIndex++, unit)
        {
            attackCallback = attackCallback,
            dieCallback = dieCallback,
            damageCallback = damageCallback,
            position = GridToWorldPosition(new(x, y))
        };

        if (!IsStart)
        {
            IsStart = true;
            StartTime = DateTime.Now;

            SoundManager.Instance.PlayBGM(SoundManager.BgmClip.battle);
        }

        units.Insert(units.Count, battleUnit);

        spawnCallback.Invoke(battleUnit.Index);
    }

    public void ExecuteFrame()
    {
        if (IsStart)
        {
            for (int i = 0; i < buildings.Count; i++)
            {
                if (buildings[i].Building.damage > 0 && buildings[i].Health > 0)
                    HandleBuilding(i);
            }

            for (int i = 0; i < units.Count; i++)
            {
                if (units[i].Health > 0)
                    HandleUnit(i, Time.deltaTime);
            }

            for (int i = _projectiles.Count - 1; i >= 0; i--)
            {
                _projectiles[i]._timer -= Time.deltaTime;

                if (_projectiles[i]._timer <= 0)
                {
                    if (_projectiles[i]._type == TargetType.unit)
                        units[_projectiles[i]._target].TakeDamage(_projectiles[i]._damage);
                    else
                        buildings[_projectiles[i]._target].TakeDamage(_projectiles[i]._damage, ref _grid, ref _blockedTiles);

                    _projectiles.RemoveAt(i);
                }
            }
        }
    }
}
