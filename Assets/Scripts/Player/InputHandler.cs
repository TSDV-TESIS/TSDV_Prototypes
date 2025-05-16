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
        public UnityEvent OnPlayerJump;
        public UnityEvent OnPlayerShadowStep;
        public UnityEvent OnPlayerBloodlust;
        public UnityEvent OnRestartScene;
        public UnityEvent<bool> OnZoomIn;
        public UnityEvent<bool> OnZoomOut;
        public UnityEvent OnInteract;

        public void OnMove(InputAction.CallbackContext context)
        {
            Vector2 movement = context.ReadValue<Vector2>();
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

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.started)
                OnPlayerJump?.Invoke();
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

        public void OnBloodlust(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnPlayerBloodlust?.Invoke();
        }

        public void OnZoomInEvent(InputAction.CallbackContext context)
        {
            if (context.performed) OnZoomIn.Invoke(true);
            else if (context.canceled) OnZoomIn.Invoke(false);
        }

        public void OnZoomOutEvent(InputAction.CallbackContext context)
        {
            if (context.performed) OnZoomOut.Invoke(true);
            else if (context.canceled) OnZoomOut.Invoke(false);
        }

        public void OnInteractEvent(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnInteract.Invoke();
        }
    }
}