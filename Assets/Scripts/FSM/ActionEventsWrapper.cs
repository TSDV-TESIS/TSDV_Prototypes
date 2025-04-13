using System;
using UnityEngine.Events;

namespace FSM
{
    [Serializable]
    public class ActionEventsWrapper
    {
        public UnityEvent onEnter;
        public UnityEvent onUpdate;
        public UnityEvent onExit;

        public void ExecuteOnEnter()
        {
            onEnter?.Invoke();
        }
        
        public void ExecuteOnUpdate()
        {
            onUpdate?.Invoke();
        }
        
        public void ExecuteOnExit()
        {
            onExit?.Invoke();
        }
    }
}