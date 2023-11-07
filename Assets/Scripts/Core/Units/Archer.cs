using UnityEngine;

using static Data;

public class Archer : FieldUnit
{
    [SerializeField] GameObject _weaponPosition;

    public override UnitId Id { get { return UnitId.archer; } }

    public void DoAttackAnim()
    {
        var b = UIBattleMain.Instance.GetPosOfBuilding(_target.Building.id);

        if (b.HasValue)
        {
            SoundManager.Instance.PlaySFX(SoundManager.SfxClip.bow);

            LaunchedObj arrow = LaunchedObjPoolManager.Instance.GetArrow();
            arrow.Initialized(LaunchedObjPoolManager.Type.arrow, _weaponPosition.transform.position, b.Value, 4.2f);
        }
    }
}
