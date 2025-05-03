using System;
using FSM;
using Player.Properties;
using UnityEngine;

namespace Player.Controllers
{
    [RequireComponent(typeof(PlayerMovement))]
    public class JumpController : Controller<PlayerAgent>
    {
        private PlayerMovement _playerMovement;
        [SerializeField] private PlayerMovementProperties playerMovementProperties;

        private void OnEnable()
        {
            _playerMovement ??= GetComponent<PlayerMovement>();
        }

        public override void OnUpdate()
        {
            _playerMovement.OnUpdate();
            _playerMovement.FreeFall();

            if (agent.Checks.IsFalling(_playerMovement.Velocity))
                agent.ChangeStateToFalling();

            if (agent.Checks.IsNearCeiling())
            {
                _playerMovement.SetVerticalVelocity(-playerMovementProperties.gravity * Time.deltaTime);
                agent.ChangeStateToFalling();
            }

            if (agent.Checks.IsGrounded())
                agent.ChangeStateToGrounded();

            if (agent.Checks.ShouldWallSlide(_playerMovement.MoveDirection, _playerMovement.Velocity))
                agent.ChangeStateToWallSlide();
        }

        public void Jump()
        {
            _playerMovement.Jump();
        }
    }
}