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
<summary>Menu 관련 Code 접기/펼치기</summary>
<div markdown="1">
  
<br>
  
<details>
<summary>&nbsp;&nbsp;&nbsp;&nbsp;ILobbyMenu 접기/펼치기</summary>
<div markdown="1">
 
```c#
using UnityEngine;

public enum eLobbyMenuType //메뉴 타입
{
    Character, //캐릭터변경
    Inventory, //인벤토리
    Shop //상점
}

public interface ILobbyMenu //메뉴들이 상속받을 인터페이스
{
    //인터페이스로 메뉴들을 불러오면 메뉴고유의 메소드를 호출 할 수 없으므로 인터페이스에 프로퍼티 추가
    eLobbyMenuType m_type { get; } //타입
    GameObject gObj { get; } //게임오브젝트 반환
    void SetUI(); //메뉴활성화
    void CloseUI(); //메뉴비활성화
}
```
  
</div>
</details>

<details>
<summary>&nbsp;&nbsp;&nbsp;&nbsp;LobbyMenu_Character 접기/펼치기</summary>
<div markdown="1">
  
<br>
인벤토리와 상점 스크립트는 이와 유사하므로 생략합니다. 

```c#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyMenu_Character : MonoBehaviour, ILobbyMenu
{
    #region Field
    [SerializeField]
    UI2DSprite m_characterSpr; //캐릭터 이미지
    [SerializeField]
    Vector3[] m_charSprPos; //캐릭터별 이미지 위치
    [SerializeField]
    UISprite m_darkAreaSpr; //alpha
    [SerializeField]
    UIButton[] m_buttons;

    [SerializeField]
    LobbyController m_lobby; //로비컨트롤러

    [SerializeField]
    TweenPosition m_charSprTween; //트윈 포지션

    [SerializeField]
    string[] m_classNameList; //클래스명 리스트
    [SerializeField]
    string[] m_charNameList; //캐릭터명 리스트
    [SerializeField]
    UILabel m_className; //클래스명
    [SerializeField]
    UILabel m_charName; //캐릭터명

    [SerializeField]
    UISprite m_charIconSpr; //캐릭터 아이콘 이미지
    [SerializeField]
    UILabel m_gemOwned; //보유한 보석 라벨
    
    int m_selectIndex = 0; //현재 화면에 띄워진 캐릭터 인덱스
    #endregion

    #region Unity Methods
    private void Awake()
    {
        //PlayerPrefs에 저장된 현재선택된 캐릭터의 인덱스값을 받아와 초기화
        LoadCharacterSprite(PlayerDataManager.Instance.GetCurHero() - 1);
    }
    #endregion

    #region Public Methods
    public eLobbyMenuType m_type { get { return eLobbyMenuType.Character; } } //메뉴타입 반환

    public GameObject gObj { get { return gameObject; } } //게임오브젝트 반환

    //메뉴 활성화
    public void SetUI()
    {
        gameObject.SetActive(true); //오브젝트 활성화
        LoadCharacterSprite(m_selectIndex); //selectIndex값대로 캐릭터를 화면에 띄워준다.
    }

    //메뉴 비활성화
    public void CloseUI()
    {
        m_darkAreaSpr.depth = 0; //Alpha Screen 뎁스 초기화
        gameObject.SetActive(false); //오브젝트 비활성화
    }

    //캐릭터별로 정해진 위치좌표 반환
    public Vector3 GetCharSprPosition(int index)
    {
        return m_charSprPos[index]; //인덱스에 맞는 캐릭터의 위치좌표 반환
    }

    //캐릭터별 이미지 반환
    public Sprite GetCharSprite()
    {
        return m_characterSpr.sprite2D; //현재 선택되어있는 캐릭터의 이미지를 반환
    }

    //캐릭터 선택 버튼 클릭 시 호출되는 메소드
    public void OnSelect()
    {
        SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick); //버튼클릭음 재생
        PlayerDataManager.Instance.SetCurHero(m_selectIndex); //PlayerPrefs에 현재 선택된 캐릭터의 인덱스값 저장
        PlayerDataManager.Instance.SaveData(); //save
        m_lobby.gameObject.SetActive(true); //메인로비 오브젝트 활성화
        gameObject.SetActive(false); //캐릭터메뉴 비활성화
    }

    //캐릭터 구매 버튼 클릭 시 호출되는 메소드
    public void OnBuyCharacter()
    {
        SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick); //버튼클릭음 재생
        
        //구매 선택을 위한 팝업 생성
        PopupManager.Instance.OpenPopupOkCancel("Notice", string.Format("[00FF00]35레벨로 성장[-]되어 있는 [00FF00]{0}[-]를 수정 40개로\r\n구입하시겠습니까?", m_charName.text), ()=> 
        {
            SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick);

            if (PlayerDataManager.Instance.DecreaseGem(40)) //보유한 보석에서 40을 차감
            {
                PopupManager.Instance.ClosePopup(); //팝업 닫기
                PlayerDataManager.Instance.BuyCharacter(m_selectIndex); //구매한 캐릭터를 보유캐릭터 리스트에 저장

                m_gemOwned.text = "보유     : [00FF00]" + PlayerDataManager.Instance.GetGem() + "[-]"; //보유한 보석 표시

                RefreshInfo(m_selectIndex); //구매했으므로 Alpha Screen을 제거해주고 선택가능하게 만들어준다.
            }
            else //보유한 보석이 부족한 경우
            {
                //보석이 부족하다는 내용 확인용 팝업 생성
                PopupManager.Instance.OpenPopupOk("Notice", "소지한 수정이 부족합니다.", () => { SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick); });
            }
        }, () => { SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick); });
    }

    //뒤로가기 버튼 클릭 시
    public void OnPressBack()
    {
        SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick); //버튼클릭음 재생
        LoadCharacterSprite(PlayerDataManager.Instance.GetCurHero() - 1); //현재 선택된 캐릭터로 이미지 변경
        m_lobby.gameObject.SetActive(true); //메인로비 오브젝트 활성화
        CloseUI(); //캐릭터메뉴 비활성화
    }

    //캐릭터 목록의 왼쪽으로 넘길 경우
    public void OnPressLeft()
    {
        SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick); //버튼음 재생
        m_selectIndex--; //인덱스 --


        if(m_selectIndex < 0) //-1이 되면 12로 변경시켜 순환유도
        {
            m_selectIndex = 12;
        }

        LoadCharacterSprite(m_selectIndex); //인덱스에 맞는 캐릭터이미지를 띄워준다.
    }

    //캐릭터 목록의 오른쪽으로 넘길 경우
    public void OnPressRight()
    {
        SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick);
        m_selectIndex++; //인덱스 ++

        if(m_selectIndex > 12)
        {
            m_selectIndex = 0;
        }

        LoadCharacterSprite(m_selectIndex); //인덱스에 맞는 캐릭터이미지를 띄워준다.
    }
    #endregion

    #region Private Methods
    //인덱스에 맞는 캐릭터이미지로 변경
    void LoadCharacterSprite(int index)
    {
        var spr = Resources.Load<Sprite>(string.Format("Character/character_{0:00}", index + 1)); //동적로드로 이미지를 가져온다.
        m_characterSpr.sprite2D = spr; //해당 캐릭터이미지로 변경

        //이미지 크기를 원래 크기로 맞춰주는것. 즉 snap
        m_characterSpr.MakePixelPerfect();
        m_characterSpr.transform.localPosition = m_charSprPos[index]; //캐릭터별 위치좌표로 초기화

        //이름 및 아이콘 초기화
        m_className.text = m_classNameList[index];
        m_charName.text = m_charNameList[index];
        m_charIconSpr.spriteName = string.Format("select_character_{0:00}", index + 1);

        //트윈 포지션 셋팅 후 재생
        m_charSprTween.from = m_characterSpr.transform.localPosition;
        m_charSprTween.to = m_charSprTween.from + Vector3.down * 20;
        m_charSprTween.ResetToBeginning();
        m_charSprTween.PlayForward();

        //보유중인 보석 초기화
        m_gemOwned.text = "보유     : [00FF00]" + PlayerDataManager.Instance.GetGem() + "[-]";
        
        RefreshInfo(index);
    }

    //인덱스에 맞는 캐릭터의 소유 여부에 따라서, Alpha Screen과 선택, 구매버튼의 유무를 결정해준다.
    void RefreshInfo(int index)
    {
        if (PlayerDataManager.Instance.IsOwnedCharacter(index)) //보유한 캐릭터일 경우
        {
            m_darkAreaSpr.depth = 0; //알파스크린을 맨앞으로 빼서 어둡게 가리지 않도록 설정
            m_buttons[0].gameObject.SetActive(true); //선택버튼 활성화
            m_buttons[1].gameObject.SetActive(false); //구매버튼 비활성화
        }
        else
        {
            m_darkAreaSpr.depth = 2;
            m_buttons[0].gameObject.SetActive(false);
            m_buttons[1].gameObject.SetActive(true);
        }
    }
    #endregion
}
```
  
