using System.Collections;
using Player.Controllers;
using Player.Properties;
using UnityEngine;

namespace Player
{
    public class Attack : MonoBehaviour
    {
        [SerializeField] private PlayerAnimationController animationController;

        [SerializeField] private GameObject attackObject;
        [SerializeField] private InputHandler handler;

        [Header("Attack properties")]
        [SerializeField] private PlayerAttackProperties attackProperties;

        private ShadowStep _shadows;
        private PlayerMovement _playerMovement;
        private MouseLook _mouseLook;
        private Coroutine _attackCoroutine;
        private bool _isAttacking;

        private void Start()
        {
            _playerMovement ??= GetComponent<PlayerMovement>();
            _mouseLook ??= GetComponent<MouseLook>();
            _shadows ??= GetComponent<ShadowStep>();
        }

        void OnEnable()
        {
            handler.OnPlayerAttack.AddListener(HandleAttack);
        }

        private void OnDisable()
        {
            handler.OnPlayerAttack.RemoveListener(HandleAttack);
        }

        private void HandleAttack()
        {
            if (_isAttacking) return;

            if (_attackCoroutine != null) StopCoroutine(_attackCoroutine);

            _attackCoroutine = StartCoroutine(HandleAttackCoroutine());
        }

        private IEnumerator HandleAttackCoroutine()
        {
            animationController.HandleAttack();
            _isAttacking = true;
            attackObject.SetActive(true);
            float timer = 0;
            float startTime = Time.time;

            _shadows.InitShadowStepShadows();
            _playerMovement.IsAttacking = true;
            while (timer < attackProperties.duration)
            {
                _playerMovement.Velocity = _mouseLook.CursorDir * attackProperties.displacementForce;
                _playerMovement.Move(_playerMovement.Velocity * Time.deltaTime);
                timer = Time.time - startTime;
                yield return null;
            }
            animationController.HandleStopAttack();
            _playerMovement.IsAttacking = false;
            
            _shadows.StopShadows();
            attackObject.SetActive(false);
            yield return new WaitForSeconds(attackProperties.coolDownDuration);
            _isAttacking = false;
        }
    }
}