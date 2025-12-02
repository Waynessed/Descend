using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SafeHouseManager : MonoBehaviour
{
    public static SafeHouseManager Instance { get; private set; }
    
    [Header("Safe House Settings")]
    public int totalSafeHouses = 6;
    public string trueEndingScene = "TrueEndingScene";
    
    [Header("Debug")]
    public bool showDebugInfo = true;
    public bool alwaysEnableTrueExit = false;
    public bool resetProgressOnStart = false;
    
    private HashSet<int> visitedSafeHouseIDs = new HashSet<int>();
    
    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
        }

        // Reset/Load logic moved to Awake
        if (resetProgressOnStart)
        {
            ResetAllProgress();
            if (showDebugInfo)
            {
                Debug.Log("🧪 调试模式：强制重置安全屋进度");
            }
        }
        else if (PlayerPrefs.GetInt("IsNewGame", 0) == 1)
        {
            ResetAllProgress();
            PlayerPrefs.SetInt("IsNewGame", 0);
            PlayerPrefs.Save();
            if (showDebugInfo)
            {
                Debug.Log("🎮 检测到新游戏，已重置安全屋进度");
            }
        }
        else
        {
            LoadVisitedSafeHouses();
        }
    }
    
    private void Start()
    {
        // Delayed update after all Start() methods complete
        StartCoroutine(DelayedUpdateTrueExit());
    }
    
    IEnumerator DelayedUpdateTrueExit()
    {
        yield return null;
        UpdateTrueExitStatus();
    }
    
    public void RegisterSafeHouseVisit(int safeHouseID)
    {
        if (!visitedSafeHouseIDs.Contains(safeHouseID))
        {
            visitedSafeHouseIDs.Add(safeHouseID);
            SaveVisitedSafeHouses();
            UpdateTrueExitStatus();
            
            if (showDebugInfo)
            {
                Debug.Log($"🏠 已访问安全屋数量: {visitedSafeHouseIDs.Count}/{totalSafeHouses}");
            }
        }
    }
    
    void UpdateTrueExitStatus()
    {
        if (alwaysEnableTrueExit)
        {
            EnableTrueExit();
            if (showDebugInfo)
            {
                Debug.Log("🧪 测试模式：真出口已启用");
            }
        }
        else if (HasVisitedAllSafeHouses())
        {
            EnableTrueExit();
            if (showDebugInfo)
            {
                Debug.Log("🎉 已访问全部安全屋！真出口已出现！");
            }
        }
    }
    
    public bool HasVisitedAllSafeHouses()
    {
        return visitedSafeHouseIDs.Count >= totalSafeHouses;
    }
    
    void EnableTrueExit()
    {
        TrueExit[] trueExits = FindObjectsOfType<TrueExit>(true);
        
        foreach (TrueExit trueExit in trueExits)
        {
            trueExit.SetActive(true);
        }
        
        if (trueExits.Length == 0 && showDebugInfo)
        {
            Debug.LogWarning("⚠️ 未找到TrueExit组件");
        }
        else if (showDebugInfo && trueExits.Length > 0)
        {
            Debug.Log($"✅ 找到 {trueExits.Length} 个TrueExit组件并已启用");
        }
    }
    
    void SaveVisitedSafeHouses()
    {
        PlayerPrefs.SetInt("VisitedSafeHouseCount", visitedSafeHouseIDs.Count);
        
        int index = 0;
        foreach (int id in visitedSafeHouseIDs)
        {
            PlayerPrefs.SetInt($"SafeHouseVisit_{index}", id);
            index++;
        }
        
        PlayerPrefs.SetInt("TotalSafeHouseVisits", index);
        PlayerPrefs.Save();
    }
    
    void LoadVisitedSafeHouses()
    {
        int count = PlayerPrefs.GetInt("VisitedSafeHouseCount", 0);
        
        for (int i = 0; i < count; i++)
        {
            int id = PlayerPrefs.GetInt($"SafeHouseVisit_{i}", -1);
            if (id != -1)
            {
                visitedSafeHouseIDs.Add(id);
            }
        }
        
        if (showDebugInfo && count > 0)
        {
            Debug.Log($"🏠 加载已访问安全屋: {visitedSafeHouseIDs.Count}个");
        }
    }
    
    public int GetVisitedCount()
    {
        return visitedSafeHouseIDs.Count;
    }
    
    public void ResetAllProgress()
    {
        int savedCount = PlayerPrefs.GetInt("TotalSafeHouseVisits", 0);
        List<int> savedIDs = new List<int>();
        
        for (int i = 0; i < savedCount; i++)
        {
            int id = PlayerPrefs.GetInt($"SafeHouseVisit_{i}", -1);
            if (id != -1)
            {
                savedIDs.Add(id);
            }
        }
        
        visitedSafeHouseIDs.Clear();
        
        PlayerPrefs.DeleteKey("VisitedSafeHouseCount");
        PlayerPrefs.DeleteKey("TotalSafeHouseVisits");
        
        for (int i = 0; i < 20; i++)
        {
            PlayerPrefs.DeleteKey($"SafeHouseVisit_{i}");
        }
        
        foreach (int id in savedIDs)
        {
            PlayerPrefs.DeleteKey($"SafeHouse_{id}_Visited");
        }
        
        PlayerPrefs.Save();
        
        if (showDebugInfo)
        {
            Debug.Log($"🔄 安全屋进度已重置（清理了 {savedIDs.Count} 个安全屋记录）");
        }
    }
    
    private void OnDestroy()
    {
        SaveVisitedSafeHouses();
    }
}