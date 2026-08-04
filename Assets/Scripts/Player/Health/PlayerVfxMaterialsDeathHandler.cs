using System;
using System.Collections;
using Health;
using UnityEngine;
using UnityEngine.VFX;

namespace Player.Health
{
    [RequireComponent(typeof(HealthPoints))]
    public class PlayerVfxMaterialsDeathHandler : MonoBehaviour
    {
        [SerializeField] private PlayerDeathProperties playerDeathProperties;
        [SerializeField] private Material playerMaterial;
        [SerializeField] private VisualEffect fireEffect;

        private static readonly int DissolveAmount = Shader.PropertyToID("_Dissolve_Amount");
        private static readonly int DissolveGradientHeight = Shader.PropertyToID("_Dissolve_GradientHeight");
        private static readonly int Dead = Shader.PropertyToID("_Dead");

        private HealthPoints _healthPoints;
        private Coroutine _deathSequenceCoroutine;

        private void Awake()
        {
            _healthPoints = GetComponent<HealthPoints>();
        }

        private void OnEnable()
        {
            SetPlayerMaterialValues();

            if (_healthPoints != null && _healthPoints.OnDeathEvent != null)
            {
                _healthPoints.OnDeathEvent.onTypedEvent.AddListener(HandleDeath);
            }
        }

        private void OnDisable()
        {
            if (_healthPoints != null && _healthPoints.OnDeathEvent != null)
            {
                _healthPoints.OnDeathEvent.onTypedEvent.RemoveListener(HandleDeath);
            }
        }

        [ContextMenu("Test Trigger Death (Manual)")]
        public void ForceTestDeath()
        {
            HandleDeath(DeathCauses.Spikes);
        }

        private void HandleDeath(DeathCauses cause)
        {
            if (fireEffect != null)
            {
                fireEffect.SendEvent(playerDeathProperties.stopVfxEventName);
            }

            if (_deathSequenceCoroutine != null) StopCoroutine(_deathSequenceCoroutine);
            _deathSequenceCoroutine = StartCoroutine(DeathSequenceCoroutine());
        }

        private IEnumerator DeathSequenceCoroutine()
        {
            if (playerMaterial != null)
            {
                playerMaterial.SetFloat(Dead, 1f);
            }

            float timeInObscure = 0f;
            while (timeInObscure < playerDeathProperties.deathObscuringTime)
            {
                float timeLerp = playerDeathProperties.obscuringCurve.Evaluate(
                    timeInObscure / playerDeathProperties.deathObscuringTime);

                float value = Mathf.Lerp(playerDeathProperties.obscureMaxValue, playerDeathProperties.obscureMinValue, timeLerp);

                if (playerMaterial != null)
                {
                    playerMaterial.SetFloat(DissolveGradientHeight, value);
                }

                timeInObscure += Time.deltaTime;
                yield return null;
            }

            if (playerMaterial != null)
            {
                playerMaterial.SetFloat(DissolveGradientHeight, playerDeathProperties.obscureMinValue);
            }

            if (playerDeathProperties.deathDissolvingStartTime > 0f)
            {
                yield return new WaitForSeconds(playerDeathProperties.deathDissolvingStartTime);
            }

            float timeInDissolve = 0f;
            while (timeInDissolve < playerDeathProperties.deathDissolvingTime)
            {
                float timeLerp = playerDeathProperties.dissolvingCurve.Evaluate(
                    timeInDissolve / playerDeathProperties.deathDissolvingTime);

                float value = Mathf.Lerp(playerDeathProperties.dissolvingMinValue, playerDeathProperties.dissolvingMaxValue, timeLerp);

                if (playerMaterial != null)
                {
                    playerMaterial.SetFloat(DissolveAmount, value);
                }

                timeInDissolve += Time.deltaTime;
                yield return null;
            }

            if (playerMaterial != null)
            {
                playerMaterial.SetFloat(DissolveAmount, playerDeathProperties.dissolvingMaxValue);
            }
        }

        private void SetPlayerMaterialValues()
        {
            if (playerMaterial == null) return;

            playerMaterial.SetFloat(Dead, 0f);
            playerMaterial.SetFloat(DissolveAmount, playerDeathProperties.dissolvingMinValue);
            playerMaterial.SetFloat(DissolveGradientHeight, playerDeathProperties.obscureMaxValue);
        }
    }
}