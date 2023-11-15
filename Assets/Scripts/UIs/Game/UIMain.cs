using System.Text;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

using static Data;

public class UIMain : SingletonMonoBehaviour<UIMain>
{
    [SerializeField] GameObject _elements;
    [SerializeField] TextMeshProUGUI _goldText;
    [SerializeField] TextMeshProUGUI _elixirText;
    [SerializeField] TextMeshProUGUI _gemsText;
    [SerializeField] TextMeshProUGUI _builderText;
    [SerializeField] Button _shopBtn;
    [SerializeField] Button _searchBtn;
    [SerializeField] Button _optionBtn;
    [SerializeField] GameBuildGrid _grid;

    [Header("Buttons")] public Transform _buttonsParent;

    readonly StringBuilder _sb = new();

    public bool IsActive { get; private set; }
    public GameBuildGrid Grid => _grid;

    protected override void OnAwake()
    {
        SetStatus(true);
    }

    protected override void OnStart()
    {
        _shopBtn.onClick.AddListener(ShopBtnClicked);
        _searchBtn.onClick.AddListener(SearchBtnClicked);
        _optionBtn.onClick.AddListener(OptionBtnClicked);

        SoundManager.Instance.PlayBGM(SoundManager.BgmClip.town);
    }

    void ShopBtnClicked()
    {
        UIBuild.Instance.Cancel();
        SetStatus(false);
        UIShop.Instance.SetStatus(true);
    }

    void SearchBtnClicked()
    {
        UIBuild.Instance.Cancel();
        SetStatus(false);
        UISearch.Instance.SetStatus(true);
    }

    void OptionBtnClicked()
    {
        UIBuild.Instance.Cancel();
        SetStatus(false);
        UISystemOption.Instance.SetStatus(true);
    }

    void SetText(TextMeshProUGUI target, int v1, int v2)
    {
        _sb.Append(v1);
        _sb.Append(" / ");
        _sb.Append(v2);
        target.text = _sb.ToString();
        _sb.Clear();
    }

    void CollectResource(ResourceType type, int resource)
    {
        if (type == ResourceType.gold)
        {
            SoundManager.Instance.PlaySFX(SoundManager.SfxClip.getGold);
            Player.Instance.CollectGold(resource);
        }
        else if (type == ResourceType.elixir)
        {
            SoundManager.Instance.PlaySFX(SoundManager.SfxClip.getElixir);
            Player.Instance.CollectElixir(resource);
        }

        SyncResourcesData();
    }

    /* 
     * <summary> 골드 수집 메서드 </summary>
     * <param name="resource">저장하려는 골드</param>
     * <returns>저장하고 남은 골드이며 0 이상의 값을 보장한다.</returns>
    */
    public int CollectGold(int resource)
    {
        if (Player.Instance.Gold < GameManager.Instance.MaxGold)
        {
            if (resource + Player.Instance.Gold > GameManager.Instance.MaxGold)
            {
                int remainedResource = resource + Player.Instance.Gold - GameManager.Instance.MaxGold;
                CollectResource(ResourceType.gold, resource - remainedResource);

                return remainedResource;
            }
            else
            {
                CollectResource(ResourceType.gold, resource);

                return 0;
            }
        }

        return resource;
    }

    /// <summary>
    /// 엘릭서 수집 메서드
    /// </summary>
    /// <param name="resource">저장하려는 엘릭서</param>
    /// <returns>저장하고 남은 엘릭서이며 0 이상의 값을 보장한다.</returns>
    public int CollectElixir(int resource)
    {
        if (Player.Instance.Elixir < GameManager.Instance.MaxElixir)
        {
            if (resource + Player.Instance.Elixir > GameManager.Instance.MaxElixir)
            {
                int remainedResource = resource + Player.Instance.Elixir - GameManager.Instance.MaxElixir;
                CollectResource(ResourceType.elixir, resource - remainedResource);

                return remainedResource;
            }
            else
            {
                CollectResource(ResourceType.elixir, resource);

                return 0;
            }
        }

        return resource;
    }

    public void UpdateBuilder()
    {
        int maxBuilder = BuildingController.Instance.CountBuildBuildingHut();
        int count = 0;

        for (int i = 0; i < _grid._buildings.Count; i++)
        {
            if (_grid._buildings[i].IsConstructing)
                count++;
        }

        SetText(_builderText, maxBuilder - count, maxBuilder);
    }

    public void SetStatus(bool status)
    {
        _elements.SetActive(status);
        IsActive = status;
    }

    public void SyncResourcesData()
    {
        SetText(_goldText, Player.Instance.Gold, GameManager.Instance.MaxGold);
        SetText(_elixirText, Player.Instance.Elixir, GameManager.Instance.MaxElixir);
        _gemsText.text = Player.Instance.Gems.ToString();
    }

    public bool IsFullGold()
    {
        return GameManager.Instance.MaxGold <= Player.Instance.Gold;
    }

    public bool IsFullElixir()
    {
        return GameManager.Instance.MaxElixir <= Player.Instance.Elixir;
    }

    public void AddArmyUnit(Unit unit)
    {
        ArmyCamp camp = (ArmyCamp)Grid.GetBuilding(BuildingId.armyCamp);
        camp.AddUnit(unit);
    }

    public void RemoveArmyUnit(UnitId id)
    {
        ArmyCamp camp = (ArmyCamp)Grid.GetBuilding(BuildingId.armyCamp);
        camp.RemoveUnit(id);
    }
}
