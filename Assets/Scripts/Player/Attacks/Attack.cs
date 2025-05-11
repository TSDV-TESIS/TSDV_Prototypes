using System.Collections;
using Events;
using Health;
using UnityEngine;

namespace Player
{
    public class Attack : MonoBehaviour
    {
        [SerializeField] private GameObject attackObject;
        [SerializeField] private InputHandler handler;
        
        [Header("Attack properties")] 
        [SerializeField] private float attackDurationSeconds = 0.5f;
        [SerializeField] private float attackDurationCooldownSeconds = 0.2f;

        private Coroutine _attackCoroutine;
        private bool _isAttacking;
    
        void OnEnable()
        {
            handler.OnPlayerAttack.AddListener(HandleAttack);
        }

        private void OnDisable()
        {
            handler.OnPlayerAttack.RemoveListener(HandleAttack);
        }

        private void HandleAttack()
        {
            if (_isAttacking) return;
        
            if(_attackCoroutine != null) StopCoroutine(_attackCoroutine);

            _attackCoroutine = StartCoroutine(HandleAttackCoroutine());
        }

        private IEnumerator HandleAttackCoroutine()
        {
            _isAttacking = true;
            attackObject.SetActive(true);
            yield return new WaitForSeconds(attackDurationSeconds);
        
            attackObject.SetActive(false);
            yield return new WaitForSeconds(attackDurationCooldownSeconds);
            _isAttacking = false;
        }
    }
}
