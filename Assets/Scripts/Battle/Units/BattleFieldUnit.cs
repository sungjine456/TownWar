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
    
    [SerializeField] Level[] levels;
    public Data.UnitId id;
    public UIBar healthBar;

    [HideInInspector] public Data.Unit unit;
    [HideInInspector] public int index = -1;
    [HideInInspector] public AnimationController aniCtrl;
    Vector3 lastPosition = Vector3.zero;
    BattleUnitState state;
    bool isChangeState;

    public int Index { get { return index; } }

    void Start()
    {
        aniCtrl = GetComponent<AnimationController>();
    }

    void Update()
    {
        if (Vector3.SqrMagnitude(lastPosition - transform.position) > 0.00000001)
        {
            SetState(BattleUnitState.Run);
            Vector3 dir = transform.position - lastPosition;
            lastPosition = transform.position;
            transform.rotation = Quaternion.LookRotation(dir);
        }
        else if (state == BattleUnitState.Run)
            SetState(BattleUnitState.Idle);

        BehaviourProcess();
    }

    void BehaviourProcess()
    {
        if (isChangeState)
        {
            isChangeState = false;

            switch (state)
            {
                case BattleUnitState.Idle:
                    aniCtrl.Play(AnimationController.AniMotion.Idle);
                    break;
                case BattleUnitState.Run:
                    aniCtrl.Play(AnimationController.AniMotion.Run);
                    break;
                case BattleUnitState.Attack:
                    aniCtrl.Play(AnimationController.AniMotion.Attack);
                    break;
                case BattleUnitState.Death:
                    aniCtrl.Play(AnimationController.AniMotion.Death);
                    break;
            }
        }
    }

    public void Initialize(int index, Data.Unit unit)
    {
        this.unit = unit;
        this.index = index;
        lastPosition = transform.position;
    }

    public void SetState(BattleUnitState state)
    {
        if (this.state != state)
        {
            isChangeState = true;
            this.state = state;
        }
    }
}
