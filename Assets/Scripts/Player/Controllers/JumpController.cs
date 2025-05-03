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
            _playerMovement.HandleWalk();
            _playerMovement.FreeFall();

            if (agent.Checks.IsFalling(_playerMovement.Velocity))
                agent.ChangeStateToFalling();

            float cornerDisplace = 0;

            if (agent.Checks.IsNearCorner(out cornerDisplace))
            {
                _playerMovement.Move(new Vector3(cornerDisplace, 0, 0));
            }
            else
            {
                if (agent.Checks.IsNearCeiling())
                {
                    _playerMovement.SetVerticalVelocity(-playerMovementProperties.gravity * Time.deltaTime);
                    agent.ChangeStateToFalling();
                }
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