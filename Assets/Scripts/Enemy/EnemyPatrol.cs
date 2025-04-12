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
        [SerializeField] private float idleSeconds;
        [Tooltip("How much distance to patrol point should the enemy be so it can stop")]
        [SerializeField] private float distanceToPatrolPoint = 0.5f;
        
        private NavMeshAgent _navMeshAgent;

        private List<Vector3> _freezedPatrolPoints;
        private int _actualPatrolPointIndex;
        private bool _shouldPatrol;
        private Coroutine _idleCoroutine;
        void OnEnable()
        {
            _shouldPatrol = true;
            _navMeshAgent ??= GetComponent<NavMeshAgent>();
            
            _freezedPatrolPoints = new List<Vector3>();
            foreach (var patrolPoint in patrolPoints)
            {
                _freezedPatrolPoints.Add(new Vector3(patrolPoint.position.x, patrolPoint.position.y, patrolPoint.position.z));
            }

            Debug.Log(_freezedPatrolPoints[0]);
            _actualPatrolPointIndex = 0;
        }

        private void Update()
        {
            _navMeshAgent.destination = _freezedPatrolPoints[_actualPatrolPointIndex];
            if ((transform.position - _freezedPatrolPoints[_actualPatrolPointIndex]).magnitude > distanceToPatrolPoint) return;

            _actualPatrolPointIndex++;
            if (_actualPatrolPointIndex >= _freezedPatrolPoints.Count) _actualPatrolPointIndex = 0;
        }
    }
}
