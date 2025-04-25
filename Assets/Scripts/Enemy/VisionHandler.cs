using System;
using Unity.Collections;
using UnityEngine;

namespace Enemy
{
    public class VisionHandler : MonoBehaviour
    {
        [SerializeField] private Transform pivot;
        [SerializeField] private float visionLength = 2f;
        [SerializeField] private float visionAngle = 70f;
        [SerializeField] private float anglePerRaycast = 5f;
        [SerializeField] private LayerMask whatIsObjective;
        [SerializeField] private LayerMask whatIsObstruction;
        [ReadOnly] [SerializeField] private float minAnglePerRaycast = 5f;
        [SerializeField] private bool shouldDrawGizmos = false;
        
        public bool CanSeeObjective()
        {
            Vector3 forward = pivot.forward;

            float angleToUse = -visionAngle / 2;
            float anglePerRaycastToUse = Mathf.Max(anglePerRaycast, minAnglePerRaycast);
            
            while (angleToUse <= visionAngle / 2)
            {
                Vector3 raycastDirection = Quaternion.AngleAxis(angleToUse, pivot.right) * pivot.forward;
                if (Physics.Raycast(pivot.position, raycastDirection, visionLength, whatIsObstruction))
                {
                    angleToUse += anglePerRaycastToUse;
                    continue;
                };
                if (Physics.Raycast(pivot.position, raycastDirection, visionLength, whatIsObjective))
                {
                    return true;
                }
    
                angleToUse += anglePerRaycastToUse;
            }

            return false;
        }

        private void OnDrawGizmos()
        {
            if (!shouldDrawGizmos) return;
            
            Gizmos.color = Color.red;
            
            float angleToUse = -visionAngle / 2;
            float anglePerRaycastToUse = Mathf.Max(anglePerRaycast, minAnglePerRaycast);

            while (angleToUse <= visionAngle / 2)
            {
                Vector3 raycastDirection = Quaternion.AngleAxis(angleToUse, pivot.right) * pivot.forward;
                Gizmos.DrawLine(pivot.position, pivot.position + raycastDirection * visionLength);
                angleToUse += anglePerRaycastToUse;
            }
        }
    }
}
