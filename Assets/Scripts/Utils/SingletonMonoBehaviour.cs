using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
{
    public static T Instance { get; private set; }

    protected virtual void OnAwake() { }
    protected virtual void OnStart() { }

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = (T)this;
            OnAwake();
        }
        else
            Destroy(gameObject);
    }

    void Start()
    {
        if (Instance == (T)this)
            OnStart();
    }
}
