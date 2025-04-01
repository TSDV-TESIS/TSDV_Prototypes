using System.Collections;
using System.Collections.Generic;
using Events.Scriptables;
using Player;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(CharacterController))]
public class Hook : MonoBehaviour
{
    [SerializeField] private InputHandler handler;
    [SerializeField] private float coolDown;

    [Header("Ray Settings")]
    [SerializeField] private float maxDistance;
    [SerializeField] private LayerMask rayLayerMask;

    [FormerlySerializedAs("speed")]
    [Header("Displacement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private AnimationCurve accelerationCurve;

    [Header("Visuals")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float throwSpeed;
    [SerializeField] private FloatEventChannel onCoolDown;

    private float lastHookTime = 0;
    private CharacterController _characterController;
    private Coroutine _displacement;
    private Coroutine _hookVisual;
    private bool _isHooked;

    private void OnEnable()
    {
        _characterController = GetComponent<CharacterController>();
        handler.OnPlayerHook.AddListener(HandleHook);
    }

    private void OnDisable()
    {
        handler.OnPlayerHook.AddListener(HandleHook);
    }

    private void HandleHook()
    {
        if (CanHook())
            ThrowHook();
    }

    private void ThrowHook()
    {
        _isHooked = true;
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, maxDistance, rayLayerMask))
        {
            if (_displacement != null)
                StopCoroutine(_displacement);
            _displacement = StartCoroutine(MoveTowards(hit.point));
        }
        else
        {
            if (_hookVisual != null)
                StopCoroutine(_hookVisual);
            _hookVisual = StartCoroutine(ShootHook(transform.forward * maxDistance, true));
        }
    }

    private bool CanHook()
    {
        return Time.time - lastHookTime > coolDown && !_isHooked;
    }

    private IEnumerator ShootHook(Vector3 position, bool shouldDecay)
    {
        lineRenderer.enabled = true;
        Vector3 initialPos = transform.position;
        float duration = Vector3.Distance(initialPos, position) / throwSpeed;
        float timer = 0;
        float startTime = Time.time;
        while (timer < duration)
        {
            lineRenderer.SetPosition(0, transform.position);
            timer = Time.time - startTime;
            Vector3 hookPosition = Vector3.Lerp(initialPos, position, timer / duration);
            lineRenderer.SetPosition(1, hookPosition);
            yield return null;
        }

        if (shouldDecay)
        {
            _isHooked = false;
            lastHookTime = Time.time;
            onCoolDown.RaiseEvent(coolDown);
            lineRenderer.enabled = false;
        }
    }

    private IEnumerator MoveTowards(Vector3 endPosition)
    {
        yield return ShootHook(endPosition, false);
        Vector3 initialPosition = transform.position;

        float travelDuration = Vector3.Distance(initialPosition, endPosition) / moveSpeed;

        float timer = 0;
        float startTime = Time.time;

        while (timer < travelDuration)
        {
            lineRenderer.SetPosition(0, transform.position);

            Vector3 movement = Vector3.Lerp(initialPosition, endPosition, accelerationCurve.Evaluate(timer / travelDuration));
            _characterController.Move(movement - transform.position);
            timer = Time.time - startTime;
            yield return null;
        }

        _isHooked = false;
        lastHookTime = Time.time;
        onCoolDown.RaiseEvent(coolDown);
        lineRenderer.enabled = false;
    }
}