using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyController : MonoBehaviour
{
    #region Field
    [SerializeField]
    GameObject m_menuBtnObj;
    [SerializeField]
    GameObject m_menuObj;
    [SerializeField]
    UI2DSprite m_charSpr;
    [SerializeField]
    UISprite m_charIconSpr;

    ILobbyMenu[] m_menu;
    UIButton[] m_menuBtns;
    TweenPosition m_charTween;
    #endregion

    #region Unity Methods
    //GetCurCharInfo를 OnEnable에서 호출해주기때문에 start에서 아래 정보들을 가져오면 너무 늦어서 널이 뜸.
    void Awake()
    {
        m_charTween = m_charSpr.GetComponent<TweenPosition>();
        m_menuBtns = m_menuBtnObj.GetComponentsInChildren<UIButton>();
        var results = m_menuObj.GetComponentsInChildren<ILobbyMenu>();

        m_menu = new ILobbyMenu[results.Length];

        for (int i = 0; i < m_menu.Length; i++)
        {
            m_menu[i] = results[i];
            m_menu[i].CloseUI();
        }

        for (int i = 0; i < m_menuBtns.Length; i++)
        {
            EventDelegate del = new EventDelegate(this, "OpenMenu");
            del.parameters[0] = Util.MakeParameter(m_menuBtns[i], typeof(UIButton));
            m_menuBtns[i].onClick.Add(del);
        }
    }

    private void OnEnable()
    {
        GetCurCharInfo();
    }
    #endregion

    #region Public Methods
    public void StartGame()
    {
        SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick);
        LoadSceneManager.Instance.LoadSceneAsync(LoadSceneManager.eSceneState.Game);
    }

    public void OpenOption()
    {
        SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick);
        PopupManager.Instance.OpenPopupOption(() => { SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick); });
    }

    public void OpenMenu(UIButton button)
    {
        SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick);

        gameObject.SetActive(false);
        var index = int.Parse(button.name.Substring(0, 2));
        m_menu[index - 1].SetUI();
    }
    #endregion

    #region Private Methods
    void GetCurCharInfo()
    {
        if (m_menu[(int)eLobbyMenuType.Character].m_type == eLobbyMenuType.Character)
        {
            LobbyMenu_Character menu = m_menu[(int)eLobbyMenuType.Character].gObj.GetComponent<LobbyMenu_Character>();
            var spr = menu.GetCharSprite();
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
