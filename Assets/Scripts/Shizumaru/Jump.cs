using System;
using Player;
using UnityEngine;

namespace Shizumaru
{
    [RequireComponent(typeof(Rigidbody))]
    public class Jump : MonoBehaviour
    {
        [SerializeField] private float jumpForce;
        [SerializeField] private InputHandler handler;
        
        [Header("Jump Settings")]
        [Tooltip("Time until it starts checking ground")]
        [SerializeField] private float jumpingBreakMilliseconds = 1000f;

        [Header("Floor layer")]
        [SerializeField] private LayerMask floor;
        
        [Header("Grounded Settings")]
        [SerializeField] private Transform feetPivot;
        [Tooltip("Raycast distance to check if player is in ground or not")]
        [SerializeField] private float groundedRaycastDistance = 0.2f;
        
        private Rigidbody _rigidbody;
        private bool _canJump;
        private bool _shouldJump;
        private RaycastHit _hit;
        private float _timeJumped;
        
        private void OnEnable()
        {
            _rigidbody ??= GetComponent<Rigidbody>();
            handler.OnPlayerMove.AddListener(HandleJump);
            _canJump = true;
        }

        private void OnDisable()
        {
            handler.OnPlayerMove.RemoveListener(HandleJump);
        }

        private void HandleJump(Vector2 movement)
        {
            if (_canJump && movement.y > 0)
            {
                _shouldJump = true;
            } 
        }

        private void Update()
        {
            CheckIfCanJump();
        }
        private void FixedUpdate()
        {
            if (_shouldJump)
            {
                _rigidbody.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
                _timeJumped = Time.time * 1000f;
                _canJump = false;
                _shouldJump = false;
            }
        }

        public bool IsJumpingBreakTimeDone()
        {
            return _timeJumped < 0.0001f || _timeJumped + jumpingBreakMilliseconds < (Time.time * 1000f);
        }
        
        private void CheckIfCanJump()
        {
            _canJump = Physics.Raycast(
                feetPivot.position,
                Vector3.down,
                out _hit,
                groundedRaycastDistance,
                floor
            ) && IsJumpingBreakTimeDone();
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(feetPivot.position, feetPivot.position + Vector3.down * groundedRaycastDistance);
        }
    }
}
