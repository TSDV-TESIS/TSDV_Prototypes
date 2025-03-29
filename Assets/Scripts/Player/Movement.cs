using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class Movement : MonoBehaviour
    {
        [SerializeField] private float velocity;
        [SerializeField] private InputHandler handler;
    
        private CharacterController _characterController;
        private Vector3 _moveDirection;
    
        void OnEnable()
        {
            _characterController ??= GetComponent<CharacterController>();
            _moveDirection = Vector3.zero;
        
            handler.OnPlayerMove.AddListener(HandleMove);
        }

        private void OnDisable()
        {
            handler.OnPlayerMove.RemoveListener(HandleMove);
        }

        void Update()
        {
            _characterController.Move(_moveDirection * (Time.deltaTime * velocity));
        }

        private void HandleMove(Vector2 movement)
        {
            _moveDirection = new Vector3(movement.x, 0, movement.y);
        }
    }
}
