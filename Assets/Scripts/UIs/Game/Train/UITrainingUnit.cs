using UnityEngine.UI;

public class UITrainingUnit : TrainUnit
{
    public Image _bar;

    protected override void Cancel()
    {
        if (Count > 1)
            Count--;
        else
        {
            TrainingController.Instance.RemoveTrainingUnit(this);
            Destroy(gameObject);
        }

        GameManager.Instance.MyPlayer.RemoveUnit(_unit.id, Data.UnitStatus.training);
    }
}
