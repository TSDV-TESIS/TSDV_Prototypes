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

        [Header("Frenzied movement")] 
        public float frenziedAcceleration;
        public float frenziedMaxSpeed;

        [Header("Jumping properties")]
        public float gravity;
        public float jumpForce;
        public float maxGravityVelocity;
        public float maxJumpVelocity;
        public float cornerCorrectionMaxDistance;
        
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
        public Vector2 wallJumpForce;
        [Tooltip("Time buffer with which the player has to input the unbound button")]
        public float unboundTime = 0.5f;
        [Tooltip("Min velocity with which it automatically checks for wallslide")]
        public float wallVelocityCheck = 20f;
        public float wallRideMaxCoyoteSeconds = 0.5f;

        [Header("Slope properties")]
        public float maxSlopeAngle = 45f;

        [Header("Gizmos")]
        public bool shouldDrawGizmos = false;

        [Header("ShadowStep")]
        public float shadowStepTime = 0.5f;

        public float shadowStepVelocity = 10f;
        public double shadowStepCooldown = 1f;
        public float bloodStepVelocity;
        public float bloodStepTime;
        public float ceilingCheckWaitTime = 0.5f;
    }
}
