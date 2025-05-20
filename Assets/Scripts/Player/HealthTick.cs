using System.Collections;
using Events;
using Health;
using Player.Properties;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(HealthPoints))]
    public class HealthTick : MonoBehaviour
    {
        [SerializeField] private HealthTickProperties healthTickProperties;

        [SerializeField] private VoidEventChannelSO onEnemiesDisabled;

        private HealthPoints _healthPoints;
        private Coroutine _tickCoroutine;

        private bool _shouldTick;

        void OnEnable()
        {
            _healthPoints ??= GetComponent<HealthPoints>();
            onEnemiesDisabled.onEvent.AddListener(DisableTick);

            _shouldTick = healthTickProperties.shouldTick;
            if (_tickCoroutine != null) StopCoroutine(_tickCoroutine);
            StartCoroutine(Tick());
        }

        private void OnDisable()
        {
            if (_tickCoroutine != null) StopCoroutine(_tickCoroutine);
        }

        private IEnumerator Tick()
        {
            while (_shouldTick)
            {
                yield return new WaitForSeconds(healthTickProperties.secondsPerTick);
                if (_shouldTick)
                    _healthPoints.TryTakeDamage(healthTickProperties.healthTakenPerTick);
            }
        }

        private void DisableTick()
        {
            Debug.Log("DISABLE TICK");
            _shouldTick = false;
            if (_tickCoroutine != null)
                StopCoroutine(_tickCoroutine);
        }
    }
}