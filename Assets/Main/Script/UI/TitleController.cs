using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;

public class TitleController : MonoBehaviour
{
    #region Field
    [SerializeField]
    GameObject m_bgObj;
    [SerializeField]
    GameObject m_titleObj;
    [SerializeField]
    UILabel m_startInfo;
    [SerializeField]
    UIEventTrigger m_eventTrigger;
    [SerializeField]
    GameObject m_CI;
    GameObject m_popup;
    #endregion

    #region Public Methods
    public void OnBackClick()
    {
        SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick);

        PopupManager.Instance.OpenPopupOkCancel("[0000FF]Notice[-]", "게임을 종료하시겠습니까?", () => {
#if UNITY_EDITOR
            SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick);
            EditorApplication.isPlaying = false;
#else
                                SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick);
                                Application.Quit();
#endif
        }, () => { SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick); }, "예", "아니오");

    }

    public void GoNextScene()
    {
        if (m_popup.transform.childCount == 0)
        {
            LoadSceneManager.Instance.LoadSceneAsync(LoadSceneManager.eSceneState.Lobby);
        }
    }

    public void SetTitle()
    {
        m_bgObj.SetActive(true);
        m_titleObj.SetActive(true);
        m_CI.SetActive(false);
        LoadSceneManager.Instance.SetState(LoadSceneManager.eSceneState.Title);

        //추후 수정.
#if UNITY_ANDROID || UNITY_IOS
        m_startInfo.text = "Touch to start!";
        m_eventTrigger.enabled = true;
#elif UNITY_STANDALONE || UNITY_EDITOR
        m_startInfo.text = "Touch to start!";
        m_eventTrigger.enabled = true;
#endif
    }
    #endregion

    #region Unity Methods
    void Start()
    {
        m_bgObj.SetActive(false);
        m_titleObj.SetActive(false);

        //로비에서 타이틀화면으로 넘어올 시 CI를 재생하지않고 곧바로 SetTitle을 실행하도록 유도한다.
        if (LoadSceneManager.Instance.GetState() == LoadSceneManager.eSceneState.Lobby)
        {
            SetTitle();
        }

        m_popup = GameObject.Find("PopupManager");
    }
    #endregion
}
