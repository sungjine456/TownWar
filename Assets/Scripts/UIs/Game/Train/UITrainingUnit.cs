using UnityEngine;

public class UITrainingUnit : TrainUnit
{
    [SerializeField] UIBar _bar;

    protected override void Cancel()
    {
        if (Count > 1)
            Count--;
        else
        {
            TrainingController.Instance.RemoveTrainingUnit(this);
            Destroy(gameObject);
        }

        Player.Instance.RemoveUnit(_unit.id, Data.UnitStatus.training);
    }

    public void UpdateTrainTime(float time)
    {
        _unit.trainedTime += time;
        _bar.FillAmount(_unit.trainedTime / _unit.trainTime);
    }
}
