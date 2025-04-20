using System;
using System.Collections;
using Events;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

namespace UI
{
    public class BloodlustPanelTween : MonoBehaviour
    {
        [Header("alpha curves")]
        [SerializeField] private AnimationCurve offAlphaCurve;
        [SerializeField] private AnimationCurve onAlphaCurve;

        [Header("Tween settings")]
        [SerializeField] private float tweenSeconds = 0.2f;

        [Header("Events")] 
        [SerializeField] private VoidEventChannelSO onBloodlustStart;
        [SerializeField] private VoidEventChannelSO onBloodlustEnd;

        [Header("Image")]
        [SerializeField] private Image imagePanel;
        private Coroutine _tweenColorsCoroutine;
        private void OnEnable()
        {
            onBloodlustStart?.onEvent.AddListener(HandleTweenOn);
            onBloodlustEnd?.onEvent.AddListener(HandleTweenOff);
        }

        private void OnDisable()
        {
            onBloodlustStart?.onEvent.RemoveListener(HandleTweenOn);
            onBloodlustEnd?.onEvent.RemoveListener(HandleTweenOff);
        }
        
        private void HandleTweenOff()
        {
            if(_tweenColorsCoroutine != null) StopCoroutine(_tweenColorsCoroutine);
            StartCoroutine(TweenColors(offAlphaCurve));
        }

        private void HandleTweenOn()
        {
            if(_tweenColorsCoroutine != null) StopCoroutine(_tweenColorsCoroutine);
            StartCoroutine(TweenColors(onAlphaCurve));
        }
        
        private IEnumerator TweenColors(AnimationCurve alphaCurve)
        {
            Color startColor = imagePanel.color;

            float startTime = Time.time;
            float timer = 0;
            while (timer < tweenSeconds)
            {
                timer = Time.time - startTime;
                float alphaValue = alphaCurve.Evaluate(timer / tweenSeconds);
                startColor.a = alphaValue;
                imagePanel.color = startColor;
                yield return null;
            }
        }
    }
}
