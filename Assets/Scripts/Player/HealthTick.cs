using System.Collections;
using Health;
using Player.Properties;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(HealthPoints))]
    public class HealthTick : MonoBehaviour
    {
        [SerializeField] private HealthTickProperties healthTickProperties;
        
        private HealthPoints _healthPoints;
        private Coroutine _tickCoroutine;
        void OnEnable()
        {
            _healthPoints ??= GetComponent<HealthPoints>();
        
            if(_tickCoroutine != null) StopCoroutine(_tickCoroutine);
            StartCoroutine(Tick());
        }

        private void OnDisable()
        {
            if(_tickCoroutine != null) StopCoroutine(_tickCoroutine);
        }

        private IEnumerator Tick()
        {
            while (healthTickProperties.shouldTick)
            {
                yield return new WaitForSeconds(healthTickProperties.secondsPerTick);
                _healthPoints.TryTakeDamage(healthTickProperties.healthTakenPerTick);
                
            }
        }
    }
}
