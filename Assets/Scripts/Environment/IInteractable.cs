using UnityEngine;

public interface IInteractable
{
    public void OnInteract();
    public void Highlight(bool shouldHighlight);
}