</div>
</details>

<details>
<summary>&nbsp;&nbsp;&nbsp;&nbsp;Util 접기/펼치기</summary>
<div markdown="1">
  
```c#
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util : MonoBehaviour
{
    #region Public Methods
    //파라메터 생성 메소드
    public static EventDelegate.Parameter MakeParameter(UnityEngine.Object _value, System.Type _type)
    {
        EventDelegate.Parameter param = new EventDelegate.Parameter(); // 이벤트 parameter 생성.  
        param.obj = _value; // 이벤트 함수에 전달하고 싶은 값.     
        param.expectedType = _type; // 값의 타입.
        
        return param; //반환
    }
    #endregion
    
    ...
    
}
```
  
</div>
</details>

<details>
<summary>&nbsp;&nbsp;&nbsp;&nbsp;LobbyController 접기/펼치기</summary>
<div markdown="1">
  
```c#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyController : MonoBehaviour
{
    #region Field
    [SerializeField]
    GameObject m_menuBtnObj; //메뉴 버튼 최상위 오브젝트
    [SerializeField]
    GameObject m_menuObj; //메뉴 최상위 오브젝트
    [SerializeField]
    UI2DSprite m_charSpr; //캐릭터 이미지
    [SerializeField]
    UISprite m_charIconSpr; //캐릭터 아이콘

    ILobbyMenu[] m_menu; //메뉴 배열
    UIButton[] m_menuBtns; //메뉴 버튼 배열
    TweenPosition m_charTween; //트윈 포지션
    #endregion

    #region Unity Methods
    void Awake()
    {
        m_charTween = m_charSpr.GetComponent<TweenPosition>();
        m_menuBtns = m_menuBtnObj.GetComponentsInChildren<UIButton>();
        var results = m_menuObj.GetComponentsInChildren<ILobbyMenu>();

        m_menu = new ILobbyMenu[results.Length]; //메뉴의 갯수만큼 배열 초기화

        for (int i = 0; i < m_menu.Length; i++)
        {
            m_menu[i] = results[i]; //메뉴 인터페이스 배열에 각 메뉴를 삽입 
            m_menu[i].CloseUI(); //메뉴를 닫아준다.
        }

        //메뉴버튼들 동적으로 이벤트 지정
        for (int i = 0; i < m_menuBtns.Length; i++)
        {
            EventDelegate del = new EventDelegate(this, "OpenMenu"); //현재 스크립트에 존재하는 "OpenMenu" 함수를 콜백함수로 지정
            del.parameters[0] = Util.MakeParameter(m_menuBtns[i], typeof(UIButton)); //버튼 자기자신을 파라메터로 넘겨줄 수 있도록 지정
            m_menuBtns[i].onClick.Add(del); //버튼에 이벤트 할당
        }
    }

    private void OnEnable()
    {
        GetCurCharInfo(); //현재 선택된 캐릭터로 변경
    }
    #endregion

    #region Public Methods
    public void OnBackClick() //뒤로가기 클릭 시
    {
        SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick); //버튼음 재생

        //타이틀로 돌아갈지 선택하기 위한 팝업 생성
        PopupManager.Instance.OpenPopupOkCancel("[0000FF]Notice[-]", "타이틀 화면으로 돌아가시겠습니까?", () => {
            SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick);
            LoadSceneManager.Instance.LoadSceneAsync(LoadSceneManager.eSceneState.Title); //타이틀 씬으로 비동기로딩 요청
            PopupManager.Instance.ClosePopup(); //팝업 닫기
        }, () => { SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick); }, "예", "아니오");
    }

    //게임시작 버튼 클릭 시
    public void StartGame()
    {
        SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick);
        LoadSceneManager.Instance.LoadSceneAsync(LoadSceneManager.eSceneState.Game); //게임 씬으로 비동기로딩 요청
    }

    //옵션 버튼 클릭 시
    public void OpenOption()
    {
        SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick);
        //옵션 팝업 생성
        PopupManager.Instance.OpenPopupOption(() => { SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick); }); 
    }

    //각 메뉴버튼 클릭 시 실행될 메소드
    public void OpenMenu(UIButton button)
    {
        SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick);

        gameObject.SetActive(false); //메인로비 비활성화
        var index = int.Parse(button.name.Substring(0, 2)); //각 메뉴버튼은 넘버링이 되어있으므로 해당 넘버링을 인덱스로 사용
        m_menu[index - 1].SetUI(); //해당 메뉴 활성화 ->> 다형성 사용!!!!
    }
    #endregion

    #region Private Methods
    void GetCurCharInfo()
    {
        if (m_menu[(int)eLobbyMenuType.Character].m_type == eLobbyMenuType.Character)
        {
            //캐릭터 메뉴 스크립트를 가져옴.
            LobbyMenu_Character menu = m_menu[(int)eLobbyMenuType.Character].gObj.GetComponent<LobbyMenu_Character>();
            //캐릭터 이미지를 받아온다.
            var spr = menu.GetCharSprite();
            //캐릭터별 위치좌표를 받아온다.
            var pos = menu.GetCharSprPosition(PlayerDataManager.Instance.GetCurHero() - 1);

            //로비의 캐릭터 이미지를 현재 선택된 것으로 변경
            m_charSpr.sprite2D = spr;
            m_charSpr.MakePixelPerfect();
            m_charSpr.transform.localPosition = pos + Vector3.up * 80f;

            //캐릭터 아이콘도 변경
            m_charIconSpr.spriteName = string.Format("select_character_{0:00}", PlayerDataManager.Instance.GetCurHero());

            //각 캐릭터의 이미지마다 Tween 포지션이 다르므로 각자의 포지션으로 변경 후 시작시켜줌.
            m_charTween.from = m_charSpr.transform.localPosition;
            m_charTween.to = m_charTween.from + Vector3.down * 20f;
            m_charTween.ResetToBeginning();
            m_charTween.PlayForward();
        }
    }
    #endregion
}
```
  
