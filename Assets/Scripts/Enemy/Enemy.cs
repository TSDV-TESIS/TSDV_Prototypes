using System;
using Events;
using Health;
using UI.Bars;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private int healthRewardOnDeath = 15;

    [Header("Events")] 
    [SerializeField] private IntEventChannelSO OnEnemyDeath;

    private int _maxHealth;
    public void HandleInitMaxHealth(int maxHealth)
    {
        _maxHealth = maxHealth;
        healthBar?.HandleInit(maxHealth);
    }

    public void OnEnemyHit(int value)
    {
        healthBar.HandleTakeDamage(value);
    }

    public void OnDeath()
    {
        OnEnemyDeath?.RaiseEvent(healthRewardOnDeath);
        gameObject.SetActive(false);
    }

    public void OnPlayerDeath()
    {
        Debug.Log("Hi?");
        gameObject.SetActive(true);
        healthBar.HandleInit(_maxHealth);
    }
}
