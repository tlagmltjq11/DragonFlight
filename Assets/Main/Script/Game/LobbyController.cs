using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyController : MonoBehaviour
{
    [SerializeField]
    GameObject m_menuBtnObj;
    [SerializeField]
    GameObject m_menuObj;
    ILobbyMenu[] m_menu;
    UIButton[] m_menuBtns;

    [SerializeField]
    UI2DSprite m_charSpr;
    TweenPosition m_charTween;
    [SerializeField]
    UISprite m_charIconSpr;

    public void StartGame()
    {
        LoadSceneManager.Instance.LoadSceneAsync(LoadSceneManager.eSceneState.Game);
    }

    public void OpenOption()
    {
        PopupManager.Instance.OpenPopupOption(null);
    }

    void GetCurCharInfo()
    {
        if(m_menu[(int)eLobbyMenuType.Character].m_type == eLobbyMenuType.Character)
        {
            LobbyMenu_Character menu = m_menu[(int)eLobbyMenuType.Character].gObj.GetComponent<LobbyMenu_Character>();
            var spr = menu.GetCharSprite();
            var pos = menu.GetCharSprPosition(PlayerDataManager.Instance.GetCurHero() - 1);
            m_charSpr.sprite2D = spr;
            m_charSpr.MakePixelPerfect();
            m_charSpr.transform.localPosition = pos + Vector3.up * 80f;
            m_charTween.from = m_charSpr.transform.localPosition;
            m_charTween.to = m_charTween.from + Vector3.down * 20f;
            m_charIconSpr.spriteName = string.Format("select_character_{0:00}", PlayerDataManager.Instance.GetCurHero());
            //m_charTween.ResetToBeginning();
            //m_charTween.PlayForward();
        }
    }

    private void OnEnable()
    {
        GetCurCharInfo();
    }

    public void OpenMenu(UIButton button)
    {
        gameObject.SetActive(false);
        var index = int.Parse(button.name.Substring(0, 2));
        m_menu[index - 1].SetUI();
    }

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

    // Update is called once per frame
    void Update()
    {
        
    }
}
