using UnityEngine;
using TMPro;

public class InteractHintUI : MonoBehaviour
{
    public GameObject root;     // the whole hint group
    public TMP_Text keyText;    // shows "E"

    public void Show(string key)
    {
        if (keyText) keyText.text = key;
        if (root) root.SetActive(true);
    }
    public void Hide()
    {
        if (root) root.SetActive(false);
    }
}
