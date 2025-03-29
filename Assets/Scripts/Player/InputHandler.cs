using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Player
{
    [CreateAssetMenu(fileName = "InputHandler", menuName = "Scriptable Objects/InputHandler")]
    public class InputHandler : ScriptableObject
    {
        public UnityEvent<Vector2> OnPlayerMove;
        public UnityEvent OnPlayerAttack;
        
        public void OnMove(InputAction.CallbackContext context)
        {
            Vector2 movement = context.ReadValue<Vector2>();
            Debug.Log(movement);
            OnPlayerMove?.Invoke(movement);
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnPlayerAttack?.Invoke();
            }
        }
    }
}
