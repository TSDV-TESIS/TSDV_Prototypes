using UnityEngine;

namespace Player.Properties
{
    [CreateAssetMenu(fileName = "LowHealthShaderProperties", menuName = "Scriptable Objects/LowHealthShaderProperties")]
    public class LowHealthShaderProperties : ScriptableObject
    {
        [Header("Default values")]
        public float vignettePower;

        [ColorUsage(true, true)]
        public Color baseColor;
        public float vignetteIntensity;
        public Vector2 noiseSpeed;
        public float breathFrequency;
        public float breathIntensity;


        [Range(0f, 1f)] public float effectIntensity;

        [Header("Animation per health")]
        public AnimationCurve vignetteIntensityCurve;
        [Range(0, 1)] public float animationChangePercentage = 0.1f;

        [Header("Update animation values")]
        public AnimationCurve updateCurve;
        public float animationTime;
    }
}