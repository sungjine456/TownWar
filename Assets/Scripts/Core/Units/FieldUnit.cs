using System;
using UnityEngine;

public enum FieldUnitState
{
    Idle,
    Attack,
    Run,
    Death
}

public class FieldUnit : MonoBehaviour
{
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
    public Vector3 _lastPosition;
    [SerializeField] FieldUnitState _state;
    bool _isChangeState;
    
    protected BattleBuilding _target;

    public virtual Data.UnitId Id { get { return Data.UnitId.warrior; } }
    public FieldUnitState CurrentState { get { return _state; } }
    public int Index { get; private set; } = -1;

    void Start()
    {
        _aniCtrl = GetComponent<AnimationController>();
    }

    void Update()
    {
        if (Vector3.SqrMagnitude(_lastPosition - transform.position) > 0.0001)
        {
            SetState(FieldUnitState.Run);

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
        else if (_state == FieldUnitState.Run)
            SetState(FieldUnitState.Idle);

        BehaviourProcess();
    }

    void BehaviourProcess()
    {
        if (_isChangeState)
        {
            _isChangeState = false;

            switch (_state)
            {
                case FieldUnitState.Idle:
                    _aniCtrl.Play(AnimationController.AniMotion.Idle);
                    break;
                case FieldUnitState.Run:
                    _aniCtrl.Play(AnimationController.AniMotion.Run);
                    break;
                case FieldUnitState.Attack:
                    _aniCtrl.Play(AnimationController.AniMotion.Attack);
                    break;
                case FieldUnitState.Death:
                    _aniCtrl.Play(AnimationController.AniMotion.Death);
                    break;
            }
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
    }

    public void SetState(FieldUnitState state)
    {
        if (_state != state)
        {
            _isChangeState = true;
            _state = state;
        }
    }
}
