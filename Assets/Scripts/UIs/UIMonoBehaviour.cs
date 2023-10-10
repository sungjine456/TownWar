using UnityEngine;

public class UIMonoBehaviour<T> : SingletonMonoBehaviour<T> where T : UIMonoBehaviour<T>
{
    [SerializeField] GameObject _elements;

    protected override void OnStart()
    {
        SetActive(this is UIMain);
    }

    public virtual void SetActive(bool active)
    {
        _elements.SetActive(active);
    }
}
