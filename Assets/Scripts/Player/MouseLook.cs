using UnityEngine;

namespace Player
{
    public class MouseLook : MonoBehaviour
    {
        [SerializeField] private InputHandler handler;
        [SerializeField] private bool is2D;
        [SerializeField] private GameObject visorPivot;

        private float _angle;

        private Vector2 _viewPortPos;
        private Vector2 cursorDir;
        public Vector2 CursorDir => cursorDir.normalized;

        void OnEnable()
        {
            handler.OnPlayerLook.AddListener(HandleLookDir);
        }

        private void OnDisable()
        {
            handler.OnPlayerLook.RemoveListener(HandleLookDir);
        }

        private void HandleLookDir(Vector2 cursorPos)
        {
            _viewPortPos = Camera.main.ScreenToViewportPoint(cursorPos);
            Vector2 playerPosOnViewport = Camera.main.WorldToViewportPoint(transform.position);
            cursorDir = _viewPortPos - new Vector2(playerPosOnViewport.x, playerPosOnViewport.y);

            _angle = Mathf.Atan2(cursorDir.x, cursorDir.y) * Mathf.Rad2Deg;
            if (is2D)
            {
                visorPivot.transform.rotation = Quaternion.AngleAxis(-_angle + 90, Vector3.forward) * transform.rotation;
                return;
            }

            transform.rotation = Quaternion.AngleAxis(_angle, Vector3.up);
        }
    }
}