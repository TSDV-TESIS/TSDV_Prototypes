using System.Collections;
using System.Collections.Generic;
using Enemy.Properties;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(EnemyAgent))]
    [RequireComponent(typeof(VisionHandler))]
    public class EnemyPatrolController : MonoBehaviour
    {
        [SerializeField] private List<Transform> patrolPoints;
        [SerializeField] private EnemyPatrolProperties properties;
        [SerializeField] private GameObject seenUiObject;
        [SerializeField] private float seenExclamationSeconds;
        
        private NavMeshAgent _navMeshAgent;
        private EnemyAgent _enemyAgent;
        private VisionHandler _visionHandler;
        
        private List<Vector3> _freezedPatrolPoints;
        private int _actualPatrolPointIndex;
        private Coroutine _idleCoroutine;
        private Coroutine _surprisedCoroutine;
        void OnEnable()
        {
            _navMeshAgent ??= GetComponent<NavMeshAgent>();
            _enemyAgent ??= GetComponent<EnemyAgent>();
            _visionHandler ??= GetComponent<VisionHandler>();
            
            _freezedPatrolPoints = new List<Vector3>();
            foreach (var patrolPoint in patrolPoints)
            {
                _freezedPatrolPoints.Add(new Vector3(patrolPoint.position.x, patrolPoint.position.y, patrolPoint.position.z));
            }
            
            _actualPatrolPointIndex = 0;
        }

        public void HandleEnter()
        {
            if(_surprisedCoroutine != null) StopCoroutine(_surprisedCoroutine);
            if(_idleCoroutine != null) StopCoroutine(_idleCoroutine);
        }
        
        public void HandleUpdate()
        {
              if (_visionHandler.CanSeeObjective())
              {
                 Debug.Log("SEEN!");
                 _enemyAgent.ChangeStateToChase();
                 return;
              }
            
              _navMeshAgent.destination = _freezedPatrolPoints[_actualPatrolPointIndex];
              if ((transform.position - _freezedPatrolPoints[_actualPatrolPointIndex]).magnitude > properties.distanceToPatrolPoint) return;
            
              _actualPatrolPointIndex++;
              if (_actualPatrolPointIndex >= _freezedPatrolPoints.Count) _actualPatrolPointIndex = 0;
            
              if(_idleCoroutine != null) StopCoroutine(_idleCoroutine);
              StartCoroutine(IdleCoroutine());
        }

        public void HandleExit()
        {
            if(_idleCoroutine != null) StopCoroutine(_idleCoroutine);
            
            if(_surprisedCoroutine != null) StopCoroutine(_surprisedCoroutine);
            StartCoroutine(SurprisedCoroutine());
        }

        private IEnumerator SurprisedCoroutine()
        {
            _navMeshAgent.isStopped = true;
            seenUiObject.SetActive(true);
            yield return new WaitForSeconds(seenExclamationSeconds);
            seenUiObject.SetActive(false);
            _navMeshAgent.isStopped = false;
        }

        private IEnumerator IdleCoroutine()
        {
            _navMeshAgent.isStopped = true;
            yield return new WaitForSeconds(Random.Range(properties.minIdleSeconds, properties.maxIdleSeconds));
            _navMeshAgent.isStopped = false;
        }
    }
}
