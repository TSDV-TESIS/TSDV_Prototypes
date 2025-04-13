using System;
using System.Collections;

namespace FSM
{
    public class Transition
    {
        public State From;
        public State To;
        public event Action TransitionAction;

        public Transition(State fromState, State toState)
        {
            From = fromState;
            To = toState;
        }

        public void AddTransitionAction(Action newAction)
        {
            TransitionAction += newAction;
        }
        
        public void Do()
        {
            From.Exit();
            TransitionAction?.Invoke();
            To.Enter();
        }
    }
}
