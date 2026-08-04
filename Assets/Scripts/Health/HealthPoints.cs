using System.Collections;
using Events;
using Events.Scriptables;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

namespace Health
{
    public class HealthPoints : MonoBehaviour, ITakeDamage
    {
        [SerializeField] private float timeScaleDivision = 20;
        [SerializeField] private float hitFrameTime = 0.1f;
        [SerializeField] private float timeUntilFrameActivate = 0.1f;
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private int initHealth = 100;
        [SerializeField] private bool canTakeDamage = true;
        [SerializeField] private bool shouldFreeze = false;

        [Header("events")] [SerializeField] private DeathEventChannelSO onDeathEvent;
        [SerializeField] private VoidEventChannelSO onReviveEvent;
        [SerializeField] private IntEventChannelSO onTakeDamageEvent;
        [SerializeField] private IntEventChannelSO onSumHealthEvent;
        [SerializeField] private IntEventChannelSO onResetPointsEvent;
        [SerializeField] private IntEventChannelSO onInitializeHealthEvent;
        [SerializeField] private IntEventChannelSO onInitializeMaxHealthEvent;
        [SerializeField] private VoidEventChannelSO onDamageAvoidedEvent;
        [SerializeField] private DeathEventChannelSO receiveLethalDamageEvent;

        [Header("Internal events")] [SerializeField] private UnityEvent onHit;
        [SerializeField] private UnityEvent onInternalDeathEvent;
        [SerializeField] private UnityEvent<int> onInternalResetEvent;
        [SerializeField] private UnityEvent<int> onInternalTakeDamageEvent;
        [SerializeField] private UnityEvent<int> onInternalInitializeMaxHealthEvent;
        [SerializeField] private UnityEvent<DeathCauses> onLethalDamageEvent;

        public int MaxHealth
        {
            get { return maxHealth; }
        }

        public DeathEventChannelSO OnDeathEvent
        {
            get { return onDeathEvent; }
        }

        private bool _isInvincible = false;
        private bool _hasBeenDead = false;

        public int CurrentHp { get; private set; }

        void Start()
        {
            CurrentHp = initHealth;
            _hasBeenDead = false;
            onInitializeHealthEvent?.RaiseEvent(CurrentHp);
            RaiseInitMaxHpEvent();
        }

        private void OnEnable()
        {
            _hasBeenDead = false;
            receiveLethalDamageEvent?.onTypedEvent.AddListener(TakeLethalDamage);
        }

        private void OnDisable()
        {
            receiveLethalDamageEvent?.onTypedEvent.RemoveListener(TakeLethalDamage);
        }

        private void OnDestroy()
        {
            onDeathEvent?.onTypedEvent?.RemoveAllListeners();
        }

        public void SetCanTakeDamage(bool value)
        {
            canTakeDamage = value;
        }

        // Is invincible =/= can take damage.
        // isInvincible is used for cheats.
        // canTakeDamage is used if the entity just cant take damage in any way programatically.
        public void SetIsInvincible(bool value)
        {
            _isInvincible = value;
        }

        public void ResetHitPoints()
        {
            _hasBeenDead = false;
            CurrentHp = maxHealth;
            onResetPointsEvent?.RaiseEvent(CurrentHp);
            onInternalResetEvent?.Invoke(CurrentHp);
            RaiseInitMaxHpEvent();
        }

        private void TakeLethalDamage(DeathCauses cause)
        {
            if (TryTakeDamage(maxHealth, cause))
                onLethalDamageEvent?.Invoke(cause);
        }

        public bool TryTakeDamage(int damage, DeathCauses cause)
        {
            if (_isInvincible) return false;

            if (!canTakeDamage)
            {
                onDamageAvoidedEvent?.RaiseEvent();
                return false;
            }

            TakeUnavoidableDamage(damage, cause);

            return true;
        }

        public void TakeUnavoidableDamage(int damage, DeathCauses cause)
        {
            if (_isInvincible) return;

            CurrentHp -= damage;

            if (shouldFreeze)
                StartCoroutine(StunTime());

            if (IsDead() && !_hasBeenDead)
            {
                _hasBeenDead = true;
                onTakeDamageEvent?.RaiseEvent(0);
                onDeathEvent?.RaiseEvent(cause);
                onInternalDeathEvent?.Invoke();
            }

            else
            {
                onTakeDamageEvent?.RaiseEvent(CurrentHp);
                onHit?.Invoke();
                onInternalTakeDamageEvent?.Invoke(CurrentHp);
            }
        }

        public IEnumerator StunTime()
        {
            yield return new WaitForSecondsRealtime(timeUntilFrameActivate);
            TimeManager.Instance.TrySetTimeScale(Time.timeScale / timeScaleDivision);
            yield return new WaitForSecondsRealtime(hitFrameTime);
            TimeManager.Instance.TrySetTimeScale(1);
        }


        public bool IsDead()
        {
            return CurrentHp <= 0;
        }

        public void RaiseInitMaxHpEvent()
        {
            onInitializeMaxHealthEvent?.RaiseEvent(MaxHealth);
            onInternalInitializeMaxHealthEvent?.Invoke(MaxHealth);
        }

        public void SumHealth(int wonHealth)
        {
            if (IsDead())
            {
                onReviveEvent.RaiseEvent();
                _hasBeenDead = false;
            }

            CurrentHp = math.min(maxHealth, wonHealth + CurrentHp);
            onSumHealthEvent?.RaiseEvent(CurrentHp);
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        public void ToggleInvulnerability()
        {
            canTakeDamage = !canTakeDamage;
        }

        public void ToggleInvulnerability(bool value)
        {
            canTakeDamage = value;
        }
#endif
    }
}