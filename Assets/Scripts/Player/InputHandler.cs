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
        public UnityEvent<Vector2> OnPlayerLook;
        public UnityEvent OnPlayerHook;
        public UnityEvent OnPlayerShadowStep;

        public UnityEvent OnRestartScene;

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

        public void OnLook(InputAction.CallbackContext context)
        {
            Vector2 lookDir = context.ReadValue<Vector2>();
            OnPlayerLook?.Invoke(lookDir);
        }

        public void OnHook(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnPlayerHook?.Invoke();
            }
        }

        public void OnShadowStep(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnPlayerShadowStep.Invoke();
        }

        public void OnReset(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnRestartScene.Invoke();
        }
    }
}