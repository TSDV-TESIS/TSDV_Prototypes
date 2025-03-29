using System;
using Player;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    [SerializeField] private InputHandler handler;
    [SerializeField] private GameObject model;
    
    private Vector3 _lookDirection;
    void OnEnable()
    {
        handler.OnPlayerMove.AddListener(HandleChangeDirection);
    }

    private void OnDisable()
    {
        handler.OnPlayerMove.RemoveListener(HandleChangeDirection);
    }

    private void Update()
    {
        if (_lookDirection.magnitude < float.Epsilon) return;
        transform.rotation = Quaternion.LookRotation(_lookDirection);
    }
    
    private void HandleChangeDirection(Vector2 direction)
    {
        _lookDirection = new Vector3(direction.x, 0, direction.y);
    }
}
