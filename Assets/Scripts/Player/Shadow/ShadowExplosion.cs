using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using Health;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ShadowExplosion : MonoBehaviour
{
    [SerializeField] private float explosionTime;
    private Collider _collider;

    private Coroutine _explosion;

    private void Awake()
    {
        _collider ??= GetComponent<Collider>();
    }

    public void Explode()
    {
        if (_explosion != null)
            StopCoroutine(_explosion);

        _explosion = StartCoroutine(Explosion());
    }

    private IEnumerator Explosion()
    {
        _collider.enabled = true;
        yield return new WaitForSeconds(explosionTime);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!this.enabled || other.CompareTag("Player"))
            return;

        if (other.transform.TryGetComponent<HealthPoints>(out HealthPoints healthPoints))
        {
            healthPoints.TryTakeDamage(healthPoints.MaxHealth);
        }
    }
}