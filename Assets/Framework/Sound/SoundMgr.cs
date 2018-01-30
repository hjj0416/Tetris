/*
 *音频管理工具
 *
 *方法：
 *          EnableMusic
 *          EnableSound
 *          PlaySound
 *          PlayMusic
 *          StartMusic
 *          StopMusic
 *          PauseMusic
 *          ResumeMusic
 *          IsPlaying
 *          SetVolume
 *          GetVolume
 * 
 */

using System.Collections.Generic;
using UnityEngine;

public class SoundMgr : MonoBehaviour 
{
    #region 单例

    private static SoundMgr mInstance;
    /// <summary>
    /// 获取资源加载实例
    /// </summary>
    /// <returns></returns>
    public static SoundMgr Instance
    {
        get
        {
            if (mInstance == null)
            {
                string objName = typeof(SoundMgr).Name;
                GameObject go = GameObject.Find(objName);
                if (go == null)
                {
                    go = new GameObject(objName);
                }
                go.transform.parent = GameObject.Find("Main").transform;
                go.transform.localScale = Vector3.one;
                mInstance = go.GetMissingComponent<SoundMgr>();
                go.AddComponent<AudioListener>();
                mInstance.m_audioSource = go.AddComponent<AudioSource>();
                mInstance.m_audioSource.playOnAwake = false;
                DontDestroyOnLoad(go);
            }
            return mInstance;
        }
    }

    #endregion

    public const string MusicSetting = "Music_Setting";
    public const string SoundSetting = "Sound_Setting";

    private bool m_enableMusic = true;
    public bool EnableMusic
    {
        get { return m_enableMusic; }
        set
        {
            m_enableMusic = value;
            PlayerPrefs.SetInt(MusicSetting, value?1:0);
            if(!value)
                StopMusic();
            else
                StartMusic();
        }
    }

    private bool m_enableSound = true;

    public bool EnableSound
    {
        get { return m_enableSound; }
        set
        {
            m_enableSound = value;
            PlayerPrefs.SetInt(SoundSetting, value?1:0);
        }
    }

    void Awake()
    {
        m_enableMusic = PlayerPrefs.GetInt(MusicSetting, 1) == 1;
        m_enableSound = PlayerPrefs.GetInt(SoundSetting, 1) == 1;
    }


    private AudioSource m_audioSource;
    
    private Dictionary<string, AudioClip>m_soundDic = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip>m_musicDic = new Dictionary<string, AudioClip>();

    /// <summary>
    /// 播放音效
    /// </summary>
    public void PlaySound(string soundName)
    {
        if (!EnableSound)
        {
            return;
        }
        if (!IsValid()) return;
        if (m_soundDic.ContainsKey(soundName))
        {
            m_audioSource.PlayOneShot(Instance.m_soundDic[soundName]);
        }
        else
        {
            var clip = Resources.Load<AudioClip>(soundName);
            if (clip == null)
            {
                Debug.LogError(soundName + " is null");
                return;
            }
            m_soundDic[soundName] = clip;
            m_audioSource.PlayOneShot(clip);
        }
    }

    public string m_curMusic = "";
    /// <summary>
    /// 播放背景音乐
    /// </summary>
    public void PlayMusic(string musicName, float delay = 0)
    {
        if (!IsValid()) return;
        if (IsPlaying() && m_curMusic.Equals(musicName)) return;
        m_audioSource.loop = true;
        if (delay < 0) delay = 0;
        if (m_musicDic.ContainsKey(musicName))
        {
            playMusic(musicName, null, delay);
        }
        else
        {
            var music = Resources.Load<AudioClip>(musicName);
            if (music == null)
            {
                Debug.LogError(musicName + " is null");
                return;
            }
            m_musicDic[musicName] = music;
            playMusic(musicName, music, delay);
        }
    }

    private void playMusic(string name, AudioClip clip, float delay = 0)
    {
        if (clip == null) clip = m_musicDic[name];
        m_audioSource.clip = clip;
        m_curMusic = name;
        if (EnableMusic)
            m_audioSource.PlayDelayed(delay);
    }

    public void StartMusic()
    {
        if (!EnableMusic) return;
        if (!IsValid()) return;
        m_audioSource.Play();
    }

    public void StopMusic()
    {
        if (!IsValid()) return;
        m_audioSource.Stop();
    }

    public void PauseMusic()
    {
        if (!IsValid()) return;
        m_audioSource.Pause();
    }

    public void ResumeMusic()
    {
        if (!EnableMusic) return;
        if (!IsValid()) return;
        m_audioSource.UnPause();
    }

    public bool IsPlaying()
    {
        if (!IsValid()) return false;
        return m_audioSource.isPlaying;
    }

    /// <summary>
    /// 设置声音大小
    /// </summary>
    /// <param name="value"></param>
    public void SetVolume(float value)
    {
        if (!IsValid()) return;
        m_audioSource.volume = Mathf.Clamp01(value);
    }

    public float GetVolume()
    {
        if (!IsValid()) return 0;
        return m_audioSource.volume;
    }


    private bool IsValid()
    {
        if (m_audioSource == null)
        {
            Debug.LogError("audio source is null");
            return false;
        }
        return true;
    }

}