</div>
</details>

<br>

**Explanation**:mortar_board:<br>
(구현설명은 주석으로 간단하게 처리했습니다!)<br>
우선 각 메뉴들은 열기, 닫기 등의 동일한 기능이 존재하므로 인터페이스로 추려낸 후 상속받게 함으로써 코드를 규격화했으며, 다형성까지 사용할 수 있도록 구성했습니다.
이를 이용하기 위해 메뉴 버튼들의 오브젝트명을 메뉴의 순서대로 넘버링하고, 다형성을 사용해 메뉴를 활성화해주는 하나의 동일한 메소드를 모든 메뉴버튼의
OnClick 이벤트 메소드로 지정해 주었습니다. 이때 버튼 자기 자신을 파라미터로 이벤트 메소드에 넘겨주어 넘버링을 SubString하고 해당 넘버값으로 메뉴를 활성화해줍니다.

```c#
    public void OpenMenu(UIButton button) //파라메터 int로 대체가능
    {
        gameObject.SetActive(false); //메인로비 비활성화
        var index = int.Parse(button.name.Substring(0, 2)); //각 메뉴버튼은 넘버링이 되어있으므로 해당 넘버링을 인덱스로 사용
        m_menu[index - 1].SetUI(); //해당 메뉴 활성화 ->> 다형성 사용
    }
```

