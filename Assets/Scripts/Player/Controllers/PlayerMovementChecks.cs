using System;
using System.Collections;
using Events.Scriptables;
using Player.Properties;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player.Controllers
{
    public class PlayerMovementChecks : MonoBehaviour
    {
        [SerializeField] private float stopCheckWallSeconds = 0.5f;

        [Header("Movement Properties")] [SerializeField]
        private PlayerMovementProperties playerMovementProperties;

        [Header("Feet pivot")] [SerializeField]
        private Transform feetPivot;

        [Header("Head pivot")] [SerializeField]
        private Transform headPivot;

        [Header("Events")] [SerializeField] private FloatEventChannel onShadowstepCooldownValueEvent;

        [NonSerialized] public Vector3 WallrideHitPosition;

        private RaycastHit _groundHit;
        private RaycastHit _ceilingHit;

        private bool _isWallSliding;
        [NonSerialized] public int WallSlideDirection;
        [NonSerialized] public bool IsShadowStepOnCooldown;

        private bool _shouldCheckWall;
        private bool _shouldUnboundWall;
        private float _wallRideInCoyoteSeconds;
        private bool _inWallrideCoyoteTime;
        private Coroutine _shouldCheckWallCoroutine;
        private Coroutine _unboundWallCoroutine;
        private Coroutine _shadowstepCooldownCoroutine;

        private void OnEnable()
        {
            _shouldCheckWall = true;
            _shouldUnboundWall = false;
            _inWallrideCoyoteTime = false;
        }

        public bool IsGrounded()
        {
            return Physics.Raycast(feetPivot.position, Vector3.down, out _groundHit,
            playerMovementProperties.checkDistance, playerMovementProperties.whatIsGround);
        }

        public bool IsFalling(Vector3 moveDirection)
        {
            return moveDirection.y < 0;
        }

        private void StopUnbounding()
        {
            if (_unboundWallCoroutine != null)
                StopCoroutine(_unboundWallCoroutine);
            _unboundWallCoroutine = null;
        }

        public bool ShouldUnboundWallslide(Vector3 moveDirection, Vector2 movementVelocity)
        {
            return ShouldUnboundByInput(moveDirection) || ShouldUnboundByCoyote(movementVelocity);
        }

        private bool ShouldUnboundByCoyote(Vector2 movementVelocity)
        {
            if (!WallRaycast(WallSlideDirection))
            {
                if (movementVelocity.y > 0)
                {
                    _wallRideInCoyoteSeconds = 0;
                    _inWallrideCoyoteTime = false;
                    return true;
                }

                if (!_inWallrideCoyoteTime)
                {
                    _inWallrideCoyoteTime = true;
                    _wallRideInCoyoteSeconds = 0;
                }

                _wallRideInCoyoteSeconds += Time.deltaTime;

                if (_wallRideInCoyoteSeconds > playerMovementProperties.wallRideMaxCoyoteSeconds)
                {
                    _wallRideInCoyoteSeconds = 0;
                    _inWallrideCoyoteTime = false;
                    return true;
                }

                return false;
            }
            else
            {
                _inWallrideCoyoteTime = false;
                _wallRideInCoyoteSeconds = 0;
                return false;
            }
        }

        private bool ShouldUnboundByInput(Vector3 moveDirection)
        {
            Debug.Log($"VALUES: {_shouldCheckWall} {_shouldUnboundWall} {_isWallSliding} {_unboundWallCoroutine != null}");
            if (!_shouldCheckWall)
            {
                StopUnbounding();
                _shouldUnboundWall = false;
                return true;
            }

            if (Mathf.Sign(moveDirection.x) == Mathf.Sign(WallSlideDirection))
            {
                StopUnbounding();
                return false;
            }

            if (_shouldUnboundWall)
            {
                _shouldUnboundWall = false;
                _isWallSliding = false;
                StopUnbounding();
                return true;
            }

            _unboundWallCoroutine ??= StartCoroutine(UnboundWallCoroutine());
            return false;
        }

        private IEnumerator UnboundWallCoroutine()
        {
            yield return new WaitForSeconds(playerMovementProperties.unboundTime);
            _shouldUnboundWall = true;
        }

        public bool ShouldWallSlide(Vector3 moveDirection, Vector2 velocity)
        {
            if (!_shouldCheckWall) return false;

            int signToCheck = Math.Sign(Math.Abs(velocity.x) > playerMovementProperties.wallVelocityCheck
                ? velocity.x
                : moveDirection.x);
            _isWallSliding = moveDirection.x != 0 && WallRaycast(signToCheck);

            WallSlideDirection = _isWallSliding ? signToCheck : 0;
            return _isWallSliding;
        }

        public bool IsNearCeiling()
        {
            if (Physics.Raycast(headPivot.position, Vector3.up, out _ceilingHit,
                playerMovementProperties.checkDistance))
            {
                Debug.Log($"Normal: {_ceilingHit.normal} is equal to V3.Down {_ceilingHit.normal == Vector3.down}");
                return _ceilingHit.normal == Vector3.down;
            }

            return false;
        }

        public bool IsNearCorner(out float cornerDisplace)
        {
            Vector3 displacement = Vector3.right * playerMovementProperties.cornerCorrectionMaxDistance;
            if (Physics.Raycast(headPivot.position - displacement, Vector3.up, out RaycastHit _leftCornerHit, playerMovementProperties.checkDistance) ^
                Physics.Raycast(headPivot.position + displacement, Vector3.up, out RaycastHit _rightCornerHit, playerMovementProperties.checkDistance))
            {
                if (_leftCornerHit.normal == Vector3.down)
                {
                    cornerDisplace = transform.position.x - _leftCornerHit.point.x;
                    return true;
                }

                if (_rightCornerHit.normal == Vector3.down)
                {
                    cornerDisplace =transform.position.x -  _rightCornerHit.point.x;
                    return true;
                }
            }

            cornerDisplace = 0;
            return false;
        }

        public bool WallRaycast(int signToCheck)
        {
            bool hasRaycast = Physics.Raycast(feetPivot.position, Vector3.right * signToCheck,
            playerMovementProperties.wallCheckDistance,
            playerMovementProperties.whatIsWall);

            if (hasRaycast)
            {
                WallrideHitPosition = feetPivot.position + Vector3.right * signToCheck;
            }

            return hasRaycast;
        }

        public bool WallRaycast(out int signThatHits)
        {
            if (WallRaycast(-1))
            {
                signThatHits = -1;
                return true;
            } if (WallRaycast(1))
            {
                signThatHits = 1;
                return true;
            }

            signThatHits = 0;
            return false;
        }

        public bool IsOnSlope()
        {
            if (!IsGrounded()) return false;

            float angle = Vector3.Angle(Vector3.up, _groundHit.normal);

            return angle < playerMovementProperties.maxSlopeAngle && !Mathf.Approximately(angle, 0);
        }

        public Vector3 GetSlopeMovementDirection(Vector3 moveDirection)
        {
            return Vector3.ProjectOnPlane(moveDirection, _groundHit.normal).normalized;
        }

        public void StopCheckingWall()
        {
            if (_shouldCheckWallCoroutine != null) StopCoroutine(_shouldCheckWallCoroutine);
            StopUnbounding();
            _shouldCheckWallCoroutine = StartCoroutine(HandleStopCheckWall());
        }

        private IEnumerator HandleStopCheckWall()
        {
            _shouldCheckWall = false;
            _isWallSliding = false;
            yield return new WaitForSeconds(stopCheckWallSeconds);
            _shouldCheckWall = true;
        }

        private void OnDrawGizmos()
        {
            if (playerMovementProperties.shouldDrawGizmos)
            {
                // Draw feet raycast
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(feetPivot.position,
                feetPivot.position + Vector3.down * playerMovementProperties.checkDistance);

                // Wall raycast
                Gizmos.color = Color.green;
                Gizmos.DrawLine(feetPivot.position,
                feetPivot.position + Vector3.right * playerMovementProperties.wallCheckDistance);
                Gizmos.DrawLine(feetPivot.position,
                feetPivot.position + Vector3.left * playerMovementProperties.wallCheckDistance);
            }
        }

        public bool IsNearWall()
        {
            return WallRaycast(out WallSlideDirection);
        }

        public void SetShadowstepOnCooldown()
        {
            if(_shadowstepCooldownCoroutine != null) StopCoroutine(_shadowstepCooldownCoroutine);
            _shadowstepCooldownCoroutine = StartCoroutine(ShadowStepOnCooldown());
        }

        private IEnumerator ShadowStepOnCooldown()
        {
            float timer = 0;
            IsShadowStepOnCooldown = true;
            while (timer < playerMovementProperties.shadowStepCooldown)
            {
                onShadowstepCooldownValueEvent?.RaiseEvent((float)(timer / playerMovementProperties.shadowStepCooldown));
                timer += Time.deltaTime;
                yield return null;
            }

            onShadowstepCooldownValueEvent?.RaiseEvent(1);
            IsShadowStepOnCooldown = false;
        }
    }
}
