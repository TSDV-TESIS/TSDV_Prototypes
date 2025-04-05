using System;
using Player;
using UnityEngine;

namespace Shizumaru
{
    public class Movement : MonoBehaviour
    {
        [SerializeField] private float velocity;
        [SerializeField] private InputHandler handler;
        
        private Vector2 _moveDirection;
        private void OnEnable()
        {
            handler.OnPlayerMove.AddListener(HandleMove);
        }

        private void OnDisable()
        {
            handler.OnPlayerMove.RemoveListener(HandleMove);
        }

        private void Update()
        {
            if (_moveDirection.x == 0) return;
            gameObject.transform.position += new Vector3((_moveDirection.x > 0 ? 1 : -1) * velocity * Time.deltaTime, 0, 0);
        }

        private void HandleMove(Vector2 direction)
        {
            _moveDirection = direction;
        }
    }
}
