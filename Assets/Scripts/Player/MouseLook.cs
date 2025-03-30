using Player;
using Unity.Mathematics;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField] private InputHandler handler;

    private float _angle;

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
        Vector2 viewPortPos = Camera.main.ScreenToViewportPoint(cursorPos);
        viewPortPos -= new Vector2(0.5f, 0.5f);

        _angle = Mathf.Atan2(viewPortPos.x, viewPortPos.y) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(_angle, Vector3.up);
    }
}