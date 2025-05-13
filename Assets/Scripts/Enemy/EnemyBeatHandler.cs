using Events;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Enemy
{
    public class EnemyBeatHandler : MonoBehaviour
    {
        [Header("Events")] 
        [SerializeField] private VoidEventChannelSO onBloodlustStart;
        [SerializeField] private VoidEventChannelSO onBloodlustEnd;
        [SerializeField] private VoidEventChannelSO onHeartbeatStart;
        [SerializeField] private VoidEventChannelSO onHeartbeatEnd;

        [System.NonSerialized] public bool IsInBloodlust;
        [System.NonSerialized] public bool IsInHeartBeat;
        private void OnEnable()
        {
            IsInBloodlust = false;
            onBloodlustStart?.onEvent.AddListener(HandleBloodlustOn);
            onBloodlustEnd?.onEvent.AddListener(HandleBloodlustOff);
            onHeartbeatStart?.onEvent.AddListener(HandleHeartbeatOn);
            onHeartbeatEnd?.onEvent.AddListener(HandleHeartbeatOff);
        }

        private void OnDisable()
        {
            onBloodlustStart?.onEvent.RemoveListener(HandleBloodlustOn);
            onBloodlustEnd?.onEvent.RemoveListener(HandleBloodlustOff);
            onHeartbeatStart?.onEvent.RemoveListener(HandleHeartbeatOn);
            onHeartbeatEnd?.onEvent.RemoveListener(HandleHeartbeatOff);
        }
        private void HandleHeartbeatOff()
        {
            IsInHeartBeat = false;
        }

        private void HandleHeartbeatOn()
        {
            IsInHeartBeat = true;
        }

        private void HandleBloodlustOff()
        {
            IsInBloodlust = false;
        }

        private void HandleBloodlustOn()
        {
            IsInBloodlust = true;
        }

    }
}
