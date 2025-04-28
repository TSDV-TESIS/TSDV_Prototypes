using Player.Properties;
using UnityEngine;

namespace Player.Controllers
{
    public class PlayerMovementChecks : MonoBehaviour
    {
        [Header("Movement Properties")]
        [SerializeField] private PlayerMovementProperties playerMovementProperties;

        [Header("Feet pivot")]
        [SerializeField] private Transform feetPivot;

        private RaycastHit _groundHit;

        private bool _isWallSliding;
        private float _wallSlideDirection;

        public bool IsGrounded()
        {
            return Physics.Raycast(feetPivot.position, Vector3.down, out _groundHit,
            playerMovementProperties.checkDistance, playerMovementProperties.whatIsGround);
        }

        public bool IsFalling(Vector3 moveDirection)
        {
            return moveDirection.y < 0;
        }

        public bool ShouldWallSlide(Vector3 moveDirection)
        {
            if (_isWallSliding)
            {
                _isWallSliding = Physics.Raycast(transform.position, Vector3.right * Mathf.Sign(_wallSlideDirection), playerMovementProperties.wallCheckDistance, playerMovementProperties.whatIsWall);

                if (moveDirection.x != 0 && Mathf.Sign(moveDirection.x) != Mathf.Sign(_wallSlideDirection))
                    _isWallSliding = false;

                return _isWallSliding;
            }

            _isWallSliding = moveDirection.x != 0 && Physics.Raycast(transform.position, Vector3.right * Mathf.Sign(moveDirection.x), playerMovementProperties.wallCheckDistance, playerMovementProperties.whatIsWall);
            _wallSlideDirection = _isWallSliding ? moveDirection.x : 0;
            return _isWallSliding;
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
        // private void OnDrawGizmos()
        // {
        //     if (playerMovementProperties.shouldDrawGizmos)
        //     {
        //         Gizmos.color = Color.blue;
        //         Gizmos.DrawLine(feetPivot.position,
        //         feetPivot.position + Vector3.down * playerMovementProperties.checkDistance);
        //         Gizmos.DrawLine(transform.position, transform.position + _moveDirection * 10f);
        //     }
        // }
    }
}