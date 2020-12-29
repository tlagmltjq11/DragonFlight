using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleController : MonoBehaviour
{
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

    public void GoNextScene()
    {
        LoadSceneManager.Instance.LoadSceneAsync(LoadSceneManager.eSceneState.Lobby);
    }

    public void SetTitle()
    {
        m_bgObj.SetActive(true);
        m_titleObj.SetActive(true);
        m_CI.SetActive(false);
        LoadSceneManager.Instance.SetState(LoadSceneManager.eSceneState.Title);

#if UNITY_ASDROID || UNITY_IOS
        m_startInfo.text = "Touch to start!";
        m_eventTrigger.enabled = true;
#elif UNITY_STANDALONE
        m_startInfo.text = "Press any key to start!";
        m_eventTrigger.enabled = false;
#endif
    }


    // Start is called before the first frame update
    void Start()
    {
        m_bgObj.SetActive(false);
        m_titleObj.SetActive(false);

        //로비에서 타이틀화면으로 넘어올 시 CI를 재생하지않고 곧바로 SetTitle을 실행하도록 유도한다.
        if (LoadSceneManager.Instance.GetState() == LoadSceneManager.eSceneState.Lobby)
        {
            SetTitle();
        }
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_STANDALONE
        if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Escape) && LoadSceneManager.Instance.GetState() == LoadSceneManager.eSceneState.Title)
        {
            if(GameObject.Find("PopupManager").transform.childCount == 0)
            {
                GoNextScene();
            }
        }
#endif
    }
}
