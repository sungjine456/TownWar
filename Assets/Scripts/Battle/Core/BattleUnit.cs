using System.Collections.Generic;

public delegate void UnitIdCallback(Data.UnitId id);

public class BattleUnit
{
    public int _index;
    public Data.Unit _data;
    public float _health;
    public int _target = -1;
    public int _mainTarget = -1;
    public BattleVector2 _position;
    public Path _path;
    public double _pathTime;
    public double _pathTraveledTime;
    public double _attackTimer;
    public Dictionary<int, float> _targets;
    public AttackCallback _attackCallback;
    public IndexCallback _dieCallback;
    public FloatCallback _damageCallback;

    public BattleUnit(Data.Unit data)
    {
        _data = data;
        _targets = new();
    }

    public void AssignTarget(int target, Path path)
    {
        _attackTimer = 0;
        _target = target;
        _path = path;

        if (path != null)
        {
            _pathTraveledTime = 0;
            _pathTime = path._length / (_data.moveSpeed * Data.CELL_SIZE);
        }
    }

    public void TakeDamage(float damage)
    {
        if (_health <= 0)
            return;

        _health -= damage;

        if (_damageCallback != null)
            _damageCallback.Invoke(_index, damage);

        if (_health <= 0)
        {
            _health = 0;

            if (_dieCallback != null)
                _dieCallback.Invoke(_index);
        }
    }
}
