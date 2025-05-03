using System;
using FSM;
using UnityEngine;

namespace Player.Controllers
{
    [RequireComponent(typeof(PlayerMovement))]
    public class GroundedController : Controller<PlayerAgent>
    {
        private PlayerMovement _playerMovement;
        [SerializeField] private InputHandler inputHandler;
        [SerializeField] private float unboundWallBufferSeconds = 0.75f;

        private void OnEnable()
        {
            _playerMovement ??= GetComponent<PlayerMovement>();
            inputHandler.OnPlayerJump.AddListener(OnJump);
        }

        private void OnDisable()
        {
            inputHandler.OnPlayerJump.RemoveListener(OnJump);
        }

        public override void OnUpdate()
        {
            _playerMovement.HandleWalk(agent.Checks.GetSlopeMovementDirection(_playerMovement.MoveDirection));
            _playerMovement.HandleDeceleration();
            
            if (agent.Checks.IsNearCeiling() && _playerMovement.Velocity.y > 0)
                _playerMovement.SetVerticalVelocity(0);

            if (!agent.Checks.IsGrounded())
                agent.ChangeStateToFalling();
        }

        private void OnJump()
        {
            agent.ChangeStateToJumping();
        }
    }
}