using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SafeHouse : MonoBehaviour
{
    private bool isVisited = false;
    public int SafeHouseID { get; private set; }  // 安全屋的唯一ID
    
    void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void Start()
    {
        // 自动分配唯一ID（基于GameObject名称哈希）
        SafeHouseID = gameObject.name.GetHashCode();
        
        // 检查是否已被访问过（使用PlayerPrefs持久化）
        string visitedKey = $"SafeHouse_{SafeHouseID}_Visited";
        isVisited = PlayerPrefs.GetInt(visitedKey, 0) == 1;
    }
    
    void OnTriggerEnter(Collider other)
    {
        // 检测玩家到达
        if (other.CompareTag("Player") && !isVisited)
        {
            RegisterVisit();
        }
    }
    
    void RegisterVisit()
    {
        // 标记为已访问
        isVisited = true;
        string visitedKey = $"SafeHouse_{SafeHouseID}_Visited";
        PlayerPrefs.SetInt(visitedKey, 1);
        PlayerPrefs.Save();
        
        // 通知安全屋管理器
        if (SafeHouseManager.Instance != null)
        {
            SafeHouseManager.Instance.RegisterSafeHouseVisit(SafeHouseID);
        }
        
        Debug.Log($"🏠 玩家到达安全屋 {SafeHouseID}");
    }
    
    // 重置所有安全屋状态（用于新游戏）- 不建议直接调用，应该用SafeHouseManager.ResetAllProgress()
    public static void ResetAllSafeHouses()
    {
        // 这个方法现在是个空实现，实际的清理在SafeHouseManager中完成
        Debug.Log("🏠 SafeHouse.ResetAllSafeHouses() called (no-op)");
    }
}

