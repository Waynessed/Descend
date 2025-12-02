using UnityEngine;
using UnityEngine.UI;

public class StaminaBarUI : MonoBehaviour
{
    public FPSPlayerControllerIS player;
    public Slider slider;
    public CanvasGroup cg;
    public float fadeWhenFull = 0.25f;  // alpha when full
    public bool autoFade = true;

    void Awake()
    {
        if (!slider) slider = GetComponent<Slider>();
        if (!cg) cg = GetComponent<CanvasGroup>();
    }

    void Update()
    {
        if (!player) return;
        float ratio = player.staminaCurrent / player.staminaMax;
        slider.value = ratio;

        if (autoFade && cg)
            cg.alpha = Mathf.Lerp(fadeWhenFull, 1f, 1f - Mathf.Clamp01(ratio - 0.99f) * 100f);
    }
}
