using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyMenu_Character : MonoBehaviour, ILobbyMenu
{
    [SerializeField]
    UI2DSprite m_characterSpr;
    [SerializeField]
    Vector3[] m_charSprPos;
    [SerializeField]
    UISprite m_darkAreaSpr;
    [SerializeField]
    UIButton[] m_buttons;

    [SerializeField]
    LobbyController m_lobby;

    [SerializeField]
    TweenPosition m_charSprTween;

    [SerializeField]
    string[] m_classNameList;
    [SerializeField]
    string[] m_charNameList;
    [SerializeField]
    UILabel m_className;
    [SerializeField]
    UILabel m_charName;

    [SerializeField]
    UISprite m_charIconSpr;

    int m_selectIndex = 0;

    public eLobbyMenuType m_type { get { return eLobbyMenuType.Character; } }

    public GameObject gObj { get { return gameObject; } }

    void RefreshInfo(int index)
    {
        if (PlayerDataManager.Instance.IsOwnedCharacter(index))
        {
            m_darkAreaSpr.depth = 0;
            m_buttons[0].gameObject.SetActive(true);
            m_buttons[1].gameObject.SetActive(false);
        }
        else
        {
            m_darkAreaSpr.depth = 2;
            m_buttons[0].gameObject.SetActive(false);
            m_buttons[1].gameObject.SetActive(true);
        }
    }

    public void SetUI()
    {
        gameObject.SetActive(true);
        LoadCharacterSprite(m_selectIndex);
    }

    public void CloseUI()
    {
        gameObject.SetActive(false);
    }

    public Vector3 GetCharSprPosition(int index)
    {
        return m_charSprPos[index];
    }

    public Sprite GetCharSprite()
    {
        return m_characterSpr.sprite2D;
    }

    void LoadCharacterSprite(int index)
    {
        var spr = Resources.Load<Sprite>(string.Format("Character/character_{0:00}", index + 1));
        m_characterSpr.sprite2D = spr;

        //이미지 크기를 원래 크기로 맞춰주는것. 즉 snap
        m_characterSpr.MakePixelPerfect();
        m_characterSpr.transform.localPosition = m_charSprPos[index];

        m_className.text = m_classNameList[index];
        m_charName.text = m_charNameList[index];
        m_charIconSpr.spriteName = string.Format("select_character_{0:00}", index + 1);

        m_charSprTween.from = m_characterSpr.transform.localPosition;
        m_charSprTween.to = m_charSprTween.from + Vector3.down * 20;
        m_charSprTween.ResetToBeginning();
        m_charSprTween.PlayForward();

        RefreshInfo(index);
    }

    public void OnSelect()
    {
        PlayerDataManager.Instance.SetCurHero(m_selectIndex);
        PlayerDataManager.Instance.SaveData();
        m_lobby.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    public void OnBuyCharacter()
    {
        PopupManager.Instance.OpenPopupOkCancel("Notice", string.Format("[00FF00]35레벨로 성장[-]되어 있는 [00FF00]{0}[-]를 수정 40개로\r\n구입하시겠습니까?", m_charName.text), ()=> 
        { 
            if(PlayerDataManager.Instance.DecreaseGem(40))
            {
                PopupManager.Instance.ClosePopup();
                PlayerDataManager.Instance.BuyCharacter(m_selectIndex);
                RefreshInfo(m_selectIndex);
            }
            else
            {
                PopupManager.Instance.OpenPopupOk("Notice", "소지한 수정이 부족합니다.", null);
            }
        }, null);
    }

    public void OnPressBack()
    {
        LoadCharacterSprite(PlayerDataManager.Instance.GetCurHero() - 1);
        m_lobby.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    public void OnPressLeft()
    {
        m_selectIndex--;


        if(m_selectIndex < 0)
        {
            m_selectIndex = 12;
        }

        LoadCharacterSprite(m_selectIndex);
    }

    public void OnPressRight()
    {
        m_selectIndex++;

        if(m_selectIndex > 12)
        {
            m_selectIndex = 0;
        }

        LoadCharacterSprite(m_selectIndex);
    }

    private void OnDisable()
    {
        m_darkAreaSpr.depth = 0;
    }

    private void Awake()
    {
        LoadCharacterSprite(PlayerDataManager.Instance.GetCurHero()-1);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
