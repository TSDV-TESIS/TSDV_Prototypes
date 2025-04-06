using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class Movement2D : MonoBehaviour
    {
        [SerializeField] private float velocity;
        [SerializeField] private InputHandler handler;

        private CharacterController _characterController;
        private Vector3 _moveDirection;
        private bool _canWalk;

        public float Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

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
            Vector3 prevPos = transform.position;
            if (_canWalk)
                _characterController.Move(_moveDirection * (Time.deltaTime * velocity));

            if (transform.position.z != 0)
                transform.position = prevPos;
        }

        private void HandleMove(Vector2 movement)
        {
            _moveDirection = new Vector3(movement.x, 0, 0);
        }

        public void SetCanWalk(bool canWalk)
        {
            _canWalk = canWalk;
        }
    }
}