using System;
using FSM;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Player.Controllers
{
    [RequireComponent(typeof(PlayerMovement))]
    public class WallSlideController : Controller<PlayerAgent>
    {
        [SerializeField] private InputHandler input;
        private PlayerMovement _movement;

        [Header("Events")] 
        [SerializeField] private UnityEvent<Vector3, int> onWallHitEnter;

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
            onWallHitEnter.Invoke(agent.Checks.WallrideHitPosition, agent.Checks.WallSlideDirection);
        }

        public override void OnUpdate()
        {
            _movement.WallSlide();

            if (agent.Checks.IsGrounded())
            {
                _movement.Grounded();
                agent.ChangeStateToGrounded();
            }

            if (agent.Checks.ShouldUnboundWallslide(_movement.MoveDirection, _movement.Velocity))
                agent.ChangeStateToFalling();
        }

        public void OnLeave()
        {
            _isActive = false;
            input.OnPlayerJump.RemoveListener(OnJump);
        }

        private void OnJump()
        {
            _movement.WallJump(agent.Checks.WallSlideDirection);
            agent.Checks.StopCheckingWall();
            agent.ChangeStateToJumping();
        }
    }
}