버튼의 OnClick 메소드와 파라미터를 지정해주는 과정 또한 인스펙터상에서 전부 해결이 가능하지만, 버튼의 이벤트를 소스 코드로 동적 할당 해보고자 모든 과정을 코드로 진행했습니다.
(프리팹 같은 경우, 외부에서 연결된 이벤트는 해제되기 때문에 이를 대처하는 연습을 하기 위함.)

</div>
</details>

<br>

<details>
<summary>Managers Code 접기/펼치기</summary>
<div markdown="1">

<br>

<details>
<summary>&nbsp;&nbsp;&nbsp;&nbsp;BuffManager 접기/펼치기</summary>
<div markdown="1">
 
```c#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BuffManager : SingletonMonoBehaviour<BuffManager> //싱글턴패턴 적용
{
    #region Field
    public enum eBuffType //버프타입
    {
        PowerShot, //파워샷 - 미구현
        Invincible, //부스터 + 무적
        Magnet, //자석
        Shield, //방어 후 무적
        Max
    }

    Dictionary<eBuffType, float> m_buffList = new Dictionary<eBuffType, float>(); //각 버프마다 지속시간 체크를 위한 딕셔너리
    float[] durations = new float[] { 10f, 2.55f, 5f, 3f }; //각 버프별 지속시간
    CameraShake m_camShake; //Invincible 효과시 카메라 흔들림 효과

    PlayerController m_player; //플레이어
    #endregion

    #region Unity Methods
    protected override void OnStart()
    {
        m_camShake = Camera.main.GetComponent<CameraShake>();
        m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        for(int i=0; i< (int)eBuffType.Max; i++)
        {
            m_buffList.Add((eBuffType)i, 0); //각 버프를 딕셔너리에 초기화
        }
    }

    void Update()
    {
        foreach (var kvp in m_buffList.ToList())
        {
            if (kvp.Value != 0) //버프가 진행중일 경우 -> 즉 남은시간이 0초가 아닌 경우
            {
                float newLen = kvp.Value - Time.deltaTime; //Time.deltaTime을 빼주며 시간체크

                if (newLen > 0f) //아직 시간이 남은경우
                {
                    m_buffList[kvp.Key] = newLen; //남은 시간 변경
                }
                else //시간이 남지 않은 경우
                {
                    m_buffList[kvp.Key] = 0f; //남은 시간을 0초로 초기화

                    switch (kvp.Key) //key값에 따른 case
                    {
                        case eBuffType.Magnet:
                            m_player.SetMagnet(false); //자석효과 해제
                            break;
                        case eBuffType.Invincible:
                            GameManager.Instance.SetState(GameManager.eGameState.Normal); //노말상태로 변경시켜 배경스크롤링, 몬스터 생성속도 및 이동속도 원상복귀
                            m_player.SetShockWave(true); //필드 정리를 위한 쇼크웨이브 발동
                            break;
                        case eBuffType.Shield:
                            m_player.ShieldOff(); //방어효과 해제
                            break;
                    }
                }
            }
        }
    }
    #endregion

    #region Public Methods
    //버프 활성화 메소드
    public void SetBuff(eBuffType buff)
    {
        if(buff == eBuffType.Invincible) //Invincible인 경우 카메라쉐이킹 효과
        {
            m_camShake.ShakeCamera();
        }

        if (m_buffList[buff] == 0f) //해당 버프가 활성화되어있지 않은 경우
        {
            m_buffList[buff] = durations[(int)buff]; //해당 버프의 지속시간만큼 대입
            switch (buff)
            {
                case eBuffType.Magnet:
                    m_player.SetMagnet(true); //자석효과 활성화
                    break;
                case eBuffType.Invincible:
                    GameManager.Instance.SetState(GameManager.eGameState.Invincible); //Invincible 상태로 변경시켜 배경스크롤링, 몬스터 생성속도 및 이동속도 증폭
                    break;
                case eBuffType.Shield:
                    m_player.ShieldOn(); //쉴드효과 활성화
                    break;
            }
        }
        else //해당 버프가 이미 활성화되어있던 경우
        {
            m_buffList[buff] = durations[(int)buff]; //시간을 리셋시켜준다.
        }
    }
    #endregion
}
```

