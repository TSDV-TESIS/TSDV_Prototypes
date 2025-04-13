using System.Collections;
using System.Collections.Generic;
using Enemy.Properties;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyPatrolController : MonoBehaviour
    {
        [SerializeField] private List<Transform> patrolPoints;
        [SerializeField] private EnemyPatrolProperties properties;
        
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

        public void HandleUpdate()
        {
            _navMeshAgent.destination = _freezedPatrolPoints[_actualPatrolPointIndex];
            if ((transform.position - _freezedPatrolPoints[_actualPatrolPointIndex]).magnitude > properties.distanceToPatrolPoint) return;
            
            _actualPatrolPointIndex++;
            if (_actualPatrolPointIndex >= _freezedPatrolPoints.Count) _actualPatrolPointIndex = 0;
            
            if(_idleCoroutine != null) StopCoroutine(_idleCoroutine);
            StartCoroutine(IdleCoroutine());
        }

        private IEnumerator IdleCoroutine()
        {
            _navMeshAgent.isStopped = true;
            yield return new WaitForSeconds(Random.Range(properties.minIdleSeconds, properties.maxIdleSeconds));
            _navMeshAgent.isStopped = false;
        }
    }
}
