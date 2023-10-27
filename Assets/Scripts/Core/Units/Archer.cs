using UnityEngine;

using static Data;

public class Archer : FieldUnit
{
    [SerializeField] GameObject _weaponPosition;

    public override UnitId Id { get { return UnitId.archer; } }

    public void DoAttackAnim()
    {
        var b = UIBattleMain.Instance.GetPosOfBuilding(_target._building.id);

        if (b.HasValue)
        {
            Arrow arrow = ArrowPoolManager.Instance.Get();
            arrow.Initialized(_weaponPosition.transform.position, b.Value);
        }
    }
}