</div>
</details>

<details>
<summary>&nbsp;&nbsp;&nbsp;&nbsp;SoundManager 접기/펼치기</summary>
<div markdown="1">
  
```c#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : DonDestroy<SoundManager> //싱글턴 + DonDestroyOnLoad 적용
{
    #region Field
    public enum eAudioType //오디오타입
    {
        BGM,
        SFX,
        Max
    }

    public enum eAudioSFXClip //효과음 클립 타입
    {
        GetCoin,
        GetGem,
        GetInvincible,
        GetItem,
        MonDie,
        NextScene,
        ButtonClick,
        Shield,
        MeteoExplosion,
        MeteoAlert,
        Result,
        PlayerDie,
        Max
    }

    public enum eAudioBGMClip //BGM 클립 타입
    {
        BGM01,
        BGM02,
        BGM03,
        Lobby,
        Max
    }

    [SerializeField]
    AudioClip[] m_sfxClip; //효과음 클립들
    [SerializeField]
    AudioClip[] m_bgmClip; //BGM 클립들

    AudioSource[] m_audio = new AudioSource[(int)eAudioType.Max]; //오디오소스 리스트 생성
    //동시재생에 제한을 두기 위한 List
    List<float> m_limitList = new List<float>();
    #endregion

    #region Unity Methods
    protected override void OnStart()
    {
        //BGM용 오디오소스 생성 및 초기화
        m_audio[(int)eAudioType.BGM] = gameObject.AddComponent<AudioSource>();
        m_audio[(int)eAudioType.BGM].loop = true; //반복시킴
        m_audio[(int)eAudioType.BGM].playOnAwake = false;
        m_audio[(int)eAudioType.BGM].rolloffMode = AudioRolloffMode.Linear;
        
        //효과음용 오디오소스 생성 및 초기화
        m_audio[(int)eAudioType.SFX] = gameObject.AddComponent<AudioSource>();
        m_audio[(int)eAudioType.SFX].loop = false;
        m_audio[(int)eAudioType.SFX].playOnAwake = false;
        m_audio[(int)eAudioType.SFX].rolloffMode = AudioRolloffMode.Linear;

        //PlayerPrefs에 저장되어있던 옵션값대로 Mute의 활성화 여부를 결정.
        MuteBGM(PlayerPrefs.GetInt("OPTION_BGM", 1) == 1 ? false : true);
        MuteSFX(PlayerPrefs.GetInt("OPTION_SFX", 1) == 1 ? false : true);
    }

    private void Update()
    {
        //더블버퍼를 통해 반복문내 List의 Remove를 통한 Count에러를 차단함.
        List<float> newList = new List<float>();

        for (int i=0; i<m_limitList.Count; i++)
        {
            float newLen = m_limitList[i] - Time.deltaTime; //Time.deltaTime을 빼주면서 오디오클립의 남은 재생시간을 체크한다.
            if(newLen > 0.0f) //재생시간이 남아있는 경우
            {
                newList.Add(newLen); //새 버퍼에 추가
            }
        }

        m_limitList = newList; //기존 버퍼로 대입
    }
    #endregion

    #region Public Methods
    public void MuteSFX(bool isOn) //효과음 mute
    {
        m_audio[(int)eAudioType.SFX].mute = isOn;
    }

    public void PlaySfx(eAudioSFXClip clip) //효과음 play
    {
        //이미 리스트에 play 요청이 들어온 오디오클립이 포함되어있는 경우 -> 즉 이미 동일한 클립이 재생중인 경우 포함시키지 않음으로써 중복재생을 막는다.
        if(!m_limitList.Contains(m_sfxClip[(int)clip].length))
        {
            m_limitList.Add(m_sfxClip[(int)clip].length); //오디오클립의 재생길이를 add
            m_audio[(int)eAudioType.SFX].PlayOneShot(m_sfxClip[(int)clip]); //play
        }  
    }
    
    public void MuteBGM(bool isOn) //BGM mute
    {
        m_audio[(int)eAudioType.BGM].mute = isOn;
    }

    public void PauseBGM() //BGM pause
    {
        m_audio[(int)eAudioType.BGM].Pause();
    }

    public void PlayBGM() //BGM play
    {
        if (m_audio[(int)eAudioType.BGM].clip == null) return;
        m_audio[(int)eAudioType.BGM].Play();
    }
    
    public void PlayBGM(eAudioBGMClip clip, float volume) //BGM play overloading
    {
        m_audio[(int)eAudioType.BGM].clip = m_bgmClip[(int)clip];
        m_audio[(int)eAudioType.BGM].volume = volume;
        m_audio[(int)eAudioType.BGM].Play();
    }
    #endregion
}
```

