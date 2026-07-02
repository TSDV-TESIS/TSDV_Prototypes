using System;
using Events;
using Events.Scriptables;
using Health;
using UnityEngine;

public class LevelTimer : MonoBehaviour
{
    [SerializeField] private VoidEventChannelSO onPlayerWin;
    [SerializeField] private DeathEventChannelSO onPlayerDeath;
    [SerializeField] private FloatEventChannel onTimerFinish;
    [SerializeField] private FloatEventChannel onTimerTick;
    [SerializeField] private BoolEventChannelSO countTimeToggleEvent;

    [SerializeField] private GlobalTime globalTimeSO;
    [SerializeField] private bool isFirstLevel = false;

    private float time = 0;
    private bool shouldCountTime;

    private void OnEnable()
    {
        onPlayerWin?.onEvent.AddListener(OnWinEndTimer);
        onPlayerDeath?.onTypedEvent.AddListener(OnLoseEndTimer);
        countTimeToggleEvent?.onTypedEvent.AddListener(ToggleTimer);
        StartTimer();
        if (isFirstLevel)
            ResetTotalTime();
    }

    private void OnDisable()
    {
        countTimeToggleEvent?.onTypedEvent.RemoveListener(ToggleTimer);
        onPlayerWin?.onEvent.RemoveListener(OnWinEndTimer);
        onPlayerDeath?.onTypedEvent.RemoveListener(OnLoseEndTimer);
    }

    private void StartTimer()
    {
        shouldCountTime = true;
        time = 0;
    }

    private void ToggleTimer(bool value)
    {
        shouldCountTime = value;
    }

    private void OnWinEndTimer()
    {
        shouldCountTime = false;
        onTimerFinish?.RaiseEvent(time);
        globalTimeSO.time += time;
    }

    private void OnLoseEndTimer(DeathCauses cause)
    {
        shouldCountTime = false;
        globalTimeSO.time += time;
    }

    private void Update()
    {
        if (!shouldCountTime)
            return;

        time += Time.deltaTime;
        onTimerTick.RaiseEvent(time);
    }

    private void ResetTotalTime()
    {
        globalTimeSO.time = 0;
    }
}