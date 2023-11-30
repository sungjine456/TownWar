using UnityEngine;
using UnityEngine.UI;

using TMPro;

using RotaryHeart.Lib.SerializableDictionary;

using static Data;

public class TrainUnit : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _nameText;
    [SerializeField] TextMeshProUGUI _countText;
    [SerializeField] Button _removeBtn;
    [SerializeField] Image _unitImage;
    [SerializeField] SerializableDictionaryBase<UnitId, Sprite> _unitSprites;

    [HideInInspector] public Unit _unit;

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

    public virtual void Initialize(Unit unitData, int count = 1)
    {
        _unit = unitData;
        Count = count;
        _unitImage.sprite = _unitSprites[unitData.id];

        switch (unitData.id)
        {
            case UnitId.warrior:
                _nameText.text = "전사";
                break;
            case UnitId.archer:
                _nameText.text = "궁수";
                break;
        }
    }
}
