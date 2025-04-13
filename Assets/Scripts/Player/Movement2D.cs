using System;
using System.Collections;
using Events;
using Player.Properties;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    // TODO Handle this with an FSM, this script is large!
    [RequireComponent(typeof(CharacterController))]
    public class Movement2D : MonoBehaviour
    {
        [Header("Input Handler")] [SerializeField]
        private InputHandler input;

        [Header("Movement Properties")] [SerializeField]
        private PlayerMovementProperties playerMovementProperties;

        [Header("Feet pivot")] [SerializeField]
        private Transform feetPivot;

        [Header("Events")] [SerializeField] private VoidEventChannelSO onPlayerDeath;
        [SerializeField] private VoidEventChannelSO onPlayerRevive;

        private CharacterController _characterController;
        private Vector3 _moveDirection;
        private bool _canWalk;
        private Vector2 _velocity;
        private Coroutine _velocityLock;

        private RaycastHit _groundHit;

        public float MaxSpeed
        {
            get => playerMovementProperties.maxSpeed;
            set => playerMovementProperties.maxSpeed = value;
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
            _velocity = new Vector2(playerMovementProperties.maxSpeed, 0);
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
            HandleYVelocityWithWalkSliding();

            HandleWalk();

            _characterController.Move(_velocity * Time.deltaTime);

            SetZPosition(prevPos);
        }

        private void HandleWalk()
        {
            _moveDirection = GetSlopeMovementDirection();

            if (_canWalk)
                _velocity.x =
                    Mathf.Clamp(
                        _velocity.x + (_moveDirection.x * playerMovementProperties.acceleration * Time.deltaTime),
                        -playerMovementProperties.maxSpeed, playerMovementProperties.maxSpeed);

            if (_moveDirection.x == 0 && IsGrounded())
                _velocity.x = Mathf.Sign(_velocity.x) *
                              Mathf.Clamp(Mathf.Abs(_velocity.x) - playerMovementProperties.friction * Time.deltaTime,
                                  0, playerMovementProperties.maxSpeed);
        }

        private void HandleYVelocityWithWalkSliding()
        {
            if (IsOnSlope()) return;
            // Wallsliding logic for falldown feeling
            _velocity.y -= IsWallSliding()
                ? playerMovementProperties.gravity / playerMovementProperties.wallFriction * Time.deltaTime
                : playerMovementProperties.gravity * Time.deltaTime;

            if (IsGrounded() && _velocity.y < 0)
                _velocity.y = 0;
        }

        private void SetZPosition(Vector3 prevPos)
        {
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

            _velocity.y = playerMovementProperties.jumpForce;

            if (IsWallSliding() && !IsGrounded())
            {
                _velocity.x = playerMovementProperties.jumpForce / 2 * Mathf.Sign(_moveDirection.x) * -1;
                if (_velocityLock != null)
                    StopCoroutine(_velocityLock);

                _velocityLock = StartCoroutine(LockAfterWallJump());
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
            return Physics.Raycast(feetPivot.position, Vector3.down, out _groundHit,
                playerMovementProperties.checkDistance, playerMovementProperties.whatIsGround);
        }

        private bool IsOnSlope()
        {
            if (!IsGrounded()) return false;

            float angle = Vector3.Angle(Vector3.up, _groundHit.normal);

            return angle < playerMovementProperties.maxSlopeAngle && !Mathf.Approximately(angle, 0);
        }

        private Vector3 GetSlopeMovementDirection()
        {
            return Vector3.ProjectOnPlane(_moveDirection, _groundHit.normal).normalized;
        }

        private bool IsWallSliding()
        {
            return _moveDirection.x != 0 &&
                   Physics.Raycast(
                       transform.position,
                       Vector3.right * Mathf.Sign(_moveDirection.x),
                       playerMovementProperties.wallCheckDistance,
                       playerMovementProperties.whatIsWall
                   );
        }

        private bool CanJump()
        {
            return IsGrounded() || IsWallSliding();
        }

        private IEnumerator LockAfterWallJump()
        {
            _canWalk = false;
            yield return new WaitForSeconds(playerMovementProperties.lockDuration);
            _canWalk = true;
        }

        private void OnDrawGizmos()
        {
            if (playerMovementProperties.shouldDrawGizmos)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(feetPivot.position,
                    feetPivot.position + Vector3.down * playerMovementProperties.checkDistance);
                Gizmos.DrawLine(transform.position, transform.position + _moveDirection * 10f);
            }
        }
    }
}