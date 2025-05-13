using System.Collections.Generic;
using Enemy;
using Events;
using Events.Scriptables;
using Health;
using UnityEngine;

namespace Player.Attacks
{
    public class MeleeWeapon : MonoBehaviour
    {
        [Header("Damage properties")]
        [SerializeField] private int damage;

        [Header("Events")] [SerializeField] private VoidEventChannelSO onFrenziedEvent;
        [SerializeField] private AkWwiseEventChannelSO onPlayEvent;
        [SerializeField] private AK.Wwise.Event decapitationEvent;    
        
        private readonly List<Collider> _hittedEnemies = new List<Collider>();

        private void OnDisable()
        {
            ResetHittedEnemiesBuffer();
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
                    onPlayEvent?.RaiseEvent(decapitationEvent);
                    onFrenziedEvent?.RaiseEvent();
                }
            }
        }

        public void ResetHittedEnemiesBuffer()
        {
            _hittedEnemies.Clear();
        }
    }
}