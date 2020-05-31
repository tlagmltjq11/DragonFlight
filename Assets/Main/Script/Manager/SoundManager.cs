using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : DonDestroy<SoundManager>
{
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
        Max
    }

    public enum eAudioBGMClip
    {
        BGM01,
        Max
    }

    //동적으로 해도 되는데 시간없으니까
    [SerializeField]
    AudioClip[] m_sfxClip;
    [SerializeField]
    AudioClip[] m_bgmClip;

    //2개짜리 객체리스트를 생성
    AudioSource[] m_audio = new AudioSource[(int)eAudioType.Max];

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
        //m_audio[(int)eAudioType.SFX].clip = m_sfxClip[(int)clip];
        //m_audio[(int)eAudioType.SFX].Play();

        m_audio[(int)eAudioType.SFX].PlayOneShot(m_sfxClip[(int)clip]);
    }

    public void PlayBGM(eAudioBGMClip clip)
    {
        m_audio[(int)eAudioType.BGM].clip = m_bgmClip[(int)clip];
        m_audio[(int)eAudioType.BGM].Play();
    }

    protected override void OnStart()
    {
        //각 원소를 애드컴포넌트를 하면서 생성해줌.
        m_audio[(int)eAudioType.BGM] = gameObject.AddComponent<AudioSource>();
        m_audio[(int)eAudioType.BGM].loop = true; //반복시킴
        m_audio[(int)eAudioType.BGM].playOnAwake = false;
        //사운드가 줄어들때 어떤식으로 줄어들게 할것인가 -> 선형적으로 줄어들게함.
        m_audio[(int)eAudioType.BGM].rolloffMode = AudioRolloffMode.Linear;

        m_audio[(int)eAudioType.SFX] = gameObject.AddComponent<AudioSource>();
        m_audio[(int)eAudioType.SFX].loop = false;
        m_audio[(int)eAudioType.SFX].playOnAwake = false;
        m_audio[(int)eAudioType.SFX].rolloffMode = AudioRolloffMode.Linear;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
