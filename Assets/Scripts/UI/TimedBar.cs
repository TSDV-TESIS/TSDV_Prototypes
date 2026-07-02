using System.Collections;
using Events;
using Player.Properties;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TimedBar : MonoBehaviour
    {
        [SerializeField] private Slider timeSlider;
        [SerializeField] private HealthTickProperties tickProperties;

        [Header("Events")] [SerializeField] private IntEventChannelSO onTakeDamage;
        [SerializeField] private IntEventChannelSO onSumHealth;
        [SerializeField] private IntEventChannelSO onInitializeSlider;

        private bool _wasTriggered = false;
        private int _maxHealthValue;
        private static readonly int HealthParam = Shader.PropertyToID("_Health");

        private Coroutine _tickCoroutine;

        private void Awake()
        {
            onInitializeSlider?.onIntEvent.AddListener(HandleInit);
        }

        void Start()
        {
            timeSlider.value = 1f;
            onSumHealth?.onIntEvent.AddListener(HandleTakeDamage);
            onTakeDamage?.onIntEvent.AddListener(HandleTakeDamage);
        }

        private void OnDestroy()
        {
            onSumHealth?.onIntEvent?.RemoveListener(HandleTakeDamage);
            onTakeDamage?.onIntEvent.RemoveListener(HandleTakeDamage);
            onInitializeSlider?.onIntEvent.RemoveListener(HandleInit);
        }


        public void HandleInit(int maxValue)
        {
            _maxHealthValue = maxValue;
            timeSlider.value = 1;
        }

        public void HandleTakeDamage(int currentHealth)
        {
            if (!_wasTriggered)
            {
                _wasTriggered = true;
            }

            if (_tickCoroutine != null) StopCoroutine(_tickCoroutine);
            _tickCoroutine = StartCoroutine(TickBar(currentHealth));
        }

        private IEnumerator TickBar(int currentHealth)
        {
            float timePassed = 0f;
            float maxValue = timeSlider.value;
            float nextValue = (float)currentHealth / _maxHealthValue;

            while (timePassed < tickProperties.secondsPerTick)
            {
                timeSlider.value = Mathf.Lerp(maxValue, nextValue, timePassed / tickProperties.secondsPerTick);

                timePassed += Time.deltaTime;
                yield return null;
            }

            timeSlider.value = nextValue;
        }
    }
}