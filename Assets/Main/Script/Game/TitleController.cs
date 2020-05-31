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
    GameObject m_ciBg;
    [SerializeField]
    UILabel m_startInfo;
    [SerializeField]
    UIEventTrigger m_eventTrigger;

    public void GoNextScene()
    {
        LoadSceneManager.Instance.LoadSceneAsync(LoadSceneManager.eSceneState.Lobby);
    }

    public void SetTitle()
    {
        m_bgObj.SetActive(true);
        m_titleObj.SetActive(true);
        m_ciBg.SetActive(false);
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

        if (PlayerPrefs.HasKey("OPTION_BGM"))
        {
            if (PlayerPrefs.GetInt("OPTION_BGM") == 1)
            {
                SoundManager.Instance.MuteBGM(false);
                SoundManager.Instance.PlayBGM();
            }
            else if (PlayerPrefs.GetInt("OPTION_BGM") == 0)
            {
                SoundManager.Instance.MuteBGM(true);
            }
        }
        else
        {
            SoundManager.Instance.MuteBGM(false);
            SoundManager.Instance.PlayBGM();
        }

        if (PlayerPrefs.HasKey("OPTION_SFX"))
        {
            if (PlayerPrefs.GetInt("OPTION_SFX") == 1)
            {
                SoundManager.Instance.MuteSFX(false);
                //SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.GetItem);
            }
            else if (PlayerPrefs.GetInt("OPTION_SFX") == 0)
            {
                SoundManager.Instance.MuteSFX(true);
            }
        }
        else
        {
            SoundManager.Instance.MuteSFX(false);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_STANDALONE
        if(Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Escape) && LoadSceneManager.Instance.GetState() == LoadSceneManager.eSceneState.Title)
        {
            if(GameObject.Find("PopupManager").transform.childCount == 0)
            {
                GoNextScene();
            }
        }
#endif
    }
}
