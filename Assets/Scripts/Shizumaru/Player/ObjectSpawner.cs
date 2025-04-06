using System;
using System.Collections.Generic;
using Events.Scriptables;
using Player;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Shizumaru.Player
{
    public class ObjectSpawner : MonoBehaviour
    {
        [SerializeField] private InputHandler handler;
        [SerializeField] private BoolEventSO onInteractionLocked;
        [SerializeField] private GameObject misile;
        [SerializeField] private LayerMask uiSpawnerLayer;
    
        private bool _isInteracting;
        private bool _isMouseDown;
        private void OnEnable()
        {
            _isMouseDown = false;
            _isInteracting = false;
            onInteractionLocked.onTypedEvent.AddListener(HandleIsInteracting);
            handler.OnPlayerClick.AddListener(HandleClick);
        }

        private void OnDisable()
        {
            onInteractionLocked.onTypedEvent.RemoveListener(HandleIsInteracting);
            handler.OnPlayerClick.RemoveListener(HandleClick);
        }

        private void HandleClick()
        {
            if (_isMouseDown)
            {
                _isMouseDown = false;
                return;
            }
            
            if (!_isInteracting) return;
            _isMouseDown = true;
            
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;

            List<RaycastResult> raycastResults = new List<RaycastResult>();
        
            EventSystem.current.RaycastAll(pointerEventData, raycastResults);
            foreach (var raycastResult in raycastResults)
            {
                if (((1<<raycastResult.gameObject.layer) & uiSpawnerLayer) != 0)
                {
                    InstantiateMisile();
                }
            }
        }

        private void InstantiateMisile()
        {
            try
            {
                Vector3 position = GetMisileSpawnPosition();

                GameObject misileObject = Instantiate(misile);
                misileObject.gameObject.transform.position = position;
            }
            catch (Exception err)
            {
                Debug.LogError($"Error spawning: {err.Message}");
            }
        }

        private Vector3 GetMisileSpawnPosition()
        {
            RaycastHit hit;
            Debug.Log("ASD?");
            if (Camera.main != null)
            {
                Debug.Log(Input.mousePosition);
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                    Debug.Log($"found {hit.transform.gameObject.name} at distance: {hit.distance} and position: {hit.point}");

                return new Vector3(hit.point.x, hit.point.y, hit.point.z - 0.5f);
            }

            throw new Exception("SPAWN_NOT_FOUND");
        }

        private void HandleIsInteracting(bool value)
        {
            _isInteracting = value;
        }
    }
}
