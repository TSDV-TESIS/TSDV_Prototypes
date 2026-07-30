using System;
using System.Collections;
using Events;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.VFX;

namespace Player.Health
{
    public class PlayerVfxMaterialsDeathHandler : MonoBehaviour
    {
        [SerializeField] private PlayerDeathProperties playerDeathProperties;
        [SerializeField] private VoidEventChannelSO onPlayerDeath;
        [SerializeField] private Material playerMaterial;
        [SerializeField] private VisualEffect fireEffect;

        private static readonly int DissolveAmount = Shader.PropertyToID("_Dissolve_Amount");
        private static readonly int DissolveGradientLight = Shader.PropertyToID("_Dissolve_GradientLight");
        private static readonly int Dead = Shader.PropertyToID("_Dead");

        private Coroutine _dissolveCoroutine;
        private Coroutine _obscureCoroutine;

        private void OnEnable()
        {
            SetPlayerMaterialValues();

            onPlayerDeath?.onEvent.AddListener(HandleDeath);
        }

        private void OnDisable()
        {
            onPlayerDeath?.onEvent.RemoveListener(HandleDeath);
        }

        private void HandleDeath()
        {
            fireEffect.SendEvent(playerDeathProperties.stopVfxEventName);
            playerMaterial.SetInt(Dead, 1);

            if (_dissolveCoroutine != null) StopCoroutine(_dissolveCoroutine);
            _dissolveCoroutine = StartCoroutine(DissolveCoroutine());

            if (_obscureCoroutine != null) StopCoroutine(_obscureCoroutine);
            _obscureCoroutine = StartCoroutine(ObscureCoroutine());
        }

        private IEnumerator ObscureCoroutine()
        {
            float timeInCoroutine = 0f;

            while (timeInCoroutine < playerDeathProperties.deathObscuringTime)
            {
                float timeLerp =
                    playerDeathProperties.obscuringCurve.Evaluate(timeInCoroutine /
                                                                  playerDeathProperties.deathObscuringTime);

                float value = Mathf.Lerp(playerDeathProperties.obscureMinValue, playerDeathProperties.obscureMaxValue,
                    timeLerp);

                playerMaterial.SetFloat(DissolveGradientLight, value);
                timeInCoroutine += Time.deltaTime;
                yield return null;
            }

            playerMaterial.SetFloat(DissolveGradientLight, playerDeathProperties.obscureMinValue);
        }

        private IEnumerator DissolveCoroutine()
        {
            yield return new WaitForSeconds(playerDeathProperties.deathDissolvingStartTime);

            float timeInCoroutine = 0f;

            while (timeInCoroutine < playerDeathProperties.deathDissolvingTime)
            {
                float timeLerp =
                    playerDeathProperties.dissolvingCurve.Evaluate(timeInCoroutine /
                                                                   playerDeathProperties.deathDissolvingTime);

                float value = Mathf.Lerp(playerDeathProperties.dissolvingMaxValue, playerDeathProperties.dissolvingMinValue,
                    timeLerp);

                Debug.Log($"DISSOLVING {value}");

                playerMaterial.SetFloat(DissolveAmount, value);
                timeInCoroutine += Time.deltaTime;
                yield return null;
            }

            playerMaterial.SetFloat(DissolveAmount, playerDeathProperties.dissolvingMaxValue);
        }

        private void SetPlayerMaterialValues()
        {
            playerMaterial.SetInt(Dead, 0);
            playerMaterial.SetFloat(DissolveAmount, playerDeathProperties.dissolvingMinValue);
            playerMaterial.SetFloat(DissolveGradientLight, playerDeathProperties.obscureMaxValue);
        }
    }
}