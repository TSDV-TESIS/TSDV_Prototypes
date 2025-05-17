using UnityEngine;

namespace Player.Shadow
{
    [RequireComponent(typeof(Collider), typeof(Rigidbody))]
    public class ShadowExplosion : MonoBehaviour
    {
        [SerializeField] private float explosionDuration;
        [SerializeField] private MeshRenderer meshRenderer;
        private Collider _collider;
        private Rigidbody _rb;

        private Coroutine _explosion;

        private void Awake()
        {
            _collider ??= GetComponent<Collider>();
            _rb ??= GetComponent<Rigidbody>();
        }

        public void Explode()
        {
            Destroy(gameObject);
        }
    }
}