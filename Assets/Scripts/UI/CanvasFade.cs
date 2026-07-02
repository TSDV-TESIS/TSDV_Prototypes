using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasFade : MonoBehaviour
{
    public static IEnumerator Fade(CanvasGroup canvasGroup, bool shouldFadeIn, float duration)
    {
        float timer = 0;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float lerp = timer / duration;
            canvasGroup.alpha = shouldFadeIn ? lerp : 1 - lerp;
            yield return null;
        }
    }

    public static IEnumerator Fade(List<CanvasGroup> canvasGroups, bool shouldFadeIn, float duration)
    {
        float timer = 0;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float lerp = timer / duration;
            foreach (CanvasGroup canvasGroup in canvasGroups)
            {
                canvasGroup.alpha = shouldFadeIn ? lerp : 1 - lerp;
            }

            yield return null;
        }
    }
}