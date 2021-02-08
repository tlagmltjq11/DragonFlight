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

<br>

<details>
<summary>&nbsp;&nbsp;&nbsp;&nbsp;PopupManager 접기/펼치기</summary>
<div markdown="1">
  
```c#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public delegate void PopupButtonDelegate(); //델리게이트

public class PopupManager : DonDestroy<PopupManager> //DonDestroyOnLoad 적용
{
    #region Field
    [SerializeField]
    GameObject m_popupOkCancelPrefab; //'예', '아니오' 가 존재하는 팝업 프리팹
    [SerializeField]
    GameObject m_popupOkPrefab; //'예' 가 존재하는 팝업 프리팹
    [SerializeField]
    GameObject m_popupOptionPrefab; //옵션 팝업 프리팹

    int m_popupDepth = 1000; //팝업 depth 시작점
    int m_depthGap = 10; //팝업간 depth 간격

    List<GameObject> m_popupList = new List<GameObject>(); //활성화된 팝업들 관리
    #endregion

    #region Unity Methods
    protected override void OnStart()
    {
        //동적로드
        m_popupOkCancelPrefab = Resources.Load("Popup/PopupOkCancel") as GameObject;
        m_popupOkPrefab = Resources.Load("Popup/PopupOk") as GameObject;
        m_popupOptionPrefab = Resources.Load("Popup/PopupOption") as GameObject;
        base.OnStart();
    }
    #endregion

    #region Public Methods
    //'예', '아니오'가 존재하는 팝업을 생성해주는 메소드
    public void OpenPopupOkCancel(string subject, string body, PopupButtonDelegate okBtnDel, PopupButtonDelegate cancelBtnDel, string okBtnStr = "OK", string cancelBtnStr = "Cancel")
    {
        //팝업생성
        var obj = Instantiate(m_popupOkCancelPrefab) as GameObject;
        
        //위에서부터 순서대로 찾아오기 때문에 가장먼저 온 패널이 가장 상위 패널이다.
        var panels = obj.GetComponentsInChildren<UIPanel>();
        
        for(int i=0; i<panels.Length; i++)
        {
            //시작점에 (팝업의 갯수 * 갭 + i) 를 더해주면서 내부 패널들의 뎁스를 맞춰준다.
            panels[i].depth = m_popupDepth + (m_popupList.Count * m_depthGap + i);
        }

        obj.transform.SetParent(transform); //모든 팝업들을 팝업매니저의 자식으로 관리.
        obj.transform.localPosition = Vector3.zero; //초기화
        var popup = obj.GetComponent<PopupOkCancel>();

        //생성된 팝업에 파라메터를 넘겨준다. -> 제목, 내용, ok메소드, cancel메소드, ok버튼네임, cancel버튼네임
        popup.SetPopup(subject, body, okBtnDel, cancelBtnDel, okBtnStr, cancelBtnStr);

        //팝업리스트에 추가
        m_popupList.Add(obj);
    }

    //'예' 가 존재하는 팝업을 생성해주는 메소드
    public void OpenPopupOk(string subject, string body, PopupButtonDelegate okBtnDel, string okBtnStr = "OK")
    {
        var obj = Instantiate(m_popupOkPrefab) as GameObject; //팝업생성

        var panels = obj.GetComponentsInChildren<UIPanel>(); //패널들을 가져옴.

        for (int i = 0; i < panels.Length; i++)
        {
            //시작점에 (팝업의 갯수 * 갭 + i) 를 더해주면서 내부 패널들의 뎁스를 맞춰준다.
            panels[i].depth = m_popupDepth + (m_popupList.Count * m_depthGap + i);
        }

        //모든 팝업들을 팝업매니저의 자식으로 관리.
        obj.transform.SetParent(transform);
        //초기화
        obj.transform.localPosition = Vector3.zero;
        var popup = obj.GetComponent<PopupOk>();

        //생성된 팝업에 파라메터를 넘겨준다. -> 제목, 내용, ok메소드, ok버튼네임
        popup.SetPopup(subject, body, okBtnDel, okBtnStr);

        m_popupList.Add(obj); //팝업리스트에 추가
    }

    //옵션 팝업을 생성해주는 메소드
    public void OpenPopupOption(PopupButtonDelegate okBtnDel)
    {
        var obj = Instantiate(m_popupOptionPrefab) as GameObject; //팝업 생성
        var panels = obj.GetComponentsInChildren<UIPanel>(); //패널들을 가져옴

        for (int i = 0; i < panels.Length; i++)
        {    
            //시작점에 (팝업의 갯수 * 갭 + i) 를 더해주면서 내부 패널들의 뎁스를 맞춰준다.
            panels[i].depth = m_popupDepth + (m_popupList.Count * m_depthGap + i);
        }

        //모든 팝업들을 팝업매니저의 자식으로 관리.
        obj.transform.SetParent(transform);
        //초기화
        obj.transform.localPosition = Vector3.zero;
        var popup = obj.GetComponent<PopupOption>();
        
        //생성된 팝업에 파라메터를 넘겨준다. -> ok메소드
        popup.SetPopup(okBtnDel);
        
        m_popupList.Add(obj); //팝업리스트에 추가
    }

    //팝업창 닫기
    public void ClosePopup()
    {
        if(m_popupList.Count > 0)
        {
            //가장 맨위에 올라와있는 팝업부터 닫을 수 밖에 없다.
            Destroy(m_popupList[m_popupList.Count - 1].gameObject);
            m_popupList.Remove(m_popupList[m_popupList.Count-1]);
        }
    }

    //팝업창을 닫을 수 있는지
    public bool CanClosePopup(KeyCode key)
    {
        if(key == KeyCode.Escape) //ESC 키가 눌린 상태라면
        {
            if(m_popupList.Count > 0)
            {
                ClosePopup(); //팝업창 닫기
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
    UILabel m_subjectLabel; //제목
    [SerializeField]
    UILabel m_bodyLabel; //내용
    [SerializeField]
    UILabel m_okBtnLabel; //ok버튼
    [SerializeField]
    UILabel m_canceBtnlLabel; //cancel버튼
    
    [SerializeField]
    UITweener m_popupTween; //트윈 스케일

    PopupButtonDelegate m_okBtnDel; //ok 버튼 델리게이트
    PopupButtonDelegate m_cancelBtnDel; //cancel 버튼 델리게이트
    #endregion

    #region Public Methods
    //팝업 초기화 및 트윈 재생
    public void SetPopup(string subject, string body, PopupButtonDelegate  okBtnDel, PopupButtonDelegate cancelBtnDel, string okBtnText = "OK", string cancelBtnText = "Cancel")
    {
        m_popupTween.ResetToBeginning();
        m_popupTween.PlayForward(); //트윈 재생

        //text 초기화
        m_subjectLabel.text = subject;
        m_bodyLabel.text = body;
        m_okBtnLabel.text = okBtnText;
        m_canceBtnlLabel.text = cancelBtnText;
        
        //델리게이트 대입
        m_okBtnDel = okBtnDel;
        m_cancelBtnDel = cancelBtnDel;
    }

    //ok버튼이 눌렸을 경우
    public void OnPressOk()
    {
        if(m_okBtnDel != null)
        {
            m_okBtnDel(); //ok 델리게이트에 대입된 메소드 실행
        }
        else
        {
            PopupManager.Instance.ClosePopup();
        }
    }

    //cancel버튼이 눌렸을 경우
    public void OnPressCancel()
    {
        if(m_cancelBtnDel != null)
        {
            m_cancelBtnDel(); //cancel 델리게이트에 대입된 메소드 실행
        }

        PopupManager.Instance.ClosePopup(); //팝업창 
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
    UILabel m_subjectLabel;  //제목
    [SerializeField]
    UILabel m_bodyLabel; //내용
    [SerializeField]
    UILabel m_okBtnLabel; //ok버튼

    [SerializeField]
    UITweener m_popupTween; //트윈 스케일

    PopupButtonDelegate m_okBtnDel; //ok버튼 델리게이트
    #endregion

    #region Public Methods
    //팝업 초기화 및 트윈 재생
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
    //모든 트윈은 아래로 받아옴
    [SerializeField]
    UITweener m_popupTween;
    PopupButtonDelegate m_okBtnDel; //ok 델리게이트

    [SerializeField]
    UIToggle m_bgmToggle; //bgm
    [SerializeField]
    UIToggle m_sfxToggle; //효과음
    [SerializeField]
    UISprite m_speedIcon; //조작속도

    int m_curSpeed; //현재 조작속도
    #endregion

    #region Unity Methods
    void Start()
    {
        var onBGM = PlayerPrefs.GetInt("OPTION_BGM", 1) == 1 ? false : true;
        var onSFX = PlayerPrefs.GetInt("OPTION_SFX", 1) == 1 ? false : true;
        m_curSpeed = PlayerPrefs.GetInt("OPTION_SPEED", 1);

        m_speedIcon.spriteName = string.Format("option_{0}x", m_curSpeed); //Atlas에서 현재 조작속도에 맞는 이미지로 변경

        //현재 bgm과 효과음의 on/off 여부에 따라 토글을 통해 이미지를 변경해준다.
        m_bgmToggle.value = !onBGM;
        m_sfxToggle.value = !onSFX;
    }
    #endregion

    #region Public Methods
    //팝업 초기화 및 트윈 재생
    public void SetPopup(PopupButtonDelegate okBtnDel)
    {
        m_popupTween.ResetToBeginning();
        m_popupTween.PlayForward(); //트윈 재생
        m_okBtnDel = okBtnDel; //델리게이트에 대입
    }

    //bgm 옵션
    public void SetBGM()
    {
        SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick); //버튼클릭음 재생

        var isOn = m_bgmToggle.value;
        SoundManager.Instance.MuteBGM(!isOn); //토글의 상태값에 따라 BGM on/off

        if(isOn)
        {
            SoundManager.Instance.PlayBGM(); //On상태라면 다시 bgm재생
        }

        PlayerPrefs.SetInt("OPTION_BGM", isOn == true ? 1 : 0); //PlayerPrefs에 저장
        PlayerPrefs.Save();
    }

    //효과음 옵션
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

    //조작속도 옵션
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

    //ok버튼이 눌렸을 경우
    public void OnPressOk()
    {
        m_okBtnDel();
        PopupManager.Instance.ClosePopup(); //
    }
    #endregion
}
```

