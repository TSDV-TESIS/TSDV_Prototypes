using Events;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class Movement2D : MonoBehaviour
    {
        [SerializeField] private InputHandler input;
        [SerializeField] private float speed;
        [SerializeField] private float gravity;
        [SerializeField] private float jumpForce;

        [SerializeField] private VoidEventChannelSO onPlayerDeath;
        [SerializeField] private VoidEventChannelSO onPlayerRevive;

        [Header("GroundCheck")]
        [SerializeField] private Transform feetPivot;
        [SerializeField] private float checkDistance;
        [SerializeField] private LayerMask whatIsGround;

        private CharacterController _characterController;
        private Vector3 _moveDirection;
        private bool _canWalk;


        private Vector2 _velocity;

        public float Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        void OnEnable()
        {
            _canWalk = true;
            _characterController ??= GetComponent<CharacterController>();
            _moveDirection = Vector3.zero;

            input.OnPlayerMove.AddListener(HandleMove);
            input.OnPlayerJump.AddListener(HandleJump);

            onPlayerDeath.onEvent.AddListener(HandleDeath);
            onPlayerRevive.onEvent.AddListener(HandleRevive);
            _velocity = new Vector2(speed, 0);
        }

        private void OnDisable()
        {
            input.OnPlayerMove.RemoveListener(HandleMove);
            input.OnPlayerJump.RemoveListener(HandleJump);

            onPlayerDeath.onEvent.RemoveListener(HandleDeath);
            onPlayerRevive.onEvent.RemoveListener(HandleRevive);
        }

        void Update()
        {
            Vector3 prevPos = transform.position;
            _velocity.y -= gravity * Time.deltaTime;
            if (IsGrounded() && _velocity.y < 0)
                _velocity.y = 0;

            _characterController.Move(Vector3.up * (_velocity.y * Time.deltaTime));

            if (_canWalk)
                _characterController.Move(_moveDirection * (Time.deltaTime * _velocity.x));

            if (transform.position.z != 0)
                transform.position = prevPos;
        }

        private void HandleMove(Vector2 movement)
        {
            _moveDirection = new Vector3(movement.x, 0, 0);
        }

        private void HandleJump()
        {
            if (!CanJump())
                return;

            _velocity.y = jumpForce;
        }

        public void SetCanWalk(bool canWalk)
        {
            _canWalk = canWalk;
        }

        private void HandleDeath()
        {
            SetCanWalk(false);
        }

        private void HandleRevive()
        {
            SetCanWalk(true);
        }

        private bool IsGrounded()
        {
            if (Physics.Raycast(feetPivot.position, Vector3.down, checkDistance, whatIsGround))
                return true;

            return false;
        }

        private bool CanJump()
        {
            return IsGrounded();
        }
    }
}