using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public Camera cam;
    public float interactDistance = 3f;
    public LayerMask interactMask;      // set to Interactable layer
    public KeyCode useKey = KeyCode.E;
    public InteractHintUI hintUI;

    IInteractable current;

    void Reset()
    {
        if (!cam) cam = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        current = null;
        if (!cam) return;

        // Center-screen ray
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        if (Physics.Raycast(ray, out var hit, interactDistance, interactMask,
                            QueryTriggerInteraction.Collide))
        {
            current = hit.collider.GetComponent<IInteractable>()
                   ?? hit.collider.GetComponentInParent<IInteractable>();
        }

        // Show/hide hint
        if (current != null) hintUI?.Show(current.GetKeyLabel());
        else                 hintUI?.Hide();

        // Interact
        if (current != null && Input.GetKeyDown(useKey))
        {
            current.Interact(gameObject);
            hintUI?.Hide();
        }
    }
}
