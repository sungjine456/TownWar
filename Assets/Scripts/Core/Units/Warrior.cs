using static Data;

public class Warrior : FieldUnit
{
    public override UnitId Id { get { return UnitId.warrior; } }

    public void DoAttackAnim()
    {
        SoundManager.Instance.PlaySFX(SoundManager.SfxClip.sword);
    }
}
