using UnityEngine;

namespace Player.Properties
{
    [CreateAssetMenu(fileName = "PlayerMovementProperties", menuName = "Scriptable Objects/PlayerMovementProperties")]
    public class PlayerMovementProperties : ScriptableObject
    {
        [Header("Movement on ground")]
        public float acceleration;
        public float friction;
        public float maxSpeed;

        [Header("Jumping properties")]
        public float gravity;
        public float jumpForce;

        [Header("Grounding properties")]
        [Tooltip("Distance from where it should start checking that player is grounded")]
        public float checkDistance;
        public LayerMask whatIsGround;

        [Header("Wall Slide properties")]
        public float wallCheckDistance;
        public LayerMask whatIsWall;
        public float lockDuration;
        public float wallSlideGravity;
        public float maxWallSlideVerticalVelocity;

        [Header("Slope properties")]
        public float maxSlopeAngle = 45f;

        [Header("Gizmos")]
        public bool shouldDrawGizmos = false;
    }
}