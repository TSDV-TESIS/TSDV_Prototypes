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
        [SerializeField] private PlayerAnimationController animationController;
        [SerializeField] private GameObject feetPivot;
        
        [Header("Input Handler")]
        [SerializeField] private InputHandler input;

        [Header("Movement Properties")]
        [SerializeField] private PlayerMovementProperties playerMovementProperties;

        [Header("Events")]
        [SerializeField] private VoidEventChannelSO onPlayerDeath;
        [SerializeField] private VoidEventChannelSO onPlayerRevive;
        [SerializeField] private VoidEventChannelSO onFrenziedStart;
        [SerializeField] private VoidEventChannelSO onFrenziedStop;
        
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

        private bool _isFrenzied;

        public float MaxSpeed
        {
            get => playerMovementProperties.maxSpeed;
            set => playerMovementProperties.maxSpeed = value;
        }

        public bool IsAttacking;

        void OnEnable()
        {
            IsAttacking = false;
            _canWalk = true;
            _characterController ??= GetComponent<CharacterController>();
            _moveDirection = Vector3.zero;

            if (playerTransform != null) playerTransform.playerTransform = transform;

            input.OnPlayerMove.AddListener(HandleMove);

            onPlayerDeath.onEvent.AddListener(HandleDeath);
            onPlayerRevive.onEvent.AddListener(HandleRevive);
            onFrenziedStart.onEvent.AddListener(HandleIsFrenzied);
            onFrenziedStop.onEvent.AddListener(HandleStopFrenzy);
            Velocity = new Vector2(playerMovementProperties.maxSpeed, 0);
        }

        private void OnDisable()
        {
            input.OnPlayerMove.RemoveListener(HandleMove);

            onPlayerDeath.onEvent.RemoveListener(HandleDeath);
            onPlayerRevive.onEvent.RemoveListener(HandleRevive);
            
            onFrenziedStart.onEvent.RemoveListener(HandleIsFrenzied);
            onFrenziedStop.onEvent.RemoveListener(HandleStopFrenzy);
        }

        private void HandleStopFrenzy()
        {
            _isFrenzied = false;
        }

        private void HandleIsFrenzied()
        {
            _isFrenzied = true;
        }

        public void HandleWalk()
        {
            HandleWalk(_moveDirection);
        }

        public void HandleWalk(Vector3 moveDirection)
        {
            if (IsAttacking) return;
            _moveDirection = moveDirection;
            
            if (_moveDirection != Vector3.zero)
            {
                animationController.HandleWalk();
            }
            else
            {
                animationController.HandleIdle();
            }
            
            if (_canWalk)
            {
                float acceleration = _isFrenzied
                    ? playerMovementProperties.frenziedAcceleration
                    : playerMovementProperties.acceleration;

                float maxSpeed = _isFrenzied
                    ? playerMovementProperties.frenziedMaxSpeed
                    : playerMovementProperties.maxSpeed;

                Velocity.x = Mathf.Clamp(
                    Velocity.x + (_moveDirection.x) * acceleration * Time.deltaTime,
                    -maxSpeed, maxSpeed
                );
            }

            Move(Velocity * Time.deltaTime);
            SetZPosition();
        }
        
        public void HandleGroundedWalk(Vector3 moveDirection, float offsetToGround)
        {
            if (IsAttacking) return;
            _moveDirection = moveDirection;

            if (_canWalk)
            {
                float acceleration = _isFrenzied
                    ? playerMovementProperties.frenziedAcceleration
                    : playerMovementProperties.acceleration;

                float maxSpeed = _isFrenzied
                    ? playerMovementProperties.frenziedMaxSpeed
                    : playerMovementProperties.maxSpeed;

                float velocityToUse = Mathf.Clamp(
                    Velocity.magnitude + (_moveDirection.magnitude) * acceleration * Time.deltaTime,
                    -maxSpeed, maxSpeed
                );

                Velocity = _moveDirection * velocityToUse;
            }

            Move(Velocity * Time.deltaTime);
            SetGroundPosition(offsetToGround);
            SetZPosition();
        }

        private void SetGroundPosition(float offset)
        {
            Debug.Log($"GROUND POSITION: {offset}");
            var vector3 = transform.position;
            vector3.y = offset;
            transform.position = vector3;
        }

        public void HandleDeceleration()
        {
            float maxSpeed = _isFrenzied
                ? playerMovementProperties.frenziedMaxSpeed
                : playerMovementProperties.maxSpeed;
            Velocity.x = Mathf.Sign(Velocity.x) * Mathf.Clamp(Mathf.Abs(Velocity.x) - playerMovementProperties.friction * Time.deltaTime, 0, maxSpeed);
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

        private void SetZPosition()
        {
            if (transform.position.z != 0)
            {
                var vector3 = transform.position;
                vector3.z = 0;
                transform.position = vector3;
            }
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

        private void SetCanWalk(bool canWalk)
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

        public void Grounded()
        {
            Velocity.y = 0;
        }

        public int GetMoveDirectionSign()
        {
            return (int)Mathf.Sign(_moveDirection.x);
        }

        public void Shadowstep(Vector2 direction, bool isBloodstep)
        {
            float velocityToUse = isBloodstep ? playerMovementProperties.bloodStepVelocity : playerMovementProperties.shadowStepVelocity;
            Velocity.x = velocityToUse * direction.x;
            Velocity.y = velocityToUse * direction.y;

            Move(Velocity * Time.deltaTime);
        }

        public void OnDrawGizmos()
        {
            Gizmos.DrawLine(gameObject.transform.position, gameObject.transform.position + _moveDirection * 10f);
        }
    }
}