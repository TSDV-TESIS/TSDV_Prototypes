using System.Collections;
using FSM;
using Player.Properties;
using UnityEngine;

namespace Player.Controllers
{
    [RequireComponent(typeof(PlayerMovement))]
    public class ShadowStepController : Controller<PlayerAgent>
    {
        [SerializeField] private PlayerMovementProperties _playerMovementProperties;
        
        private PlayerMovement _playerMovement;
        private Coroutine _shadowstepCoroutine;
        
        private void OnEnable()
        {
            _playerMovement ??= GetComponent<PlayerMovement>();
        }
        
        public void OnEnter()
        {
            if(_shadowstepCoroutine != null) StopCoroutine(_shadowstepCoroutine);
            _shadowstepCoroutine = StartCoroutine(Shadowstep());
        }

        public override void OnUpdate()
        {
        }
        
        private IEnumerator Shadowstep()
        {
            float timer = 0;
            int direction = _playerMovement.GetMoveDirectionSign();
            bool changedToWallslide = false;
            while (timer < _playerMovementProperties.shadowStepTime)
            {
                _playerMovement.Shadowstep(direction);
                timer += Time.deltaTime;
                if (agent.Checks.IsNearWall())
                {
                    changedToWallslide = true;
                    agent.Checks.SetShadowstepOnCooldown();
                    agent.ChangeStateToWallSlide();
                    break;
                }
                yield return null;
            }

            if(!changedToWallslide)
                ExitShadowstep();
        }

        private void ExitShadowstep()
        {
            agent.Checks.SetShadowstepOnCooldown();

            if(agent.Checks.IsNearWall())
                agent.ChangeStateToWallSlide();
            else if(!agent.Checks.IsGrounded())
                agent.ChangeStateToFalling();
            else
                agent.ChangeStateToGrounded();
            
        }
    }
}
