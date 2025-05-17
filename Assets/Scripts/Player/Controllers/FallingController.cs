using System;
using FSM;
using Player.Properties;
using UnityEngine;

namespace Player.Controllers
{
    [RequireComponent(typeof(PlayerMovement))]
    public class FallingController : Controller<PlayerAgent>
    {
        private PlayerMovement _playerMovement;
        [SerializeField] private InputHandler inputHandler;
        [SerializeField] private PlayerMovementProperties playerMovementProperties;


        private void OnEnable()
        {
            _playerMovement ??= GetComponent<PlayerMovement>();
            inputHandler?.OnPlayerShadowStep.AddListener(HandleShadowstep);
        }

        private void Start()
        {
            inputHandler?.OnPlayerShadowStep.RemoveListener(HandleShadowstep);
        }

        private void OnDisable()
        {
            inputHandler?.OnPlayerShadowStep.RemoveListener(HandleShadowstep);
        }

        private void HandleShadowstep()
        {
            agent.ChangeStateToShadowStep();
        }

        public override void OnUpdate()
        {
            _playerMovement.HandleWalk();
            _playerMovement.FreeFall();

            if (agent.Checks.IsNearCeiling())
            {
                _playerMovement.SetVerticalVelocity(-playerMovementProperties.gravity * Time.deltaTime);
            }

            if (agent.Checks.IsGrounded())
            {
                _playerMovement.Grounded();
                agent.ChangeStateToGrounded();
            }

            if (agent.Checks.ShouldWallSlide(_playerMovement.MoveDirection, _playerMovement.Velocity))
                agent.ChangeStateToWallSlide();
        }
    }
}