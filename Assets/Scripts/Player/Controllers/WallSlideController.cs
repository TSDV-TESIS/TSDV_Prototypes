using System;
using FSM;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player.Controllers
{
    [RequireComponent(typeof(PlayerMovement))]
    public class WallSlideController : Controller<PlayerAgent>
    {
        [SerializeField] private InputHandler input;
        private PlayerMovement _movement;

        private void OnEnable()
        {
            _movement ??= GetComponent<PlayerMovement>();
            input.OnPlayerJump.AddListener(OnJump);
        }

        private void OnDisable()
        {
            input.OnPlayerJump.RemoveListener(OnJump);
        }

        public override void OnUpdate()
        {
            _movement.WallSlide();

            if (agent.Checks.IsGrounded())
                agent.ChangeStateToGrounded();

            if (!agent.Checks.ShouldWallSlide(_movement.MoveDirection))
                agent.ChangeStateToFalling();
        }

        private void OnJump()
        {
            _movement.WallJump();
            agent.ChangeStateToJumping();
        }
    }
}