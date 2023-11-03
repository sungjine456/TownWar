using UnityEngine;

using static Utils;

public class LaunchedObj : MonoBehaviour
{
    [SerializeField] float _moveSpeed;
    Vector3 _targetPos;
    LaunchedObjPoolManager.Type _type;

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, _targetPos, _moveSpeed * Time.deltaTime);

        if (V3Equal(transform.position, _targetPos))
            LaunchedObjPoolManager.Instance.Remove(_type, this);
    }

    public void Initialized(LaunchedObjPoolManager.Type type, Vector3 startPos, Vector3 targetPos, float speed)
    {
        _moveSpeed = speed;
        _type = type;
        transform.position = startPos;
        _targetPos = new(targetPos.x, startPos.y, targetPos.z);
        transform.rotation = Quaternion.LookRotation(targetPos - transform.position);
        transform.eulerAngles = new Vector3(-90, transform.eulerAngles.y, transform.eulerAngles.z);
    }
}
