using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIBattleResult : SingletonMonoBehaviour<UIBattleResult>
{
    [SerializeField] GameObject _elements;
    [SerializeField] Button _closeBtn;
    [SerializeField] TextMeshProUGUI _plunderGoldText;
    [SerializeField] TextMeshProUGUI _plunderElixirText;
    [SerializeField] Image[] _stars;
    [SerializeField] TextMeshProUGUI _percentText;
    [SerializeField] TextMeshProUGUI _resultText;

    protected override void OnAwake()
    {
        _closeBtn.onClick.AddListener(Close);
        _elements.SetActive(false);
    }

    void Close()
    {
        SceneManager.LoadScene("Game");
    }

    public void SetResult(int gold, int elixir, int percent, int starCounts)
    {
        _elements.SetActive(true);
        _plunderGoldText.text = gold.ToString();
        _plunderElixirText.text = elixir.ToString();
        _percentText.text = percent.ToString();
        _resultText.text = starCounts != 0 ? "승 리" : "패 배";
        _resultText.color = starCounts != 0 ? Color.yellow : Color.red;

        if (starCounts > 3)
            starCounts = 3;

        for (int i = 0; i < starCounts; i++)
        {
            _stars[i].color = Color.yellow;
        }
    }
}
