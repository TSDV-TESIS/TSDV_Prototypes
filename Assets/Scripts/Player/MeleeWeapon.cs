using System;
using System.Collections.Generic;
using Enemy;
using Events;
using Health;
using UnityEngine;

namespace Player.Weapon
{
    public class MeleeWeapon : MonoBehaviour
    {
        [Header("Damage properties")]
        [SerializeField] private int damage;

        [Header("Events")] 
        [SerializeField] private VoidEventChannelSO onBloodlustStart;
        [SerializeField] private VoidEventChannelSO onBloodlustEnd;
        [SerializeField] private VoidEventChannelSO onHeartbeatStart;
        [SerializeField] private VoidEventChannelSO onHeartbeatEnd;
        [SerializeField] private AK.Wwise.Event decapitationEvent;    
        
        private List<Collider> _hittedEnemies = new List<Collider>();

        private bool _isInBloodlust;
        private bool _isInHeartbeat;
        private void OnEnable()
        {
            _isInBloodlust = false;
            onBloodlustStart?.onEvent.AddListener(HandleBloodlustOn);
            onBloodlustEnd?.onEvent.AddListener(HandleBloodlustOff);
            onHeartbeatStart?.onEvent.AddListener(HandleHeartbeatOn);
            onHeartbeatEnd?.onEvent.AddListener(HandleHeartbeatOff);
        }

        private void OnDisable()
        {
            ResetHittedEnemiesBuffer();
            onBloodlustStart?.onEvent.RemoveListener(HandleBloodlustOn);
            onBloodlustEnd?.onEvent.RemoveListener(HandleBloodlustOff);
            onHeartbeatStart?.onEvent.RemoveListener(HandleHeartbeatOn);
            onHeartbeatEnd?.onEvent.RemoveListener(HandleHeartbeatOff);
        }

        private void HandleHeartbeatOff()
        {
            _isInHeartbeat = false;
        }

        private void HandleHeartbeatOn()
        {
            _isInHeartbeat = true;
        }

        private void HandleBloodlustOff()
        {
            _isInBloodlust = false;
        }

        private void HandleBloodlustOn()
        {
            _isInBloodlust = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!this.enabled || _hittedEnemies.Contains(other) || other.CompareTag("Player"))
                return;
            
            if (other.transform.TryGetComponent<ITakeDamage>(out ITakeDamage takeDamageInterface))
            {
                takeDamageInterface.TryTakeDamage(damage);
                _hittedEnemies.Add(other);

                if (other.gameObject.TryGetComponent<EnemyBeatHandler>(out EnemyBeatHandler enemyBeatHandler) && enemyBeatHandler.IsInHeartBeat && enemyBeatHandler.IsInBloodlust)
                {
                    Debug.Log("HERE!!!!!!");
                    decapitationEvent.Post(gameObject);
                }
            }
        }
        
        public void SetIsInteractive(bool value)
        {
            gameObject.SetActive(value);
        }

        public void ResetHittedEnemiesBuffer()
        {
            _hittedEnemies.Clear();
        }
    }
}