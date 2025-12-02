using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TrueExit : MonoBehaviour
{
    [Header("Exit Settings")]
    public string playerTag = "Player";
    public string trueEndingSceneName = "TrueEndingScene";
    
    [Header("Transition Settings")]
    public CanvasGroup fadeOverlay;        // 可选：黑屏淡入效果
    public float fadeTime = 2f;
    public float exitDelay = 1.5f;        // 延迟时间
    
    private bool isEnabled = false;
    private bool hasTriggered = false;
    
    void Start()
    {
        // 初始化：禁用真出口
        // 注意：如果已经被 SafeHouseManager 启用，不再禁用
        // 让 SafeHouseManager 完全控制激活状态
        if (!isEnabled)
        {
            gameObject.SetActive(false);
            Debug.Log("🎯 真出口初始化（已禁用）");
        }
        else
        {
            Debug.Log("🎯 真出口已在 Awake 中被 SafeHouseManager 启用");
        }
        
        // 初始化黑屏
        if (fadeOverlay != null)
        {
            fadeOverlay.alpha = 0f;
        }
    }
    
    // 被SafeHouseManager调用以启用真出口
    public void SetActive(bool active)
    {
        isEnabled = active;
        gameObject.SetActive(active);
        
        if (active)
        {
            Debug.Log("✅ 真出口已激活！");
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) && isEnabled && !hasTriggered)
        {
            Debug.Log("🎉 玩家到达真出口！触发真结局！");
            TriggerTrueEnding();
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(playerTag) && isEnabled && !hasTriggered)
        {
            Debug.Log("🎉 玩家到达真出口（通过碰撞）！触发真结局！");
            TriggerTrueEnding();
        }
    }

    void TriggerTrueEnding()
    {
        hasTriggered = true;
        Debug.Log("🏆 真结局触发！");
        
        // 标记游戏结束
        if (PlayerState.Instance != null)
        {
            PlayerState.Instance.isGameOver = true;
        }
        
        // 停止背景音乐
        if (AudioManager.instance != null)
        {
            AudioManager.instance.StopMusic();
            AudioManager.instance.StopPlayerFootsteps();
        }
        
        // 解锁光标
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // 延迟加载真结局场景
        StartCoroutine(LoadTrueEndingScene());
    }

    IEnumerator LoadTrueEndingScene()
    {
        Debug.Log("⏳ 开始加载真结局场景...");
        
        // 如果有黑屏效果，执行淡入
        if (fadeOverlay != null)
        {
            Debug.Log("🎬 执行黑屏淡入...");
            float timer = 0f;
            while (timer < fadeTime)
            {
                timer += Time.deltaTime;
                fadeOverlay.alpha = Mathf.Lerp(0f, 1f, timer / fadeTime);
                yield return null;
            }
            fadeOverlay.alpha = 1f;
        }
        else
        {
            // 没有黑屏就等待延迟
            Debug.Log($"⏰ 等待 {exitDelay} 秒...");
            yield return new WaitForSeconds(exitDelay);
        }
        
        Debug.Log($"🎬 准备加载真结局场景: {trueEndingSceneName}");
        
        // 尝试检查场景索引
        int sceneIndex = SceneManager.GetSceneByName(trueEndingSceneName).buildIndex;
        if (sceneIndex >= 0)
        {
            Debug.Log($"✅ 场景 '{trueEndingSceneName}' 在 Build Settings 中（索引: {sceneIndex}）");
            SceneManager.LoadScene(trueEndingSceneName);
        }
        else if (Application.CanStreamedLevelBeLoaded(trueEndingSceneName))
        {
            Debug.Log($"✅ 场景 '{trueEndingSceneName}' 可流式加载");
            SceneManager.LoadScene(trueEndingSceneName);
        }
        else
        {
            Debug.LogError($"❌ 场景 '{trueEndingSceneName}' 不存在或未添加到 Build Settings！");
            Debug.LogError($"当前 Build Settings 中的场景:");
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                Debug.LogError($"  [{i}] {sceneName}");
            }
            Debug.LogError("请检查：File → Build Settings → Scenes In Build");
        }
    }
}

