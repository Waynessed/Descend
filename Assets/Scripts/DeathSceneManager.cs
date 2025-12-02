using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class DeathSceneManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI deathText;
    public Button mainMenuButton;
    
    void Start()
    {
        // 显示死亡信息
        if (deathText != null)
        {
            deathText.text = "You Lost Your Mind...";
        }
        
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(OnMainMenu);
        }
        
        // 确保光标可见
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        Debug.Log("💀 死亡场景已加载");
    }
    
    void OnMainMenu()
    {
        Debug.Log("🏠 返回主菜单");
        
        // 先停止所有音频
        if (AudioManager.instance != null)
        {
            AudioManager.instance.StopMusic();           // 停止背景音乐
            AudioManager.instance.StopPlayerFootsteps(); // 停止玩家脚步声
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
        
        // 然后播放主菜单音乐
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayMusic(AudioManager.instance.mainMenuMusic);
        }
        
        SceneManager.LoadScene("StartScene");
    }
}