using System;
using Events;
using UnityEngine;

namespace UI
{
    public class UIPanelEnabler : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private VoidEventChannelSO enabler;
        [SerializeField] private VoidEventChannelSO disabler;

        private void OnEnable()
        {
            enabler?.onEvent.AddListener(HandleEnable);
            disabler?.onEvent.AddListener(HandleDisable);
        }

        private void OnDisable()
        {
            enabler?.onEvent.RemoveListener(HandleEnable);
            disabler?.onEvent.RemoveListener(HandleDisable);
        }

        private void HandleEnable()
        {
            panel.SetActive(true);
        }

        private void HandleDisable()
        {
            panel.SetActive(false);
        }
    }
}
