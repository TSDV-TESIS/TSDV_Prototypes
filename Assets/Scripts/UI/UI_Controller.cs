using System.Collections;
using Events;
using UnityEngine;

public class UI_Controller : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private float panelDuration;
    [SerializeField] private Loader loader;
    [SerializeField] private VoidEventChannelSO onPlayerDeath;

    void OnEnable()
    {
        onPlayerDeath.onEvent.AddListener(HandlePlayerDeath);
    }

    private void HandlePlayerDeath()
    {
        StartCoroutine(GameOverScreen());
    }

    private IEnumerator GameOverScreen()
    {
        gameOverPanel.SetActive(true);
        yield return new WaitForSeconds(panelDuration);
        loader.RestartScene();
    }
}