using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HintManager : MonoBehaviour
{
    public static HintManager Instance { get; private set; }

    [Header("Refs")]
    [SerializeField] private GameObject hintPanel;
    [SerializeField] private TextMeshProUGUI hintText;

    [Header("Timing")]
    [SerializeField] private float showSeconds = 3f;
    [SerializeField] private float fadeSeconds = 0.6f;

    CanvasGroup cg;
    Coroutine running;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        cg = hintPanel.GetComponent<CanvasGroup>();
        if (cg == null) cg = hintPanel.AddComponent<CanvasGroup>();
        hintPanel.SetActive(false);
        cg.alpha = 0f;
    }

    public void ShowHint(string msg, float? seconds = null)
    {
        if (running != null) StopCoroutine(running);
        running = StartCoroutine(ShowRoutine(msg, seconds ?? showSeconds));
    }

    IEnumerator ShowRoutine(string msg, float visible)
    {
        hintText.text = msg;
        hintPanel.SetActive(true);

        // fade in
        for (float t = 0; t < fadeSeconds; t += Time.unscaledDeltaTime)
        {
            cg.alpha = Mathf.Lerp(0f, 1f, t / fadeSeconds);
            yield return null;
        }
        cg.alpha = 1f;

        yield return new WaitForSecondsRealtime(visible);

        // fade out
        for (float t = 0; t < fadeSeconds; t += Time.unscaledDeltaTime)
        {
            cg.alpha = Mathf.Lerp(1f, 0f, t / fadeSeconds);
            yield return null;
        }
        cg.alpha = 0f;
        hintPanel.SetActive(false);
        running = null;
    }
}
