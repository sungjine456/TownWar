using UnityEngine;

public class ParticleHelper : MonoBehaviour
{
    [SerializeField] ParticleSystem ps;

    public void OnParticleSystemStopped()
    {
        ParticlePoolManager.Instance.Remove(ps);
    }
}
