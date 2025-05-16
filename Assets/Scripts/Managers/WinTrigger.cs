using System;
using Events;
using Events.Scriptables;
using UnityEngine;

namespace Managers
{
    public class WinTrigger : MonoBehaviour
    {
        [SerializeField] private string nextLevel;
        
        [Header("Events")]
        [SerializeField] private VoidEventChannelSO onEnemiesDisabled;
        [SerializeField] private StringEventChannelSO onLoadScene;
        
        private bool _canWin;

        private void OnEnable()
        {
            _canWin = false;
            onEnemiesDisabled?.onEvent.AddListener(HandleCanWin);
        }

        private void OnDisable()
        {
            onEnemiesDisabled?.onEvent.RemoveListener(HandleCanWin);
        }

        private void HandleCanWin()
        {
            _canWin = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log($"here? {other.CompareTag("Player")} {_canWin}");
            if (other.CompareTag("Player") && _canWin)
            {
                onLoadScene?.RaiseEvent(nextLevel);
                _canWin = false;
            } 
        }
    }
}
