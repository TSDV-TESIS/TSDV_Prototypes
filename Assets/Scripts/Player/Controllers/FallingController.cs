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
            _playerMovement.OnUpdate();

            if (agent.Checks.IsGrounded())
                agent.ChangeStateToGrounded();
        }
    }
}