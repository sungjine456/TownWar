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
    [SerializeField] GameBuildGrid _grid;

    [Header("Buttons")] public Transform _buttonsParent;

    readonly StringBuilder sb = new();
    int maxGold;
    int maxElixir;

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

    void SetText(TextMeshProUGUI target, int v1, int v2)
    {
        sb.Append(v1);
        sb.Append(" / ");
        sb.Append(v2);
        target.text = sb.ToString();
        sb.Clear();
    }

    void CollectResource(ResourceType type, int resource)
    {
        if (type == ResourceType.gold)
            GameManager.Instance.MyPlayer.CollectGold(resource);
        else if (type == ResourceType.elixir)
            GameManager.Instance.MyPlayer.CollectElixir(resource);

        SyncResourcesData();
    }

    /// <summary>
    /// 골드 수집 메서드
    /// </summary>
    /// <param name="resource">저장하려는 골드</param>
    /// <returns>저장하고 남은 골드이며 0 이상의 값을 보장한다.</returns>
    public int CollectGold(int resource)
    {
        if (GameManager.Instance.MyPlayer.Gold < maxGold)
        {
            if (resource + GameManager.Instance.MyPlayer.Gold > maxGold)
            {
                int remainedResource = resource + GameManager.Instance.MyPlayer.Gold - maxGold;
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
        if (GameManager.Instance.MyPlayer.Elixir < maxElixir)
        {
            if (resource + GameManager.Instance.MyPlayer.Elixir > maxElixir)
            {
                int remainedResource = resource + GameManager.Instance.MyPlayer.Elixir - maxElixir;
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

    public void AddMaxGold(int maxGold)
    {
        this.maxGold += maxGold;

        SyncResourcesData();
    }

    public void AddMaxElixir(int maxElixir)
    {
        this.maxElixir += maxElixir;

        SyncResourcesData();
    }

    public void SetStatus(bool status)
    {
        _elements.SetActive(status);
        IsActive = status;
    }

    public void SyncResourcesData()
    {
        SetText(_goldText, GameManager.Instance.MyPlayer.Gold, maxGold);
        SetText(_elixirText, GameManager.Instance.MyPlayer.Elixir, maxElixir);
        _gemsText.text = GameManager.Instance.MyPlayer.Gems.ToString();
    }
}
