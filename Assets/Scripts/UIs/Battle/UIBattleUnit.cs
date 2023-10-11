using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UIBattleUnit : MonoBehaviour
{
    [SerializeField] Button _btn;
    [SerializeField] TextMeshProUGUI _countText;
    [SerializeField] TextMeshProUGUI _levelText;
    [SerializeField] TextMeshProUGUI _nameText;

    [HideInInspector] public Queue<Data.Unit> _units;
    [HideInInspector] public Data.UnitId _id;

    public int Count { get { return _units.Count; } }

    void Start()
    {
        _btn.onClick.AddListener(Click);
    }

    void Click() => UIBattleUnits.Instance.SelectUnit(_id);

    public void Initialized(Queue<Data.Unit> units, int level)
    {
        if (units.Count > 0)
        {
            _id = units.Peek().id;
            _units = units;
            _levelText.text = level.ToString();
            _nameText.text = units.Peek().id.ToString();
            _countText.text = units.Count.ToString();
            Active(false);
            gameObject.SetActive(true);
        }
    }

    public void Active(bool isActive)
    {
        Utils.ChangeButtonColor(_btn, isActive ? Color.white : Color.gray);
    }

    public Data.Unit Pop()
    {
        if (Count > 0)
        {
            _countText.text = (_units.Count - 1).ToString();
            return _units.Dequeue();
        }   

        return null;
    }
}
