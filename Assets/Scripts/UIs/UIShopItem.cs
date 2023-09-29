using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using static Data;

public class UIShopItem : MonoBehaviour
{
    [SerializeField] BuildingId _id;
    [SerializeField] Button _button;

    void Start()
    {
        _button.onClick.AddListener(Clicked);
    }

    void Clicked()
    {
        UIShop.Instance.SetActive(false);
        UIMain.Instance.SetActive(true);

        var prefab = GameManager.Instance.GetBuildingPrefab(_id);
        
        if (prefab != null)
        {
            Instantiate(prefab, Vector3.zero, Quaternion.identity);
        }
    }
}
