using System.Collections.Generic;

public class BattleUnit
{
    public int target = -1;
    public int mainTarget = -1;
    public BattleVector2 position;
    public Path path;
    public double pathTime;
    public double pathTraveledTime;
    public double attackTimer;
    public AttackBuildingCallback attackCallback;
    public IndexCallback dieCallback;
    public FloatCallback damageCallback;

    public int Index { get; private set; }
    public float Health { get; private set; }
    public Data.Unit Data { get; private set; }
    public Dictionary<int, float> Targets { get; private set; }

    public BattleUnit(int index, Data.Unit data)
    {
        Index = index;
        Data = data;
        Health = data.health;
        Targets = new();
    }

    public void AssignTarget(int target, Path path)
    {
        attackTimer = 0;
        this.target = target;
        this.path = path;

        if (path is not null)
        {
            pathTraveledTime = 0;
            pathTime = path.length / (Data.moveSpeed * global::Data.CELL_SIZE);
        }
    }

    public void TakeDamage(float damage)
    {
        if (Health <= 0)
            return;

        Health -= damage;

        damageCallback?.Invoke(Index, damage);

        if (Health <= 0)
        {
            Health = 0;

            dieCallback?.Invoke(Index);
        }
    }
}
