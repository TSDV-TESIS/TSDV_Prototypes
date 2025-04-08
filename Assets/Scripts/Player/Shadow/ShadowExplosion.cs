using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using Health;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class ShadowExplosion : MonoBehaviour
{
    [SerializeField] private float explosionDuration;
    [SerializeField] private MeshRenderer meshRenderer;
    private Collider _collider;
    private Rigidbody _rb;

    private Coroutine _explosion;

    private void Awake()
    {
        _collider ??= GetComponent<Collider>();
        _rb ??= GetComponent<Rigidbody>();
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
        meshRenderer.enabled = false;
        yield return new WaitForSeconds(explosionDuration);
        _rb.detectCollisions = false;
        Destroy(gameObject,0.1f);
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