using System;
using System.Collections;
using Events;
using Events.ScriptableObjects;
using Events.Scriptables;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

namespace CameraScripts
{
    public class CameraController : MonoBehaviour
    {
        [FormerlySerializedAs("camera")] [SerializeField] private CinemachineCamera mainCamera;
        [SerializeField] private CinemachinePositionComposer composer;
        [SerializeField] private CinemachineBasicMultiChannelPerlin shakeController;
        [SerializeField] private NoiseSettings shakeSettings;

        [SerializeField] private ShakeProfileEventChannel onCameraShakeEventChannelSo;
        [SerializeField] private VoidEventChannelSO onCameraShakeStopEventChannelSo;

        [Header("Events")] [SerializeField] private CameraZoomZoneEventSO onCameraZoomZoneEnter;
        [SerializeField] private VoidEventChannelSO onCameraZoomZoneLeave;

        private CameraProperties _cameraProperties;
        private Coroutine _cameraShake;

        private float _initialCameraDistance;

        private void Awake()
        {
            SetComposerSettings();
            onCameraShakeEventChannelSo.onTypedEvent.AddListener(HandleCameraShake);
            onCameraShakeStopEventChannelSo.onEvent.AddListener(HandleCameraShakeStop);
        }

        private void OnEnable()
        {
            onCameraZoomZoneEnter?.onTypedEvent.AddListener(HandleCameraZoomZoneEnter);
            onCameraZoomZoneLeave?.onEvent.AddListener(HandleCameraZoomZoneLeave);
        }

        private void OnDestroy()
        {
            onCameraShakeEventChannelSo.onTypedEvent.RemoveListener(HandleCameraShake);
            onCameraShakeStopEventChannelSo.onEvent.RemoveListener(HandleCameraShakeStop);
            onCameraZoomZoneEnter.onTypedEvent.RemoveListener(HandleCameraZoomZoneEnter);
            onCameraZoomZoneLeave.onEvent.RemoveListener(HandleCameraZoomZoneLeave);
        }

        private void HandleCameraZoomZoneLeave()
        {
            composer.CameraDistance = _initialCameraDistance;
        }

        private void HandleCameraZoomZoneEnter(CameraZoomZoneProperties newProperties)
        {
            composer.CameraDistance = newProperties.zoomIn;
        }

        private void SetComposerSettings()
        {
            mainCamera.Lens.FieldOfView = _cameraProperties.FOV;
            composer.CameraDistance = _cameraProperties.cameraDistance;
            _initialCameraDistance = _cameraProperties.cameraDistance;

            composer.Composition.ScreenPosition = _cameraProperties.screenPosition;
            composer.Composition.DeadZone.Enabled = _cameraProperties.deadZone;
            composer.Composition.DeadZone.Size = _cameraProperties.deadZoneSize;

            composer.TargetOffset = _cameraProperties.targetOffset;
            composer.Damping = _cameraProperties.damping;
        }

        private void HandleCameraShake(CameraShakeProfile shakeProfile)
        {
            if (_cameraShake != null)
                StopCoroutine(_cameraShake);

            _cameraShake = StartCoroutine(CameraShake(shakeProfile.noiseParams, shakeProfile.duration));
        }

        private void HandleCameraShakeStop()
        {
            if (_cameraShake != null)
                StopCoroutine(_cameraShake);
        }

        private IEnumerator CameraShake(NoiseSettings.TransformNoiseParams noiseParams, float duration)
        {
            shakeController.enabled = true;
            shakeSettings.PositionNoise[0] = noiseParams;
            shakeController.NoiseProfile = shakeSettings;
            yield return new WaitForSeconds(duration);
            shakeController.NoiseProfile = null;
            shakeController.enabled = false;
        }

        public void SetNewFov(CameraProperties cameraProperties)
        {
            _cameraProperties = cameraProperties;
            SetComposerSettings();
        }
    }
}