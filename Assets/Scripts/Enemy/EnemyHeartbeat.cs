using System;
using System.Collections;
using Events;
using UnityEngine;
using UnityEngine.UI;

namespace Enemy
{
    public class EnemyHeartbeat : MonoBehaviour
    {
        [Header("Colors")] 
        [SerializeField] private Color inHeartBeatColor;
        [SerializeField] private Color offHeartBeatColor;

        [Header("Tween settings")] 
        [SerializeField] private float tweenSeconds = 0.2f;
        [SerializeField] private AnimationCurve easeAnimation;
        
        [Header("Events")] 
        [SerializeField] private VoidEventChannelSO onBloodlustStart;
        [SerializeField] private VoidEventChannelSO onBloodlustEnd;
        [SerializeField] private VoidEventChannelSO onHeartbeatStart;
        [SerializeField] private VoidEventChannelSO onHeartbeatEnd;

        [Header("UI")] 
        [SerializeField] private Image panel;

        private Coroutine _tweenCoroutine;
        private void OnEnable()
        {
            HandleStopBeating();
            onBloodlustStart?.onEvent.AddListener(HandleStartBeating);
            onBloodlustEnd?.onEvent.AddListener(HandleStopBeating);
            onHeartbeatStart?.onEvent.AddListener(HandleOnHeartBeat);
            onHeartbeatEnd?.onEvent.AddListener(HandleOffHeartbeat);
        }
        
        private void OnDisable()
        {
            onBloodlustStart?.onEvent.RemoveListener(HandleStartBeating);
            onBloodlustEnd?.onEvent.RemoveListener(HandleStopBeating);
            onHeartbeatStart?.onEvent.RemoveListener(HandleOnHeartBeat);
            onHeartbeatEnd?.onEvent.RemoveListener(HandleOffHeartbeat);
        }

        private void HandleOffHeartbeat()
        {
            HandleTweenColorTo(offHeartBeatColor);
        }

        private void HandleOnHeartBeat()
        {
            HandleTweenColorTo(inHeartBeatColor);
        }
        

        private void HandleStopBeating()
        {
            panel.enabled = false;
        }

        private void HandleStartBeating()
        {
            Debug.Log("HERE!");
            panel.enabled = true;
        }
        
        private void HandleTweenColorTo(Color color)
        {
            if(_tweenCoroutine != null) StopCoroutine(_tweenCoroutine);

            StartCoroutine(TweenColorsCoroutine(color));
        }

        private IEnumerator TweenColorsCoroutine(Color color)
        {
            Color startColor = panel.color;

            float startTime = Time.time;
            float timer = 0;
            while (timer < tweenSeconds)
            {
                timer = Time.time - startTime;
                float value = easeAnimation.Evaluate(timer / tweenSeconds);
                Color useColor = panel.color;
                useColor.r = Mathf.Lerp(startColor.r, color.r, value);
                useColor.g = Mathf.Lerp(startColor.g, color.g, value);
                useColor.b = Mathf.Lerp(startColor.b, color.b, value);
                useColor.a = Mathf.Lerp(startColor.a, color.a, value);
                panel.color = useColor;
                yield return null;
            }
        }
    }
}
