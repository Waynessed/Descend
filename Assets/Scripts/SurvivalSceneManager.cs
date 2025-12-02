using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class SurvivalSceneManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI titleText;           // "You really escaped ??"
    public TextMeshProUGUI warningText;         // "072..."
    public Button mainMenuButton;
    public Image backgroundImage;               // ✅ 新增背景图

    [Header("Fade Effect")]
    public CanvasGroup fadeOverlay;             
    public float fadeInTime = 1f;
    public float fadeOutTime = 1f;

    [Header("Background Fade Settings")]
    public float backgroundFadeTime = 1.5f; 

    [Header("Text Display Settings")]
    public float firstTextDelay = 1f;
    public float secondFadeDelay = 3f;
    public float warningTextDelay = 2f;
    public float typewriterSpeed = 0.05f;

    [Header("Button Settings")]
    public float buttonShowDelay = 3f;

    [Header("Audio")]
    public AudioClip ambientSound;
    public AudioClip warningSound;

    [Header("Messages")]
    [TextArea] public string titleMessage = "You really escaped ??";
    [TextArea] public string finalWarning = "072, catch it if you want to survive";

    void Start()
    {
        Debug.Log("🎬 SurvivalSceneManager 启动");

        if (titleText) titleText.text = "";
        if (warningText) warningText.text = "";
        if (mainMenuButton) mainMenuButton.gameObject.SetActive(false);
        if (backgroundImage) 
        {
            backgroundImage.gameObject.SetActive(true);
            Color c = backgroundImage.color;
            c.a = 0f;
            backgroundImage.color = c; // ✅ 初始完全透明
        }

        if (fadeOverlay) fadeOverlay.alpha = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (ambientSound && AudioManager.instance != null)
            AudioManager.instance.PlaySFX(ambientSound);

        StartCoroutine(PlayCinematicSequence());
    }

    IEnumerator PlayCinematicSequence()
    {
        Debug.Log("🎬 开始播放电影化序列");

        yield return FadeOut();
        yield return new WaitForSeconds(firstTextDelay);

        if (titleText)
            yield return TypeText(titleText, titleMessage);

        yield return new WaitForSeconds(secondFadeDelay);
        yield return FadeIn();

        if (titleText) titleText.text = "";

        Debug.Log("🎬 开始第二幕：怪物视角");

        yield return FadeOut();
        yield return new WaitForSeconds(warningTextDelay);

        if (warningSound && AudioManager.instance != null)
            AudioManager.instance.PlaySFX(warningSound);

        if (warningText)
        {
            warningText.color = Color.red;
            yield return TypeText(warningText, finalWarning);
            yield return BlinkText(warningText, 3);
        }

        // ✅ 显示背景和主菜单按钮
        yield return new WaitForSeconds(buttonShowDelay);
        if (backgroundImage)
            yield return FadeImage(backgroundImage, 0f, 1f, backgroundFadeTime); // 淡入背景

        if (mainMenuButton)
        {
            mainMenuButton.gameObject.SetActive(true);
            mainMenuButton.onClick.AddListener(OnMainMenu);
        }
    }

    IEnumerator TypeText(TextMeshProUGUI textComponent, string message)
    {
        textComponent.text = "";
        foreach (char c in message)
        {
            textComponent.text += c;
            yield return new WaitForSeconds(typewriterSpeed);
        }
    }

    IEnumerator BlinkText(TextMeshProUGUI textComponent, int blinkCount)
    {
        Color originalColor = textComponent.color;

        for (int i = 0; i < blinkCount; i++)
        {
            float t = 0f;
            while (t < 0.3f)
            {
                t += Time.deltaTime;
                textComponent.color = Color.Lerp(originalColor, Color.clear, t / 0.3f);
                yield return null;
            }

            t = 0f;
            while (t < 0.3f)
            {
                t += Time.deltaTime;
                textComponent.color = Color.Lerp(Color.clear, originalColor, t / 0.3f);
                yield return null;
            }
        }

        textComponent.color = originalColor;
    }

    IEnumerator FadeOut()
    {
        if (fadeOverlay == null) yield break;

        float t = 0f;
        while (t < fadeOutTime)
        {
            t += Time.deltaTime;
            fadeOverlay.alpha = Mathf.Lerp(1f, 0f, t / fadeOutTime);
            yield return null;
        }
        fadeOverlay.alpha = 0f;
    }

    IEnumerator FadeIn()
    {
        if (fadeOverlay == null) yield break;

        float t = 0f;
        while (t < fadeInTime)
        {
            t += Time.deltaTime;
            fadeOverlay.alpha = Mathf.Lerp(0f, 1f, t / fadeInTime);
            yield return null;
        }
        fadeOverlay.alpha = 1f;
    }

    IEnumerator FadeImage(Image img, float from, float to, float duration)
    {
        float t = 0f;
        Color c = img.color;
        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, t / duration);
            img.color = c;
            yield return null;
        }
        c.a = to;
        img.color = c;
    }

    void OnMainMenu()
    {
        Debug.Log("🏠 返回主菜单");
        StartCoroutine(ReturnToMainMenu());
    }

    IEnumerator ReturnToMainMenu()
    {

        yield return FadeIn();

        if (AudioManager.instance != null)
            AudioManager.instance.PlayMusic(AudioManager.instance.mainMenuMusic);

        SceneManager.LoadScene("StartScene");
    }
}
