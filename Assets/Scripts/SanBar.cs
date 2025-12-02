using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SanBar : MonoBehaviour
{
    private Slider slider;
    [SerializeField] TextMeshProUGUI healthCounter;
    [SerializeField] GameObject playerState;
    
    private PlayerState playerStateScript;
    private float currentSan, maxSan;
    private float lastSan = -1f;
    private bool isFirstFrame = true;

    void Awake()
    {
        slider = GetComponent<Slider>();
        playerStateScript = playerState.GetComponent<PlayerState>();
    }

    void Update()
    {
        currentSan = playerStateScript.currentSan;
        maxSan = playerStateScript.maxSan;

        slider.value = 1 - (currentSan / maxSan);
        
        // 显示整数SAN值
        healthCounter.text = Mathf.RoundToInt(currentSan).ToString();

        // 检查 AudioManager 是否存在
        if (AudioManager.instance == null)
        {
            return;
        }

        // 第一帧时初始化音乐和心跳
        if (isFirstFrame)
        {
            AudioManager.instance.PlayGameplayMusic(); 
            AudioManager.instance.UpdateHeartbeatBySAN(currentSan);
            isFirstFrame = false;
        }

        // ⭐ 实时更新心跳频率（每帧检查）
        if (lastSan != currentSan)
        {
            AudioManager.instance.UpdateHeartbeatBySAN(currentSan);
        }
        
        lastSan = currentSan;
    }
}