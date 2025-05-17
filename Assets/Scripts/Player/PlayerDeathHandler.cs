using Events;
using Health;
using UnityEngine;

namespace Player
{
    public class PlayerDeathHandler : MonoBehaviour
    {
        [SerializeField] private VoidEventChannelSO onPlayerDeath;
        [SerializeField] private HealthPoints healthPoints;
        [SerializeField] private PlayerAgent agent;
        [SerializeField] private float waitSeconds = 0.5f;
    
        private Vector3 _startPosition;
        void Start()
        {
            _startPosition = gameObject.transform.position;
        }

        private void OnEnable()
        {
            onPlayerDeath?.onEvent?.AddListener(HandleDeath);
        }

        private void OnDisable()
        {
            onPlayerDeath?.onEvent.RemoveListener(HandleDeath);
        }

        private void HandleDeath()
        {
            healthPoints.ResetHitPoints();

            gameObject.transform.position = _startPosition;
            agent.StopFsm();
        }
    }
}
