using UnityEngine;
using UnityEngine.UI;

public class UIButton : MonoBehaviour
{
    public Button _button;
    public RectTransform _rect;

    void Awake()
    {
        _rect.anchorMin = Vector2.zero;
        _rect.anchorMax = Vector2.zero;
    }
}
