using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ReadableNote : MonoBehaviour, IInteractable
{
    [Header("UI References")]
    [Tooltip("The regular canvas for this note (e.g., Canvas_Note/Note_3).")]
    public GameObject normalCanvas;

    [Tooltip("Optional: canvas to show AFTER all safe rooms visited. If set, it overrides normalCanvas.")]
    public GameObject finalOverrideCanvas;

    [Header("Settings")]
    public string keyLabel = "E";
    public AudioClip openSfx;

    void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
        gameObject.layer = LayerMask.NameToLayer("Interactable");
    }

    public string GetKeyLabel() => keyLabel;

    public void Interact(GameObject interactor)
    {
        if (interactor == null)
        {
            interactor = GameObject.FindGameObjectWithTag("Player");
            if (interactor == null)
            {
                Debug.LogError($"[ReadableNote] Interactor is null and no GameObject tagged 'Player' was found. ({name})");
                return;
            }
        }

        bool allVisited = SafeHouseManager.Instance &&
                          SafeHouseManager.Instance.HasVisitedAllSafeHouses();

        GameObject targetCanvas =
            (allVisited && finalOverrideCanvas != null) ? finalOverrideCanvas : normalCanvas;

        if (!targetCanvas)
        {
            Debug.LogWarning($"[ReadableNote] Missing canvas on {name}");
            return;
        }

        var controller = targetCanvas.GetComponent<NoteCanvasController>();
        if (controller) controller.Refresh();

        var noteUI = targetCanvas.GetComponent<NoteUI>();
        if (!noteUI) noteUI = targetCanvas.AddComponent<NoteUI>();

        noteUI.OpenNote(interactor);

        if (openSfx) AudioSource.PlayClipAtPoint(openSfx, transform.position);
    }
}
