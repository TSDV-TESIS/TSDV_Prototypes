using System.Collections;
using Events;
using Events.Scriptables;
using Leaderboard;
using Player;
using TMPro;
using UI.Leaderboard;
using UnityEngine;

public class WinPanel : MonoBehaviour
{
    [SerializeField] private VoidEventChannelSO onChangeLevel;
    [SerializeField] private VoidEventChannelSO onLevelReset;
    [SerializeField] private FloatEventChannel onTimerFinish;
    [SerializeField] private VoidEventChannelSO onGamePaused;

    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject goSignObject;
    [SerializeField] private LevelCompleteAnimation levelCompletePhase;
    [SerializeField] private TieredTimes medals;
    [SerializeField] private TextMeshProUGUI timeTMPro;
    [SerializeField] private TextMeshProUGUI timePersonalBestTMPro;
    [SerializeField] private LeaderboardRequestHandler leaderboardRequestHandler;
    [SerializeField] private float countDownDuration = 3;
    [SerializeField] private LeaderboardData leaderboardData;
    [SerializeField] private PlayerName playerName;

    private string timeSuffix = "Level Cleared in: ";
    private string recordSuffix = "Best Time: ";
    private string recordText = "New Personal Best!";

    private Coroutine countDown;
    private Coroutine completePhase;

    private float levelClearTime;

    private void OnEnable()
    {
        panel.SetActive(false);
        onTimerFinish?.onFloatEvent.AddListener(HandleTimerFinish);
        leaderboardData?.requestObtained.AddListener(SetPersonalBest);
    }

    private void OnDisable()
    {
        onTimerFinish?.onFloatEvent.RemoveListener(HandleTimerFinish);
        leaderboardData?.requestObtained.RemoveListener(SetPersonalBest);
    }

    private void HandleTimerFinish(float time)
    {
        panel.SetActive(true);
        goSignObject.SetActive(false);
        if (completePhase != null)
            StopCoroutine(completePhase);

        levelCompletePhase.gameObject.SetActive(true);
        completePhase = StartCoroutine(levelCompletePhase.LevelCompleteStart(time));

        leaderboardRequestHandler.HandleSetTime((int)(time * 1000));
        TimeManager.Instance.PauseTime(true);
        onGamePaused?.RaiseEvent();
        if (countDown != null)
            StopCoroutine(countDown);

        countDown = StartCoroutine(NextLevelCoroutine(time));

        timeTMPro.text = timeSuffix + TimeFormatting.GetFormattedTime(time);
    }

    private void SetPersonalBest()
    {
        if (leaderboardData.hasError)
            return;

        foreach (LeaderboardRow row in leaderboardData.data)
        {
            if (row.name == playerName.playerName)
            {
                timePersonalBestTMPro.text = recordSuffix + TimeFormatting.GetFormattedTime(row.timeBeaten);
                break;
            }
        }
    }

    private IEnumerator NextLevelCoroutine(float levelClearTime)
    {
        yield return medals.UnlockMedalsCoroutine(levelClearTime);
    }

    public void NextLevel()
    {
        Debug.Log("NEXT LEVEL PRESSED");
        onChangeLevel?.RaiseEvent();
        TimeManager.Instance.PauseTime(false);
    }

    public void ResetLevel()
    {
        Debug.Log("Restart LEVEL PRESSED");
        onLevelReset?.RaiseEvent();
        TimeManager.Instance.PauseTime(false);
    }
}