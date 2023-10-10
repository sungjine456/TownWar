using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UITrainUnit : MonoBehaviour
{
    [SerializeField] Button _btn;
    [SerializeField] TextMeshProUGUI _population;
    [SerializeField] Data.UnitId _id;

    Data.UnitToTrain _data;

    public int Level { get; set; } = 1;

    void Start()
    {
        _data = TrainingController.Instance._unitInfo.GetUnitData(_id, Level);
        _population.text = _data.numberOfPopulation.ToString();

        _btn.onClick.AddListener(Clicked);
    }

    void Clicked()
    {
        TrainingController.Instance.TrainUnit(_data);
    }
}
