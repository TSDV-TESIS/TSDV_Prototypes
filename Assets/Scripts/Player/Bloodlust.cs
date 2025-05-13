using System;
using Events;
using Events.Scriptables;
using UnityEngine;

namespace Player
{
    public class Bloodlust : MonoBehaviour
    {
        [SerializeField] private BloodlustProperties bloodlustProperties;
        
        [Header("Handlers")]
        [SerializeField] private InputHandler handler;
    
        [Header("Events")] 
        [SerializeField] private VoidEventChannelSO onBloodlustStart;
        [SerializeField] private VoidEventChannelSO onBloodlustEnd;
        [SerializeField] private FloatEventChannel onBloodlustUsage;
        [SerializeField] private VoidEventChannelSO onFrenzyStart;

        private bool _isInBloodlust;
        private float _bloodlustUsagePercentageLeft;
        private float _bloodlustUsageSeconds;
        private void OnEnable()
        {
            _bloodlustUsageSeconds = 0;
            _bloodlustUsagePercentageLeft = 1;
            _isInBloodlust = false;
            handler.OnPlayerBloodlust.AddListener(HandleBloodlust);
        }

        private void OnDisable()
        {
            handler.OnPlayerBloodlust.RemoveListener(HandleBloodlust);
        }

        void Update()
        {
            if (_bloodlustUsagePercentageLeft == 1 && !_isInBloodlust ||
                _bloodlustUsagePercentageLeft == 0 && _isInBloodlust) return;
            if (_isInBloodlust)
            {
                _bloodlustUsageSeconds += Time.deltaTime; 
            }
            else
            {
                _bloodlustUsageSeconds -= Time.deltaTime * bloodlustProperties.bloodlustRecoveryVelocity;
            }
            _bloodlustUsagePercentageLeft = 1 - _bloodlustUsageSeconds / bloodlustProperties.bloodlustDuration;
            
            if (_bloodlustUsagePercentageLeft > 1) _bloodlustUsagePercentageLeft = 1;
            if (_bloodlustUsagePercentageLeft < 0) _bloodlustUsagePercentageLeft = 0;

            if (_bloodlustUsagePercentageLeft == 0 && _isInBloodlust)
            {
                onBloodlustEnd?.RaiseEvent();
                _isInBloodlust = false;
            }
            onBloodlustUsage?.RaiseEvent(_bloodlustUsagePercentageLeft);
        }

        private void HandleBloodlust()
        {
            if (!_isInBloodlust && !CanDoBloodlust())
            {
                Debug.Log("Player cannot go into bloodlust");
                return;
            }

            if (_isInBloodlust)
            {
                onBloodlustEnd?.RaiseEvent();
                _isInBloodlust = false;
            }
            else
            {
                onBloodlustStart?.RaiseEvent();
                _isInBloodlust = true;
            }
        }

        private bool CanDoBloodlust()
        {
            return _bloodlustUsagePercentageLeft > bloodlustProperties.minimumBloodlustPercentageUsage;
        }
    }
}
