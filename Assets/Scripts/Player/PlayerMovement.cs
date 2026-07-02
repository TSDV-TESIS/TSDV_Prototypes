using System;
using System.Collections;
using Events;
using Events.Scriptables;
using Health;
using Player.Properties;
using UnityEngine;
using UnityEngine.Events;

namespace Player
{
    // TODO Handle this with an FSM, this script is large!
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Input Handler")] [SerializeField] private InputHandler input;

        [Header("Movement Properties")] [SerializeField] private PlayerMovementProperties playerMovementProperties;

        [Header("Events")] [SerializeField] private DeathEventChannelSO onPlayerDeath;
        [SerializeField] private VoidEventChannelSO onPlayerRevive;
        [SerializeField] private VoidEventChannelSO onFrenziedStart;
        [SerializeField] private VoidEventChannelSO onFrenziedStop;
        [SerializeField] private VoidEventChannelSO onPlayerSpawn;

        [Header("Save properties")] [SerializeField] private PlayerTransform playerTransform;

        [Header("Unity Events")] [SerializeField] private UnityEvent<float> onWalk;

        [SerializeField] private UnityEvent onStop;

        private CharacterController _characterController;
        private bool _canWalk;
        [NonSerialized] public Vector2 Velocity;
        private Coroutine _velocityLock;
        private Coroutine _knockBackVelocityLock;

        private Vector3 _moveDirection;
        [NonSerialized] public bool ShouldAddFasterFallingValues;

        public Vector3 MoveDirection => _moveDirection;

        private Coroutine lateStart;

        public float MaxSpeed
        {
            get => playerMovementProperties.maxSpeed;
            set => playerMovementProperties.maxSpeed = value;
        }

        private void Awake()
        {
            _characterController ??= GetComponent<CharacterController>();
        }

        void OnEnable()
        {
            _canWalk = true;
            _moveDirection = Vector3.zero;

            if (playerTransform != null) playerTransform.playerTransform = transform;

            input.OnPlayerMove.AddListener(HandleMove);
            onPlayerDeath.onTypedEvent.AddListener(HandleDeath);
            onPlayerRevive.onEvent.AddListener(HandleRevive);

            if (lateStart != null)
                StopCoroutine(lateStart);

            lateStart = StartCoroutine(LateStart());
        }

        private void Start()
        {
            onPlayerSpawn.onEvent.Invoke();
        }

        private IEnumerator LateStart()
        {
            yield return null;
            Velocity = new Vector2(playerMovementProperties.maxSpeed, 0);
        }

        private void OnDisable()
        {
            input.OnPlayerMove.RemoveListener(HandleMove);

            onPlayerDeath.onTypedEvent.RemoveListener(HandleDeath);
            onPlayerRevive.onEvent.RemoveListener(HandleRevive);
        }

        public void HandleWalk()
        {
            HandleWalk(_moveDirection);
        }

        public void HandleWalkWith(Vector3 moveDirection, Action<float, float> setVelocity)
        {
            _moveDirection = moveDirection;
            if (_canWalk)
            {
                float acceleration = playerMovementProperties.acceleration;
                float maxSpeed = playerMovementProperties.maxSpeed;

                setVelocity(acceleration, maxSpeed);
            }

            Move(Velocity * Time.deltaTime);
            SetZPosition();
        }

        public void HandleWalk(Vector3 moveDirection)
        {
            HandleWalkWith(moveDirection, SetXVelocity);
        }

        public void HandleGroundedWalk(Vector3 moveDirection)
        {
            HandleWalkWith(moveDirection, (acceleration, maxSpeed) =>
            {
                if (Mathf.Approximately(moveDirection.x, 0)) return;

                float velocityMagnitude = Mathf.Clamp(
                    Velocity.magnitude +
                    moveDirection.magnitude * acceleration * Time.deltaTime,
                    -maxSpeed, maxSpeed
                );

                Velocity = velocityMagnitude * moveDirection;
            });
        }

        private void SetXVelocity(float acceleration, float maxSpeed)
        {
            Velocity.x = Mathf.Clamp(
                Velocity.x + (_moveDirection.x) * acceleration * Time.deltaTime,
                -maxSpeed, maxSpeed
            );
        }

        public void HandleDeceleration()
        {
            float maxSpeed = playerMovementProperties.maxSpeed;
            Velocity.x = Mathf.Sign(Velocity.x) *
                         Mathf.Clamp(Mathf.Abs(Velocity.x) - playerMovementProperties.friction * Time.deltaTime, 0,
                             maxSpeed);
            if (Mathf.Abs(Velocity.x) >= playerMovementProperties.maxSpeedIdle)
            {
                onWalk?.Invoke(Velocity.x);
            }
            else
            {
                onStop?.Invoke();
            }
        }

