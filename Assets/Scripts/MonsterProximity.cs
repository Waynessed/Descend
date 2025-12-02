using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class MonsterProximity : MonoBehaviour
{
    [Header("Detection Settings")]
    public float detectionRadius = 15f;
    public float catchRadius = 10f;
    public string playerTag = "Player";

    [Header("Sanity Impact")]
    public float sanityDecayRate = 3f;

    [Header("Death Settings")]
    public string deathAnimationSceneName = "DeathAnimation";  // 死亡动画场景名称
    public string deathSceneName = "DeathScene";                // 最终死亡场景名称
    public float deathDelay = 0.5f;                              // 延迟加载死亡动画场景的时间

    [Header("Audio Settings")]
    public AudioClip farRoarSFX;                 // 远距离怒吼（>50米）
    public AudioClip nearRoarSFX;                // 近距离怒吼（≤50米）
    public AudioClip heavyFootstepSFX;           // ⭐ 脚步声音频（15秒）
    public float roarDistanceThreshold = 50f;    // 距离阈值（米）
    public float roarInterval = 10f;             // 怒吼间隔（秒）

    [Header("Footstep Settings")]
    public AudioSource footstepAudioSource;      // ⭐ 脚步声专用AudioSource（在Inspector中手动指定）
    public float maxFootstepVolume = 2.5f;       // ⭐ 最大音量
    public float maxFootstepDistance = 180f;     // ⭐ 超过此距离听不到脚步声
    public float minFootstepDistance = 50f;       // ⭐ 此距离内音量最大

    [Header("Visual Feedback (Optional)")]
    public Color detectionGizmoColor = Color.yellow;
    public Color catchGizmoColor = Color.red;

    [Header("Debug")]
    public bool showDistanceDebug = true;
    public float debugInterval = 0.5f;

    private GameObject player;
    private bool playerInRange = false;
    private bool isCaught = false;
    private float debugTimer = 0f;
    private float roarTimer = 0f;

    private NavMeshAgent monsterAgent;        // 怪物导航组件，用于检测移动

    void Start()
    {
        player = GameObject.FindGameObjectWithTag(playerTag);

        if (player == null)
        {
            Debug.LogError("❌ MonsterProximity: 未找到Player对象！请确保Player有 'Player' Tag");
        }
        else
        {
            Debug.Log("✅ MonsterProximity 已初始化");
        }

        // 获取NavMeshAgent组件
        monsterAgent = GetComponent<NavMeshAgent>();

        // 初始化怒吼计时器
        roarTimer = roarInterval;

        // ⭐ 如果未手动指定AudioSource，尝试自动创建
        if (footstepAudioSource == null)
        {
            footstepAudioSource = gameObject.AddComponent<AudioSource>();
            if (heavyFootstepSFX != null)
            {
                footstepAudioSource.clip = heavyFootstepSFX;
            }
            footstepAudioSource.loop = true;  // 循环播放
            footstepAudioSource.playOnAwake = false;
            footstepAudioSource.spatialBlend = 0f;  // 2D音效（不受3D空间影响）
            footstepAudioSource.outputAudioMixerGroup = null;  // 不使用MixerGroup
            footstepAudioSource.priority = 128;  // 标准优先级
            Debug.Log("🔊 自动创建AudioSource");
        }
        
        // 确保AudioSource配置正确
        if (footstepAudioSource != null)
        {
            if (heavyFootstepSFX != null && footstepAudioSource.clip != heavyFootstepSFX)
            {
                footstepAudioSource.clip = heavyFootstepSFX;
            }
            footstepAudioSource.loop = true;
            footstepAudioSource.spatialBlend = 0f;
            Debug.Log($"🔊 AudioSource初始化完成: clip={footstepAudioSource.clip?.name ?? "NULL"}, enabled={footstepAudioSource.enabled}");
        }
    }

    void Update()
    {
        if (player == null || PlayerState.Instance == null) return;
        if (PlayerState.Instance.isGameOver || isCaught) return;

        // 计算与玩家的距离
        float distance = Vector3.Distance(transform.position, player.transform.position);

        // ⭐ 根据怪物移动状态控制脚步声音频
        UpdateFootstepByMovement(distance);

        // 定期打印距离
        if (showDistanceDebug)
        {
            debugTimer += Time.deltaTime;
            if (debugTimer >= debugInterval)
            {
                //Debug.Log($"👹 怪物与玩家距离: {distance:F2} 米 " +
                         //$"(抓捕范围: {catchRadius}米, 理智减少范围: {detectionRadius}米) " +
                         //$"脚步声音量: {footstepAudioSource.volume:F2}");
                debugTimer = 0f;
            }
        }

        // 怒吼计时器（始终运行）
        roarTimer -= Time.deltaTime;
        if (roarTimer <= 0f)
        {
            PlayRoarByDistance(distance);
            roarTimer = roarInterval;
        }

        // 检查是否抓住玩家
        if (distance <= catchRadius)
        {
            Debug.Log($"💀 抓住玩家！距离: {distance:F2} 米");
            CatchPlayer();
            return;
        }

        // 检查玩家是否在理智减少范围内
        if (distance <= detectionRadius)
        {
            if (!playerInRange)
            {
                playerInRange = true;
                Debug.Log($"👹 怪物接近玩家！距离: {distance:F2} 米，理智加速下降");
            }

            PlayerState.Instance.DecreaseSanity(sanityDecayRate * Time.deltaTime);
        }
        else
        {
            if (playerInRange)
            {
                playerInRange = false;
                Debug.Log($"✅ 怪物远离玩家，当前距离: {distance:F2} 米");
            }
        }
    }

        // ⭐ 根据怪物移动状态和距离更新脚步声音频
    void UpdateFootstepByMovement(float distance)
    {
        if (footstepAudioSource == null || heavyFootstepSFX == null)
        {
            Debug.LogWarning($"⚠️ 脚步声音频未设置：footstepAudioSource={footstepAudioSource}, heavyFootstepSFX={heavyFootstepSFX}");
            return;
        }

        // 检测怪物是否在移动（速度阈值）
        bool isMoving = false;
        if (monsterAgent != null)
        {
            isMoving = monsterAgent.velocity.sqrMagnitude > 0.1f;
        }
        else
        {
            Debug.LogWarning("⚠️ MonsterProximity无法找到NavMeshAgent组件！");
        }

        // 根据距离计算音量
        float volume = 0.5f;
        if (distance <= minFootstepDistance)
        {
            volume = maxFootstepVolume;
        }
        else if (distance >= maxFootstepDistance)
        {
            volume = 0f;  // 太远完全听不到
        }
        else
        {
            float normalizedDistance = (distance - minFootstepDistance) / (maxFootstepDistance - minFootstepDistance);
            volume = Mathf.Lerp(maxFootstepVolume, 0f, normalizedDistance);
        }

        // 只在移动时播放脚步声
        if (isMoving && volume > 0f)
        {
            if (!footstepAudioSource.isPlaying)
            {
                footstepAudioSource.Play();
            }
            // Scale by global SFX volume (respects settings menu)
            if (AudioManager.instance != null)
                volume *= AudioManager.instance.GetSFXVolume();

            footstepAudioSource.volume = volume;

        }
        else
        {
            if (footstepAudioSource.isPlaying)
            {
                footstepAudioSource.Stop();
                Debug.Log($"👹 停止脚步声 - isMoving:{isMoving}, volume:{volume:F2}");
            }
        }
    }

    // 根据距离播放对应的怒吼音效
    void PlayRoarByDistance(float distance)
    {
        if (AudioManager.instance == null) return;

        // pick the correct clip
        AudioClip clip = (distance > roarDistanceThreshold) ? farRoarSFX : nearRoarSFX;
        if (clip == null) return;

        // get the current SFX volume from AudioManager
        float volume = AudioManager.instance.GetSFXVolume();

        // play the roar in 3D space at the monster’s position
        AudioSource.PlayClipAtPoint(clip, transform.position, volume);

        Debug.Log($"👹 怪物发出怒吼！（距离: {distance:F2}米, 音量:{volume:F2}）");
    }

    void CatchPlayer()
    {
        if (isCaught) return;

        isCaught = true;
        Debug.Log("💀 玩家被怪物抓住了！");

        // ⭐ 停止脚步声
        if (footstepAudioSource != null && footstepAudioSource.isPlaying)
        {
            footstepAudioSource.Stop();
        }

        if (PlayerState.Instance != null)
        {
            PlayerState.Instance.isGameOver = true;
        }

        DisablePlayerControls();

        if (AudioManager.instance != null)
        {
            AudioManager.instance.StopMusic();
        }

        Invoke(nameof(LoadDeathAnimationScene), deathDelay);
    }

    void DisablePlayerControls()
    {
        if (player != null)
        {
            var playerMovement = player.GetComponent<FPSPlayerControllerIS>();
            if (playerMovement != null)
            {
                playerMovement.enabled = false;
            }

            var fpsController = player.GetComponent<CharacterController>();
            if (fpsController != null)
            {
                fpsController.enabled = false;
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void LoadDeathAnimationScene()
    {
        if (AudioManager.instance != null)
            {
                AudioManager.instance.StopMusic();           // 停止背景音乐
                AudioManager.instance.StopPlayerFootsteps(); // 停止玩家脚步声
            }
        Debug.Log("🎬 加载死亡动画场景: " + deathAnimationSceneName);
        SceneManager.LoadScene(deathAnimationSceneName);
    }

    // void OnDrawGizmosSelected()
    // {
    //     Gizmos.color = detectionGizmoColor;
    //     Gizmos.DrawWireSphere(transform.position, detectionRadius);

    //     Gizmos.color = catchGizmoColor;
    //     Gizmos.DrawWireSphere(transform.position, catchRadius);

    //     // 绘制50米距离阈值
    //     Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);  // 橙色半透明
    //     Gizmos.DrawWireSphere(transform.position, roarDistanceThreshold);

    //     // ⭐ 绘制脚步声范围
    //     Gizmos.color = new Color(0f, 1f, 1f, 0.2f);  // 青色半透明
    //     Gizmos.DrawWireSphere(transform.position, maxFootstepDistance);

    //     // ⭐ 绘制最大音量范围
    //     Gizmos.color = new Color(1f, 0f, 1f, 0.3f);  // 紫色半透明
    //     Gizmos.DrawWireSphere(transform.position, minFootstepDistance);
    // }
}