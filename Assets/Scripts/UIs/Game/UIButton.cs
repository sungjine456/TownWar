using UnityEngine;
using UnityEngine.UI;

using static Utils;

public class UIButton : MonoBehaviour
{
    public Button _button;
    public RectTransform _rect;

    bool _isActive;

    public bool IsActive { get { return _isActive; } private set { _isActive = value; } }

    void Awake()
    {
        _rect.anchorMin = Vector2.zero;
        _rect.anchorMax = Vector2.zero;

        SetActive(true);
    }

    public void SetActive(bool active)
    {
        IsActive = active;

        if (active)
            ChangeButtonColor(_button, Color.white);
        else
            ChangeButtonColor(_button, Color.red);
    }
}
