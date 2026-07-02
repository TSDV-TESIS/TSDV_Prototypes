using System;
using Events;
using Events.Scriptables;
using FSM;
using UnityEngine;
using UnityEngine.Events;

namespace Player.Controllers
{
    [RequireComponent(typeof(PlayerMovement))]
    public class GroundedController : Controller<PlayerAgent>
    {
        private PlayerMovement _playerMovement;
        [SerializeField] private InputHandler inputHandler;
        [SerializeField] private float unboundWallBufferSeconds = 0.75f;
        [SerializeField] private StepsMaterialEventChannelSO onWalkEvent;
        [SerializeField] private VoidEventChannelSO onStopWalkEvent;

        private void OnEnable()
        {
            _playerMovement ??= GetComponent<PlayerMovement>();
            inputHandler.OnPlayerJump.AddListener(OnJump);
            inputHandler.OnPlayerShadowStep.AddListener(OnShadowstep);
            inputHandler.OnPlayerAttack.AddListener(OnAttack);

            agent.MovementChecks.ResetShadowStepsOnAir();
            agent.AttackChecks.ResetAttacksOnJump();
        }

        private void Start()
        {
            float clearance = agent.MovementChecks.GetGroundClearance();
            if (clearance == 0) return;

            _playerMovement.Grounded(clearance);
        }

        private void OnDisable()
        {
            inputHandler.OnPlayerJump.RemoveListener(OnJump);
            inputHandler.OnPlayerShadowStep.RemoveListener(OnShadowstep);
            inputHandler.OnPlayerAttack.RemoveListener(OnAttack);
        }

        public override void OnUpdate()
        {
            _playerMovement.HandleGroundedWalk(agent.MovementChecks.GetSlopeMovementDirection(_playerMovement.MoveDirection));
            _playerMovement.HandleDeceleration();

            if (agent.MovementChecks.IsNearCeiling() && _playerMovement.Velocity.y > 0)
                _playerMovement.SetVerticalVelocity(0);

            if (!agent.MovementChecks.IsGrounded())
                agent.ChangeStateToFalling();
            else if (_playerMovement.Velocity.x != 0)
            {
                if (agent.MovementChecks.GroundHit.transform.TryGetComponent(out WalkableFloor floor))
                    onWalkEvent?.RaiseEvent(floor.Material);
            }
            else
                onStopWalkEvent?.RaiseEvent();
        }

        private void OnJump()
        {
            agent.ChangeStateToJumping();
        }

        private void OnShadowstep()
        {
            agent.ChangeStateToShadowStep();
        }

        private void OnAttack()
        {
            agent.ChangeStateToAttack();
        }
    }
}