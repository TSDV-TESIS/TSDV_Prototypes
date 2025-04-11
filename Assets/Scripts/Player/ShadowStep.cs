using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using Health;
using Unity.Mathematics;
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

        [Header("Shadow")]
        [SerializeField] private GameObject shadowPrefab;
        [SerializeField] private float spawnRate;
        [SerializeField] private float shadowDecayDelay;
        [SerializeField] private VoidEventChannelSO onPlayerShouldDie;
        [SerializeField] private VoidEventChannelSO onPlayerDeath;

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

        private float _accumulatedDecay = 0;


        private List<GameObject> _shadows = new List<GameObject>();
        private Coroutine _spawnShadows;

        void OnEnable()
        {
            input.OnPlayerShadowStep.AddListener(ShadowStepToggle);
            _playerMovement ??= GetComponent<Movement2D>();
            _characterController ??= GetComponent<CharacterController>();
            _healthPoints ??= GetComponent<HealthPoints>();
            _prevMaterial = playerRenderer.material;
            _prevSpeed = _playerMovement.MaxSpeed;
            _defaultLayerMask = _characterController.excludeLayers;

            onPlayerShouldDie.onEvent.AddListener(HandlePlayerDeath);
        }

        private void Update()
        {
            if (!_isShadow)
                return;

            _accumulatedDecay += healthDecayPerSecond * Time.deltaTime;
            if (_accumulatedDecay > 1)
            {
                _healthPoints.TryTakeDamage(Mathf.FloorToInt(_accumulatedDecay));
                _accumulatedDecay -= 1;
            }
        }

        private void ShadowStepToggle()
        {
            _isShadow = !_isShadow;
            if (_isShadow)
            {
                playerRenderer.material = shadowMat;
                _playerMovement.MaxSpeed = shadowVelocity;
                _characterController.excludeLayers = trespassableColliders;

                if (_spawnShadows != null)
                    StopCoroutine(_spawnShadows);

                _spawnShadows = StartCoroutine(SpawnShadows());
            }
            else
            {
                playerRenderer.material = _prevMaterial;
                _playerMovement.MaxSpeed = _prevSpeed;
                _characterController.excludeLayers = _defaultLayerMask;
            }

            playerCollider.isTrigger = _isShadow;
        }

        private void HandlePlayerDeath()
        {
            if (_isShadow)
            {
                StopCoroutine(_spawnShadows);
                StartCoroutine(DeathCollapse());
            }
            else
            {
                onPlayerDeath.RaiseEvent();
            }
        }

        private IEnumerator SpawnShadows()
        {
            ClearShadows();
            float timer = spawnRate;
            float startTime = Time.time;
            _shadows.Add(Instantiate(shadowPrefab, transform.position, quaternion.identity));

            while (_isShadow)
            {
                timer = Time.time - startTime;
                if (timer > spawnRate)
                {
                    startTime = Time.time;
                    _shadows.Add(Instantiate(shadowPrefab, transform.position, quaternion.identity));
                }

                yield return null;
            }

            yield return CollapseShadows();
        }

        private IEnumerator CollapseShadows()
        {
            for (int i = 0; i < _shadows.Count; i++)
            {
                _shadows[i].GetComponent<ShadowExplosion>().Explode();
                yield return new WaitForSeconds(shadowDecayDelay);
            }

            _shadows.Clear();
        }

        private IEnumerator DeathCollapse()
        {
            ShadowStepToggle();
            yield return CollapseShadows();
            if (_healthPoints.IsDead())
            {
                onPlayerDeath.RaiseEvent();
            }
        }

        private void ClearShadows()
        {
            for (int i = 0; i < _shadows.Count; i++)
            {
                Destroy(_shadows[i]);
            }

            _shadows.Clear();
        }
    }
}