using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SanityPickup : MonoBehaviour, IInteractable
{
    [Range(0f, 100f)] public float recoverAmount = 60f;
    public string keyLabel = "E";
    public AudioClip pickSfx;

    void Reset()
    {
        // Either a trigger or a normal collider works because we raycast with Collide
        var col = GetComponent<Collider>();
        col.isTrigger = true;
        // Put all pickups on a dedicated layer so the raycast can filter
        gameObject.layer = LayerMask.NameToLayer("Interactable");
    }

    public string GetKeyLabel() => keyLabel;

    public void Interact(GameObject interactor)
    {
        // Recover sanity
        if (PlayerState.Instance != null)
            PlayerState.Instance.IncreaseSanity(recoverAmount);

        // Play pickup sound
        if (pickSfx) AudioSource.PlayClipAtPoint(pickSfx, transform.position);

        // 🔔 Show hint
        if (HintManager.Instance != null)
            HintManager.Instance.ShowHint("Sanity recovers!", 2.5f);

        // Remove the object
        Destroy(gameObject);
    }

}
