using System.Collections;
using Health;
using UnityEngine;
using UnityEngine.VFX;

namespace Enemy.Attack
{
    [RequireComponent(typeof(Collider))]
    public class ArrowAttack : MonoBehaviour
    {
        [SerializeField] private int damage = 1000;
        [SerializeField] private ArrowAttackProperties properties;
        [SerializeField] private Light pointLight;
        [SerializeField] private float dimDuration;
        [SerializeField] private VisualEffect arrowVFX;
        [SerializeField] private VisualEffect hitVFX;
        [SerializeField] private GameObject decalPrefab;

        private Collider _collider;
        private Coroutine _arrowDestroyCoroutine;
        private Coroutine _hitCoroutine;
        private Coroutine _lightDimCoroutine;
        private bool _canAttack;

        private bool _isTraveling;
        private float _velocity;
        private Vector3 _direction;

        private void OnEnable()
        {
            _isTraveling = false;
            _collider ??= GetComponent<Collider>();
            arrowVFX.SendEvent(properties.vfxStartEvent);
        }

        private void Update()
        {
            if (_isTraveling)
                transform.position += _velocity * _direction * Time.deltaTime;
        }

        public void SetVelocityDirectionAndAttack(float velocity, Vector3 direction)
        {
            transform.parent = null;
            _direction = direction.normalized;
            _velocity = velocity;
            _isTraveling = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            HandleTriggerAttack(other);

            if (!_isTraveling) return;

            if ((properties.whatIsStoppableColliders & (1 << other.gameObject.layer)) != 0)
            {
                _isTraveling = false;
                GlueAndDestroy(other.gameObject);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            HandleTriggerAttack(other);
        }

        private void HandleTriggerAttack(Collider other)
        {
            if (!_isTraveling) return;

            if (other.CompareTag("Player") && other.TryGetComponent<ITakeDamage>(out ITakeDamage takeDamageObject))
            {
                takeDamageObject.TryTakeDamage(damage, DeathCauses.External);
                if (_hitCoroutine != null) StopCoroutine(_hitCoroutine);

                _hitCoroutine = StartCoroutine(WaitPlayerHitVfx());
                _isTraveling = false;
            }
        }

        private void GlueAndDestroy(GameObject otherGameObject)
        {
            _collider.enabled = false;
            gameObject.transform.parent = otherGameObject.transform;
            arrowVFX.SendEvent(properties.vfxStopEvent);
            arrowVFX.SendEvent(properties.hitVfxStartEvent);
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 1, properties.whatIsStoppableColliders))
                Instantiate(decalPrefab, hit.point, Quaternion.LookRotation(-hit.normal));

            if (_lightDimCoroutine != null) StopCoroutine(_lightDimCoroutine);
            _lightDimCoroutine = StartCoroutine(LightDim());

            if (_arrowDestroyCoroutine != null) StopCoroutine(_arrowDestroyCoroutine);
            _arrowDestroyCoroutine = StartCoroutine(WaitAndDestroy());
        }

        private IEnumerator WaitAndDestroy()
        {
            yield return new WaitForSeconds(properties.destroySeconds);
            Destroy(gameObject);
        }

        private IEnumerator WaitPlayerHitVfx()
        {
            _collider.enabled = false;
            _isTraveling = false;

            arrowVFX.enabled = false;
            hitVFX.SendEvent(properties.hitJesterVfxStartEvent);
            yield return WaitAndDestroy();
        }

        private IEnumerator LightDim()
        {
            float timer = 0;
            float startTime = Time.time;
            float initialIntensity = pointLight.intensity;
            while (timer < dimDuration)
            {
                timer = Time.time - startTime;
                pointLight.intensity = Mathf.Lerp(initialIntensity, 0, timer / dimDuration);
                yield return null;
            }
        }

        public void StartCharge()
        {
            arrowVFX.SendEvent("Charge");
        }

        public void SetChargeValue(float value)
        {
            arrowVFX.SetFloat("Charge_Intensity", value);
        }

        public void SetLoop()
        {
            arrowVFX.SendEvent("loop");
        }
    }
}