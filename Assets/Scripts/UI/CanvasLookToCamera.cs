using UnityEngine;

namespace UI
{
    public class CanvasLookToCamera : MonoBehaviour
    {
        void Update()
        {
            transform.LookAt(transform.position + Vector3.back);
        }
    }
}
