using System;
using Events;
using Events.Scriptables;
using Player;
using UnityEngine;

namespace Managers
{
    public class WinTrigger : MonoBehaviour
    {
        [Header("Objects")] [SerializeField] private GameObject openDoor;

        [SerializeField] private string nextLevel;

        [Header("Events")] [SerializeField] private VoidEventChannelSO onEnemiesDisabled;
        [SerializeField] private VoidEventChannelSO onWinSequenceStart;
        [SerializeField] private VoidEventChannelSO onChangeLevel;

        [SerializeField] private Vector3ChannelSO onDoorPosition;
        [SerializeField] private StringEventChannelSO onLoadScene;

        private bool _canWin;
        private BoxCollider _collider;

        private void OnEnable()
        {
            _collider ??= GetComponent<BoxCollider>();
            _collider.isTrigger = false;
            gameObject.layer = LayerMask.NameToLayer("Wall");
            _canWin = false;
            onEnemiesDisabled?.onEvent.AddListener(HandleCanWin);
            onChangeLevel?.onEvent.AddListener(PlayNextLevel);
        }

        private void OnDisable()
        {
            onEnemiesDisabled?.onEvent.RemoveListener(HandleCanWin);
        }

        private void Update()
        {
            if (_canWin)
                onDoorPosition?.RaiseEvent(Camera.main.WorldToScreenPoint(transform.position));
        }

        private void HandleCanWin()
        {
            _collider.isTrigger = true;
            gameObject.layer = LayerMask.NameToLayer("WinDoor");
            openDoor.SetActive(true);
            _canWin = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && _canWin)
            {
                onWinSequenceStart?.RaiseEvent();

                if (other.gameObject.TryGetComponent<PlayerMovement>(out PlayerMovement playerMovement))
                    playerMovement.Stop();

                _canWin = false;
            }
        }

        private void PlayNextLevel()
        {
            onLoadScene?.RaiseEvent(nextLevel);
        }
    }
}