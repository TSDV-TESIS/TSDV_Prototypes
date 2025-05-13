using System.Collections;
using FSM;
using Health;
using Player.Properties;
using UnityEngine;

namespace Player.Controllers
{
    [RequireComponent(typeof(PlayerMovement))]
    public class ShadowStepController : Controller<PlayerAgent>
    {
        [SerializeField] private PlayerMovementProperties _playerMovementProperties;
        [SerializeField] private ShadowStepProperties _shadowStepProperties;

        private PlayerMovement _playerMovement;
        private MouseLook _mouseLook;
        private CharacterController _characterController;
        private HealthPoints _healthPoints;
        private Coroutine _shadowstepCoroutine;

        private void OnEnable()
        {
            _playerMovement ??= GetComponent<PlayerMovement>();
            _mouseLook ??= GetComponent<MouseLook>();
            _characterController ??= GetComponent<CharacterController>();
            _healthPoints ??= GetComponent<HealthPoints>();
        }

        public void OnEnter()
        {
            if (_shadowstepCoroutine != null) StopCoroutine(_shadowstepCoroutine);
            _shadowstepCoroutine = StartCoroutine(Shadowstep());
        }

        public override void OnUpdate()
        {
        }

        private IEnumerator Shadowstep()
        {
            float timer = 0;
            Vector2 direction = _mouseLook.CursorDir.normalized;
            bool changedToWallslide = false;

            _characterController.excludeLayers |= _shadowStepProperties.avoidableObjects;
            _healthPoints.SetCanTakeDamage(false);
            while (timer < _playerMovementProperties.shadowStepTime)
            {
                _playerMovement.Shadowstep(direction);
                timer += Time.deltaTime;
                yield return null;
            }

            _healthPoints.SetCanTakeDamage(true);
            _characterController.excludeLayers ^= _shadowStepProperties.avoidableObjects;

            if (agent.Checks.IsNearWall())
            {
                changedToWallslide = true;
                agent.Checks.SetShadowstepOnCooldown();
                agent.ChangeStateToWallSlide();
            }

            if (!changedToWallslide)
                ExitShadowstep();
        }

        private void ExitShadowstep()
        {
            agent.Checks.SetShadowstepOnCooldown();

            if (agent.Checks.IsNearWall())
                agent.ChangeStateToWallSlide();
            else if (!agent.Checks.IsGrounded())
                agent.ChangeStateToFalling();
            else
                agent.ChangeStateToGrounded();
        }
    }
}