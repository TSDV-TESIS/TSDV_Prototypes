using UnityEngine;

namespace FSM
{
    public abstract class Controller<T> : MonoBehaviour
    {
        [SerializeField] protected T agent;

        public abstract void OnUpdate();
    }
}