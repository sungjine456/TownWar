using UnityEngine;

using static Utils;

public class Arrow : MonoBehaviour
{
    readonly float _moveSpeed = 4.2f;
    Vector3 _targetPos;

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, _targetPos, _moveSpeed * Time.deltaTime);

        if (V3Equal(transform.position, _targetPos))
            ArrowPoolManager.Instance.Remove(this);
    }

    public void Initialized(Vector3 startPos, Vector3 targetPos)
    {
        transform.position = startPos;
        _targetPos = new(targetPos.x, startPos.y, targetPos.z);
        transform.rotation = Quaternion.LookRotation(targetPos - transform.position);
        transform.eulerAngles = new Vector3(-90, transform.eulerAngles.y, transform.eulerAngles.z);
    }
}
