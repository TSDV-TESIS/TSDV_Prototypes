using System;
using System.Collections;
using Events;
using Events.Scriptables;
using Health;
using Player.Properties;
using UnityEngine;

namespace Player.Health
{
    [RequireComponent(typeof(HealthPoints))]
    public class HealthShaderManager : MonoBehaviour
    {
        private static readonly int BreathFrequencyParam = Shader.PropertyToID("_BreathFrequency");
        private static readonly int BreathIntensityParam = Shader.PropertyToID("_BreathIntensity");
        private static readonly int NoiseSpeedParam = Shader.PropertyToID("_Noise_Speed");
        private static readonly int VignetteIntensityParam = Shader.PropertyToID("_VignetteIntensity");
        private static readonly int VignettePowerParam = Shader.PropertyToID("_VignettePower");
        private static readonly int ColorBaseParam = Shader.PropertyToID("_Color_base");

        // ---- ID PARA NUESTRA NUEVA VARIABLE DEL SHADER ----
        private static readonly int EffectIntensityParam = Shader.PropertyToID("_EffectIntensity");

        [Header("Properties")][SerializeField] private LowHealthShaderProperties shaderProperties;
        [SerializeField] private Material lowHealthShaderMaterial;

        [Header("Events")][SerializeField] private IntEventChannelSO onTakeDamage;
        [SerializeField] private DeathEventChannelSO onPlayerDeath;
        [SerializeField] private IntEventChannelSO onSumHealth;
        [SerializeField] private VoidEventChannelSO onEnemiesDied;

        private HealthPoints _healthPoints;
        private float _lastHealthPercentage;
        private Coroutine _updateShaderCoroutine;

        private void OnEnable()
        {
            _healthPoints ??= GetComponent<HealthPoints>();
            _lastHealthPercentage = 0f;

            ResetShaderValues();

            onTakeDamage?.onIntEvent.AddListener(UpdateShaderValues);
            onSumHealth?.onIntEvent.AddListener(UpdateOnSumHealth);
            onPlayerDeath?.onTypedEvent.AddListener(StopShader);
            onEnemiesDied?.onEvent.AddListener(ResetShaderValues);
        }

        private void OnDisable()
        {
            onTakeDamage?.onIntEvent.RemoveListener(UpdateShaderValues);
            onSumHealth?.onIntEvent.RemoveListener(UpdateOnSumHealth);
            onPlayerDeath?.onTypedEvent.RemoveListener(StopShader);
            onEnemiesDied?.onEvent.RemoveListener(ResetShaderValues);
            ResetShaderValues();
        }

        private void UpdateOnSumHealth(int newHealth)
        {
            if (_updateShaderCoroutine != null) StopCoroutine(_updateShaderCoroutine);
            float healthPercentageLeft = (float)_healthPoints.CurrentHp / _healthPoints.MaxHealth;
            float animationCurvePart = 1 - healthPercentageLeft;

            UpdateIntensityValue(animationCurvePart);

            _lastHealthPercentage = animationCurvePart;
        }

        private void StopShader(DeathCauses cause)
        {
            if (_updateShaderCoroutine != null) StopCoroutine(_updateShaderCoroutine);
            ResetShaderValues();
            lowHealthShaderMaterial.SetFloat(BreathFrequencyParam, 0f);
        }

        private void ResetShaderValues()
        {
            lowHealthShaderMaterial.SetFloat(VignettePowerParam, shaderProperties.vignettePower);
            lowHealthShaderMaterial.SetFloat(VignetteIntensityParam, shaderProperties.vignetteIntensityCurve.Evaluate(0) * shaderProperties.vignetteIntensity);
            lowHealthShaderMaterial.SetColor(ColorBaseParam, shaderProperties.baseColor);
            lowHealthShaderMaterial.SetVector(NoiseSpeedParam, shaderProperties.noiseSpeed);
            lowHealthShaderMaterial.SetFloat(BreathFrequencyParam, shaderProperties.breathFrequency);
            lowHealthShaderMaterial.SetFloat(BreathIntensityParam, shaderProperties.breathIntensity);

            // Al resetear, apagamos la opacidad del efecto por defecto
            lowHealthShaderMaterial.SetFloat(EffectIntensityParam, 0f);
        }

        private void UpdateShaderValues(int newHealth)
        {
            float healthPercentageLeft = (float)newHealth / _healthPoints.MaxHealth;
            float animationCurvePart = 1 - healthPercentageLeft;
            if (Math.Abs(_lastHealthPercentage - animationCurvePart) < shaderProperties.animationChangePercentage) return;

            if (_updateShaderCoroutine != null) StopCoroutine(_updateShaderCoroutine);
            _updateShaderCoroutine = StartCoroutine(UpdateShaderCoroutine(_lastHealthPercentage, animationCurvePart));

            _lastHealthPercentage = animationCurvePart;
        }

        private IEnumerator UpdateShaderCoroutine(float animationStart, float animationEnd)
        {
            float timeUpdating = 0f;

            while (timeUpdating < shaderProperties.animationTime)
            {
                float timePart = shaderProperties.updateCurve.Evaluate(timeUpdating / shaderProperties.animationTime);

                float animationCurvePart = Mathf.Lerp(animationStart, animationEnd, timePart);
                UpdateIntensityValue(animationCurvePart);

                timeUpdating += Time.deltaTime;
                yield return null;
            }
        }

        private void UpdateIntensityValue(float animationCurvePart)
        {
 
            float currentHealthPercent = 1f - animationCurvePart;
            float targetOpacity = Mathf.InverseLerp(0.67f, 0.23f, currentHealthPercent);
            targetOpacity = Mathf.Clamp01(targetOpacity);
            lowHealthShaderMaterial.SetFloat(EffectIntensityParam, targetOpacity);

            float intensity = shaderProperties.vignetteIntensity *
                              shaderProperties.vignetteIntensityCurve.Evaluate(animationCurvePart);

            lowHealthShaderMaterial.SetFloat(VignetteIntensityParam, intensity);
        }
    }
}