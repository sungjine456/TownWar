using UnityEngine;
using UnityEngine.UI;

public class UITrainedUnit : TrainUnit
{
    [SerializeField] Button _button;

    bool _isReady;

    void Enable(bool enable)
    {
        _isReady = enable;
        var colors = _button.colors;

        if (enable)
            colors.normalColor = Color.white;
        else
            colors.normalColor = Color.gray;

        _button.colors = colors;
    }

    public override void Initialize(Data.Unit unitData, int count = 1)
    {
        base.Initialize(unitData, count);

        Enable(false);
    }

    protected override void Cancel()
    {
        if (Count > 1)
            Count--;
        else
        {
            TrainingController.Instance.RemoveTrainedUnit(this, _isReady);

            Destroy(gameObject);
        }

        if (_isReady)
        {
            GameManager.Instance.MyPlayer.RemoveUnit(_unit.id, Data.UnitStatus.army);

            TrainingController.Instance.SwitchUnitState();
        }
        else
            GameManager.Instance.MyPlayer.RemoveUnit(_unit.id, Data.UnitStatus.trained);
    }

    public void SetReady() => Enable(true);
}