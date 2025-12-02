using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TrueEndScene : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI deathText;
    public Button mainMenuButton;
    
    void Start()
    {
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(OnMainMenu);
        }
        
        // 确保光标可见
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    void OnMainMenu()
    {
        Debug.Log("🏠 返回主菜单");
        
        // 播放主菜单音乐
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayMusic(AudioManager.instance.mainMenuMusic);
        }
        
        SceneManager.LoadScene("StartScene");
    }
}