</div>
</details>

<details>
<summary>&nbsp;&nbsp;&nbsp;&nbsp;MonsterManager 접기/펼치기</summary>
<div markdown="1">

```c#
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : SingletonMonoBehaviour<MonsterManager> //싱글턴패턴 적용
{
    #region Field
    [Serializable]
    public class MonsterPartsSprite //몬스터 타입별로 파츠를 지정해주기 위해 클래스로 구성
    {
        public Sprite[] m_parts; //몬스터를 이루는 양날개, 몸통, 눈 등 이미지 파츠들
    }

    public enum eMonsterType //몬스터 타입
    {
        None = -1,
        White,
        Yellow,
        Pink,
        Bomb, //폭탄 드래곤
        Max
    }

    [SerializeField]
    GameObject m_monsterPrefab; //몬스터 프리팹 -> 파츠만 변경시켜서 사용
    [SerializeField]
    MonsterPartsSprite[] m_monsterParts; //몬스터파츠 클래스형 배열
    GameObjectPool<MonsterController> m_monsterPool; //오브젝트 풀링

    //가장왼쪽위 몬스터의 시작좌표
    Vector2 m_startPos = new Vector2(-2.26f, 6f);
    float m_posXGap = 1.12f; //몬스터간 좌표 갭

    PlayerController m_player; //플레이어
    List<MonsterController> m_monsterList = new List<MonsterController>(); //Active되어있는 몬스터들만 들어있는 리스트
    int m_lineNumber; //라인넘버 -> 몇번째로 생성된 라인인지를 나타냄
    float m_spawnTimeScale = 1f; //몬스터의 생성속도와 이동속도의 기준치
    float m_spawnInterval = 2.549f; //몬스터 생성메소드의 호출 간격
    #endregion

    #region Unity Methods
    protected override void OnStart()
    {
        m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        //오브젝트 풀링 적용
        m_monsterPool = new GameObjectPool<MonsterController>(20, () =>
        {
            var obj = Instantiate(m_monsterPrefab) as GameObject;
            obj.transform.SetParent(transform);
            obj.transform.localPosition = Vector3.zero; 
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;
            var mon = obj.GetComponent<MonsterController>();
            mon.InitMonster(m_player); // 각 몬스터에게 플레이어를 넘겨줌

            return mon;
        });

        InvokeRepeating("CreateMonsters", 3f, m_spawnInterval / m_spawnTimeScale); //몬스터 생성 메소드를 간격마다 호출
    }
    #endregion

    #region Public Methods
    public void StopCreateMonsters() //몬스터 생성 중지
    {
        CancelInvoke("CreateMonsters");
    }

    //몬스터의 생성속도 및 이동속도 변경
    public void SetSpawnInterval(float scale) 
    {
        m_spawnTimeScale = scale;

        //이미 생성되어있는 몬스터들의 이동 스케일을 바꿔준다.
        for (int i = 0; i < m_monsterList.Count; i++)
        {
            m_monsterList[i].SetSpeedScale(scale);
        }

        //기존 리피팅 취소
        CancelInvoke("CreateMonsters");
        InvokeRepeating("CreateMonsters", 0f, m_spawnInterval / m_spawnTimeScale); //scale을 기준으로 몬스터 생성속도 변경
    }

    //몬스터 생성
    public void CreateMonsters()
    {
        bool isBomb = false; //각 라인마다 폭탄 드래곤의 생성여부를 체크
        for (int i = 0; i < 5; i++) //5마리 생성
        {
            var type = (eMonsterType)UnityEngine.Random.Range((int)eMonsterType.White, (int)eMonsterType.Max); //타입을 랜덤으로 결정

            if (type == eMonsterType.Bomb && !isBomb) //폭탄드래곤이 생성된적이 없고, 폭탄드래곤이 나온 경우
            {
                isBomb = true;
            }
            else if (type == eMonsterType.Bomb && isBomb) //폭탄드래곤이 나왔는데 이미 생성되어있는 경우 -> 한 라인에 폭탄드래곤은 한마리만 존재해야함.
            {
                do
                {
                    type = (eMonsterType)UnityEngine.Random.Range((int)eMonsterType.White, (int)eMonsterType.Max);
                } while (type == eMonsterType.Bomb); //폭탄드래곤이 아닌 다른 타입이 나올때까지 반복
            }

            var mon = m_monsterPool.Get(); //오브젝트풀링에서 드래곤을 하나 받아온다.
            mon.SetMonster(type, m_lineNumber, m_spawnTimeScale); //타입, 라인넘버, 스케일값을 초기화
            mon.transform.position = Vector3.right * (m_startPos.x + i * m_posXGap) + Vector3.up * m_startPos.y; //드래곤이 활성화될 위치를 계산하여 대입
            mon.gameObject.SetActive(true); //활성화

            m_monsterList.Add(mon); //활성화되어있는 몬스터 리스트에 추가
        } 
        m_lineNumber++; //라인넘버 ++
    }

    //몬스터 제거
    public void RemoveMonster(MonsterController mon)
    {
        mon.gameObject.SetActive(false); //몬스터 오브젝트 비활성화
        if (m_monsterList.Remove(mon))
        {
            m_monsterPool.Set(mon); //풀링에 반환
        }
    }

    //Bomb몬스터가 죽을때 해당 라인의 모든 몬스터를 제거.
    public void RemoveMonsterAll(int lineNum)
    {
        for (int i = 0; i < m_monsterList.Count; i++)
        {
            if (m_monsterList[i].LineNum == lineNum) //제거된 폭탄드래곤과 같은 라인넘버를 가진 드래곤들을 모두 제거 
            {
                m_monsterList[i].m_isAlive = false;
                m_monsterList[i].SetDie();
                m_monsterList[i].gameObject.SetActive(false);
                m_monsterPool.Set(m_monsterList[i]); //풀링에 반환
            }
        }

        //한번에 리스트에서 지워준다.
        m_monsterList.RemoveAll(mon => !mon.m_isAlive);
    }

    //몬스터의 파츠 이미지를 
    public Sprite[] GetMonsterParts(eMonsterType type)
    {
        //해당 타입의 스프라이트들을 넘겨줌.
        return m_monsterParts[(int)type].m_parts;
    }
    #endregion
}
```

