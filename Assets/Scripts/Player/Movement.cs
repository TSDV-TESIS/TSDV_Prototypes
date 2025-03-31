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
        private bool _canWalk;
        void OnEnable()
        {
            _canWalk = true;
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
            if(_canWalk)
                _characterController.Move(_moveDirection * (Time.deltaTime * velocity));
        }

        private void HandleMove(Vector2 movement)
        {
            _moveDirection = new Vector3(movement.x, 0, movement.y);
        }

        public void SetCanWalk(bool canWalk)
        {
            _canWalk = canWalk;
        }
    }
}
