using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMap : MonoBehaviour
{
    [SerializeField] Button _btn;
    [SerializeField] int _number;

    void Start()
    {
        _btn.onClick.AddListener(Clicked);
    }

    void Clicked()
    {
        BattleManager.LoadedNumberOfMap = _number;
        SceneManager.LoadScene("Battle");
    }
}
