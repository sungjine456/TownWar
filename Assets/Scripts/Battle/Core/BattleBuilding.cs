using System.Collections.Generic;

public delegate void DamageCallback(BattleBuilding building);

public class BattleBuilding
{
    public Data.Building _building;
    public float _health;
    public int _target = -1;
    public double _attackTimer;
    public BattleVector2 _worldCenterPosition;
    public AttackUnitCallback _attackCallback;
    public IndexCallback _destroyCallback;
    public DamageCallback _damageCallback;

    public BattleBuilding(Data.Building building)
    {
        _building = building;
        _health = building.health;
        _worldCenterPosition = new((building.x + building.columns / 2f) * Data.CELL_SIZE, (building.y + building.rows / 2f) * Data.CELL_SIZE);
    }

    public void TakeDamage(float damage, ref BattleGrid grid, ref List<Tile> blockedTiles)
    {
        if (_health <= 0)
            return;

        _health -= damage;

        _damageCallback?.Invoke(this);

        if (_health <= 0)
        {
            _health = 0;

            for (int x = _building.x; x < _building.x + _building.columns; x++)
            {
                for (int y = _building.y; y < _building.y + _building.rows; y++)
                {
                    grid[x, y].Blocked = false;

                    for (int i = 0; i < blockedTiles.Count; i++)
                    {
                        if (blockedTiles[i]._position._x == x && blockedTiles[i]._position._y == y)
                        {
                            blockedTiles.RemoveAt(i);
                            break;
                        }
                    }
                }
            }

            _destroyCallback?.Invoke(_building.id);
        }
    }
}
