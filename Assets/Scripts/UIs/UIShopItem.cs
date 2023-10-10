using UnityEngine;
using UnityEngine.UI;
using TMPro;

using static Data;

public class UIShopItem : MonoBehaviour
{
    [SerializeField] BuildingId _id;
    [SerializeField] Button _button;
    [SerializeField] TextMeshProUGUI _text;

    bool _disabled;

    public BuildingId Id { get { return _id; } }

    void Start()
    {
        _button.onClick.AddListener(Clicked);
    }

    void Clicked()
    {
        if (!_disabled)
        {
            UIShop.Instance.SetActive(false);
            UIMain.Instance.SetActive(true);

            BuildManager.Instance.CreateBuilding(_id);
        }
        else
            AlertManager.Instance.Error("자원이 부족합니다.");
    }

    public void SetDisabled(bool disabled)
    {
        ColorBlock colors = _button.colors;

        _disabled = disabled;

        if (disabled)
        {
            colors.normalColor = Color.gray;
            colors.highlightedColor = Color.gray;
            colors.pressedColor = Color.gray;
            colors.selectedColor = Color.gray;
        }
        else
        {
            colors.normalColor = Color.white;
            colors.highlightedColor = Color.white;
            colors.pressedColor = Color.white;
            colors.selectedColor = Color.white;
        }

        _button.colors = colors;
    }
}
