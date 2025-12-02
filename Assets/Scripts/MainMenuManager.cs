using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject homePanel;           // HomePanel
    public GameObject settingPanel;        // SettingPanel

    [Header("Buttons")]
    public Button newGameButton;          
    public Button settingsButton;         

    void Start()
    {
        Debug.Log("MainMenuManager 启动");
        
        // ⭐ WebGL浏览器快捷键禁用（在游戏启动时自动初始化）
        #if UNITY_WEBGL && !UNITY_EDITOR
        WebGLBrowserShortcutManager.Initialize();
        #endif
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 检查引用
        if (newGameButton == null) Debug.LogError("❌ newGameButton 未连接！");
        if (settingsButton == null) Debug.LogError("❌ settingsButton 未连接！");
        if (homePanel == null) Debug.LogError("❌ homePanel 未连接！");
        if (settingPanel == null) Debug.LogError("❌ settingPanel 未连接！");
        
        // 绑定按钮
        if (newGameButton != null)
            newGameButton.onClick.AddListener(OnNewGame);
        
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnOpenSettings);

        // 初始状态：显示主菜单，隐藏设置
        if (homePanel != null) homePanel.SetActive(true);
        if (settingPanel != null) settingPanel.SetActive(false);
        
        Debug.Log("✅ MainMenuManager 初始化完成");
    }

    void OnNewGame()
    {
        Debug.Log("🎮 点击了New Game按钮");
        
        // ⭐ 设置新游戏标志（让SafeHouseManager在Start()时重置进度）
        PlayerPrefs.SetInt("IsNewGame", 1);
        PlayerPrefs.Save();
        
        // 停止主菜单音乐
        if (AudioManager.instance != null)
        {
            AudioManager.instance.StopMusic();
            Debug.Log("🔇 主菜单音乐已停止");
        }
        
        // 加载游戏场景（SafeHouseManager会在Start()时检测IsNewGame标志并重置进度）
        Debug.Log("📂 正在加载 GameScene...");
        SceneManager.LoadScene("GameScene");
    }

    void OnOpenSettings()
    {
        Debug.Log("⚙️ 点击了Settings按钮");
        
        if (homePanel != null)
        {
            homePanel.SetActive(false);
            Debug.Log("✅ HomePanel 已隐藏");
        }
        
        if (settingPanel != null)
        {
            settingPanel.SetActive(true);
            Debug.Log("✅ SettingPanel 已显示");
        }
    }
}