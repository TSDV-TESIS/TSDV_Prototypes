using System.Collections.Generic;
using FSM;
using UnityEngine;

public class EnemyAgent : Agent
{
    [Header("Events")] 
    [SerializeField] private ActionEventsWrapper patrolEvents;
    [SerializeField] private ActionEventsWrapper chaseEvents;
    [SerializeField] private ActionEventsWrapper attackEvents;

    private State _patrolState;
    private State _chaseState;
    private State _attackState;

    public void ChangeStateToPatrol()
    {
        Fsm.ChangeState(_patrolState);
    }

    public void ChangeStateToChase()
    {
        Fsm.ChangeState(_chaseState);
    }

    public void ChangeStateToAttack()
    {
        Fsm.ChangeState(_attackState);
    }

    protected override List<State> GetStates()
    {
        #region states

        _patrolState = CreateState(patrolEvents);
        _chaseState = CreateState(chaseEvents);
        _attackState = CreateState(attackEvents);

        #endregion

        #region transitions

        CreateTransitionForState(_patrolState, _chaseState);
        CreateTransitionForState(_chaseState, _attackState);
        CreateTransitionForState(_attackState, _chaseState);
        CreateTransitionForState(_chaseState, _patrolState);

        #endregion

        return new List<State>()
        {
            _patrolState,
            _chaseState,
            _attackState
        };
    }
}
