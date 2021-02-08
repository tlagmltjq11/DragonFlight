# DragonFlight
프로젝트 설명은 아래 [링크](#1)를 통해 영상으로 확인할 수 있고, 코드와 같은 부가설명은 [About Dev](#2) 부분을 참고해주세요.<br>
<br>

### About Project.:two_men_holding_hands:
라인게임즈에서 개발한 드래곤플라이트를 모작한 프로젝트입니다.<br>
Mobile, PC 모두 플레이 가능합니다.<br>
<br>

### Video.:video_camera: <div id="1">이미지를 클릭해주세요.</div>
[![시연영상](https://img.youtube.com/vi/p3UIaGcnMSU/0.jpg)](https://www.youtube.com/watch?v=p3UIaGcnMSU&feature=youtu.be)
<br>
<br>

### About Dev.:nut_and_bolt: <div id="2"></div>
<br>

<details>
<summary>Popup 관련 Code 접기/펼치기</summary>
<div markdown="1">

<details>
<summary>&nbsp;&nbsp;&nbsp;&nbsp;PopupManager 접기/펼치기</summary>
<div markdown="1">
  
```c#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public delegate void PopupButtonDelegate();

public class PopupManager : DonDestroy<PopupManager>
{
    #region Field
    [SerializeField]
    GameObject m_popupOkCancelPrefab; //동적로드 방식을 사용할 것임.
    [SerializeField]
    GameObject m_popupOkPrefab;
    [SerializeField]
    GameObject m_popupOptionPrefab;

    int m_popupDepth = 1000;
    int m_depthGap = 10;

    List<GameObject> m_popupList = new List<GameObject>();
    #endregion

    #region Unity Methods
    protected override void OnStart()
    {
        m_popupOkCancelPrefab = Resources.Load("Popup/PopupOkCancel") as GameObject;
        m_popupOkPrefab = Resources.Load("Popup/PopupOk") as GameObject;
        m_popupOptionPrefab = Resources.Load("Popup/PopupOption") as GameObject;
        base.OnStart();
    }
    #endregion

    #region Public Methods
    public void OpenPopupOkCancel(string subject, string body, PopupButtonDelegate okBtnDel, PopupButtonDelegate cancelBtnDel, string okBtnStr = "OK", string cancelBtnStr = "Cancel")
    {
        var obj = Instantiate(m_popupOkCancelPrefab) as GameObject;
        
        //위에서부터 순서대로 찾아오기 때문에 가장먼저 온 패널이 가장 상위 패널임.
        var panels = obj.GetComponentsInChildren<UIPanel>();
        
        for(int i=0; i<panels.Length; i++)
        {
            //시작점을 현재 팝업의 갯수 * 갭을 해주며 + i 를 해주면서 내부 패널들의 뎁스를 1씩 늘려 맞춰준다.
            panels[i].depth = m_popupDepth + (m_popupList.Count * m_depthGap + i);
        }

        //모든 팝업들을 팝업매니저의 자식으로 만들어 관리할것임.
        obj.transform.SetParent(transform);

        //초기화
        obj.transform.localPosition = Vector3.zero;

        var popup = obj.GetComponent<PopupOkCancel>();

        //만들어진 팝업에 넘겨줌.
        popup.SetPopup(subject, body, okBtnDel, cancelBtnDel, okBtnStr, cancelBtnStr);

        m_popupList.Add(obj);
    }

    public void OpenPopupOk(string subject, string body, PopupButtonDelegate okBtnDel, string okBtnStr = "OK")
    {
        var obj = Instantiate(m_popupOkPrefab) as GameObject;

        //위에서부터 순서대로 찾아오기 때문에 가장먼저 온 패널이 가장 상위 패널임.
        var panels = obj.GetComponentsInChildren<UIPanel>();

        for (int i = 0; i < panels.Length; i++)
        {
            //시작점을 현재 팝업의 갯수 * 갭을 해주며 + i 를 해주면서 내부 패널들의 뎁스를 1씩 늘려 맞춰준다.
            panels[i].depth = m_popupDepth + (m_popupList.Count * m_depthGap + i);
        }

        //모든 팝업들을 팝업매니저의 자식으로 만들어 관리할것임.
        obj.transform.SetParent(transform);

        //초기화
        obj.transform.localPosition = Vector3.zero;

        var popup = obj.GetComponent<PopupOk>();

        //만들어진 팝업에 넘겨줌.
        popup.SetPopup(subject, body, okBtnDel, okBtnStr);

        m_popupList.Add(obj);
    }

    public void OpenPopupOption(PopupButtonDelegate okBtnDel)
    {
        var obj = Instantiate(m_popupOptionPrefab) as GameObject;

        //위에서부터 순서대로 찾아오기 때문에 가장먼저 온 패널이 가장 상위 패널임.
        var panels = obj.GetComponentsInChildren<UIPanel>();

        for (int i = 0; i < panels.Length; i++)
        {
            //시작점을 현재 팝업의 갯수 * 갭을 해주며 + i 를 해주면서 내부 패널들의 뎁스를 1씩 늘려 맞춰준다.
            panels[i].depth = m_popupDepth + (m_popupList.Count * m_depthGap + i);
        }

        //모든 팝업들을 팝업매니저의 자식으로 만들어 관리할것임.
        obj.transform.SetParent(transform);

        //초기화
        obj.transform.localPosition = Vector3.zero;

        var popup = obj.GetComponent<PopupOption>();
        popup.SetPopup(okBtnDel);
        m_popupList.Add(obj);
    }

    public void ClosePopup()
    {
        if(m_popupList.Count > 0)
        {
            Destroy(m_popupList[m_popupList.Count - 1].gameObject);
            //마지막에 생성된 팝업부터 없어질거라는 것을 아니까.
            m_popupList.Remove(m_popupList[m_popupList.Count-1]);
        }
    }

    public bool CanClosePopup(KeyCode key)
    {
        if(key == KeyCode.Escape)
        {
            if(m_popupList.Count > 0)
            {
                ClosePopup();
                return true;
            }
        }
        return false;
    }
    #endregion
}
```
  
</div>
</details>

<details>
<summary>&nbsp;&nbsp;&nbsp;&nbsp;PopupOption 접기/펼치기</summary>
<div markdown="1">

```c#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupOption : MonoBehaviour
{
    #region Field
    //모든 트윈은 아래로 받아오기 가능.
    [SerializeField]
    UITweener m_popupTween;
    PopupButtonDelegate m_okBtnDel;

    [SerializeField]
    UIToggle m_bgmToggle;
    [SerializeField]
    UIToggle m_sfxToggle;

    [SerializeField]
    UISprite m_speedIcon;

    int m_curSpeed;
    #endregion

    #region Unity Methods
    void Start()
    {
        var onBGM = PlayerPrefs.GetInt("OPTION_BGM", 1) == 1 ? false : true;
        var onSFX = PlayerPrefs.GetInt("OPTION_SFX", 1) == 1 ? false : true;
        m_curSpeed = PlayerPrefs.GetInt("OPTION_SPEED", 1);

        m_speedIcon.spriteName = string.Format("option_{0}x", m_curSpeed);

        m_bgmToggle.value = !onBGM;
        m_sfxToggle.value = !onSFX;
    }
    #endregion

    #region Public Methods
    //파라메터로 안주면 ok, cancel값으로 들어간다는 의미. 또한 생략할 수 있게끔 맨뒤에 위치시켜야 한다.
    public void SetPopup(PopupButtonDelegate okBtnDel)
    {
        m_popupTween.ResetToBeginning();
        m_popupTween.PlayForward();
        m_okBtnDel = okBtnDel;
    }

    public void SetBGM()
    {
        SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick);

        var isOn = m_bgmToggle.value;
        SoundManager.Instance.MuteBGM(!isOn);

        if(isOn)
        {
            SoundManager.Instance.PlayBGM();
        }

        PlayerPrefs.SetInt("OPTION_BGM", isOn == true ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetSFX()
    {
        var isOn = m_sfxToggle.value;
        SoundManager.Instance.MuteSFX(!isOn);

        if (isOn)
        {
            SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.GetItem);
        }

        PlayerPrefs.SetInt("OPTION_SFX", isOn == true ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetSpeed()
    {
        SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick);

        m_curSpeed++;
        if(m_curSpeed > 3)
        {
            m_curSpeed = 1;
        }

        m_speedIcon.spriteName = string.Format("option_{0}x", m_curSpeed);

        PlayerPrefs.SetInt("OPTION_SPEED", m_curSpeed);
        PlayerPrefs.Save();
    }

    public void OnPressOk()
    {
        m_okBtnDel();
        PopupManager.Instance.ClosePopup();
    }
    #endregion
}
```

</div>
</details>

<details>
<summary>&nbsp;&nbsp;&nbsp;&nbsp;PopupOkCancel 접기/펼치기</summary>
<div markdown="1">
  
```c#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupOkCancel : MonoBehaviour
{
    #region Field
    [SerializeField]
    UILabel m_subjectLabel;
    [SerializeField]
    UILabel m_bodyLabel;
    [SerializeField]
    UILabel m_okBtnLabel;
    [SerializeField]
    UILabel m_canceBtnlLabel;
    
    //모든 트윈은 아래로 받아오기 가능.
    [SerializeField]
    UITweener m_popupTween;

    PopupButtonDelegate m_okBtnDel;
    PopupButtonDelegate m_cancelBtnDel;
    #endregion

    #region Public Methods
    //파라메터로 안주면 ok, cancel값으로 들어간다는 의미. 또한 생략할 수 있게끔 맨뒤에 위치시켜야 한다.
    public void SetPopup(string subject, string body, PopupButtonDelegate  okBtnDel, PopupButtonDelegate cancelBtnDel, string okBtnText = "OK", string cancelBtnText = "Cancel")
    {
        m_popupTween.ResetToBeginning();
        m_popupTween.PlayForward();

        m_subjectLabel.text = subject;
        m_bodyLabel.text = body;
        m_okBtnLabel.text = okBtnText;
        m_canceBtnlLabel.text = cancelBtnText;
        m_okBtnDel = okBtnDel;
        m_cancelBtnDel = cancelBtnDel;
    }

    public void OnPressOk()
    {
        if(m_okBtnDel != null)
        {
            m_okBtnDel();
        }
        else
        {
            PopupManager.Instance.ClosePopup();
        }
    }

    public void OnPressCancel()
    {
        if(m_cancelBtnDel != null)
        {
            m_cancelBtnDel();
        }

        PopupManager.Instance.ClosePopup();
    }
    #endregion
}
```

</div>
</details>

<details>
<summary>&nbsp;&nbsp;&nbsp;&nbsp;PopupOk 접기/펼치기</summary>
<div markdown="1">
 
```c#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupOk : MonoBehaviour
{
    #region Field
    [SerializeField]
    UILabel m_subjectLabel;
    [SerializeField]
    UILabel m_bodyLabel;
    [SerializeField]
    UILabel m_okBtnLabel;

    //모든 트윈은 아래로 받아오기 가능.
    [SerializeField]
    UITweener m_popupTween;

    PopupButtonDelegate m_okBtnDel;
    #endregion

    #region Public Methods
    //파라메터로 안주면 ok, cancel값으로 들어간다는 의미. 또한 생략할 수 있게끔 맨뒤에 위치시켜야 한다.
    public void SetPopup(string subject, string body, PopupButtonDelegate okBtnDel, string okBtnText = "OK")
    {
        m_popupTween.ResetToBeginning();
        m_popupTween.PlayForward();

        m_subjectLabel.text = subject;
        m_bodyLabel.text = body;
        m_okBtnLabel.text = okBtnText;
        m_okBtnDel = okBtnDel;
    }

    public void OnPressOk()
    {

        m_okBtnDel();
        PopupManager.Instance.ClosePopup();
    }
    #endregion
}
```

</div>
</details>

```c#
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
```

</div>
</details>

### Difficult Point.:sweat_smile:
<br>
<br>
