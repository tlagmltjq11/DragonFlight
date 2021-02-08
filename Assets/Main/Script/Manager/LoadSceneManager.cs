using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
public class LoadSceneManager : DonDestroy<LoadSceneManager>
{
    #region Field
    public enum eSceneState
    {
        None = -1,
        CI,
        Title,
        Lobby,
        Game

    }

    eSceneState m_state = eSceneState.CI;
    eSceneState m_loadState = eSceneState.None;
    string m_progressLabel;
    AsyncOperation m_loadSceneState;
    SpriteRenderer m_loadingBgSpr;
    #endregion

    #region Public Methods
    public void SetState(eSceneState state)
    {
        m_state = state;
    }

    public eSceneState GetState()
    {
        return m_state;
    }

    public void LoadSceneAsync(eSceneState state)
    {
        //load를 하고 있다면
        if (m_loadState != eSceneState.None)
        {
            return;
        }

        SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.NextScene);

        m_loadState = state;
        ///m_loadingBgSpr.enabled = true;
        m_loadSceneState = SceneManager.LoadSceneAsync(state.ToString());
    }
    #endregion

    #region Unity Methods
    protected override void OnAwake()
    {

    }

    protected override void OnStart()
    {

    }

    void Update()
    {
        if(m_loadSceneState != null && m_loadState != eSceneState.None)
        {
            if(m_loadSceneState.isDone)
            {
                m_loadSceneState = null;

                m_state = m_loadState;
                m_loadState = eSceneState.None;
                m_progressLabel = "100";
            }
            else
            {
                m_progressLabel = ((int)(m_loadSceneState.progress * 100)).ToString();
            }
        }
        else
        {
            //씬이 로드중이 아니라면
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                //닫힐 팝업이 없다면
                if(!PopupManager.Instance.CanClosePopup(KeyCode.Escape))
                {
                    switch(m_state)
                    {
                        case eSceneState.Title:
                            PopupManager.Instance.OpenPopupOkCancel("[0000FF]Notice[-]", "게임을 종료하시겠습니까?", () => {
#if UNITY_EDITOR
                                SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick);
                                EditorApplication.isPlaying = false;
#else
                                SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick);
                                Application.Quit();
#endif
                            }, () => { SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick); }, "예", "아니오");
                            break;
                        case eSceneState.Lobby:
                            PopupManager.Instance.OpenPopupOkCancel("[0000FF]Notice[-]", "타이틀 화면으로 돌아가시겠습니까?", () => {
                                SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick);
                                LoadSceneAsync(eSceneState.Title);
                                PopupManager.Instance.ClosePopup();
                            }, () => { SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick); }, "예", "아니오");
                            break;
                        case eSceneState.Game:
                            PopupManager.Instance.OpenPopupOkCancel("[0000FF]Notice[-]", "로비 화면으로 돌아가시겠습니까?", () => {
                                SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick);
                                LoadSceneAsync(eSceneState.Lobby);
                                Time.timeScale = 1;
                                PopupManager.Instance.ClosePopup();
                            }, () => { SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick); }, "예", "아니오");
                            break;
                    }
                }
            }
        }
    }
    #endregion
}
