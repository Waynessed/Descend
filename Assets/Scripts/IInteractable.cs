using UnityEngine;
public interface IInteractable
{
    string GetKeyLabel();        // e.g. "E"
    void Interact(GameObject interactor); // the Player
}
