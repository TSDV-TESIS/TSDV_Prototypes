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

            if (agent.Checks.IsFalling(_playerMovement.MoveDirection))
                agent.ChangeStateToFalling();

            if (agent.Checks.IsGrounded())
                agent.ChangeStateToGrounded();
        }

        public void Jump()
        {
            _playerMovement.Jump();
        }
    }
}