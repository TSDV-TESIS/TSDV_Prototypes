using System;
using System.Collections;
using UnityEngine;

public class Misile : MonoBehaviour
{
    [SerializeField] private float fullVelocity = 10;
    [SerializeField] private AnimationCurve acceleration;
    [SerializeField] private float timeUntilFullVelocity = 3f;
    [SerializeField] private float secondsUntilDestroy = 6f;
    
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

    void Update()
    {
        gameObject.transform.position += Vector3.right * _velocity * Time.deltaTime;
    }
}
