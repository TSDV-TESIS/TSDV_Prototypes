using System.Collections;
using Events;
using Events.Scriptables;
using Player;
using Unity.Cinemachine;
using UnityEngine;

namespace Shizumaru.Managers
{
    public class InteractTimeManager : MonoBehaviour
    {
        [SerializeField] private InputHandler handler;
        [SerializeField] private CinemachineCamera normalCamera;
        [SerializeField] private GameObject canvas;
        
        [Header("Time blend settings")]
        [SerializeField] private AnimationCurve timeBlendStop;
        [SerializeField] private AnimationCurve timeBlendIn;
        [SerializeField] private float durationSeconds = 1;
        [SerializeField] private float waitSeconds = 1;
        
        [Header("Events")]
        [SerializeField] private VoidEventChannelSO onInteractActionStarted;
        [SerializeField] private BoolEventSO onInteractActionLocked;

        private bool _isInteracting;
        private bool _canInteract;
        private bool _isInteractionBlending;
        private Coroutine _interactCoroutine;
        private void OnEnable()
        {
            _isInteracting = false;
            _isInteractionBlending = false;
            _canInteract = false;
            handler?.OnInteract?.AddListener(HandleInteract);
        }

        private void OnDisable()
        {
            handler?.OnInteract?.RemoveListener(HandleInteract);
        }

        private void Update()
        {
            if (!_canInteract) return;
            _canInteract = false;
            
            if(_interactCoroutine != null) StopCoroutine(_interactCoroutine);

            _interactCoroutine = StartCoroutine(InteractCoroutine());
        }


        private void HandleInteract()
        {
            if (_isInteractionBlending) return;

            _canInteract = true;
        }

        private IEnumerator InteractCoroutine()
        {
            _isInteractionBlending = true;
            _isInteracting = !_isInteracting;

            onInteractActionStarted?.RaiseEvent();
            normalCamera.gameObject.SetActive(!_isInteracting);
            float timeBlending = Time.unscaledTime;
            while (Time.unscaledTime - timeBlending < durationSeconds + waitSeconds)
            {
                if (Time.unscaledTime - timeBlending < waitSeconds)
                {
                    yield return null;
                    continue;
                }
                Debug.Log((Time.unscaledTime - timeBlending) / durationSeconds);
                Time.timeScale = (_isInteracting ? timeBlendStop : timeBlendIn).Evaluate((Time.unscaledTime - timeBlending - waitSeconds) / durationSeconds);
                
                yield return null;
            }

            canvas.SetActive(_isInteracting);
            onInteractActionLocked?.RaiseEvent(_isInteracting);
            _isInteractionBlending = false;
        }
    }
}
