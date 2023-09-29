using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShopItem : MonoBehaviour
{
    [SerializeField] Button _button;

    void Start()
    {
        _button.onClick.AddListener(Clicked);
    }

    void Clicked()
    {

    }
}
