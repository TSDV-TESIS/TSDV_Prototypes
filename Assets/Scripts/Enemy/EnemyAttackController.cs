using System;
using System.Collections;
using Player.Properties;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(EnemyAgent))]
    public class EnemyAttackController : MonoBehaviour
    {
        [SerializeField] private GameObject attackObject;
        [SerializeField] private PlayerTransform playerTransform;
        [SerializeField] private float enemySpeedInAttack = 2f;
        [SerializeField] private float secondsInAttack = 2f;
        
        private Coroutine _attackCoroutine;

        private NavMeshAgent _navMeshAgent;
        private EnemyAgent _enemyAgent;
        private void OnEnable()
        {
            _navMeshAgent ??= GetComponent<NavMeshAgent>();
            _enemyAgent ??= GetComponent<EnemyAgent>();
        }

        public void HandleEnter()
        {
            if(_attackCoroutine != null) StopCoroutine(_attackCoroutine);
            StartCoroutine(AttackCoroutine());
        }

        private IEnumerator AttackCoroutine()
        {
            attackObject.SetActive(true);
            _navMeshAgent.destination = playerTransform.playerTransform.position;
            _navMeshAgent.speed = enemySpeedInAttack;

            float timeDiff = 0;
            while (timeDiff < secondsInAttack)
            {
                _navMeshAgent.destination = playerTransform.playerTransform.position;
                timeDiff += Time.deltaTime;
                yield return null;
            }
            
            attackObject.SetActive(false);
            _enemyAgent.ChangeStateToChase();
        }
    }
}
