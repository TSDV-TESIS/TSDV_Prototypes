using System;
using FSM;
using UnityEngine;

namespace Player.Controllers
{
    [RequireComponent(typeof(PlayerMovement))]
    public class JumpController : Controller<PlayerAgent>
    {
        private PlayerMovement _playerMovement;

        private void OnEnable()
        {
            _playerMovement ??= GetComponent<PlayerMovement>();
        }

        public override void OnUpdate()
        {
            _playerMovement.OnUpdate();
            _playerMovement.FreeFall();

            if (agent.Checks.IsFalling(_playerMovement.MoveDirection))
                agent.ChangeStateToFalling();

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