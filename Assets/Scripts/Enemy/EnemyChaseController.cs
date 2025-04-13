using System.Collections;
using Player.Properties;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(EnemyAgent))]
    [RequireComponent(typeof(VisionHandler))]
    public class EnemyChaseController : MonoBehaviour
    {
        [SerializeField] private PlayerTransform playerTransform;
        [SerializeField] private float chaseSpeed = 7f;
        [SerializeField] private float maxChaseSeconds = 3f;
        [SerializeField] private GameObject questionMarkUi;
        [SerializeField] private float secondsQuestionMark = 0.75f;
        
        private EnemyAgent _enemyAgent;
        private NavMeshAgent _navMeshAgent;
        private VisionHandler _visionHandler;
        
        private Coroutine _visionCoroutine;
        private Coroutine _questionMarkCoroutine;

        private bool _shouldCheckVision;
        private bool _lostSight;
        void OnEnable()
        {
            _lostSight = false;
            _shouldCheckVision = false;
            _visionHandler ??= GetComponent<VisionHandler>();
            _enemyAgent ??= GetComponent<EnemyAgent>();
            _navMeshAgent ??= GetComponent<NavMeshAgent>();
        }

        public void HandleEnter()
        {
            _shouldCheckVision = true;
            _lostSight = false;
            _navMeshAgent.destination = playerTransform.playerTransform.position;
            _navMeshAgent.speed = chaseSpeed;
            
            if(_visionCoroutine != null) StopCoroutine(_visionCoroutine);
            StartCoroutine(VisionCoroutine());
        }

        public void HandleUpdate()
        {
            _navMeshAgent.destination = playerTransform.playerTransform.position;
        }
        
        public void HandleStartAttack()
        {
            _shouldCheckVision = false;
            _lostSight = false;
            _enemyAgent.ChangeStateToAttack();
        }

        public void HandleExit()
        {
            _shouldCheckVision = false;
            if (_lostSight)
            {
                if (_questionMarkCoroutine != null) StopCoroutine(_questionMarkCoroutine);
                _questionMarkCoroutine = StartCoroutine(QuestionMarkCoroutine());
                _lostSight = false;
            }
        }

        private IEnumerator QuestionMarkCoroutine()
        {
            questionMarkUi.SetActive(true);
            yield return new WaitForSeconds(secondsQuestionMark);
            questionMarkUi.SetActive(false);
        }

        private IEnumerator VisionCoroutine()
        {
            while (_shouldCheckVision)
            {
                yield return new WaitForSeconds(maxChaseSeconds);
                if (_shouldCheckVision)
                {
                    if (!_visionHandler.CanSeeObjective())
                    {
                        _lostSight = true;
                        _enemyAgent.ChangeStateToPatrol();
                    }
                }
                else
                {
                    break;
                }
            } 
        }
    }
}
