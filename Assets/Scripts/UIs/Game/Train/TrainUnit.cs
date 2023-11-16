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
    [SerializeField] RawImage unitTexture;
    [SerializeField] SerializableDictionaryBase<UnitId, Texture> unitTextures;

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
        unitTexture.texture = unitTextures[unitData.id];

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
