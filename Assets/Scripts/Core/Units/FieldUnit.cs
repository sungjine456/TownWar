using System;

using UnityEngine;

public class FieldUnit : MonoBehaviour
{
    const float STAYING_TIME = 0.5f;

    [Serializable]
    class Level
    {
        public int level;
        public GameObject mesh;
    }
    
    [SerializeField] Level[] _levels;
    public UIBar _healthBar;

    [HideInInspector] public Data.Unit _unit;
    [HideInInspector] public AnimationController _aniCtrl;
    [HideInInspector] public Vector3 _lastPosition;
    bool _isChangeState;
    bool _isStaying;
    float _stayingTime;
    
    protected BattleBuilding _target;

    public virtual Data.UnitId Id { get { return Data.UnitId.warrior; } }
    public AniMotion CurrentState { get { return _aniCtrl.Motion; } }
    public int Index { get; private set; } = -1;

    void Awake()
    {
        _aniCtrl = GetComponent<AnimationController>();
    }

    void Update()
    {
        if (_isStaying)
        {
            _stayingTime += Time.deltaTime;

            if (_stayingTime > STAYING_TIME)
            {
                _stayingTime = 0f;
                _isStaying = false;
            }
        }

        if (!_isStaying && Vector3.SqrMagnitude(_lastPosition - transform.position) > 0.00000001)
        {   
            SetState(AniMotion.Run);

            if (GameManager.Instance.IsBattling)
            {
                Vector3 dir = transform.position - _lastPosition;
                _lastPosition = transform.position;
                transform.rotation = Quaternion.LookRotation(dir);
            }
            else
                transform.SetPositionAndRotation(
                    Vector3.MoveTowards(transform.position, _lastPosition, 2 * Time.deltaTime),
                    Quaternion.LookRotation(_lastPosition - transform.position));
        }
        else if (_aniCtrl.Motion == AniMotion.Run)
        {
            _isStaying = true;
            SetState(AniMotion.Idle);
        }

        BehaviourProcess();
    }

    void BehaviourProcess()
    {
        if (_isChangeState)
        {
            _isChangeState = false;

            _aniCtrl.Play();
        }
    }

    public void SetTarget(BattleBuilding target)
    {
        _target = target;
    }

    public void Initialize(int index, Data.Unit unit)
    {
        _unit = unit;
        Index = index;
        _lastPosition = transform.position;
        SetState(AniMotion.Idle);
    }

    public void SetState(AniMotion state)
    {
        if (_aniCtrl.Motion != state)
        {
            _isChangeState = true;
            _aniCtrl.SetMotion(state);
        }
    }

    public void EndDethAnim()
    {
        Destroy(gameObject);
    }
}
