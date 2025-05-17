using System.Collections;
using Events.Scriptables;
using UnityEngine;

namespace TimeScripts
{
    public class HitStop : MonoBehaviour
    {
        [Header("Events")]
        
        [SerializeField] private FloatEventChannel onHitStop;

        [SerializeField] private float reducedTimeInHit = 0.1f;
        
        private Coroutine _hitStopCoroutine;
        
        private void OnEnable()
        {
            onHitStop?.onFloatEvent.AddListener(HandleHitStop);
        }

        private void OnDisable()
        {
            onHitStop?.onFloatEvent.RemoveListener(HandleHitStop);
        }

        private void HandleHitStop(float hitStopTime)
        {
            if(_hitStopCoroutine != null) StopCoroutine(_hitStopCoroutine);
            StartCoroutine(HitStopCoroutine(hitStopTime));
        }
        
        private IEnumerator HitStopCoroutine(float hitStopTime)
        {
            float timer = 0;
            Time.timeScale = reducedTimeInHit;
            while (timer < hitStopTime)
            {
                timer += Time.unscaledDeltaTime;
                yield return null;
            }

            Time.timeScale = 1;
        }
    }
}
