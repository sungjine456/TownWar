using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIBar : MonoBehaviour
{
    [SerializeField] Image _bar;
    public RectTransform _rect;
    [SerializeField] TextMeshProUGUI _time;

    void Awake()
    {
        _rect.anchorMin = Vector2.zero;
        _rect.anchorMax = Vector2.zero;
    }

    public void FillAmount(float amount)
    {
        _bar.fillAmount = amount;
    }

    public void AdjustUI(int buildTime, TimeSpan span)
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        _bar.fillAmount = 1f - ((float)span.TotalSeconds / buildTime);

        if (_time)
            _time.text = span.ToString(@"hh\:mm\:ss");
    }
}
