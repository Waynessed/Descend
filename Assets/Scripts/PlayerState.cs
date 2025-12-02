using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerState : MonoBehaviour
{
    public static PlayerState Instance { get; set; }

    // ---- Player San ----
    [SerializeField] public float currentSan;
    [SerializeField] public float maxSan;

    [Header("Game Over Settings")]
    public bool isGameOver = false;
    public string deathSceneName = "DeathScene";

    [Header("Sanity Decay Settings")]
    public float normalDecayRate = 1f;
    public float normalDecayInterval = 2f;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        currentSan = maxSan;
        isGameOver = false;
        StartCoroutine(decreaseSan());
        Debug.Log("🎮 PlayerState 初始化，当前理智: " + currentSan);
    }

    IEnumerator decreaseSan()
    {
        while (true)
        {
            if (isGameOver)
            {
                yield break;
            }

            // ⭐ 如果在教程中，不扣 SAN
            if (TutorialManager.isTutorialActive)
            {
                yield return new WaitForSeconds(normalDecayInterval);
                continue;  // 跳过这次扣除
            }

            currentSan -= normalDecayRate;
            currentSan = Mathf.Clamp(currentSan, 0f, maxSan);
            
            if (currentSan <= 0)
            {
                TriggerDeathEnding();
                yield break;
            }
            
            yield return new WaitForSeconds(normalDecayInterval);
        }
    }

    // 🆕 手动减少理智值（怪物接近、恐怖事件等）
    public void DecreaseSanity(float amount)
    {
        if (isGameOver) return;
        
        // ⭐ 如果在教程中，不扣 SAN
        if (TutorialManager.isTutorialActive)
        {
            Debug.Log("📚 教程期间不扣除 SAN 值");
            return;
        }
        
        currentSan -= amount;
        currentSan = Mathf.Clamp(currentSan, 0f, maxSan);
        
        // 检查是否理智归零
        if (currentSan <= 0)
        {
            TriggerDeathEnding();
        }
    }

    // 🆕 增加理智值（找到安全区等）
    public void IncreaseSanity(float amount)
    {
        if (isGameOver) return;
        
        currentSan += amount;
        currentSan = Mathf.Clamp(currentSan, 0f, maxSan);
        
        Debug.Log("💚 理智值恢复: " + amount + " | 当前: " + currentSan);
    }

    // 🆕 触发死亡结局
    public void TriggerDeathEnding()
    {
        if (isGameOver) return;
        
        isGameOver = true;
        Debug.Log("💀 理智归零！触发死亡结局...");
        
        // 停止音乐
        if (AudioManager.instance != null)
        {
            AudioManager.instance.StopMusic();
            AudioManager.instance.StopPlayerFootsteps();
        }
        
        // 解锁光标
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // 延迟2秒加载死亡场景
        StartCoroutine(LoadDeathSceneDelayed(2f));
    }

    // 🆕 延迟加载死亡场景
    IEnumerator LoadDeathSceneDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log("📂 加载死亡场景: " + deathSceneName);
        SceneManager.LoadScene(deathSceneName);
    }

    // 🆕 获取理智值百分比（用于UI效果）
    public float GetSanityPercentage()
    {
        return currentSan / maxSan;
    }

}