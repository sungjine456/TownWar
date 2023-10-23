using System;
using UnityEngine;

public enum BattleUnitState
{
    Idle,
    Attack,
    Run,
    Death
}

public class BattleFieldUnit : MonoBehaviour
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
    Vector3 _lastPosition;
    BattleUnitState _state;
    bool _isChangeState;
    
    protected BattleBuilding _target;

    public virtual Data.UnitId Id { get { return Data.UnitId.warrior; } }
    public int Index { get; private set; } = -1;

    void Start()
    {
        _aniCtrl = GetComponent<AnimationController>();
    }

    void Update()
    {
        if (Vector3.SqrMagnitude(_lastPosition - transform.position) > 0.00000001)
        {
            SetState(BattleUnitState.Run);
            Vector3 dir = transform.position - _lastPosition;
            _lastPosition = transform.position;
            transform.rotation = Quaternion.LookRotation(dir);
        }
        else if (_state == BattleUnitState.Run)
            SetState(BattleUnitState.Idle);

        BehaviourProcess();
    }

    void BehaviourProcess()
    {
        if (_isChangeState)
        {
            _isChangeState = false;

            switch (_state)
            {
                case BattleUnitState.Idle:
                    _aniCtrl.Play(AnimationController.AniMotion.Idle);
                    break;
                case BattleUnitState.Run:
                    _aniCtrl.Play(AnimationController.AniMotion.Run);
                    break;
                case BattleUnitState.Attack:
                    _aniCtrl.Play(AnimationController.AniMotion.Attack);
                    break;
                case BattleUnitState.Death:
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

    public void SetState(BattleUnitState state)
    {
        if (_state != state)
        {
            _isChangeState = true;
            _state = state;
        }
    }
}
