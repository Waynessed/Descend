using UnityEngine;
using TMPro;
using System.Collections;
using System.Text;

public class TutorialManager : MonoBehaviour
{
    [Header("Tutorial State")]
    public static bool isTutorialActive = false;

    [Header("Refs")]
    public MonsterChase monster;
    public Transform player;
    public CanvasGroup fadeOverlay;
    public TextMeshProUGUI tutorialText;
    public MonoBehaviour playerMove;
    public GameObject skipHintUI; 

    [Header("Waypoint / Guide Point")]
    public Transform goToPoint;
    public float arriveRadius = 2f;

    [Header("Audio")]
    public AudioClip runWarningClip;

    [Header("Timing")]
    public float hintDelay = 0.4f;
    public float fadeTime = 0.6f;
    public float preChaseHold = 1.2f;
    public float introTextDuration = 4f;

    [Header("Text Effects")]
    public float typewriterSpeed = 0.05f;
    public bool useTypewriter = true;
    public float playerNumberPause = 1.5f;

    [Header("Skip Settings")]
    public KeyCode skipKey = KeyCode.Space;
    private bool skipRequested = false;

    [Header("Copy")]
    [TextArea] public string playerNumber = "";
    [TextArea] public string introMessage = "";
    [TextArea] public string moveHint = "";
    [TextArea] public string goToEntryHint = " ";
    [TextArea] public string wiredMessage = "";
    [TextArea] public string runWarning = "run!!!";

    int stage = -1;
    bool started = false;
    bool runningCutscene = false;

    public void BeginTutorialOnce()
    {
        if (started) return;
        started = true;
        isTutorialActive = true;
        BeginTutorial();
    }

    public void BeginTutorial()
    {
        isTutorialActive = true;
        
        if (monster) monster.aiEnabled = false;
        if (playerMove) playerMove.enabled = false;
        if (fadeOverlay) fadeOverlay.alpha = 0f;
        if (tutorialText) tutorialText.text = "";

        stage = 0;
        runningCutscene = false;
        StartCoroutine(IntroSequence());
    }

    void Start()
    {
        isTutorialActive = true;
        if (monster) monster.aiEnabled = false;
        if (fadeOverlay) fadeOverlay.alpha = 0f;
        if (playerMove) playerMove.enabled = false;
    }

    void Update()
    {
        // ⭐ 检测跳过按键
        if (Input.GetKeyDown(skipKey))
        {
            skipRequested = true;
            Debug.Log("⏭️ 跳过请求");
        }

        if (!started || runningCutscene) return;

        switch (stage)
        {
            case 1:
                // ⭐ 按空格或移动 → 进入下一阶段
                if (skipRequested || 
                    Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) ||
                    Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
                {
                    skipRequested = false;
                    stage = 2; 
                    StartCoroutine(ShowGoToEntryAndWaitForPlayer());
                }
                break;
        }
    }

    IEnumerator IntroSequence()
    {
        runningCutscene = true;

        if (skipHintUI) skipHintUI.SetActive(true);

        // 1) Player number
        if (tutorialText)
        {
            tutorialText.fontSize = 54;
            tutorialText.color = Color.red;
            tutorialText.text = "";

            yield return TypeTextSkippable(tutorialText, playerNumber, typewriterSpeed * 2f);
        }

        yield return WaitSkippable(playerNumberPause);

        // 2) Main message
        if (tutorialText)
        {
            tutorialText.fontSize = 54;
            tutorialText.color = Color.white;
            tutorialText.text = "";

            yield return TypeTextSkippable(tutorialText, introMessage, typewriterSpeed);
        }

        yield return WaitSkippable(introTextDuration);

        // 3) Move hint
        if (tutorialText)
        {
            yield return FadeText(0f, 0.5f);
            tutorialText.fontSize = 54;
            tutorialText.color = Color.white;
            tutorialText.text = "";

            yield return TypeTextSkippable(tutorialText, moveHint, typewriterSpeed);
        }

        if (playerMove) playerMove.enabled = true;

        stage = 1;
        runningCutscene = false;
    }

    IEnumerator ShowGoToEntryAndWaitForPlayer()
    {
        runningCutscene = true;
        
        // 1. 显示 "go to the entry" 提示
        yield return WaitSkippable(hintDelay);
        
        if (tutorialText)
        {
            tutorialText.fontSize = 54;
            tutorialText.color = Color.white;
            tutorialText.text = "";

            yield return TypeTextSkippable(tutorialText, goToEntryHint, typewriterSpeed);
        }
        
        // 2. ⭐ 等待玩家走到入口 OR 按空格跳过
        runningCutscene = false;
        while (goToPoint && Vector3.Distance(player.position, goToPoint.position) > arriveRadius)
        {
            // ⭐ 如果按空格，直接跳过等待
            if (skipRequested)
            {
                skipRequested = false;
                Debug.Log("⏭️ 跳过等待，直接进入追逐阶段");
                break;
            }
            yield return null;
        }
        
        // 3. 玩家到达或跳过，立即开始黑幕和后续流程
        runningCutscene = true;
        yield return StartChaseCutscene();
    }

