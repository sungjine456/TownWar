public enum TargetType
{
    unit, building
}

public class Projectile
{
    public int _target;
    public float _damage;
    public float _timer;
    public TargetType _type = TargetType.unit;
}
