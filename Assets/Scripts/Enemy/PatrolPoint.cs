using System;
using UnityEngine;

namespace Enemy
{
    public class PatrolPoint : MonoBehaviour
    {
        [SerializeField] private float radius = 2f;
        [SerializeField] private Color color = Color.green;

        private Vector3 _position;
        private void OnEnable()
        {
            _position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        }

        private void OnDrawGizmos()
        {
            #if UNITY_EDITOR
            if (!Application.isPlaying) _position = transform.position;
            #endif

            Gizmos.color = color;
        
            Gizmos.DrawSphere(_position, radius);
        }
    }
}
