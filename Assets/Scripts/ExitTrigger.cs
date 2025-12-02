using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ExitTrigger : MonoBehaviour
{
    [Header("Exit Settings")]
    public string playerTag = "Player";
    public string survivalSceneName = "SurvivalScene";
    
    [Header("⭐ Transition Settings")]
    public CanvasGroup fadeOverlay;        // 可选：黑屏淡入效果
    public float fadeTime = 1f;
    public float escapeDelay = 1.5f;       // 延迟时间
    
    private bool hasTriggered = false;

    void Start()
    {
        Debug.Log("✅ 出口已设置（自动触发模式）");
        
        // ⭐ 测试：查找 Player
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
        {
            Debug.Log($"✅ 找到 Player: {player.name}");
            
            // 检查 Player 的 Collider
            if (player.GetComponent<CharacterController>() != null)
            {
                Debug.Log("✅ Player 有 CharacterController");
            }
            else if (player.GetComponent<Collider>() != null)
            {
                Debug.Log("✅ Player 有 Collider");
            }
            else
            {
                Debug.LogError("❌ Player 没有 Collider 或 CharacterController！");
            }
        }
        else
        {
            Debug.LogError($"❌ 找不到 Tag 为 '{playerTag}' 的对象！");
        }
        
        // ⭐ 检查自己的 Collider
        BoxCollider boxCol = GetComponent<BoxCollider>();
        if (boxCol != null)
        {
            Debug.Log($"✅ Exit 有 BoxCollider, Is Trigger: {boxCol.isTrigger}");
            if (!boxCol.isTrigger)
            {
                Debug.LogError("❌ BoxCollider 的 Is Trigger 没有勾选！");
            }
        }
        else
        {
            Debug.LogError("❌ Exit 没有 BoxCollider！");
        }

        // 初始化黑屏
        if (fadeOverlay != null)
        {
            fadeOverlay.alpha = 0f;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // ⭐ 添加详细 Debug
        Debug.Log($"🔍 OnTriggerEnter 检测到碰撞：{other.gameObject.name}, Tag: {other.tag}");
        
        // ⭐ 改成自动触发：碰到就立即触发
        if (other.CompareTag(playerTag) && !hasTriggered)
        {
            Debug.Log("🚪 玩家到达出口，自动触发逃离！");
            TriggerEscape();
        }
        else if (!other.CompareTag(playerTag))
        {
            Debug.LogWarning($"⚠️ Tag 不匹配！期望: '{playerTag}', 实际: '{other.tag}'");
        }
        else if (hasTriggered)
        {
            Debug.LogWarning("⚠️ 已经触发过了，忽略");
        }
    }
    
    // ⭐ 新增：也监听 OnCollisionEnter（如果 Is Trigger 没勾选）
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"🔍 OnCollisionEnter 检测到碰撞：{collision.gameObject.name}, Tag: {collision.gameObject.tag}");
        
        if (collision.gameObject.CompareTag(playerTag) && !hasTriggered)
        {
            Debug.Log("🚪 玩家到达出口（通过 Collision），自动触发逃离！");
            TriggerEscape();
        }
    }

    void TriggerEscape()
    {
        hasTriggered = true;
        Debug.Log("🎉 玩家逃离成功！");
        
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
        
        // 延迟加载生存场景
        StartCoroutine(LoadSurvivalSceneDelayed());
    }

    IEnumerator LoadSurvivalSceneDelayed()
    {
        Debug.Log("⏳ 开始加载场景流程...");
        
        // ⭐ 如果有黑屏效果，执行淡入
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
            Debug.Log($"⏰ 等待 {escapeDelay} 秒...");
            yield return new WaitForSeconds(escapeDelay);
        }
        
        Debug.Log($"🎬 准备加载场景: {survivalSceneName}");
        
        // ⭐ 检查场景是否存在
        if (Application.CanStreamedLevelBeLoaded(survivalSceneName))
        {
            Debug.Log($"✅ 场景 '{survivalSceneName}' 存在，开始加载");
            SceneManager.LoadScene(survivalSceneName);
        }
        else
        {
            Debug.LogError($"❌ 场景 '{survivalSceneName}' 不存在或未添加到 Build Settings！");
            Debug.LogError("请检查：File → Build Settings → Scenes In Build");
        }
    }
}