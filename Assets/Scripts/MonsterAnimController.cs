using UnityEngine;
using System.Collections;

/// <summary>
/// 怪物动画控制器，用于控制怪物动画的播放和速度
/// </summary>
public class MonsterAnimController : MonoBehaviour
{
    [Header("动画控制")]
    public Animator monsterAnimator;
    [Tooltip("动画触发器参数名称")]
    public string animTrigger = "";
    [Tooltip("动画状态名称（备用）")]
    public string animName = "";
    
    [Header("速度设置")]
    [Range(0.1f, 3f)] public float animSpeed = 1f;
    
    [Header("播放设置")]
    public float startDelay = 0f;
    
    void Start()
    {
        // 如果没有指定Animator，尝试获取当前GameObject上的
        if (monsterAnimator == null)
        {
            monsterAnimator = GetComponent<Animator>();
        }
        
        if (monsterAnimator == null)
        {
            Debug.LogError("⚠️ MonsterAnimController: 未找到Animator组件！");
            return;
        }
        
        // 启动播放协程
        StartCoroutine(PlayAnimation());
    }
    
    /// <summary>
    /// 播放动画
    /// </summary>
    IEnumerator PlayAnimation()
    {
        // 延迟
        if (startDelay > 0f)
        {
            yield return new WaitForSeconds(startDelay);
        }
        
        // 设置动画速度
        monsterAnimator.speed = animSpeed;
        Debug.Log($"🎬 怪物动画速度设置为: {animSpeed}x");
        
        // 方式1：优先使用触发器
        if (!string.IsNullOrEmpty(animTrigger))
        {
            Debug.Log($"🎬 触发怪物动画: {animTrigger}");
            monsterAnimator.SetTrigger(animTrigger);
        }
        // 方式2：使用动画状态名称
        else if (!string.IsNullOrEmpty(animName))
        {
            Debug.Log($"🎬 播放怪物动画: {animName}");
            monsterAnimator.Play(animName);
        }
        else
        {
            Debug.LogWarning("⚠️ MonsterAnimController: 动画触发器或名称都未设置！");
        }
    }
    
    /// <summary>
    /// 外部调用：设置动画速度
    /// </summary>
    public void SetSpeed(float speed)
    {
        if (monsterAnimator != null)
        {
            animSpeed = Mathf.Clamp(speed, 0.1f, 3f);
            monsterAnimator.speed = animSpeed;
            Debug.Log($"🎬 怪物动画速度更新为: {animSpeed}x");
        }
    }
    
    /// <summary>
    /// 外部调用：播放动画
    /// </summary>
    public void PlayAnim()
    {
        StartCoroutine(PlayAnimation());
    }
}

