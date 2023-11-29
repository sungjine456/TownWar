using UnityEngine;

public class RotationUI : MonoBehaviour
{
    [SerializeField] bool reverse;

    void Update()
    {
        float z = transform.eulerAngles.z;

        if (reverse)
            z += 10f * Time.deltaTime;
        else
            z -= 10f * Time.deltaTime;

        transform.eulerAngles = new(0, 0, z);
    }
}
