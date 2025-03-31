using System;
using Events;
using Health;
using UnityEngine;

[RequireComponent(typeof(HealthPoints))]
public class RewardHealth : MonoBehaviour
{
    [SerializeField] private IntEventChannelSO onRewardHealth;

    private HealthPoints _healthPoints;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        _healthPoints ??= GetComponent<HealthPoints>();
        onRewardHealth?.onIntEvent.AddListener(HandleRewardPoints);
    }

    private void OnDisable()
    {
        onRewardHealth?.onIntEvent.RemoveListener(HandleRewardPoints);
    }

    private void HandleRewardPoints(int reward)
    {
        _healthPoints.SumHealth(reward);
    }
}
