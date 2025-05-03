using FSM;
using UnityEngine;

namespace Player.Controllers
{
    [RequireComponent(typeof(PlayerMovement))]
    public class FallingController : Controller<PlayerAgent>
    {
        private PlayerMovement _playerMovement;

        private void OnEnable()
        {
            _playerMovement ??= GetComponent<PlayerMovement>();
        }

        public override void OnUpdate()
        {
            _playerMovement.HandleWalk();
            _playerMovement.FreeFall();

            if (agent.Checks.IsGrounded())
                agent.ChangeStateToGrounded();

            if (agent.Checks.ShouldWallSlide(_playerMovement.MoveDirection, _playerMovement.Velocity))
                agent.ChangeStateToWallSlide();
        }
    }
}