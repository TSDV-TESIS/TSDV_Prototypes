using System;
using Health;
using UnityEngine;

namespace Enemy.Attack
{
    public class EnemyAttack : MonoBehaviour
    {
        [SerializeField] private int damage = 10;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && other.TryGetComponent<ITakeDamage>(out ITakeDamage takeDamageObject))
            {
                takeDamageObject.TryTakeDamage(damage);
            }
        }
    }
}
