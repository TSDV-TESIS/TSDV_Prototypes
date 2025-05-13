using System;
using System.Collections;
using Events;
using UnityEngine;

namespace Player
{
    public class Frenzy : MonoBehaviour
    {
        [SerializeField] private FrenzyProperties frenzyProperties;
        
        [Header("Events")]
        [SerializeField] private VoidEventChannelSO onFrenzyStart;
        [SerializeField] private VoidEventChannelSO onFrenzyEnd;

        private Coroutine _frenzyCoroutine;
        private void OnEnable()
        {
            onFrenzyStart?.onEvent.AddListener(HandleStartFrenzy);
        }

        private void OnDisable()
        {
            onFrenzyStart?.onEvent.RemoveListener(HandleStartFrenzy);
        }
        
        private void HandleStartFrenzy()
        {
            if(_frenzyCoroutine != null) StopCoroutine(_frenzyCoroutine);
            _frenzyCoroutine = StartCoroutine(FrenziedCoroutine());
        }

        private IEnumerator FrenziedCoroutine()
        {
            float timer = 0;
            while (timer < frenzyProperties.frenziedDuration)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            onFrenzyEnd?.RaiseEvent();
        }
    }
}
