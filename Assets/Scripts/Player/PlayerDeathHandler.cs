using System;
using System.Collections;
using Events;
using Health;
using Player;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PlayerDeathHandler : MonoBehaviour
{
    [SerializeField] private VoidEventChannelSO onPlayerDeath;
    [SerializeField] private HealthPoints healthPoints;
    [SerializeField] private UnityEvent OnInternalPlayerDeath;
    [SerializeField] private Movement movement;
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
        OnInternalPlayerDeath?.Invoke();

        StartCoroutine(ResetCoroutine());
    }

    private IEnumerator ResetCoroutine()
    {
        gameObject.transform.position = _startPosition;
        movement?.SetCanWalk(false);
        yield return new WaitForSeconds(waitSeconds);
        movement?.SetCanWalk(true);
    }
}
