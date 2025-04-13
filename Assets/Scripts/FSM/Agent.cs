using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM
{
    public abstract class Agent : MonoBehaviour
    {
        protected FSM Fsm;
        private Coroutine _updateCoroutine;
        
        protected abstract List<State> GetStates();

        protected virtual void OnEnable()
        { 
            Fsm = new FSM(GetStates());
        }

        protected virtual void OnDisable()
        {
            Fsm.Disable();
        }

        protected virtual void Update()
        {
            Fsm.Update();
        }

        protected State CreateState(ActionEventsWrapper actions)
        {
            State state = new State();
            
            state.EnterAction += actions.ExecuteOnEnter;
            state.UpdateAction += actions.ExecuteOnUpdate;
            state.ExitAction += actions.ExecuteOnExit;

            return state;
        }

        protected void CreateTransitionForState(State fromState, State toState)
        {
            Transition transition = new Transition(fromState, toState);
            fromState.AddTransition(transition);
        }
    }
}
