using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
public class LoadSceneManager : DonDestroy<LoadSceneManager>
{

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


    protected override void OnAwake()
    {

    }

    protected override void OnStart()
    {
        m_loadingBgSpr = GetComponentInChildren<SpriteRenderer>();
        //제일 먼저 찾게된 자식의 스프라이트 렌더러를 반환해준다.
        //m_loadingBgSpr = GetComponentInChildren<SpriteRenderer>();
        //m_loadingBgSpr.enabled = false;
    }
    /* awake, start등이 상속받아서 이미 존재하기에 지워준다.
    private void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
        }
        else
        {
            //로드신매니저를 처음으로 인스턴스한것이 아니기에 지워버린다.
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        //제일 먼저 찾게된 자식의 스프라이트 렌더러를 반환해준다.
        m_loadingBgSpr = GetComponentInChildren<SpriteRenderer>();
        m_loadingBgSpr.enabled = false;
    }
    */

    // Update is called once per frame
    void Update()
    {
        if(m_loadSceneState != null && m_loadState != eSceneState.None)
        {
            if(m_loadSceneState.isDone)
            {
                //m_loadingBgSpr.enabled = false;
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
                                PopupManager.Instance.ClosePopup();
                            }, () => { SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick); }, "예", "아니오");
                            break;
                    }
                }
            }
        }
    }
}