</div>
</details>

<br>

**Explanation**:mortar_board:<br>
(구현설명은 주석으로 간단하게 처리했습니다!)<br>

*BuffManager*<br>
플레이어를 관리하는 Class를 경량화하기 위해 버프들을 관리하는 매니저를 따로 작성했습니다. 
<버프타입, 남은 지속시간>으로 이루어진 key-value pair를 데이터로 갖는 Dictionary 자료구조를 이용했습니다.
Dictionary의 연관된 데이터를 짝지어 관리할 수 있는 점을 이용해 버프들의 남은 지속시간을 효율적으로 체크할 수 있었습니다.

*SoundManager*<br>
싱글턴패턴을 적용한 SoundManager 같은 경우 모든 사운드클립 및 오디오소스를 관리하며 재생 및 중단을 수행하게끔 정리했습니다.
이를 통해 사운드 재생이 필요한 곳에서 쉽게 참조하여 간편하게 사용할 수 있었습니다. 또한, 몬스터나 메테오와 같이 동시에
같은 사운드클립에 대해 재생을 중복해서 요청하는 경우 소리가 증폭되고 깨지는 현상이 발생했습니다. 이를 방지하기 위해 현재 재생중인
오디오 클립의 남은 재생 길이를 List로 관리했고, 새로 들어온 재생요청에 대해서 해당 요청 클립의 길이가 List 내에 데이터로써 존재한다면
재생을 시키지 않는 것으로 해결했습니다.
(재생이 끝나면 List 내에서 제거)

