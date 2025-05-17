using System;
using UnityEngine;

namespace Player.Controllers
{
    public class InteractController : MonoBehaviour
    {
        [SerializeField] private InputHandler _inputHandler;
        private IInteractable _interactableObject;

        private void Start()
        {
            _inputHandler.OnInteract.AddListener(TryInteract);
        }

        private void OnTriggerEnter(Collider other)
        {
            _interactableObject = other.GetComponent<IInteractable>();
            _interactableObject?.Highlight(true);
        }

        private void OnTriggerExit(Collider other)
        {
            _interactableObject?.Highlight(false);
            _interactableObject = null;
        }

        private void TryInteract()
        {
            if (_interactableObject != null)
                _interactableObject.OnInteract();
        }
    }
}