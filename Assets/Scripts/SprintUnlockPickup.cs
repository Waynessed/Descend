using UnityEngine;

/// <summary>
/// Drop this on any scene object to make it a pickup that unlocks sprint.
/// Works with a raycast-based interactor that calls IInteractable.Interact().
/// </summary>
[RequireComponent(typeof(Collider))]
public class SprintUnlockPickup : MonoBehaviour, IInteractable
{
    [Header("UI")]
    [SerializeField] private string prompt = "Unlock Sprint";
    [SerializeField] private string keyLabel = "E";

    [Header("Behavior")]
    [Tooltip("Also refill stamina to full when sprint gets unlocked.")]
    public bool refillStaminaOnUnlock = true;

    [Tooltip("Destroy this pickup after use.")]
    public bool destroyOnPickup = true;

    [Header("FX (optional)")]
    public AudioClip pickupSfx;
    public GameObject pickupVfxPrefab;

    private Collider col;
    private Renderer[] rends;

    void Reset()
    {
        col = GetComponent<Collider>();
        col.isTrigger = true; // interact via raycast; won't block the player
        gameObject.layer = LayerMask.NameToLayer("Interactable"); // create this layer once
    }

    void Awake()
    {
        col = GetComponent<Collider>();
        if (col) col.isTrigger = true;
        rends = GetComponentsInChildren<Renderer>(true);

        // If the layer doesn't exist, this call is harmless; set it manually in the Inspector if needed.
        if (gameObject.layer == 0) // Default
            gameObject.layer = LayerMask.NameToLayer("Interactable");
    }

    // ===== IInteractable =====
    public string GetKeyLabel() => keyLabel;

    public void Interact(GameObject interactor)
    {
        // Find the player's controller (on the player or a parent)
        var pc = interactor.GetComponent<FPSPlayerControllerIS>();
        if (!pc) pc = interactor.GetComponentInParent<FPSPlayerControllerIS>();

        if (pc)
        {
            pc.sprintUnlocked = true;

            if (refillStaminaOnUnlock)
                pc.staminaCurrent = pc.staminaMax;
        }
        else
        {
            // Fallback: try scene-wide (shouldn't be needed if called by the player)
            pc = FindObjectOfType<FPSPlayerControllerIS>();
            if (pc)
            {
                pc.sprintUnlocked = true;
                if (refillStaminaOnUnlock)
                    pc.staminaCurrent = pc.staminaMax;
            }
            else
            {
                Debug.LogWarning("[SprintUnlockPickup] No FPSPlayerControllerIS found.");
            }
        }

        // FX
        if (pickupSfx) AudioSource.PlayClipAtPoint(pickupSfx, transform.position);
        if (pickupVfxPrefab) Instantiate(pickupVfxPrefab, transform.position, Quaternion.identity);

        // Show sprint hint on pickup
        if (HintManager.Instance != null)
            HintManager.Instance.ShowHint("You can now sprint!\nPress Shift to run", 4f);


        if (destroyOnPickup) Destroy(gameObject);
        else SetVisible(false); // hide if you prefer not to destroy
    }

    private void SetVisible(bool v)
    {
        if (col) col.enabled = v;
        if (rends != null) foreach (var r in rends) r.enabled = v;
    }
}
