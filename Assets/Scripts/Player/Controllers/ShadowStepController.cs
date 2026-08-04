using System.Collections;
using Events;
using FSM;
using Health;
using Player.Properties;
using UnityEngine;

namespace Player.Controllers
{
    [RequireComponent(typeof(PlayerMovement))]
    public class ShadowStepController : Controller<PlayerAgent>
    {
        [SerializeField] private PlayerMovementProperties playerMovementProperties;
        [SerializeField] private GameObject bloodStepCollider;
        [SerializeField] private Vector3 displacementIfBlocked = new Vector3(0.01f, 0.01f, 0);
        [SerializeField] private VoidEventChannelSO onShadowStep;
        [SerializeField] private VoidEventChannelSO onShadowStepStop;

        private PlayerMovement _playerMovement;
        private MouseLook _mouseLook;
        private CharacterController _characterController;
        private CapsuleCollider _collider;
        private HealthPoints _healthPoints;
        private Coroutine _shadowstepCoroutine;

        private void OnEnable()
        {
            _playerMovement ??= GetComponent<PlayerMovement>();
            _mouseLook ??= GetComponent<MouseLook>();
            _characterController ??= GetComponent<CharacterController>();
            _collider ??= GetComponent<CapsuleCollider>();
            _healthPoints ??= GetComponent<HealthPoints>();
        }

        public void OnEnter()
        {
            if (_shadowstepCoroutine != null) StopCoroutine(_shadowstepCoroutine);
            _shadowstepCoroutine = StartCoroutine(Shadowstep());

            onShadowStepStop.onEvent.AddListener(StopShadowstep);
        }

        public override void OnUpdate()
        {
        }

        private IEnumerator Shadowstep()
        {
            Debug.Log("START Shadowstep");
            onShadowStep?.RaiseEvent();

            float timer = 0;
            Vector2 direction = _mouseLook.CursorDir.normalized;
            bool changedToWallslide = false;

            _characterController.excludeLayers |= playerMovementProperties.avoidableObjects;
            _collider.excludeLayers |= playerMovementProperties.avoidableObjects;

            float duration = playerMovementProperties.shadowStepTime;

            while (timer < duration && !changedToWallslide)
            {
                if (timer >= playerMovementProperties.shadowStepIframes.x && timer <= playerMovementProperties.shadowStepIframes.y)
                    _healthPoints.SetCanTakeDamage(false);
                else
                    _healthPoints.SetCanTakeDamage(true);

                if (agent.MovementChecks.IsNearCeiling())
                {
                    Debug.Log("STOP BECAUSE NEAR CEILING");
                    _playerMovement.SetVerticalVelocity(-playerMovementProperties.gravity * Time.deltaTime);
                    _healthPoints.SetCanTakeDamage(true);
                    break;
                }


                _playerMovement.Shadowstep(direction);

                if (agent.MovementChecks.IsNearNonAvoidableWall())
                {
                    Debug.Log("STOP BECAUSE WALLSLIDE");
                    changedToWallslide = true;
                    agent.MovementChecks.SetShadowstepOnCooldown();
                }

                timer += Time.deltaTime;
                yield return null;
            }

            _characterController.excludeLayers ^= playerMovementProperties.avoidableObjects;
            _collider.excludeLayers ^= playerMovementProperties.avoidableObjects;

            _characterController.Move(displacementIfBlocked);
            ExitShadowstep();
        }

        private void ExitShadowstep()
        {
            Debug.Log("Exiting");
            agent.MovementChecks.SetShadowstepOnCooldown();
            _playerMovement.ExitShadowstep();
            Debug.Log("Exited normal stuff");

            if (agent.MovementChecks.IsNearNonAvoidableWall())
            {
                Debug.Log("WALLSOLIDE");
                if (!agent.MovementChecks.IsGrounded())
                {
                    Debug.LogWarning("WALLSLIDE");
                    agent.ChangeStateToWallSlide();
                }
                else
                    agent.ChangeStateToKnockBack();
            }
            else if (!agent.MovementChecks.IsGrounded())
            {
                Debug.Log("Not wallslide... falling!");
                agent.MovementChecks.SetShadowStepOnAirUsed();
                agent.ChangeStateToFalling();
            }
            else
            {
                Debug.Log("Grounded!");
                agent.ChangeStateToGrounded();
            }

            onShadowStepStop.onEvent.RemoveListener(StopShadowstep);
        }

        private void StopShadowstep()
        {
            if (_shadowstepCoroutine != null) StopCoroutine(_shadowstepCoroutine);
            ExitShadowstep();
        }
    }
}