        public void FreeFall()
        {
            float maxVelocity = playerMovementProperties.maxGravityVelocity;
            float acceleration = playerMovementProperties.gravity;

            if (ShouldAddFasterFallingValues)
            {
                maxVelocity += playerMovementProperties.maxDownPressedVelocity;
                acceleration += playerMovementProperties.maxDownPressedAddedAcceleration;
            }

            Velocity.y = Mathf.Clamp(Velocity.y - acceleration * Time.deltaTime, -maxVelocity,
                playerMovementProperties.maxJumpVelocity);
        }

        public void WallSlide()
        {
            Velocity.x = 0;
            Velocity.y = Mathf.Clamp(Velocity.y - playerMovementProperties.wallSlideGravity * Time.deltaTime,
                -playerMovementProperties.maxWallSlideVerticalVelocity, Velocity.y);

            _characterController.Move(Velocity * Time.deltaTime);
        }

        public void AddWallslideMomentum()
        {
            if (Velocity.y <= 0) return;

            Vector3 velocityDirection = Velocity.normalized;

            float angle = Vector3.Angle(
                transform.up,
                velocityDirection
            ) * Mathf.Deg2Rad;

            float cosValue = Mathf.Cos(angle);
            float influence = playerMovementProperties.wallSlideMomentumAngleInfluence.Evaluate(cosValue);

            Velocity.x = 0;
            Velocity.y = playerMovementProperties.wallSlideMomentum * influence;
        }

        public void Move(Vector3 displacement)
        {
            _characterController.Move(displacement);
        }

        public void Stop()
        {
            Velocity.x = 0;
            _canWalk = false;
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
            _moveDirection = new Vector3(movement.x, movement.y, 0);
        }

        public void Jump()
        {
            Velocity.y = playerMovementProperties.jumpForce;
        }

        public void WallJump(float checksWallSlideDirection)
        {
            Velocity.y = playerMovementProperties.wallJumpForce.y;
            Velocity.x = playerMovementProperties.wallJumpForce.x * Mathf.Sign(checksWallSlideDirection) * -1;
            if (_velocityLock != null)
                StopCoroutine(_velocityLock);

            _velocityLock = StartCoroutine(LockAfterWallJump());
        }

        public void KnockBack(int knockBackDirection)
        {
            knockBackDirection *= -1;
            knockBackDirection = Mathf.Clamp(knockBackDirection, -1, 1);
            float force = playerMovementProperties.knockBackForce;
            float angle = playerMovementProperties.knockBackAngle;
            Velocity.x = knockBackDirection * force * Mathf.Cos(angle * Mathf.Deg2Rad);
            Velocity.y = force * Mathf.Sin(angle * Mathf.Deg2Rad);
            Debug.Log($"KnockBack velocity: {Velocity}");

            if (_knockBackVelocityLock != null)
                StopCoroutine(_knockBackVelocityLock);

            _knockBackVelocityLock = StartCoroutine(KnockBackLock());
        }

        private void SetCanWalk(bool canWalk)
        {
            _canWalk = canWalk;
        }

        private void HandleDeath(DeathCauses cause)
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

        private IEnumerator KnockBackLock()
        {
            _canWalk = false;
            yield return new WaitForSeconds(playerMovementProperties.knockBackLockDuration);
            _canWalk = true;
        }

        public void Grounded(float groundY)
        {
            Velocity.y = 0;
            _characterController.enabled = false;
            transform.position = new Vector3(transform.position.x, groundY, transform.position.z);
            _characterController.enabled = true;
        }

        public void Shadowstep(Vector2 direction)
        {
            float velocityToUse = playerMovementProperties.shadowStepVelocity;
            Velocity.x = velocityToUse * direction.x;
            Velocity.y = velocityToUse * direction.y;

            Move(Velocity * Time.deltaTime);
        }

        public void OnDrawGizmos()
        {
            Gizmos.DrawLine(gameObject.transform.position, gameObject.transform.position + _moveDirection * 10f);
        }

        public void ExitShadowstep()
        {
            Velocity.y *= playerMovementProperties.exitShadowstepMomentumMantained.y;
            Velocity.x = _moveDirection.x != 0
                ? Velocity.x
                : Velocity.x * playerMovementProperties.exitShadowstepMomentumMantained.x;
        }

        public void JumpCancel()
        {
            Velocity.y = 0f;
        }
    }
}