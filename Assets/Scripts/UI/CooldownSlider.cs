using System;
using Events.Scriptables;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CooldownSlider : MonoBehaviour
    {
        [SerializeField] private FloatEventChannel onChangeEvent;
        [SerializeField] private Slider slider;
        
        public void OnEnable()
        {
            slider.minValue = 0f;
            slider.maxValue = 1f;
            onChangeEvent?.onFloatEvent.AddListener(HandleNewValue);
        }

        public void OnDisable()
        {
            onChangeEvent?.onFloatEvent.RemoveListener(HandleNewValue);
        }

        private void HandleNewValue(float percentage)
        {
            slider.value = percentage;
        }
    }
}
