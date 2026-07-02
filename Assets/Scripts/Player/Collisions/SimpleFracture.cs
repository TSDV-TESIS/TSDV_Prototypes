using System.Collections;
using UnityEngine;

public class SimpleFracture : MonoBehaviour
{
    [Header("Physics Config")] public GameObject fracturedMesh;
    public float explosionForce = 400f;

    [Header("Shader Config (Dissolve)")] [Tooltip("Seconds it takes to turn black before dissolving")] public float timeToTurnBlack = 0.5f;

    [Tooltip("total seconds to dissolve pieces")] public float dissolveDuration = 2.0f;

    private readonly string prop_IsBreak = "_Is_Break_Wall";
    private readonly string prop_GradientHeight = "_Dissolve_GradientHeight";
    private readonly string prop_Amount = "_Dissolve_Amount";

    public void ExecuteShatter()
    {
        if (GetComponent<Collider>()) GetComponent<Collider>().enabled = false;

        if (fracturedMesh)
        {
            fracturedMesh.transform.SetParent(null);

            Rigidbody[] rbs = fracturedMesh.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rb in rbs)
            {
                rb.isKinematic = false;
                rb.AddForce(explosionForce * (rb.transform.position - transform.position), ForceMode.Impulse);
            }

            Renderer[] renderers = fracturedMesh.GetComponentsInChildren<Renderer>();

            if (renderers.Length > 0)
            {
                DissolveController controller = fracturedMesh.AddComponent<DissolveController>();
                controller.StartDissolve(renderers, timeToTurnBlack, dissolveDuration);
            }

            Destroy(gameObject, 0.1f);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

public class DissolveController : MonoBehaviour
{
    // IDs de las propiedades para acceder r�pido al shader
    private static readonly int ID_IsBreak = Shader.PropertyToID("_Is_Break_Wall");
    private static readonly int ID_GradientHeight = Shader.PropertyToID("_Dissolve_GradientHeight");
    private static readonly int ID_Amount = Shader.PropertyToID("_Dissolve_Amount");

    private Renderer[] _renderers;
    private MaterialPropertyBlock _propBlock;

    public void StartDissolve(Renderer[] renderers, float blackTime, float dissolveTime)
    {
        _renderers = renderers;
        _propBlock = new MaterialPropertyBlock();
        StartCoroutine(DissolveRoutine(blackTime, dissolveTime));
    }

    private IEnumerator DissolveRoutine(float blackTime, float dissolveTime)
    {
        // --- FASE 1: ACTIVAR Y PONER NEGRO ---
        // Establecemos Is_Break_Wall = 1 y animamos GradientHeight de 2 a 0

        float timer = 0f;

        // Configuraci�n Inicial
        foreach (Renderer r in _renderers)
        {
            r.GetPropertyBlock(_propBlock);
            _propBlock.SetFloat(ID_IsBreak, 1); // Activamos el efecto
            _propBlock.SetFloat(ID_GradientHeight, 2f); // Valor inicial
            _propBlock.SetFloat(ID_Amount, -0.6f); // Valor inicial
            r.SetPropertyBlock(_propBlock);
        }

        // Animaci�n de "ennegrecimiento"
        while (timer < blackTime)
        {
            timer += Time.deltaTime;
            float progress = timer / blackTime;

            foreach (Renderer r in _renderers)
            {
                r.GetPropertyBlock(_propBlock);
                // Interpolamos de 2 a 0
                _propBlock.SetFloat(ID_GradientHeight, Mathf.Lerp(2f, 0f, progress));
                r.SetPropertyBlock(_propBlock);
            }

            yield return null; // Esperamos un frame
        }

        // --- FASE 2: DISOLUCI�N REAL ---
        // Animamos Dissolve_Amount de -0.6 a 0.6

        timer = 0f;
        float startAmount = -0.6f;
        float endAmount = 0.6f;

        while (timer < dissolveTime)
        {
            timer += Time.deltaTime;
            float progress = timer / dissolveTime;

            foreach (Renderer r in _renderers)
            {
                r.GetPropertyBlock(_propBlock);
                _propBlock.SetFloat(ID_Amount, Mathf.Lerp(startAmount, endAmount, progress));
                r.SetPropertyBlock(_propBlock);
            }

            yield return null;
        }

        // --- FASE 3: LIMPIEZA ---
        // Destruimos los pedazos despu�s de que termine el efecto
        Destroy(gameObject);
    }
}