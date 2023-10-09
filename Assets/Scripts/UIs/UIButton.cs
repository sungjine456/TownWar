using UnityEngine;
using UnityEngine.UI;

public class UIButton : MonoBehaviour
{
    public Button button;
    public RectTransform rect;

    void Awake()
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.zero;
    }

    void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
    }
}
