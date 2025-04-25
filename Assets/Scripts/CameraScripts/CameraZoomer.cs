using System;
using Events.Scriptables;
using Player;
using Unity.Cinemachine;
using UnityEngine;

namespace CameraScripts
{
    [RequireComponent(typeof(CinemachinePositionComposer))]
    public class CameraZoomer : MonoBehaviour
    {
        [SerializeField] private float zoomVelocity = 5f;
        
        [Header("Events")] 
        [SerializeField] private InputHandler handler;

        private CinemachinePositionComposer _positionComposer;
        private bool _isZooming;
        private bool _isZoomingIn;
        private void OnEnable()
        {
            _positionComposer ??= GetComponent<CinemachinePositionComposer>();
            _isZooming = false;
            _isZoomingIn = false;
            
            handler?.OnZoomIn?.AddListener(HandleZoomIn);
            handler?.OnZoomOut?.AddListener(HandleZoomOut);
        }

        private void OnDisable()
        {
            handler?.OnZoomIn?.RemoveListener(HandleZoomIn);
            handler?.OnZoomOut?.RemoveListener(HandleZoomOut);
        }

        void Update()
        {
            if (_isZooming)
            {
                _positionComposer.CameraDistance += (_isZoomingIn ? -1 : 1) * zoomVelocity * Time.deltaTime;
            }
        }
        
        private void HandleZoomOut(bool isPressing)
        {
            _isZooming = isPressing;
            if (_isZooming) _isZoomingIn = false;
        }

        private void HandleZoomIn(bool isPressing)
        {
            _isZooming = isPressing;
            if (_isZooming) _isZoomingIn = true;
        }
    }
}
