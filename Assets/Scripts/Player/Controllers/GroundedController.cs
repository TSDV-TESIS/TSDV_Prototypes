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

        private void OnEnable()
        {
            _playerMovement ??= GetComponent<PlayerMovement>();
            inputHandler.OnPlayerJump.AddListener(OnJump);
            agent.Checks.ClearSlidedWall();
        }

        private void OnDisable()
        {
            inputHandler.OnPlayerJump.RemoveListener(OnJump);
        }

        public override void OnUpdate()
        {
            _playerMovement.OnUpdate();

            if (!agent.Checks.IsGrounded())
                agent.ChangeStateToFalling();
        }

        private void OnJump()
        {
            agent.ChangeStateToJumping();
        }
    }
}