using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyMenu_Shop : MonoBehaviour, ILobbyMenu
{
    #region Field
    public enum eItemType
    {
        item_bomb, //Canon
        item_b1,
        item_b2,
        item_b3,
        item_b4,
        item_b5,
        item_b6,
        item_b7,
        Max
    }

    [SerializeField]
    UILabel m_manual;
    [SerializeField]
    UILabel m_price;
    [SerializeField]
    UILabel m_name;
    [SerializeField]
    UIButton m_purchase;
    [SerializeField]
    UISprite m_goldSpr;

    public Item[] m_itemList;
    Item m_curItem;

    #endregion

    #region Unity Methods
    void Start()
    {

    }
    #endregion

    #region Public Methods
    public eLobbyMenuType m_type { get { return eLobbyMenuType.Shop; } }

    public GameObject gObj { get { return gameObject; } }

    public void SetUI()
    {
        gameObject.SetActive(true);
        SetCurItem();
    }

    public void CloseUI()
    {
        gameObject.SetActive(false);
    }

    public void SetCurItem()
    {
        bool check = false;

        for (int i = 0; i < m_itemList.Length; i++)
        {
            if (!m_itemList[i].m_isSaled)
            {
                m_itemList[i].OnSelect();
                LoadItems(m_itemList[i].m_type); //앞순서부터 첫번째로 안팔린 상태의 아이템을 메뉴얼창에 로드시킨다.
                m_curItem = m_itemList[i]; //현재 선택된 아이템
                check = true;
                break;
            }
        }

        //살 수 있는 아이템이 하나도 없다면
        if(!check)
        {
            m_goldSpr.enabled = false;
            m_purchase.isEnabled = false;
        }
    }

    public void OnBuyItem()
    {
        if(m_curItem != null)
        {
            PlayerPrefs.SetInt("IsOwned" + m_curItem.m_type.ToString(), 1);
            m_curItem.SalesCompleted();

            SetCurItem();
        }
    }

    public void OnSelectItem(Item item)
    {
        //이전에 선택되어있던 아이템을 꺼준다.
        for (int i = 0; i < m_itemList.Length; i++)
        {
            if(m_itemList[i].IsSelected())
            {
                m_itemList[i].Release();
                break;
            }
        }

        m_curItem = item;
        //현재 아이템의 선택효과를 켜준다.
        item.Selected();
        //해당 아이템의 설명을 띄워준다.
        LoadItems(item.m_type);

        if(item.m_isSaled)
        {
            m_purchase.isEnabled = false;
        }
        else
        {
            m_purchase.isEnabled = true;
        }
    }
    #endregion

    #region Private Methods
    void LoadItems(eItemType type)
    {
        m_goldSpr.enabled = true;
        m_name.text = m_itemList[(int)type].m_name;
        m_manual.text = m_itemList[(int)type].m_manual;
        m_price.text = m_itemList[(int)type].m_price.ToString();
    }
    #endregion
}