    IEnumerator StartChaseCutscene()
    {
        // 立即开始黑幕闪屏
        yield return WaitSkippable(0.8f);
        yield return Fade(1f, fadeTime);
        yield return WaitSkippable(0.15f);
        yield return Fade(0f, fadeTime);

        // 黑幕后显示 "visit the green area"
        if (tutorialText)
        {
            tutorialText.fontSize = 54;
            tutorialText.color = Color.white;
            tutorialText.text = "";

            yield return TypeTextSkippable(tutorialText, wiredMessage, typewriterSpeed);
        }
        
        // 显示1.5秒
        yield return WaitSkippable(1.5f);
        
        // ⭐ 隐藏跳过提示
        if (skipHintUI) skipHintUI.SetActive(false);
        
        // 然后清空文本，准备显示红色警告
        if (tutorialText)
        {
            tutorialText.text = "";
            tutorialText.fontSize = 54;
            tutorialText.color = Color.red;
        }

        if (runWarningClip && AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(runWarningClip);
        }

        yield return StartCoroutine(RunWarningAnimation());
        yield return WaitSkippable(preChaseHold);

        if (monster) monster.aiEnabled = true;

        isTutorialActive = false;

        if (tutorialText)
        {
            yield return FadeText(0f, 1f);
            tutorialText.text = "";
        }

        runningCutscene = false;
        stage = 99;
    }

    IEnumerator TypeTextSkippable(TextMeshProUGUI textComponent, string message, float speed)
    {
        skipRequested = false;
        textComponent.text = "";

        foreach (char c in message)
        {
            if (skipRequested)
            {
                textComponent.text = message;
                skipRequested = false;
                Debug.Log("⏭️ 文本已跳过");
                yield break;
            }

            textComponent.text += c;
            yield return new WaitForSeconds(speed);
        }
    }

    IEnumerator WaitSkippable(float duration)
    {
        float elapsed = 0f;
        skipRequested = false;

        while (elapsed < duration)
        {
            if (skipRequested)
            {
                skipRequested = false;
                Debug.Log("⏭️ 等待已跳过");
                yield break;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator RunWarningAnimation()
    {
        if (!tutorialText) yield break;
        string text = runWarning;
        float shakeIntensity = 20f;
        float scaleIntensity = 1.5f;
        int pulseCount = 3;

        Vector3 originalScale = tutorialText.transform.localScale;

        skipRequested = false;

        foreach (char c in text)
        {
            if (skipRequested)
            {
                tutorialText.text = text;
                tutorialText.transform.localScale = originalScale;
                tutorialText.transform.localPosition = Vector3.zero;
                tutorialText.color = Color.red;
                skipRequested = false;
                Debug.Log("⏭️ 警告动画已跳过");
                yield break;
            }

            tutorialText.text += c;
            tutorialText.transform.localPosition = new Vector3(
                Random.Range(-shakeIntensity, shakeIntensity),
                Random.Range(-shakeIntensity, shakeIntensity),
                0
            );
            yield return new WaitForSeconds(0.08f);
        }

        for (int i = 0; i < pulseCount; i++)
        {
            if (skipRequested)
            {
                tutorialText.transform.localScale = originalScale;
                tutorialText.transform.localPosition = Vector3.zero;
                skipRequested = false;
                yield break;
            }

            float t = 0f;
            while (t < 0.15f)
            {
                t += Time.deltaTime;
                float scale = Mathf.Lerp(1f, scaleIntensity, t / 0.15f);
                tutorialText.transform.localScale = originalScale * scale;
                yield return null;
            }
            t = 0f;
            while (t < 0.15f)
            {
                t += Time.deltaTime;
                float scale = Mathf.Lerp(scaleIntensity, 1f, t / 0.15f);
                tutorialText.transform.localScale = originalScale * scale;
                yield return null;
            }
        }

        tutorialText.transform.localScale = originalScale;
        tutorialText.transform.localPosition = Vector3.zero;
        tutorialText.color = Color.red;
    }

    IEnumerator Fade(float target, float duration)
    {
        if (!fadeOverlay) yield break;
        float start = fadeOverlay.alpha;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            fadeOverlay.alpha = Mathf.Lerp(start, target, t / duration);
            yield return null;
        }
        fadeOverlay.alpha = target;
    }

    IEnumerator FadeText(float targetAlpha, float duration)
    {
        if (!tutorialText) yield break;
        Color startColor = tutorialText.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            tutorialText.color = Color.Lerp(startColor, targetColor, t / duration);
            yield return null;
        }
        tutorialText.color = targetColor;
    }

    void OnDrawGizmosSelected()
    {
        if (goToPoint)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(goToPoint.position, arriveRadius);
        }
    }
}