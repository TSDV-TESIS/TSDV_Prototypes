using UnityEngine;

namespace Player.Controllers
{
    public class PlayerAnimationController : MonoBehaviour
    {
        [SerializeField] private Animator playerAnimator;
        private static readonly int Walking = Animator.StringToHash("Walking");
        private static readonly int Attack1 = Animator.StringToHash("Attack");

        public void HandleWalk()
        {
            playerAnimator.SetBool(Walking, true);
        }

        public void HandleIdle()
        {
            playerAnimator.SetBool(Walking, false);
        }

        public void HandleAttack()
        {
            playerAnimator.SetBool(Attack1, true);
        }

        public void HandleStopAttack()
        {
            playerAnimator.SetBool(Attack1, false);
        }
    }
}
