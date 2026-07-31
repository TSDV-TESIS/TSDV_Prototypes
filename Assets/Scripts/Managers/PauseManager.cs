using Events;
using Events.Scriptables;
using Player;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private InputHandler input;

    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject settingsScreen;

    [Header("Main Menu")] [SerializeField] private string mainMenuScene;
    [SerializeField] private StringEventChannelSO onMainMenu;

    [SerializeField] private VoidEventChannelSO onGamePaused;
    [SerializeField] private VoidEventChannelSO onGameUnpaused;
    private bool isPaused = false;

    private void OnEnable()
    {
        input.onPauseToggle.AddListener(TogglePause);
        isPaused = false;
        TimeManager.Instance.PauseTime(isPaused);
        onGameUnpaused?.RaiseEvent();
    }

    private void OnDisable()
    {
        input.onPauseToggle.RemoveListener(TogglePause);
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Debug.Log($"PAUSE CALLED: {isPaused}");
        panel.SetActive(isPaused);
        if (isPaused)
        {
            pauseScreen.SetActive(true);
            settingsScreen.SetActive(false);
            onGamePaused?.RaiseEvent();
        }
        else
            onGameUnpaused?.RaiseEvent();

        TimeManager.Instance.PauseTime(isPaused);
    }

    public void ToggleSettingsScreen()
    {
        pauseScreen.SetActive(!pauseScreen.activeSelf);
        settingsScreen.SetActive(!settingsScreen.activeSelf);
    }

    public void GoToMainMenu()
    {
        TimeManager.Instance.PauseTime(false);
        onMainMenu?.RaiseEvent(mainMenuScene);
    }
}