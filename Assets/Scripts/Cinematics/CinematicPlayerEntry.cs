using System.Collections;
using System.Collections.Generic;
using Events.Scriptables;
using Health;
using Player;
using UnityEngine;

public class CinematicPlayerEntry : MonoBehaviour
{
    [SerializeField] private PlayerName playerInfo;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private HealthPoints playerHealth;
    [SerializeField] private float animationDuration;
    [SerializeField] private float fireBurstDuration;

    [SerializeField] private AnimationCurve timeScaleCurve;
    [SerializeField] private List<CanvasGroup> hudCanvasGroups;
    [SerializeField] private InputHandler inputHandler;

    [SerializeField] private BoolEventChannelSO shouldTimerCount;
    [SerializeField] private PlayerVfxController playerVfxController;

    [Header("CameraPanning")] [SerializeField] private float cameraDisplacementDuration;
    [SerializeField] private Transform mainCameraTransform;

    private readonly int playerEntryTrigger = Animator.StringToHash("PlayerEntry");

    private Coroutine sequence;

    private void Start()
    {
        if (playerInfo.isInitialized)
        {
            inputHandler.TogglePlayerLock(false);
            gameObject.SetActive(false);
            return;
        }

        if (sequence != null)
            StopCoroutine(sequence);

        sequence = StartCoroutine(CinematicSequence());
    }

    private IEnumerator CinematicSequence()
    {
        SequenceInit();
        float timer = 0;
        playerVfxController.isInitialized = false;
        playerVfxController.LerpAura(0);
        while (timer < animationDuration)
        {
            timer += Time.deltaTime;
            float t = timer / animationDuration;
            TimeManager.Instance.TrySetTimeScale(timeScaleCurve.Evaluate(t));

            if (timer > animationDuration - fireBurstDuration)
            {
                float fireTime = (timer - (animationDuration - fireBurstDuration)) / fireBurstDuration;
                playerVfxController.LerpAura(fireTime);
            }

            yield return null;
        }

        playerVfxController.isInitialized = true;

        TimeManager.Instance.TrySetTimeScale(1);

        timer = 0;

        Vector3 position = transform.position;
        Vector3 targetPosition = mainCameraTransform.position;
        while (timer < cameraDisplacementDuration)
        {
            timer += Time.deltaTime;
            float t = timer / cameraDisplacementDuration;
            foreach (CanvasGroup canvasGroup in hudCanvasGroups)
            {
                canvasGroup.alpha = t;
            }

            transform.position = Vector3.Lerp(position, targetPosition, t);
            yield return null;
        }

        SequenceFinish();
    }

    private void SequenceInit()
    {
        foreach (CanvasGroup canvasGroup in hudCanvasGroups)
        {
            canvasGroup.alpha = 0;
        }

        playerAnimator.SetTrigger(playerEntryTrigger);
        playerHealth.SetIsInvincible(true);
        inputHandler.TogglePlayerLock(true);
        shouldTimerCount.onTypedEvent?.Invoke(false);
    }

    private void SequenceFinish()
    {
        foreach (CanvasGroup canvasGroup in hudCanvasGroups)
        {
            canvasGroup.alpha = 1;
        }

        playerInfo.isInitialized = true;
        gameObject.SetActive(false);
        playerMovement.enabled = true;
        inputHandler.TogglePlayerLock(false);
        playerHealth.SetIsInvincible(false);
        shouldTimerCount.onTypedEvent?.Invoke(true);
    }
}