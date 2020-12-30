using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SoundManager : DonDestroy<SoundManager>
{
    #region Field
    public enum eAudioType
    {
        BGM,
        SFX,
        Max
    }

    public enum eAudioSFXClip
    {
        GetCoin,
        GetGem,
        GetInvincible,
        GetItem,
        MonDie,
        NextScene,
        ButtonClick,
        Shield,
        MeteoExplosion,
        MeteoAlert,
        Result,
        PlayerDie,
        Max
    }

    public enum eAudioBGMClip
    {
        BGM01,
        BGM02,
        BGM03,
        Lobby,
        Max
    }

    [SerializeField]
    AudioClip[] m_sfxClip;
    [SerializeField]
    AudioClip[] m_bgmClip;

    //2개짜리 객체리스트를 생성
    AudioSource[] m_audio = new AudioSource[(int)eAudioType.Max];
    //동시재생에 제한을 두기 위한 딕셔너리
    Dictionary<float, float> m_limitDict = new Dictionary<float, float>();
    #endregion

    #region Unity Methods
    protected override void OnStart()
    {
        m_audio[(int)eAudioType.BGM] = gameObject.AddComponent<AudioSource>();
        m_audio[(int)eAudioType.BGM].loop = true; //반복시킴
        m_audio[(int)eAudioType.BGM].playOnAwake = false;
        m_audio[(int)eAudioType.BGM].rolloffMode = AudioRolloffMode.Linear;

        m_audio[(int)eAudioType.SFX] = gameObject.AddComponent<AudioSource>();
        m_audio[(int)eAudioType.SFX].loop = false;
        m_audio[(int)eAudioType.SFX].playOnAwake = false;
        m_audio[(int)eAudioType.SFX].rolloffMode = AudioRolloffMode.Linear;

        MuteBGM(PlayerPrefs.GetInt("OPTION_BGM", 1) == 1 ? false : true);
        MuteSFX(PlayerPrefs.GetInt("OPTION_SFX", 1) == 1 ? false : true);

        for(int i=0; i<m_sfxClip.Length; i++)
        {
            m_limitDict.Add(m_sfxClip[i].length, m_sfxClip[i].length);
        }
    }

    private void Update()
    {
        foreach (var kvp in m_limitDict.ToList())
        {
            if (kvp.Value != 0)
            {
                float newLen = kvp.Value - Time.deltaTime;
                if (newLen > 0f)
                {
                   m_limitDict[kvp.Key] = newLen;
                }
                else
                {
                    m_limitDict[kvp.Key] = 0f;
                }
            }
        }
    }
    #endregion

    #region Public Methods
    public void MuteBGM(bool isOn)
    {
        m_audio[(int)eAudioType.BGM].mute = isOn;
    }

    public void PauseBGM()
    {
        m_audio[(int)eAudioType.BGM].Pause();
    }

    public void PlayBGM()
    {
        if (m_audio[(int)eAudioType.BGM].clip == null) return;
        m_audio[(int)eAudioType.BGM].Play();
    }
    public void MuteSFX(bool isOn)
    {
        m_audio[(int)eAudioType.SFX].mute = isOn;
    }

    public void PlaySfx(eAudioSFXClip clip)
    {
        if(m_limitDict[m_sfxClip[(int)clip].length] == 0f)
        {
            m_limitDict[m_sfxClip[(int)clip].length] = m_sfxClip[(int)clip].length;
            m_audio[(int)eAudioType.SFX].PlayOneShot(m_sfxClip[(int)clip]);
        }
    }

    public void PlayBGM(eAudioBGMClip clip, float volume)
    {
        m_audio[(int)eAudioType.BGM].clip = m_bgmClip[(int)clip];
        m_audio[(int)eAudioType.BGM].volume = volume;
        m_audio[(int)eAudioType.BGM].Play();
    }
    #endregion
}
