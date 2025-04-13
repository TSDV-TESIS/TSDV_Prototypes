using System.Collections;
using Player.Properties;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(EnemyAgent))]
    public class EnemyChaseController : MonoBehaviour
    {
        [SerializeField] private PlayerTransform playerTransform;
        [SerializeField] private float chaseSpeed = 7f;
        
        private EnemyAgent _enemyAgent;
        private NavMeshAgent _navMeshAgent;
        
        private Coroutine _enterCoroutine;
        void OnEnable()
        {
            _enemyAgent ??= GetComponent<EnemyAgent>();
            _navMeshAgent ??= GetComponent<NavMeshAgent>();
        }

        public void HandleEnter()
        {
            _navMeshAgent.destination = playerTransform.playerTransform.position;
            _navMeshAgent.speed = chaseSpeed;
        }

        public void HandleUpdate()
        {
            _navMeshAgent.destination = playerTransform.playerTransform.position;
        }
        
        public void HandleStartAttack()
        {
            _enemyAgent.ChangeStateToAttack();
        }
    }
}
