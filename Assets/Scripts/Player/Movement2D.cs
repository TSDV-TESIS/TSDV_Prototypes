using System.Collections;
using Events;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class Movement2D : MonoBehaviour
    {
        [SerializeField] private InputHandler input;
        [SerializeField] private float acceleration;
        [SerializeField] private float friction;
        [SerializeField] private float maxSpeed;
        [SerializeField] private float gravity;
        [SerializeField] private float jumpForce;

        [SerializeField] private VoidEventChannelSO onPlayerDeath;
        [SerializeField] private VoidEventChannelSO onPlayerRevive;

        [Header("Ground Check")]
        [SerializeField] private Transform feetPivot;
        [SerializeField] private float checkDistance;
        [SerializeField] private LayerMask whatIsGround;

        [Header("Wall Check")]
        [SerializeField] private float wallCheckDistance;
        [SerializeField] private LayerMask whatIsWall;

        [Header("Wall Slide")]
        [SerializeField] private float lockDuration;

        private CharacterController _characterController;
        private Vector3 _moveDirection;
        private bool _canWalk;


        private Vector2 _velocity;
        private Coroutine velocityLock;

        public float MaxSpeed
        {
            get { return maxSpeed; }
            set { maxSpeed = value; }
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
            _velocity = new Vector2(maxSpeed, 0);
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
            _velocity.y -= IsWallSliding() ? gravity / 2 * Time.deltaTime : gravity * Time.deltaTime;
            if (IsGrounded() && _velocity.y < 0)
                _velocity.y = 0;

            if (_canWalk)
                _velocity.x = Mathf.Clamp(_velocity.x + (_moveDirection.x * acceleration * Time.deltaTime), -maxSpeed, maxSpeed);

            if (_moveDirection.x == 0 && IsGrounded())
                _velocity.x = Mathf.Sign(_velocity.x) * Mathf.Clamp(Mathf.Abs(_velocity.x) - friction * Time.deltaTime, 0, maxSpeed);

            Debug.Log(_characterController.Move(_velocity * Time.deltaTime));

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

            if (IsWallSliding() && !IsGrounded())
            {
                _velocity.x = jumpForce / 2 * Mathf.Sign(_moveDirection.x) * -1;
                if (velocityLock != null)
                    StopCoroutine(velocityLock);

                velocityLock = StartCoroutine(LockAfterWallJump());
            }
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

        private bool IsWallSliding()
        {
            return _moveDirection.x != 0 && Physics.Raycast(transform.position, Vector3.right * Mathf.Sign(_moveDirection.x), wallCheckDistance, whatIsWall);
        }

        private bool CanJump()
        {
            return IsGrounded() || IsWallSliding();
        }

        private IEnumerator LockAfterWallJump()
        {
            _canWalk = false;
            yield return new WaitForSeconds(lockDuration);
            _canWalk = true;
        }
    }
}