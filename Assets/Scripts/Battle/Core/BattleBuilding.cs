using System.Collections.Generic;

public delegate void DamageCallback(BattleBuilding building);

public class BattleBuilding
{
    public int target = -1;
    public double attackTimer;
    public BattleVector2 worldCenterPosition;
    public AttackUnitCallback attackCallback;
    public IndexCallback destroyCallback;
    public DamageCallback damageCallback;

    public Data.Building Building { get; private set; }
    public float Health { get; private set; }

    public BattleBuilding(Data.Building building)
    {
        Building = building;
        Health = building.health;
        worldCenterPosition = new((building.x + building.columns / 2f) * Data.CELL_SIZE, (building.y + building.rows / 2f) * Data.CELL_SIZE);
    }

    public void TakeDamage(float damage, ref BattleGrid grid, ref List<Tile> blockedTiles)
    {
        if (Health <= 0)
            return;

        Health -= damage;

        damageCallback?.Invoke(this);

        if (Health <= 0)
        {
            for (int x = Building.x; x < Building.x + Building.columns; x++)
            {
                for (int y = Building.y; y < Building.y + Building.rows; y++)
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

            destroyCallback?.Invoke(Building.id);
        }
    }
}
