﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    #region Field
    public enum eGameState
    {
        Normal,
        Invincible,
        Result,
    }

    eGameState m_state;
    [SerializeField]
    PlayerController m_player;
    [SerializeField]
    BgController m_bgController;
    [SerializeField]
    GameObject m_gameUI;
    [SerializeField]
    GameResult m_resultUI;
    int m_gameSpeed;
    #endregion

    #region Unity Methods
    private void Start()
    {
        m_gameSpeed = PlayerPrefs.GetInt("OPTION_SPEED", 1);
        Time.timeScale = m_gameSpeed;
    }
    #endregion

    #region Public Methods
    public void SetPause()
    {
        if (Time.timeScale == 0)
        {
            SoundManager.Instance.PlayBGM();
            Time.timeScale = m_gameSpeed;
        }
        else
        {
            SoundManager.Instance.PauseBGM();
            Time.timeScale = 0;
        }
    }

    public eGameState GetState()
    {
        return m_state;
    }

    public void SetState(eGameState state)
    {

        if (m_state == state)
        {
            return;
        }

        m_state = state;

        switch(m_state)
        {
            case eGameState.Normal:
                m_player.EndInvincible();
                m_bgController.SetSpeedScale(1f);
                MonsterManager.Instance.SetSpawnInterval(1f);
                break;

            case eGameState.Invincible:
                m_player.SetInvincible();
                m_bgController.SetSpeedScale(4f);
                MonsterManager.Instance.SetSpawnInterval(4f);
                break;
            case eGameState.Result:
                m_player.SetDie();
                m_bgController.SetSpeedScale(1f);
                MonsterManager.Instance.StopCreateMonsters();
                MeteoManager.Instance.StopCreateMeteo();
                m_gameUI.SetActive(false);
                SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.Result);
                m_resultUI.SetUI();
                break;
        }
    }
    #endregion
}
