using UnityEngine;

using static Utils;

public class LaunchedObj : MonoBehaviour
{
    [SerializeField] float _moveSpeed;

    Vector3 _targetPos;
    LaunchedObjType _type;

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, _targetPos, _moveSpeed * Time.deltaTime);

        if (V3Equal(transform.position, _targetPos))
            LaunchedObjPoolManager.Instance.Remove(_type, this);
    }

    public void Initialized(LaunchedObjType type, Vector3 startPos, Vector3 targetPos, float speed)
    {
        _moveSpeed = speed;
        _type = type;
        transform.position = startPos;
        _targetPos = new(targetPos.x, targetPos.y + 0.5f, targetPos.z);
        transform.rotation = Quaternion.LookRotation(_targetPos - transform.position);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x - 90, transform.eulerAngles.y, transform.eulerAngles.z);
    }
}