</div>
</details>

<br>

**Explanation**:mortar_board:<br>
(구현설명은 주석으로 간단하게 처리했습니다!)<br>
어느 상황에서든지 팝업창을 사용할 수 있게 DonDestoryOnLoad를 적용한 PopupManager를 구성했습니다. 사용 용도에 따라 선택용 팝업(PopupOkCancel), 확인용 팝업(PopupOk), 옵션용 팝업(PopupOption)으로 구분 지었으며, 각 버튼 클릭 시 처리해야 하는 부분들은 Delegate를 이용해서 팝업 생성과 동시에 넘겨주도록 구현했습니다. 그리하여, 동일한 팝업 Prefab으로 여러 상황을 대처할 수 있게 되었습니다.

<br>


해당 팝업들은 아래와 같이 사용되었습니다.
```c#
//LoadSceneManager 간략화
public class LoadSceneManager : DonDestroy<LoadSceneManager>
{
    AsyncOperation m_loadSceneState;
    
    #region Unity Methods
    void Update()
    {
        if(m_loadSceneState != null && m_loadState != eSceneState.None)
        {
            if(m_loadSceneState.isDone) //씬 로드가 완료된 경우
            {
                m_loadSceneState = null;
                m_state = m_loadState; //현재 씬 상태를 로드된 씬으로 초기화
                m_loadState = eSceneState.None; //로드할 씬 상태 none으로 초기화
                m_progressLabel = "100"; 
            }
            else
            {
                m_progressLabel = ((int)(m_loadSceneState.progress * 100)).ToString(); //로딩이 진행 중임을 보여줌.
            }
        }
        else //씬 로드중이 아니라면
        {
            if(Input.GetKeyDown(KeyCode.Escape)) //esc키를 눌렀을 경우
            {
                //닫힐 팝업이 없는경우 -> 닫힐 팝업이 있었다면, esc키를 통해 해당 팝업이 닫혔을 것.
                if(!PopupManager.Instance.CanClosePopup(KeyCode.Escape))
                {
                    switch(m_state) //현재 씬 상태에 따라 case
                    {
                        case eSceneState.Title: //타이틀인 경우 게임종료 팝업을 띄워준다.
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
                            
                        case eSceneState.Lobby: //로비인 경우 타이틀로 돌아가는 팝업을 띄워준다.
                            PopupManager.Instance.OpenPopupOkCancel("[0000FF]Notice[-]", "타이틀 화면으로 돌아가시겠습니까?", () => {
                                SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick);
                                LoadSceneAsync(eSceneState.Title); //비동기로딩
                                PopupManager.Instance.ClosePopup();
                            }, () => { SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick); }, "예", "아니오");
                            break;
                            
                        case eSceneState.Game: //게임중일 경우 로비로 돌아가는 팝업을 띄워준다.
                            PopupManager.Instance.OpenPopupOkCancel("[0000FF]Notice[-]", "로비 화면으로 돌아가시겠습니까?", () => {
                                SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick);
                                LoadSceneAsync(eSceneState.Lobby); //비동기로딩
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

<br>

<details>
<summary>Managers Code 접기/펼치기</summary>
<div markdown="1">

<details>
<summary>&nbsp;&nbsp;&nbsp;&nbsp;PopupOption 접기/펼치기</summary>
<div markdown="1">
</div>
</details>

</div>
</details>

### Difficult Point.:sweat_smile:
<br>
<br>
