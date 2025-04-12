using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyPatrol : MonoBehaviour
    {
        [SerializeField] private List<Transform> patrolPoints;
        [Tooltip("How much distance to patrol point should the enemy be so it can stop")]
        [SerializeField] private float distanceToPatrolPoint = 0.5f;
        
        [Header("Idle seconds")]
        [SerializeField] private float minIdleSeconds = 1;
        [SerializeField] private float maxIdleSeconds = 4;
        
        private NavMeshAgent _navMeshAgent;

        private List<Vector3> _freezedPatrolPoints;
        private int _actualPatrolPointIndex;
        private Coroutine _idleCoroutine;
        void OnEnable()
        {
            _navMeshAgent ??= GetComponent<NavMeshAgent>();
            
            _freezedPatrolPoints = new List<Vector3>();
            foreach (var patrolPoint in patrolPoints)
            {
                _freezedPatrolPoints.Add(new Vector3(patrolPoint.position.x, patrolPoint.position.y, patrolPoint.position.z));
            }
            
            _actualPatrolPointIndex = 0;
        }

        private void Update()
        {
            _navMeshAgent.destination = _freezedPatrolPoints[_actualPatrolPointIndex];
            if ((transform.position - _freezedPatrolPoints[_actualPatrolPointIndex]).magnitude > distanceToPatrolPoint) return;
            
            _actualPatrolPointIndex++;
            if (_actualPatrolPointIndex >= _freezedPatrolPoints.Count) _actualPatrolPointIndex = 0;
            
            if(_idleCoroutine != null) StopCoroutine(_idleCoroutine);
            StartCoroutine(IdleCoroutine());
        }

        private IEnumerator IdleCoroutine()
        {
            _navMeshAgent.isStopped = true;
            yield return new WaitForSeconds(Random.Range(minIdleSeconds, maxIdleSeconds));
            _navMeshAgent.isStopped = false;
        }
    }
}
