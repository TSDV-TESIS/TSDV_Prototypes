using System;
using UnityEngine;

namespace Environment
{
    public class WallDoorButton : MonoBehaviour, IInteractable
    {
        [SerializeField] private Material highlightMaterial;
        [SerializeField] private GameObject model;
        [SerializeField] private GameObject door;

        private Material _defaultMaterial;
        private MeshRenderer _renderer;

        private void OnEnable()
        {
            _renderer ??= model.GetComponent<MeshRenderer>();
            _defaultMaterial = _renderer.material;
        }

        public void OnInteract()
        {
            Debug.Log("PlayerInteracted");
            door.SetActive(false);
        }

        public void Highlight(bool shouldHighlight)
        {
            _renderer.material = shouldHighlight ? highlightMaterial : _defaultMaterial;
        }
    }
}