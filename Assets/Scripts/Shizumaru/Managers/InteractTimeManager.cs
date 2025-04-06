using System;
using System.Collections;
using Events;
using Player;
using Unity.Cinemachine;
using UnityEngine;

namespace Shizumaru.Managers
{
    public class InteractTimeManager : MonoBehaviour
    {
        [SerializeField] private InputHandler handler;
        [SerializeField] private CinemachineCamera normalCamera;
        
        [Header("Time blend settings")]
        [SerializeField] private AnimationCurve timeBlendStop;
        [SerializeField] private AnimationCurve timeBlendIn;
        [SerializeField] private float durationSeconds = 1;
        [SerializeField] private float waitSeconds = 1;
        
        [Header("Events")]
        [SerializeField] private VoidEventChannelSO onInteractActionStarted;
        [SerializeField] private VoidEventChannelSO onInteractActionLocked;
        [SerializeField] private VoidEventChannelSO onInteractActionFinished;

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

            Debug.Log("Here?");
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
            Debug.Log("PRE WHILE");
            while (Time.unscaledTime - timeBlending < durationSeconds + waitSeconds)
            {
                if (Time.unscaledTime - timeBlending < waitSeconds)
                {
                    yield return null;
                    continue;
                }
                Debug.Log($"WHILE! {timeBlending - Time.unscaledTime}");
                Time.timeScale = (_isInteracting ? timeBlendStop : timeBlendIn).Evaluate(Time.unscaledTime - timeBlending - waitSeconds);
                
                yield return null;
            }

            onInteractActionLocked?.RaiseEvent();
            _isInteractionBlending = false;
        }
    }
}
