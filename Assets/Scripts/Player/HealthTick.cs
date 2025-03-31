using System;
using System.Collections;
using Health;
using UnityEngine;

[RequireComponent(typeof(HealthPoints))]
public class HealthTick : MonoBehaviour
{
    [SerializeField] private int healthTakenPerTick = 15;
    [SerializeField] private float secondsPerTick = 2f;
    [SerializeField] private bool shouldTick = true;
    
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
        while (shouldTick)
        {
            yield return new WaitForSeconds(secondsPerTick);
            _healthPoints.TryTakeDamage(healthTakenPerTick);
        }
    }
}
