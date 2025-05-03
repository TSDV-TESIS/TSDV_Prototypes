using System;
using System.Collections;
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

        [NonSerialized] public Vector3 WallrideHitPosition;
        
        private RaycastHit _groundHit;

        private bool _isWallSliding;
        [NonSerialized] public int WallSlideDirection;

        private bool _shouldCheckWall;
        private bool _shouldUnboundWall;
        private float _wallRideInCoyoteSeconds;
        private bool _inWallrideCoyoteTime;
        private Coroutine _shouldCheckWallCoroutine;
        private Coroutine _unboundWallCoroutine;

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
            if(_unboundWallCoroutine != null)
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
    }
}