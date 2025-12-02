using UnityEngine;

public class NoteCanvasController : MonoBehaviour
{
    [Header("Show only AFTER all safe rooms visited")]
    public GameObject[] finalOnly;     

    [Header("Hide AFTER all safe rooms visited (optional)")]
    public GameObject[] normalOnly;   


    public void Refresh()
    {
        bool allVisited = SafeHouseManager.Instance &&
                          SafeHouseManager.Instance.HasVisitedAllSafeHouses();

        if (finalOnly != null)
            foreach (var go in finalOnly) if (go) go.SetActive(allVisited);

        if (normalOnly != null)
            foreach (var go in normalOnly) if (go) go.SetActive(!allVisited);
    }

    void OnEnable() { Refresh(); }
}
