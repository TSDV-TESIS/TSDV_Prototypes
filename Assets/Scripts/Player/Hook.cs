using System;
using Player;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Hook : MonoBehaviour
{
    [SerializeField] private InputHandler handler;
    [SerializeField] private float coolDown;
    [SerializeField] private float maxDistance;
    [SerializeField] private LayerMask rayLayerMask;

    private float lastHookTime = 0;
    private CharacterController _characterController;

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
            _characterController.Move(hit.point - transform.position);
        }
    }

    private bool CanHook()
    {
        return Time.time - lastHookTime > coolDown;
    }
}