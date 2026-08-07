using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthAnimator : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private Image targetImage;

    [Header("Shader Settings")]
    [Tooltip("Nombre exacto del Reference Name en Shader Graph")]
    [SerializeField] private string healthPropertyName = "_Health";

    private Material instancedMaterial;
    private Coroutine activeRoutine;
    private int healthPropID;

    private void Awake()
    {
        if (targetImage != null)
        {
            // Creamos una instancia única del material para no alterar el Asset en disco
            instancedMaterial = Instantiate(targetImage.material);
            targetImage.material = instancedMaterial;
        }

        healthPropID = Shader.PropertyToID(healthPropertyName);
    }

   
    public void FadeInHealth()
    {
        StartHealthFade(1f, 1f);
    }


    public void FadeOutHealth()
    {
        StartHealthFade(0f, 0.7f);
    }

    private void StartHealthFade(float targetValue, float duration)
    {
        if (instancedMaterial == null) return;

        if (activeRoutine != null)
            StopCoroutine(activeRoutine);

        activeRoutine = StartCoroutine(AnimateHealth(targetValue, duration));
    }

    private IEnumerator AnimateHealth(float targetValue, float duration)
    {
        float startValue = instancedMaterial.GetFloat(healthPropID);
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float current = Mathf.Lerp(startValue, targetValue, time / duration);
            instancedMaterial.SetFloat(healthPropID, current);
            yield return null;
        }

        instancedMaterial.SetFloat(healthPropID, targetValue);
    }
}