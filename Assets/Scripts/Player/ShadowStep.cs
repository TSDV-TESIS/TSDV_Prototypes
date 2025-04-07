using System;
using Health;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Movement2D), typeof(HealthPoints))]
    public class ShadowStep : MonoBehaviour
    {
        [SerializeField] private InputHandler input;
        [SerializeField] private Collider playerCollider;
        [SerializeField] private float shadowVelocity;
        [SerializeField] private float healthDecayPerSecond;
        [SerializeField] private LayerMask trespassableColliders;
        
        [Header("Visuals")]
        [SerializeField] private MeshRenderer playerRenderer;
        [SerializeField] private Material shadowMat;

        private Movement2D _playerMovement;
        private CharacterController _characterController;
        private HealthPoints _healthPoints;
        private bool _isShadow = false;
        private float _prevSpeed;
        private Material _prevMaterial;
        private LayerMask _defaultLayerMask;

        private float accumulatedDecay = 0;

        void OnEnable()
        {
            input.OnPlayerShadowStep.AddListener(ShadowStepToggle);
            _playerMovement ??= GetComponent<Movement2D>();
            _characterController ??= GetComponent<CharacterController>();
            _healthPoints ??= GetComponent<HealthPoints>();
            _prevMaterial = playerRenderer.material;
            _prevSpeed = _playerMovement.Velocity;
            _defaultLayerMask = _characterController.excludeLayers;
        }

        private void Update()
        {
            if (!_isShadow)
                return;

            accumulatedDecay += healthDecayPerSecond * Time.deltaTime;
            if (accumulatedDecay > 1)
            {
                _healthPoints.TryTakeDamage(Mathf.FloorToInt(accumulatedDecay));
                accumulatedDecay -= 1;
            }
        }

        private void ShadowStepToggle()
        {
            _isShadow = !_isShadow;
            if (_isShadow)
            {
                playerRenderer.material = shadowMat;
                _playerMovement.Velocity = shadowVelocity;
                _characterController.excludeLayers = trespassableColliders;
            }
            else
            {
                playerRenderer.material = _prevMaterial;
                _playerMovement.Velocity = _prevSpeed;
                _characterController.excludeLayers = _defaultLayerMask;
            }

            playerCollider.isTrigger = _isShadow;
        }
    }
}