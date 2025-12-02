using UnityEngine;

/// <summary>
/// WebGL浏览器快捷键管理器
/// 自动在游戏启动时禁用浏览器快捷键，并在所有场景中保持有效
/// 使用方法：将此脚本挂载到场景中的任意GameObject上，或者通过代码调用Initialize()
/// </summary>
public class WebGLBrowserShortcutManager : MonoBehaviour
{
    private static WebGLBrowserShortcutManager instance;

    void Awake()
    {
        // 确保只有一个实例存在
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            // 添加WebGLBrowserShortcutDisabler组件（如果还没有）
            WebGLBrowserShortcutDisabler disabler = GetComponent<WebGLBrowserShortcutDisabler>();
            if (disabler == null)
            {
                disabler = gameObject.AddComponent<WebGLBrowserShortcutDisabler>();
            }
            
            Debug.Log("[WebGL] 浏览器快捷键管理器已初始化");
        }
        else
        {
            // 如果已经存在实例，销毁当前对象
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 静态初始化方法，可以从任何地方调用
    /// </summary>
    public static void Initialize()
    {
        if (instance == null)
        {
            GameObject managerObj = new GameObject("WebGLBrowserShortcutManager");
            managerObj.AddComponent<WebGLBrowserShortcutManager>();
        }
    }

    /// <summary>
    /// 手动禁用浏览器快捷键
    /// </summary>
    public static void DisableShortcuts()
    {
        WebGLBrowserShortcutDisabler.Disable();
    }

    /// <summary>
    /// 手动启用浏览器快捷键
    /// </summary>
    public static void EnableShortcuts()
    {
        WebGLBrowserShortcutDisabler.Enable();
    }
}
