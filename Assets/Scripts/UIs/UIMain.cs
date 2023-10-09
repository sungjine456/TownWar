using System.Text;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIMain : UIMonoBehaviour<UIMain>
{
    [SerializeField] TextMeshProUGUI _goldText;
    [SerializeField] Button _shopBtn;

    [Header("Buttons")] public Transform _buttonsParent;
    public UIButton _collectGold;

    public bool IsActive { get; private set; }

    readonly StringBuilder sb = new();

    protected override void OnStart()
    {
        _shopBtn.onClick.AddListener(ClickedShopButton);
    }

    void ClickedShopButton()
    {
        SetActive(false);
        UIShop.Instance.SetActive(true);
        UIBuild.Instance.Cancel();
    }

    public void SetGold(int gold, int maxGold)
    {
        sb.Append(gold);
        sb.Append(" / ");
        sb.Append(maxGold);
        _goldText.text = sb.ToString();
        sb.Clear();
    }

    public override void SetActive(bool active)
    {
        base.SetActive(active);
        IsActive = active;
    }
}
