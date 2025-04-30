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

        private bool _isActive;
        private void OnEnable()
        {
            _movement ??= GetComponent<PlayerMovement>();
            _isActive = false;
        }

        private void OnDisable()
        {
            if(_isActive)
                input.OnPlayerJump.RemoveListener(OnJump);
        }

        public void OnEnter()
        {
            _isActive = true;
            input.OnPlayerJump.AddListener(OnJump);
        }

        public override void OnUpdate()
        {
            _movement.WallSlide();

            if (agent.Checks.IsGrounded())
            {
                _movement.StopWallSlide();
                agent.ChangeStateToGrounded();
            }

            if (!agent.Checks.ShouldWallSlide(_movement.MoveDirection))
                agent.ChangeStateToFalling();
        }

        public void OnLeave()
        {
            _isActive = false;
            input.OnPlayerJump.RemoveListener(OnJump);
        }

        private void OnJump()
        {
            _movement.WallJump();
            agent.Checks.StopCheckingWall();
            agent.ChangeStateToJumping();
        }
    }
}