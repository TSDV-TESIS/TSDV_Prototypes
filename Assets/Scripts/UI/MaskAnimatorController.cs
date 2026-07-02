using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MaskAnimatorController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Slider timeSlider; 
        [SerializeField] private Animator maskAnimator; 
        [SerializeField] private Image flameImage; 

        [Header("Flame Settings")]
        [Range(0f, 1f)]
        [SerializeField] private float minHealthThreshold = 0.3f; 

        private Material _flameMaterial;
        private static readonly int HealthPercentHash = Animator.StringToHash("HealthPercent");
        private static readonly int ShaderHealthHash = Shader.PropertyToID("_Health");

        void Start()
        {

            if (timeSlider == null) timeSlider = GetComponent<Slider>();
            if (maskAnimator == null) maskAnimator = GetComponent<Animator>();

            if (flameImage != null)
            {
                _flameMaterial = flameImage.material;
            }
        }

        void Update()
        {
            if (timeSlider == null) return;

            float currentHealth = timeSlider.value; 

            if (maskAnimator != null)
            {
                maskAnimator.SetFloat(HealthPercentHash, currentHealth);
            }

            if (_flameMaterial != null)
            {
                float clampedFlameHealth = Mathf.Lerp(minHealthThreshold, 1f, currentHealth);

                _flameMaterial.SetFloat(ShaderHealthHash, clampedFlameHealth);
            }
        }
    }
}