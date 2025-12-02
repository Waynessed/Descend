using UnityEngine;
using System.Runtime.InteropServices;

/// <summary>
/// WebGL浏览器快捷键禁用器
/// 在游戏运行时禁用常见的浏览器快捷键，防止与游戏按键冲突
/// </summary>
public class WebGLBrowserShortcutDisabler : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void DisableBrowserShortcuts();

    [DllImport("__Internal")]
    private static extern void EnableBrowserShortcuts();
#else
    // 在编辑器或非WebGL平台上，这些函数不做任何事情
    private static void DisableBrowserShortcuts() { }
    private static void EnableBrowserShortcuts() { }
#endif

    void Awake()
    {
        // 只在WebGL构建时禁用浏览器快捷键
#if UNITY_WEBGL && !UNITY_EDITOR
        DisableBrowserShortcuts();
        Debug.Log("[WebGL] 浏览器快捷键已禁用");
#endif
    }

    void OnDestroy()
    {
        // 可选：在对象销毁时恢复快捷键（通常不需要）
        // #if UNITY_WEBGL && !UNITY_EDITOR
        //     EnableBrowserShortcuts();
        // #endif
    }

    /// <summary>
    /// 手动禁用浏览器快捷键（如果需要）
    /// </summary>
    public static void Disable()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        DisableBrowserShortcuts();
        Debug.Log("[WebGL] 浏览器快捷键已手动禁用");
#endif
    }

    /// <summary>
    /// 手动启用浏览器快捷键（如果需要）
    /// </summary>
    public static void Enable()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        EnableBrowserShortcuts();
        Debug.Log("[WebGL] 浏览器快捷键已手动启用");
#endif
    }
}
