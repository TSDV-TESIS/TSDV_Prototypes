using System.Collections;
using Events.Scriptables;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class TimedSlider : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private FloatEventChannel onStartCountDown;
        [SerializeField] private bool shouldEnableDisable;

        private Coroutine _CountDown;

        private void OnEnable()
        {
            onStartCountDown.onFloatEvent.AddListener(HandleStartCountDown);
        }

        private void OnDisable()
        {
            onStartCountDown.onFloatEvent.RemoveListener(HandleStartCountDown);
        }

        void HandleStartCountDown(float duration)
        {
            if (_CountDown != null)
                StopCoroutine(_CountDown);

            _CountDown = StartCoroutine(CountDown(duration));
        }

        private IEnumerator CountDown(float duration)
        {
            float startTime = Time.time;
            float timer = 0;
            slider.gameObject.SetActive(true);
            while (timer < duration)
            {
                timer = Time.time - startTime;
                slider.value = timer / duration;
                yield return null;
            }

            if (shouldEnableDisable)
                slider.gameObject.SetActive(false);
        }
    }
}