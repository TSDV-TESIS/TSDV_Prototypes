using System.Collections;
using Player;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Hook : MonoBehaviour
{
    [SerializeField] private InputHandler handler;
    [SerializeField] private float coolDown;

    [Header("Ray Settings")]
    [SerializeField] private float maxDistance;
    [SerializeField] private LayerMask rayLayerMask;

    [Header("Displacement")]
    [SerializeField] private float speed;

    private float lastHookTime = 0;
    private CharacterController _characterController;
    private Coroutine _displacement;

    private void OnEnable()
    {
        _characterController = GetComponent<CharacterController>();
        handler.OnPlayerHook.AddListener(HandleHook);
    }

    private void OnDisable()
    {
        handler.OnPlayerHook.AddListener(HandleHook);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void HandleHook()
    {
        if (CanHook())
            ThrowHook();
    }

    private void ThrowHook()
    {
        lastHookTime = Time.time;
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, maxDistance, rayLayerMask))
        {
            if (_displacement != null)
                StopCoroutine(_displacement);
            _displacement = StartCoroutine(MoveTowards(hit.point));
        }
    }

    private bool CanHook()
    {
        return Time.time - lastHookTime > coolDown;
    }

    private IEnumerator MoveTowards(Vector3 endPosition)
    {
        Vector3 initialPosition = transform.position;

        float travelDuration = Vector3.Distance(initialPosition, endPosition) / speed;

        float timer = 0;
        float startTime = Time.time;

        while (timer < travelDuration)
        {
            Vector3 movement = Vector3.Lerp(initialPosition, endPosition, timer / travelDuration);
            _characterController.Move(movement - transform.position);
            timer = Time.time - startTime;
            yield return null;
        }
    }
}