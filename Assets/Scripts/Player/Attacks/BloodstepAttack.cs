using System;
using Events;
using Events.Scriptables;
using Health;
using UnityEngine;

namespace Player.Attacks
{
    public class BloodstepAttack : MonoBehaviour
    {
        [Header("Properties")] 
        [SerializeField] private BloodStepProperties bloodStepProperties;

        [Header("Events")]
        [SerializeField] private FloatEventChannel onHitStop;

        public void OnTriggerEnter(Collider other)
        {
            if (!this.enabled || other.CompareTag("Player"))
                return;

            if (other.transform.TryGetComponent<ITakeDamage>(out ITakeDamage takeDamageInterface))
            {
                takeDamageInterface.TryTakeDamage(bloodStepProperties.damage);
                onHitStop?.RaiseEvent(bloodStepProperties.hitStopSeconds);
            }
        }
    }
}
