using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TrainUnit : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _nameText;
    [SerializeField] TextMeshProUGUI _countText;
    [SerializeField] Button _removeBtn;

    [HideInInspector] public Data.Unit _unit;

    int _count;

    public int Count
    {
        get => _count;
        set
        {
            _count = value;
            _countText.text = value.ToString();
        }
    }

    void Start()
    {
        _removeBtn.onClick.AddListener(Cancel);
    }

    protected virtual void Cancel() { }

    public virtual void Initialize(Data.Unit unitData, int count = 1)
    {
        _unit = unitData;
        Count = count;

        switch (unitData.id)
        {
            case Data.UnitId.warrior:
                _nameText.text = "전사";
                break;
            case Data.UnitId.archer:
                _nameText.text = "궁수";
                break;
        }
    }
}
