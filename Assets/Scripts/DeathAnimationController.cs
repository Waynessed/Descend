using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathAnimationController : MonoBehaviour
{
    [Header("Animation Settings")]
    public Animator monsterAnimator;          // 怪物Animator组件
    public float animationSpeed = 1f;        // 动画播放速度（1.0 = 正常速度）
    public float sceneDuration = 3f;        // 场景停留时间（秒）
    
    [Header("Scene Settings")]
    public string nextSceneName = "DeathScene";  // 下一个要加载的场景名称
    
    private void Start()
    {
        // 自动查找Animator（如果未手动指定）
        if (monsterAnimator == null)
        {
            // 首先在当前GameObject查找
            monsterAnimator = GetComponent<Animator>();
            if (monsterAnimator == null)
            {
                // 尝试在子对象中查找
                monsterAnimator = GetComponentInChildren<Animator>();
            }
            if (monsterAnimator == null)
            {
                // 在整个场景中查找第一个Animator（通常怪物会有Animator）
                Animator[] allAnimators = FindObjectsOfType<Animator>();
                if (allAnimators.Length > 0)
                {
                    monsterAnimator = allAnimators[0];
                    Debug.Log($"🔍 自动找到场景中的Animator: {monsterAnimator.gameObject.name}");
                }
            }
        }
        
        // 设置动画速度
        if (monsterAnimator != null)
        {
            monsterAnimator.speed = animationSpeed;
            Debug.Log($"🎬 死亡动画开始播放 - 速度: {animationSpeed}x, 持续时间: {sceneDuration}秒");
        }
        else
        {
            Debug.LogWarning("⚠️ DeathAnimationController: 未找到Animator组件！动画可能无法正常播放。");
        }
        
        // 确保光标可见
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // 延迟加载下一个场景
        Invoke(nameof(LoadNextScene), sceneDuration);
    }
    
    void LoadNextScene()
    {
        Debug.Log($"🎬 死亡动画播放完成，加载下一个场景: {nextSceneName}");
        SceneManager.LoadScene(nextSceneName);
    }
}

