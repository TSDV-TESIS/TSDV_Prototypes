using System;
using System.Collections.Generic;
using Health;
using UnityEngine;

namespace Player.Weapon
{
    public class MeleeWeapon : MonoBehaviour
    {
        [SerializeField] private int damage;

        private List<Collider> _hittedEnemies = new List<Collider>();
        
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