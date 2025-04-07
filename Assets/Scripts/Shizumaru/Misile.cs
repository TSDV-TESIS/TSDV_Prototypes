using System.Collections;
using Health;
using UnityEngine;

namespace Shizumaru
{
    public class Misile : MonoBehaviour
    {
        [Header("Misile Movement")]
        [SerializeField] private float fullVelocity = 10;
        [SerializeField] private AnimationCurve acceleration;
        [SerializeField] private float timeUntilFullVelocity = 3f;
        [SerializeField] private float secondsUntilDestroy = 6f;
    
        [Header("Misile Properties")]
        [SerializeField] private int damage;
    
        private Coroutine _disappearCoroutine;
        private Coroutine _accelerationCoroutine;
        private float _velocity;
        private void OnEnable()
        {
            if(_disappearCoroutine != null) StopCoroutine(_disappearCoroutine);
            _disappearCoroutine = StartCoroutine(DestroyCoroutine());
        
            _velocity = acceleration.Evaluate(0) * fullVelocity;
        
            if(_accelerationCoroutine != null) StopCoroutine(_accelerationCoroutine);
            _accelerationCoroutine = StartCoroutine(AccelerateCoroutine());
        }
    
        private void Update()
        {
            gameObject.transform.position += Vector3.right * (_velocity * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.TryGetComponent<ITakeDamage>(out ITakeDamage takeDamageInterface))
            {
                takeDamageInterface.TryTakeDamage(damage);
                if(_disappearCoroutine != null) StopCoroutine(_disappearCoroutine);
                Destroy(gameObject);
            }
        }

        private IEnumerator AccelerateCoroutine()
        {
            float timeAccelerating = Time.time;
            while (Time.time - timeAccelerating < timeUntilFullVelocity)
            {
                _velocity = acceleration.Evaluate((Time.time - timeAccelerating) / timeUntilFullVelocity) * fullVelocity;
                Debug.Log(_velocity);
                yield return null;
            }
        }

        private IEnumerator DestroyCoroutine()
        {
            yield return new WaitForSeconds(secondsUntilDestroy);
            Destroy(gameObject);
        }
    }
}
