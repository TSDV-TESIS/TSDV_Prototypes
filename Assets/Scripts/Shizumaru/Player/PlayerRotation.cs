using System;
using Player;
using UnityEngine;

namespace Shizumaru
{
    public class PlayerRotation : MonoBehaviour
    {
        [SerializeField] private InputHandler handler;
        [SerializeField] private GameObject model;
        
        private Vector2 _movement;

        private void OnEnable()
        {
            handler.OnPlayerMove.AddListener(HandleRotate);
        }

        private void OnDisable()
        {
            handler.OnPlayerMove.RemoveListener(HandleRotate);
        }

        private void HandleRotate(Vector2 movement)
        {
            if (movement.x == 0) return;

            Quaternion rotation = model.transform.rotation;
            rotation.eulerAngles = new Vector3(0, movement.x > 0 ? 90 : -90, 0);
            model.transform.rotation = rotation;
        }
    }
}
