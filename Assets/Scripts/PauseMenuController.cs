using UnityEngine;
public class PauseMenuController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject settingsPanel;  // 你的 SettingPanel
    
    [Header("Input Settings")]
    public KeyCode pauseKey = KeyCode.Escape;  // 按 ESC 打开设置
    
    [Header("Player References")]
    public MonoBehaviour playerMovement;  // 玩家移动脚本
    public MonoBehaviour playerLook;      // 玩家视角脚本（如果有）
    public MonoBehaviour playerInteract;  // 玩家交互脚本（如果有）
    
    private bool isPaused = false;
    
    void Update()
    {
        // 检测按键
        if (Input.GetKeyDown(pauseKey))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    
    // ⭐ 暂停游戏
    public void PauseGame()
    {
        isPaused = true;
        
        // 1. 暂停游戏时间（影响所有使用 Time.deltaTime 的逻辑）
        Time.timeScale = 0f;
        
        // 2. 显示设置面板
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
        
        // 3. 禁用玩家控制
        DisablePlayerControls();
        
        // 4. 显示并解锁鼠标
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // 5. ⭐ 音乐不暂停（AudioSource 不受 timeScale 影响）
        // AudioManager 的音乐会继续播放
        
        Debug.Log("⏸️ 游戏已暂停");
    }
    
    // ⭐ 恢复游戏
    public void ResumeGame()
    {
        isPaused = false;
        
        // 1. 恢复游戏时间
        Time.timeScale = 1f;
        
        // 2. 隐藏设置面板
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
        
        // 3. 启用玩家控制
        EnablePlayerControls();
        
        // 4. 锁定鼠标（第一人称游戏）
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        Debug.Log("▶️ 游戏已恢复");
    }
    
    // 禁用玩家控制
    void DisablePlayerControls()
    {
        if (playerMovement != null)
            playerMovement.enabled = false;
        
        if (playerLook != null)
            playerLook.enabled = false;
        
        if (playerInteract != null)
            playerInteract.enabled = false;
    }
    
    // 启用玩家控制
    void EnablePlayerControls()
    {
        if (playerMovement != null)
            playerMovement.enabled = true;
        
        if (playerLook != null)
            playerLook.enabled = true;
        
        if (playerInteract != null)
            playerInteract.enabled = true;
    }

    // 供 SettingsManager 的 Back 按钮调用
    public void OnSettingsBackClicked()
    {
        ResumeGame();
    }
    
}