*MonsterManager*<br>
싱글턴패턴을 적용한 MonsterManager 같은 경우 모든 몬스터에 대한 관리를 수행하게끔 구성했습니다.
특히, 몬스터들로 구성된 한 라인을 생성할 때 (빨강)폭탄드래곤을 꼭 포함하도록 강제했으며, 각 몬스터에게 line 값을 저장 시켜
폭탄드래곤 처치 시 같은 line에 존재하는 모든 드래곤을 처치할 수 있도록 했습니다. 또한 기본적인 몬스터 프리팹에
타입별로 스프라이트 이미지만 변경 시켜 모든 몬스터들을 표현할 수 있도록 했습니다.

</div>
</details>

<br>

<details>
<summary>Optimization 접기/펼치기</summary>
<div markdown="1">

<br>

**Explanation**:scissors:<br>
* Object Pooling 기법을 사용하여, 반복되는 생성-삭제 작업으로인한 GC의 잦은 호출을 방지했습니다.
* Atlas를 사용하여 드로우콜을 낮추고자 했습니다.

<br>

</div>
</details>

<br>

### Difficult Point.:sweat_smile:
* 무엇보다 NGUI에 대해서 새로 배워야했던 것이 고비로 다가왔던 적이 많은 것 같습니다. 예를들자면 위 Menu 관련 설명에도 나와있지만 코드상에서 NGUI버튼의 
Onclick 이벤트 메소드를 동적으로 할당해줄때, EventDelegate를 매개변수로 넘겨주어야 하며, 또한 EventDelegate의 매개변수를 지정해주려면
EventDelegata.Parameter를 생성해 지정해주어야 했습니다. 이때 매개변수로 넘겨줄 수 있는 형식은 UnityEngine.Object 였기 때문에, int값을 넘겨줄 수 없는
상황이었습니다. 결국 버튼 오브젝트의 이름앞에 넘버링을하고 버튼을 매개변수로 넘겨준 후, 해당 넘버링을 substring해서 사용하게끔 하여 해결했지만 UGUI를 사용할때와는
달리 좀 더 복잡해 꽤 많은 시간이 소요되었으며, 비효율적인 방식이라 생각되어 아쉬웠던 기억이 있습니다.
그렇다고 NGUI가 마냥 불편한것은 아니었습니다. Tween 기능을 제공하는것이 굉장히 마음에 들었고, 개인적으로 Depth관리 방식은 UGUI에 비해 훨씬 편했습니다. 
->> 추후 https://berabue.blogspot.com/2014/05/unity-ngui.html 해당 링크에서 변수명을 넘겨줌으로써 정수값을 전달할 수 있는 방식을 알게되었습니다.
->> 또한 개발당시 사용한 NGUI는 3.6.2 버전이었는데 버전의 차이인지, http://blog.naver.com/PostView.nhn?blogId=star_breeze&logNo=221009506490 해당 링크와 같은 방식을 사용할 수 없었습니다.

