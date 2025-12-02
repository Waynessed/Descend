using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;  // ⭐ 新增
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject audioPanel;
    public GameObject graphicsPanel;    // ⭐ 新增
    public GameObject controlsPanel;    // ⭐ 新增

    [Header("Tab Buttons")]
    public Button audioButton;
    public Button graphicsButton;       // ⭐ 新增
    public Button controlsButton;       // ⭐ 新增

    [Header("Tab Button Underlines")]
    public GameObject audioUnderline;
    public GameObject graphicsUnderline;    // ⭐ 新增
    public GameObject controlsUnderline;    // ⭐ 新增

    [Header("Tab Visual Settings")]
    public float selectedScale = 1.2f;
    public float normalScale = 1.0f;
    public Color selectedColor = Color.white;
    public Color normalColor = new Color(0.6f, 0.6f, 0.6f, 1f);

    [Header("Audio Sliders")]
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider effectVolumeSlider;

    [Header("Volume Value Texts")]
    public TextMeshProUGUI masterVolumeText;
    public TextMeshProUGUI musicVolumeText;
    public TextMeshProUGUI effectVolumeText;

    [Header("Save Buttons")]
    public Button saveAudioButton;
    public Button saveGraphicsButton;   // ⭐ 新增

    [Header("Other Buttons")]
    public Button backButton;
    public Button restartButton;        // ⭐ 新增

    [Header("Navigation")]
    public GameObject homePage;
    
    [Header("⭐ Restart Settings")]
    public string mainMenuSceneName = "MainMenu";
    public GameObject restartConfirmDialog;  // 可选：确认对话框

    void Start()
    {
        Debug.Log("🔧 SettingsManager Start() 开始执行");

        // ⭐ WebGL浏览器快捷键禁用（自动初始化）
        #if UNITY_WEBGL && !UNITY_EDITOR
        WebGLBrowserShortcutManager.Initialize();
        #endif

        // 检查back button
        if (backButton == null)
        {
            Debug.LogError("❌ backButton 未连接！");
        }
        else
        {
            Debug.Log("✅ backButton 已找到: " + backButton.name);
            backButton.onClick.AddListener(OnBackClicked);
            Debug.Log("✅ OnBackClicked 已通过代码绑定到backButton");
        }

        // ⭐ 检查restart button
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(ShowRestartConfirmation);
            Debug.Log("✅ RestartButton 已绑定");
        }

        // 检查homePage
        if (homePage == null)
        {
            Debug.LogError("❌ homePage 未连接！");
        }
        else
        {
            Debug.Log("✅ homePage 已找到: " + homePage.name);
        }

        // 绑定Tab按钮事件
        if (audioButton != null)
            audioButton.onClick.AddListener(() => SwitchPanel("audio"));
        
        // ⭐ 新增
        if (graphicsButton != null)
            graphicsButton.onClick.AddListener(() => SwitchPanel("graphics"));
        
        if (controlsButton != null)
            controlsButton.onClick.AddListener(() => SwitchPanel("controls"));

        // 绑定Audio保存按钮
        if (saveAudioButton != null)
            saveAudioButton.onClick.AddListener(SaveAudioSettings);
        
        // ⭐ 绑定Graphics保存按钮
        if (saveGraphicsButton != null)
            saveGraphicsButton.onClick.AddListener(SaveGraphicsSettings);

        // 绑定滑动条事件
        if (masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        if (effectVolumeSlider != null)
            effectVolumeSlider.onValueChanged.AddListener(OnEffectVolumeChanged);

        // 加载保存的设置
        LoadAllSettings();

        // 应用当前音量设置到AudioManager
        ApplyAudioSettings();

        // 默认显示Audio面板
        SwitchPanel("audio");

        Debug.Log("✅ SettingsManager 初始化完成");
    }

    void SwitchPanel(string panelName)
    {
        // 隐藏所有面板
        if (audioPanel != null)
            audioPanel.SetActive(false);
        if (graphicsPanel != null)  // ⭐ 新增
            graphicsPanel.SetActive(false);
        if (controlsPanel != null)  // ⭐ 新增
            controlsPanel.SetActive(false);

        // 重置所有按钮状态
        ResetButtonStyle(audioButton, audioUnderline);
        ResetButtonStyle(graphicsButton, graphicsUnderline);    // ⭐ 新增
        ResetButtonStyle(controlsButton, controlsUnderline);    // ⭐ 新增

        // 显示选中的面板并设置选中样式
        switch (panelName)
        {
            case "audio":
                if (audioPanel != null)
                {
                    audioPanel.SetActive(true);
                    SetButtonSelected(audioButton, audioUnderline);
                }
                break;

            case "graphics":  // ⭐ 新增
                if (graphicsPanel != null)
                {
                    graphicsPanel.SetActive(true);
                    SetButtonSelected(graphicsButton, graphicsUnderline);
                }
                break;

            case "controls":  // ⭐ 新增
                if (controlsPanel != null)
                {
                    controlsPanel.SetActive(true);
                    SetButtonSelected(controlsButton, controlsUnderline);
                }
                break;
        }
    }

    void SetButtonSelected(Button button, GameObject underline)
    {
        if (button != null)
        {
            button.transform.localScale = new Vector3(selectedScale, selectedScale, 1f);
            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.color = selectedColor;
            }
        }

        if (underline != null)
        {
            underline.SetActive(true);
        }
    }

    void ResetButtonStyle(Button button, GameObject underline)
    {
        if (button != null)
        {
            button.transform.localScale = new Vector3(normalScale, normalScale, 1f);
            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.color = normalColor;
            }
        }

        if (underline != null)
        {
            underline.SetActive(false);
        }
    }

    void SaveAudioSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolumeSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
        PlayerPrefs.SetFloat("EffectVolume", effectVolumeSlider.value);
        PlayerPrefs.Save();

        AudioListener.volume = masterVolumeSlider.value / 100f;

        Debug.Log("✅ Audio settings saved!");

        ApplyAudioSettings();
    }
    
    // ⭐ 保存Graphics设置（由 GraphicsSettings 脚本调用）
    void SaveGraphicsSettings()
    {
        // GraphicsSettings 脚本会处理保存
        Debug.Log("✅ Graphics settings saved!");
    }

    void LoadAllSettings()
    {
        if (masterVolumeSlider != null)
            masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 100);
        if (musicVolumeSlider != null)
            musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 100);
        if (effectVolumeSlider != null)
            effectVolumeSlider.value = PlayerPrefs.GetFloat("EffectVolume", 100);
    }

    void OnBackClicked()
    {
        Debug.Log("🔙 点击了Back按钮");

        // ⭐ 优先检查是否在游戏中（有 PauseMenuController）
        PauseMenuController pauseMenu = FindObjectOfType<PauseMenuController>();
        
        if (pauseMenu != null)
        {
            // 在游戏中，恢复游戏
            pauseMenu.OnSettingsBackClicked();
            Debug.Log("✅ 游戏已恢复");
            gameObject.SetActive(false);
            return;  // ⭐ 提前返回，不执行下面的代码
        }
        
        // 在主菜单中，返回主页
        if (homePage != null)
        {
            homePage.SetActive(true);
            Debug.Log("✅ 已返回HomePanel (主菜单)");
        }
        else
        {
            Debug.LogWarning("⚠️ homePage 未设置，但这是正常的（在游戏场景中）");
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        gameObject.SetActive(false);
    }
    
    void ApplyAudioSettings()
    {
        if (AudioManager.instance != null)
        {
            float masterVol = masterVolumeSlider.value / 100f;
            float musicVol = musicVolumeSlider.value / 100f;
            float sfxVol = effectVolumeSlider.value / 100f;

            AudioManager.instance.SetMusicVolume(masterVol * musicVol);
            AudioManager.instance.SetSFXVolume(masterVol * sfxVol);
            
            Debug.Log("🎚️ 音量设置已应用");
        }
    }

    void OnMasterVolumeChanged(float value)
    {
        if (masterVolumeText != null)
            masterVolumeText.text = value.ToString("F0");
        ApplyAudioSettings();
    }

    void OnMusicVolumeChanged(float value)
    {
        if (musicVolumeText != null)
            musicVolumeText.text = value.ToString("F0");
        ApplyAudioSettings();
    }

    void OnEffectVolumeChanged(float value)
    {
        if (effectVolumeText != null)
            effectVolumeText.text = value.ToString("F0");
        ApplyAudioSettings();
    }
    
    // ⭐⭐⭐ Restart 功能 ⭐⭐⭐
    
    // 显示重启确认对话框
    public void ShowRestartConfirmation()
    {
        if (restartConfirmDialog != null)
        {
            restartConfirmDialog.SetActive(true);
            Debug.Log("⚠️ 显示重启确认对话框");
        }
        else
        {
            // 没有对话框，直接重启
            ConfirmRestart();
        }
    }
    
    // 确认重启
    public void ConfirmRestart()
    {
        Debug.Log("🔄 确认重启，重新开始游戏");
        
        // ⭐ 设置新游戏标志（让SafeHouseManager在重新加载游戏场景时重置进度）
        PlayerPrefs.SetInt("IsNewGame", 1);
        PlayerPrefs.Save();
        
        // 停止所有音频
        if (AudioManager.instance != null)
        {
            AudioManager.instance.StopMusic();           // 停止背景音乐
            AudioManager.instance.StopPlayerFootsteps(); // 停止玩家脚步声
            AudioManager.instance.StopHeartbeat();
        }
        
        // 停止所有其他AudioSource（包括怪物脚步声、心跳等）
        AudioSource[] allAudioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (AudioSource source in allAudioSources)
        {
            if (source.isPlaying)
            {
                source.Stop();
            }
        }
        
        Time.timeScale = 1f;  // 恢复游戏时间
        // ⭐ 直接重新加载游戏场景（而不是返回主菜单）
        SceneManager.LoadScene("GameScene");
    }
    
    // 取消重启
    public void CancelRestart()
    {
        if (restartConfirmDialog != null)
        {
            restartConfirmDialog.SetActive(false);
            Debug.Log("❌ 取消重启");
        }
    }
}