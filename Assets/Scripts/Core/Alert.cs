using System.Collections;

using UnityEngine;

using TMPro;

public class Alert : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _message;

    IEnumerator Coroutine_Remove()
    {
        yield return YieldInstructionCache.WaitForSeconds(3f);
        AlertManager.Instance.Remove(this);
    }

    public void Initialize(string msg, Color32 color)
    {
        gameObject.SetActive(true);

        _message.text = msg;
        _message.color = color;

        StartCoroutine(nameof(Coroutine_Remove));
    }
}
