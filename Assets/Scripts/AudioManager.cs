using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource musicSource;      
    public AudioSource sfxSource;        // 用于一次性音效（怪物吼叫等）

    [Header("Audio Clips")]
    public AudioClip mainMenuMusic;      
    public AudioClip gameplayMusic;      
    public AudioClip playerFootstepSFX;  // ⭐ 玩家脚步声（循环播放）
    public AudioClip heartbeatSFX;       // 心跳音频

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float musicVolume = 0.7f;     
    [Range(0f, 1f)]
    public float sfxVolume = 1f;
    [Range(0f, 1f)]
    public float footstepVolume = 0.5f;  // ⭐ 脚步声音量

    [Header("Heartbeat Settings")]
    [Range(0f, 1f)]
    public float heartbeatVolume = 0.5f;
    public float minHeartbeatPitch = 0.8f;
    public float maxHeartbeatPitch = 2.0f;
    public float heartbeatStartSAN = 150f;

    public static AudioManager instance; 
    
    private AudioSource heartbeatAudioSource;    // 心跳专用
    private AudioSource footstepAudioSource;     // ⭐ 玩家脚步声专用
    private float currentSAN = 100f;          

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  
            Debug.Log("✅ AudioManager 创建成功");
        }
        else
        {
            Destroy(gameObject);
            Debug.Log("⚠️ 发现重复的 AudioManager，已销毁");
            return;
        }

        // 创建心跳专用的AudioSource
        heartbeatAudioSource = gameObject.AddComponent<AudioSource>();
        heartbeatAudioSource.loop = true;
        heartbeatAudioSource.playOnAwake = false;
        heartbeatAudioSource.volume = heartbeatVolume;

        // ⭐ 创建玩家脚步声专用的AudioSource
        footstepAudioSource = gameObject.AddComponent<AudioSource>();
        footstepAudioSource.loop = true;  // 循环播放
        footstepAudioSource.playOnAwake = false;
        footstepAudioSource.volume = footstepVolume;
    }

    void Start()
    {
        if (mainMenuMusic != null)
        {
            PlayMusic(mainMenuMusic);
            Debug.Log("🎵 开始播放主菜单音乐");
        }
        else
        {
            Debug.LogWarning("⚠️ 主菜单音乐未设置！");
        }
    }

    // 播放背景音乐
    public void PlayMusic(AudioClip clip)
    {
        if (musicSource != null && clip != null)
        {
            if (musicSource.clip == clip && musicSource.isPlaying)
            {
                return;
            }

            musicSource.clip = clip;
            musicSource.volume = musicVolume;
            musicSource.loop = true;
            musicSource.Play();
            Debug.Log("🎵 播放音乐: " + clip.name);
        }
        else
        {
            Debug.LogError("❌ MusicSource 或 AudioClip 为空！");
        }
    }

    // 播放游戏音乐
    public void PlayGameplayMusic()
    {
        if (gameplayMusic != null)
        {
            PlayMusic(gameplayMusic);
        }
        else
        {
            Debug.LogWarning("⚠️ 游戏音乐未设置！");
        }
    }

    // 播放主菜单音乐
    public void PlayMenuMusic()
    {
        if (mainMenuMusic != null)
        {
            PlayMusic(mainMenuMusic);
        }
    }

    // ⭐ 开始播放玩家脚步声
    public void StartPlayerFootsteps()
    {
        if (footstepAudioSource != null && playerFootstepSFX != null)
        {
            if (!footstepAudioSource.isPlaying)
            {
                footstepAudioSource.clip = playerFootstepSFX;
                footstepAudioSource.Play();
                Debug.Log("👣 玩家脚步声开始播放");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ 玩家脚步声音效未设置！");
        }
    }

    // ⭐ 停止玩家脚步声
    public void StopPlayerFootsteps()
    {
        if (footstepAudioSource != null && footstepAudioSource.isPlaying)
        {
            footstepAudioSource.Stop();
            Debug.Log("👣 玩家脚步声停止");
        }
    }

    // ⭐ 设置玩家脚步声音量
    public void SetFootstepVolume(float volume)
    {
        footstepVolume = Mathf.Clamp01(volume);
        if (footstepAudioSource != null)
        {
            footstepAudioSource.volume = footstepVolume;
        }
    }

    // 根据SAN值更新心跳
    public void UpdateHeartbeatBySAN(float sanValue)
    {
        currentSAN = sanValue;

        if (sanValue >= heartbeatStartSAN)
        {
            StopHeartbeat();
            return;
        }

        if (!heartbeatAudioSource.isPlaying)
        {
            StartHeartbeat();
        }

        float normalizedSAN = Mathf.Clamp01(sanValue / heartbeatStartSAN);
        float targetPitch = Mathf.Lerp(maxHeartbeatPitch, minHeartbeatPitch, normalizedSAN);
        heartbeatAudioSource.pitch = targetPitch;

        float targetVolume = Mathf.Lerp(heartbeatVolume * 1.2f, heartbeatVolume * 0.6f, normalizedSAN);
        heartbeatAudioSource.volume = targetVolume;
    }

    // 开始播放心跳
    void StartHeartbeat()
    {
        if (heartbeatSFX != null)
        {
            heartbeatAudioSource.clip = heartbeatSFX;
            heartbeatAudioSource.Play();
            Debug.Log("💓 心跳开始播放");
        }
        else
        {
            Debug.LogWarning("⚠️ 心跳音效未设置！");
        }
    }

    // 停止心跳
    public void StopHeartbeat()
    {
        if (heartbeatAudioSource.isPlaying)
        {
            heartbeatAudioSource.Stop();
            Debug.Log("💓 心跳停止");
        }
    }

    public void StopMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop();
            Debug.Log("🔇 音乐已停止");
        }
    }

    public void PauseMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Pause();
            Debug.Log("⏸️ 音乐已暂停");
        }
    }

    public void ResumeMusic()
    {
        if (musicSource != null && !musicSource.isPlaying)
        {
            musicSource.UnPause();
            Debug.Log("▶️ 音乐已恢复");
        }
    }

    // 播放一次性音效（怪物吼叫等）
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip, sfxVolume);
            Debug.Log("🔊 播放音效: " + clip.name);
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
        Debug.Log("🎚️ 音乐音量设置为: " + (musicVolume * 100) + "%");
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        
        // 同时更新心跳音量
        heartbeatVolume = sfxVolume;
        if (heartbeatAudioSource != null)
        {
            float normalizedSAN = Mathf.Clamp01(currentSAN / heartbeatStartSAN);
            float targetVolume = Mathf.Lerp(heartbeatVolume * 1.2f, heartbeatVolume * 0.6f, normalizedSAN);
            heartbeatAudioSource.volume = targetVolume;
        }

        // ⭐ 同时更新玩家脚步声音量
        footstepVolume = sfxVolume;
        if (footstepAudioSource != null)
        {
            footstepAudioSource.volume = footstepVolume;
        }
        
        Debug.Log("🎚️ 音效音量设置为: " + (sfxVolume * 100) + "%");
    }

    public float GetMusicVolume()
    {
        return musicVolume * 100f;
    }

    public float GetSFXVolume()
    {
        return sfxVolume * 100f;
    }

}