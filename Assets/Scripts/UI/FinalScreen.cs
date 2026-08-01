using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Events.Scriptables;
using UnityEngine;

public class FinalScreen : MonoBehaviour
{
    [Header("Main Menu")] [SerializeField] private string mainMenuScene;
    [SerializeField] private StringEventChannelSO changeScene;

    [SerializeField] private float waitTime;

    private Coroutine countDown;

    void Start()
    {
        if (countDown != null)
            StopCoroutine(countDown);

        countDown = StartCoroutine(ReturnToMainMenu());
    }

    private IEnumerator ReturnToMainMenu()
    {
        yield return new WaitForSeconds(waitTime);
        changeScene.RaiseEvent(mainMenuScene);
    }
}