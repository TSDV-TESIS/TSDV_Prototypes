using System.Collections;
using Events;
using Events.Scriptables;
using Player;
using UnityEngine;

public class FinishCinematic : MonoBehaviour
{
    [SerializeField] private Transform winDoor;
    [SerializeField] private BoolEventChannelSO shouldTimerCount;
    [SerializeField] private VoidEventChannelSO onPlayerWin;
    [SerializeField] private VoidEventChannelSO onFinishSequenceStart;
    [SerializeField] private InputHandler inputHandler;

    [Header("SequenceSettings")] [SerializeField] private float duration;
    [Header("SequenceSettings")] [SerializeField] private float waitToMovePlayer;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform playerFinishPosition;

    private readonly int finishAnimTrigger = Animator.StringToHash("FinishLevel");

    private Coroutine sequence;

    void Start()
    {
        transform.position = winDoor.position;
        onFinishSequenceStart.onEvent.AddListener(StartSequence);
    }

    private void StartSequence()
    {
        if (sequence != null)
            StopCoroutine(sequence);
        sequence = StartCoroutine(FinishSequence());
    }

    private IEnumerator FinishSequence()
    {
        inputHandler.TogglePlayerLock(true);
        shouldTimerCount.onTypedEvent.Invoke(false);
        animator.SetTrigger(finishAnimTrigger);
        yield return new WaitForSeconds(waitToMovePlayer);
        float timer = 0;
        Vector3 initialPos = playerTransform.position;
        while (timer < duration - waitToMovePlayer)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            playerTransform.position = Vector3.Lerp(initialPos, playerFinishPosition.position, t);
            yield return null;
        }

        inputHandler.TogglePlayerLock(false);
        onPlayerWin?.RaiseEvent();
    }
}