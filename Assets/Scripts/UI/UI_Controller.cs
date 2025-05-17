using System;
using System.Collections;
using Events;
using Events.Scriptables;
using Managers;
using UnityEngine;

namespace UI
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private float panelDuration;
        [SerializeField] private Loader loader;
        [SerializeField] private VoidEventChannelSO onPlayerDeath;
        [SerializeField] private Vector3ChannelSO onDoorPosition;
        [SerializeField] private RectTransform goObject;
        
        [Header("Go Sign properties")]
        [SerializeField] private Vector2 maxGoPositions;
        [SerializeField] private Vector2 minGoPositions;
        [SerializeField] private Vector2 minInScreenPosition;
        [SerializeField] private Vector2 maxInScreenPosition;
    
        void OnEnable()
        {
            onPlayerDeath.onEvent.AddListener(HandlePlayerDeath);
            onDoorPosition.onTypedEvent.AddListener(HandleDoorPosition);
        }

        private void OnDisable()
        {
            onPlayerDeath?.onEvent.RemoveListener(HandlePlayerDeath);
            onDoorPosition.onTypedEvent.RemoveListener(HandleDoorPosition);
        }

        private void HandleDoorPosition(Vector3 position)
        {
            if (DoorIsInScreen(position))
            {
                goObject.gameObject.SetActive(false);
                return;
            }
            
            goObject.gameObject.SetActive(true);

            var vector3 = goObject.localPosition;
            vector3.x = Mathf.Clamp(position.x, minGoPositions.x, maxGoPositions.x);
            vector3.y = Mathf.Clamp(position.y, minGoPositions.y, maxGoPositions.y);
            Debug.Log($"POSITION: {vector3} OBTAINED: {position}");
            goObject.localPosition = vector3;
        }

        private bool DoorIsInScreen(Vector3 position)
        {
            Debug.Log($"CHECKS: {position.x > minInScreenPosition.x} {position.x < maxInScreenPosition.x} {position.y > minInScreenPosition.y} {position.y < maxInScreenPosition.y}");
            return position.x > minInScreenPosition.x && position.x < maxInScreenPosition.x &&
                   position.y > minInScreenPosition.y && position.y < maxInScreenPosition.y;
        }

        private void HandlePlayerDeath()
        {
            StartCoroutine(GameOverScreen());
        }

        private IEnumerator GameOverScreen()
        {
            gameOverPanel.SetActive(true);
            yield return new WaitForSeconds(panelDuration);
            loader.RestartScene();
        }
    }
}
