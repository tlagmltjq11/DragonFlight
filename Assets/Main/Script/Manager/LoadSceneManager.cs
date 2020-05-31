using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
public class LoadSceneManager : DonDestroy<LoadSceneManager>
{
    //DonDestroy를 상속받았기 때문에 싱글턴을 따로 구현하지 않아도 된다.
    /*
    //static 이므로 동일한 변수를 만들 수 없음. 딱 이것만 존재
    static LoadSceneManager m_instance;
    
    public static LoadSceneManager Instance
    {
        get
        {
            return m_instance;
        }
    }
    */

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
        m_loadState = state;
        ///m_loadingBgSpr.enabled = true;
        m_loadSceneState = SceneManager.LoadSceneAsync(state.ToString());
    }

    /*
    private void OnGUI()
    {
        GUI.Label(new Rect(Screen.width / 2, Screen.height / 2 + 50, 150, 50), m_progressLabel);
    }
    */

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
                                EditorApplication.isPlaying = false;
#else
                                Application.Quit();
#endif
                            }, null, "예", "아니오");
                            break;
                        case eSceneState.Lobby:
                            PopupManager.Instance.OpenPopupOkCancel("[0000FF]Notice[-]", "타이틀 화면으로 돌아가시겠습니까?", () => {
                                LoadSceneAsync(eSceneState.Title);
                                PopupManager.Instance.ClosePopup();
                            }, null, "예", "아니오");
                            break;
                        case eSceneState.Game:
                            PopupManager.Instance.OpenPopupOkCancel("[0000FF]Notice[-]", "로비 화면으로 돌아가시겠습니까?", () => {
                                LoadSceneAsync(eSceneState.Lobby);
                                PopupManager.Instance.ClosePopup();
                            }, null, "예", "아니오");
                            break;
                    }
                }
            }
        }
    }
}
