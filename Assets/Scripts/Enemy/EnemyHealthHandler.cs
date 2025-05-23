using System;
using System.Collections;
using Events;
using Events.Scriptables;
using UI.Bars;
using UnityEngine;
using UnityEngine.Serialization;

namespace Enemy
{
    public class EnemyHealthHandler : MonoBehaviour
    {
        [SerializeField] private int healthRewardOnDeath = 15;
        [SerializeField] private ParticleSystem splashBloodParticles;
        [SerializeField] private GameObject[] objectsToDisable;
        [SerializeField] private float disableSeconds;

        [Header("Events")] 
        [SerializeField] private GameObjectEventChannelSO onEnemyEnabled;
        [SerializeField] private IntEventChannelSO onEnemyDeath;
        [SerializeField] private VoidEventChannelSO onBloodlustStart;
        [SerializeField] private VoidEventChannelSO onBloodlustEnd;
        [SerializeField] private GameObjectEventChannelSO onEnemyDisabled;
        
        private Coroutine _disableCoroutine;
        private bool _isInBloodlust;
        private void OnEnable()
        {
            _isInBloodlust = false;
            onBloodlustStart?.onEvent.AddListener(HandleFrenzyStart);
            onBloodlustEnd?.onEvent.AddListener(HandleFrenzyEnd);
        }

        private void Start()
        {
            onEnemyEnabled?.RaiseEvent(gameObject);
        }

        private void OnDisable()
        {
            onBloodlustStart?.onEvent.RemoveListener(HandleFrenzyStart);
            onBloodlustEnd?.onEvent.RemoveListener(HandleFrenzyEnd);
        }

        private void HandleFrenzyEnd()
        {
            _isInBloodlust = false;
        }

        private void HandleFrenzyStart()
        {
            Debug.Log("FRENZY START ON ENEMY");
            _isInBloodlust = true;
        }
        
        public void HandleInitMaxHealth(int maxHealth)
        {
        }

        public void OnEnemyHit(int value)
        {
        }

        public void OnDeath()
        {
            onEnemyDeath?.RaiseEvent(healthRewardOnDeath);
            
           if(_isInBloodlust)
                splashBloodParticles.Play();
            
            foreach (GameObject obj in objectsToDisable)
            {
                obj.SetActive(false);
            }
            onEnemyDisabled?.RaiseEvent(gameObject);
            
            if(_disableCoroutine != null) StopCoroutine(_disableCoroutine);
            _disableCoroutine = StartCoroutine(DisableCoroutine());
        }

        private IEnumerator DisableCoroutine()
        {
            float timer = 0;
            while (timer < disableSeconds)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            foreach (GameObject obj in objectsToDisable)
            {
                obj.SetActive(true);
            }
            gameObject.SetActive(false);
        }
    }
}
