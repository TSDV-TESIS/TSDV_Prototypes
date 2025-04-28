using System;
using System.Collections;
using Events;
using Player.Controllers;
using Player.Properties;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    // TODO Handle this with an FSM, this script is large!
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Input Handler")]
        [SerializeField] private InputHandler input;

        [Header("Movement Properties")]
        [SerializeField] private PlayerMovementProperties playerMovementProperties;

        [Header("Events")]
        [SerializeField] private VoidEventChannelSO onPlayerDeath;
        [SerializeField] private VoidEventChannelSO onPlayerRevive;

        [Header("Save properties")]
        [SerializeField] private PlayerTransform playerTransform;

        [Header("Movement Checks")]
        [SerializeField] private PlayerMovementChecks playerMovementChecks;

        private CharacterController _characterController;
        private bool _canWalk;
        private Vector2 _velocity;
        private Coroutine _velocityLock;

        private Vector3 moveDirection;

        public Vector3 MoveDirection => moveDirection;

        public float MaxSpeed
        {
            get => playerMovementProperties.maxSpeed;
            set => playerMovementProperties.maxSpeed = value;
        }

        void OnEnable()
        {
            _canWalk = true;
            _characterController ??= GetComponent<CharacterController>();
            moveDirection = Vector3.zero;

            if (playerTransform != null) playerTransform.playerTransform = transform;

            input.OnPlayerMove.AddListener(HandleMove);

            onPlayerDeath.onEvent.AddListener(HandleDeath);
            onPlayerRevive.onEvent.AddListener(HandleRevive);
            _velocity = new Vector2(playerMovementProperties.maxSpeed, 0);
        }

        private void OnDisable()
        {
            input.OnPlayerMove.RemoveListener(HandleMove);

            onPlayerDeath.onEvent.RemoveListener(HandleDeath);
            onPlayerRevive.onEvent.RemoveListener(HandleRevive);
        }

        public void OnUpdate()
        {
            Vector3 prevPos = transform.position;
            HandleYVelocityWithWallSliding();

            HandleWalk();

            _characterController.Move(_velocity * Time.deltaTime);

            SetZPosition(prevPos);
        }

        public void HandleWalk()
        {
            moveDirection = playerMovementChecks.GetSlopeMovementDirection(moveDirection);

            if (_canWalk)
                _velocity.x =
                    Mathf.Clamp(
                    _velocity.x + (moveDirection.x * playerMovementProperties.acceleration * Time.deltaTime),
                    -playerMovementProperties.maxSpeed, playerMovementProperties.maxSpeed);

            //if (_moveDirection.x == 0 && IsGrounded())
            _velocity.x = Mathf.Sign(_velocity.x) * Mathf.Clamp(Mathf.Abs(_velocity.x) - playerMovementProperties.friction * Time.deltaTime, 0, playerMovementProperties.maxSpeed);
        }

        private void HandleYVelocityWithWallSliding()
        {
            if (playerMovementChecks.IsOnSlope()) return;

            // Wallsliding logic for falldown feeling
            _velocity.y -= playerMovementChecks.IsWallSliding(moveDirection)
                ? playerMovementProperties.gravity / playerMovementProperties.wallFriction * Time.deltaTime
                : playerMovementProperties.gravity * Time.deltaTime;

            if (playerMovementChecks.IsGrounded() && _velocity.y < 0)
                _velocity.y = 0;
        }

        private void SetZPosition(Vector3 prevPos)
        {
            if (transform.position.z != 0)
                transform.position = prevPos;
        }

        private void HandleMove(Vector2 movement)
        {
            moveDirection = new Vector3(movement.x, 0, 0);
            Debug.Log(moveDirection);
        }

        public void Jump()
        {
            _velocity.y = playerMovementProperties.jumpForce;
        }

        public void WallJump()
        {
            _velocity.x = playerMovementProperties.jumpForce / 2 * Mathf.Sign(moveDirection.x) * -1;
            if (_velocityLock != null)
                StopCoroutine(_velocityLock);

            _velocityLock = StartCoroutine(LockAfterWallJump());
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

        private IEnumerator LockAfterWallJump()
        {
            _canWalk = false;
            yield return new WaitForSeconds(playerMovementProperties.lockDuration);
            _canWalk = true;
        }
    }
}