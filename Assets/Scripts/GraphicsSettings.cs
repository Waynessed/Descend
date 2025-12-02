using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class GraphicsSettings : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Dropdown qualityDropdown;
    public UnityEngine.UI.Toggle fullscreenToggle;
    
    void Start()
    {
        SetupQualityDropdown();
        SetupFullscreenToggle();
    }
    
    // 设置画质下拉菜单
    void SetupQualityDropdown()
    {
        if (qualityDropdown == null)
        {
            return;
        }
        
        // 1. 清空选项
        qualityDropdown.ClearOptions();
        
        // 2. 获取 Unity 的所有画质等级名称
        List<string> qualityNames = new List<string>(QualitySettings.names);
        
        // 3. 添加到 Dropdown
        qualityDropdown.AddOptions(qualityNames);
        
        // 4. ⭐ 加载保存的画质设置
        int savedQuality = PlayerPrefs.GetInt("QualityLevel", QualitySettings.GetQualityLevel());
        QualitySettings.SetQualityLevel(savedQuality);
        
        // 5. 设置当前画质等级
        qualityDropdown.value = savedQuality;
        qualityDropdown.RefreshShownValue();
        
        // 6. 绑定事件
        qualityDropdown.onValueChanged.AddListener(SetQuality);
    }

    // 改变画质（当用户选择时调用）
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        
        // ⭐ 保存画质设置
        PlayerPrefs.SetInt("QualityLevel", qualityIndex);
        PlayerPrefs.Save();
    }
    
    // ⭐ 新增：设置全屏切换按钮
    void SetupFullscreenToggle()
    {
        if (fullscreenToggle == null)
        {
            return;
        }
        
        // 1. 加载保存的全屏设置
        bool savedFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        
        // 2. 应用全屏设置
        Screen.fullScreen = savedFullscreen;
        
        // 3. 设置 Toggle 状态
        fullscreenToggle.isOn = savedFullscreen;
        
        // 4. 绑定事件
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
    }
    
    // 切换全屏模式（当用户点击 Toggle 时调用）
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        
        // ⭐ 保存全屏设置
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();

    }
    
}