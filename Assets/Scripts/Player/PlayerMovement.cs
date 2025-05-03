using System;
using System.Collections;
using Events;
using Player.Controllers;
using Player.Properties;
using UnityEngine;
using UnityEngine.Events;

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

        [Header("Events")]
        [SerializeField] private UnityEvent<float> onWalk;
        [SerializeField] private UnityEvent onStop;

        private CharacterController _characterController;
        private bool _canWalk;
        [NonSerialized] public Vector2 Velocity;
        private Coroutine _velocityLock;

        private Vector3 _moveDirection;

        public Vector3 MoveDirection => _moveDirection;

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

            if (playerTransform != null) playerTransform.playerTransform = transform;

            input.OnPlayerMove.AddListener(HandleMove);

            onPlayerDeath.onEvent.AddListener(HandleDeath);
            onPlayerRevive.onEvent.AddListener(HandleRevive);
            Velocity = new Vector2(playerMovementProperties.maxSpeed, 0);
        }

        private void OnDisable()
        {
            input.OnPlayerMove.RemoveListener(HandleMove);

            onPlayerDeath.onEvent.RemoveListener(HandleDeath);
            onPlayerRevive.onEvent.RemoveListener(HandleRevive);
        }

        public void HandleWalk()
        {
            HandleWalk(_moveDirection);
        }

        public void HandleWalk(Vector3 moveDirection)
        {
            _moveDirection = moveDirection;
            Vector3 prevPos = transform.position;

            Velocity.x = Mathf.Clamp(
            Velocity.x + (_moveDirection.x * playerMovementProperties.acceleration * Time.deltaTime),
            -playerMovementProperties.maxSpeed, playerMovementProperties.maxSpeed);

            Move(Velocity * Time.deltaTime);
            SetZPosition(prevPos);
        }

        public void HandleDeceleration()
        {
            Velocity.x = Mathf.Sign(Velocity.x) * Mathf.Clamp(Mathf.Abs(Velocity.x) - playerMovementProperties.friction * Time.deltaTime, 0, playerMovementProperties.maxSpeed);
            if (Velocity.x != 0)
                onWalk?.Invoke(Mathf.Sign(Velocity.x));
            else
                onStop?.Invoke();
        }

        public void FreeFall()
        {
            Velocity.y = Mathf.Clamp(Velocity.y - playerMovementProperties.gravity * Time.deltaTime, -playerMovementProperties.maxGravityVelocity, playerMovementProperties.maxJumpVelocity);
        }

        public void WallSlide()
        {
            Velocity.x = 0;
            Velocity.y = Mathf.Clamp(Velocity.y - playerMovementProperties.wallSlideGravity * Time.deltaTime, -playerMovementProperties.maxWallSlideVerticalVelocity, Velocity.y);

            _characterController.Move(Velocity * Time.deltaTime);
        }

        public void Move(Vector3 displacement)
        {
            _characterController.Move(displacement);
        }

        private void SetZPosition(Vector3 prevPos)
        {
            if (transform.position.z != 0)
                transform.position = prevPos;
        }

        public void SetVerticalVelocity(float value)
        {
            Velocity.y = value;
        }

        private void HandleMove(Vector2 movement)
        {
            _moveDirection = new Vector3(movement.x, 0, 0);
        }

        public void Jump()
        {
            Velocity.y = playerMovementProperties.jumpForce;
        }

        public void WallJump(float checksWallSlideDirection)
        {
            Velocity.y = playerMovementProperties.wallJumpForce.y;
            Velocity.x = playerMovementProperties.wallJumpForce.x * Mathf.Sign(checksWallSlideDirection) * -1;
            Debug.Log(Velocity.x);
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

        public void StopWallSlide()
        {
            Velocity.y = 0;
        }
    }
}