using System;
using Events;
using Events.Scriptables;
using UnityEngine;

namespace Managers
{
    public class WinTrigger : MonoBehaviour
    {
        [Header("Objects")] 
        [SerializeField] private GameObject closedDoor;
        [SerializeField] private GameObject openDoor;
        
        [SerializeField] private string nextLevel;
        
        [Header("Events")]
        [SerializeField] private VoidEventChannelSO onEnemiesDisabled;

        [SerializeField] private Vector3ChannelSO onDoorPosition;
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

        private void Update()
        {
            if(_canWin)
                onDoorPosition?.RaiseEvent(Camera.main.WorldToScreenPoint(transform.position));
        }

        private void HandleCanWin()
        {
            closedDoor.SetActive(false);
            openDoor.SetActive(true);
            _canWin = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log($"Check if can win {other.CompareTag("Player")} {_canWin}");
            if (other.CompareTag("Player") && _canWin)
            {
                Debug.Log("TRIGGERED!");
                onLoadScene?.RaiseEvent(nextLevel);
                _canWin = false;
            } 
        }
    }
}
