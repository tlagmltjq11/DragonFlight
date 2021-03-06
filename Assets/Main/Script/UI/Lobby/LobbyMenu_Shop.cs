﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyMenu_Shop : MonoBehaviour, ILobbyMenu
{
    #region Field
    public enum eItemType
    {
        BlazeBullet,
        WingBullet,
        FireBullet,
        DarkBullet,
        HighShield,
        LowShield,
        WhiteWing,
        YellowWing,
        Dummy1,
        Dummy2,
        Dummy3,
        Dummy4,
        Dummy5,
        Dummy6,
        Dummy7,
        Dummy8,
        Dummy9,
        Dummy10,
        Max
    }

    public enum eShopType
    {
        Weapon,
        Armor,
        Acc,
        Max
    }

    [SerializeField]
    UILabel m_manual;
    [SerializeField]
    UILabel m_price;
    [SerializeField]
    UILabel m_name;
    [SerializeField]
    UILabel m_stat;
    [SerializeField]
    UILabel m_class;
    [SerializeField]
    UIButton m_purchase;
    [SerializeField]
    UIButton[] m_shopTypeBtn;
    [SerializeField]
    UISprite m_goldSpr;
    [SerializeField]
    LobbyController m_lobby;
    [SerializeField]
    UILabel m_goldOwned;
    [SerializeField]
    UIGrid m_grid;

    public Item[] m_itemList;

    Item m_curItem;

    bool m_temp = false;

    #endregion

    #region Public Methods
    public eLobbyMenuType m_type { get { return eLobbyMenuType.Shop; } }

    public GameObject gObj { get { return gameObject; } }

    public void SetUI()
    {
        gameObject.SetActive(true);
        OnWeapon();
    }

    public void CloseUI()
    {
        gameObject.SetActive(false);
        m_temp = false;
    }

    public void OnWeapon()
    {
        //중복재생방지
        if (m_shopTypeBtn[(int)eShopType.Weapon].isEnabled && m_temp == true)
        {
            SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick);
        }

        m_temp = true;

        for (int i=0; i<(int)eShopType.Max; i++)
        {
            if(m_shopTypeBtn[i].isEnabled == false)
            {
                m_shopTypeBtn[i].isEnabled = true;

                break;
            }
        }

        m_shopTypeBtn[(int)eShopType.Weapon].isEnabled = false;

        for (int i = 0; i < m_itemList.Length; i++)
        {
            if(m_itemList[i].m_class == Item.eItemClass.Weapon)
            {
                m_itemList[i].gameObject.SetActive(true);
            }
            else
            {
                m_itemList[i].gameObject.SetActive(false);
            }
        }

        m_grid.Reposition();
        SetCurItem();
    }

    public void OnArmor()
    {
        //중복재생방지
        if (m_shopTypeBtn[(int)eShopType.Armor].isEnabled)
        {
            SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick);
        }

        for (int i = 0; i < (int)eShopType.Max; i++)
        {
            if (m_shopTypeBtn[i].isEnabled == false)
            {
                m_shopTypeBtn[i].isEnabled = true;

                break;
            }
        }

        m_shopTypeBtn[(int)eShopType.Armor].isEnabled = false;

        for (int i = 0; i < m_itemList.Length; i++)
        {
            if (m_itemList[i].m_class == Item.eItemClass.Armor)
            {
                m_itemList[i].gameObject.SetActive(true);
            }
            else
            {
                m_itemList[i].gameObject.SetActive(false);
            }
        }

        m_grid.Reposition();
        SetCurItem();
    }

    public void OnAcc()
    {
        //중복재생방지
        if (m_shopTypeBtn[(int)eShopType.Acc].isEnabled)
        {
            SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick);
        }

        for (int i = 0; i < (int)eShopType.Max; i++)
        {
            if (m_shopTypeBtn[i].isEnabled == false)
            {
                m_shopTypeBtn[i].isEnabled = true;

                break;
            }
        }

        m_shopTypeBtn[(int)eShopType.Acc].isEnabled = false;

        for (int i = 0; i < m_itemList.Length; i++)
        {
            if (m_itemList[i].m_class == Item.eItemClass.Acc)
            {
                m_itemList[i].gameObject.SetActive(true);
            }
            else
            {
                m_itemList[i].gameObject.SetActive(false);
            }
        }

        m_grid.Reposition();
        SetCurItem();
    }

    public void SetCurItem()
    {
        bool check = false;

        for (int i = 0; i < m_itemList.Length; i++)
        {
            if (m_itemList[i].gameObject.activeSelf)
            {
                if (!m_itemList[i].m_isSaled)
                {
                    OnSelectItem(m_itemList[i]);
                    LoadItems(m_itemList[i].m_type); //앞순서부터 첫번째로 안팔린 상태의 아이템을 메뉴얼창에 로드시킨다.
                    m_curItem = m_itemList[i]; //현재 선택된 아이템
                    check = true;
                    break;
                }
            }
        }

        //살 수 있는 아이템이 하나도 없다면
        if(!check)
        {
            m_goldSpr.enabled = false;
            m_purchase.isEnabled = false;
            m_goldOwned.text = "보유      : [00FF00]" + PlayerDataManager.Instance.GetGold() + "[-]";
        }
    }

    public void OnPressBack()
    {
        SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick);
        m_lobby.gameObject.SetActive(true);
        CloseUI();
    }

    public void OnBuyItem()
    {
        if(m_curItem != null)
        {
            SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick);

            PopupManager.Instance.OpenPopupOkCancel("Notice", string.Format("아이템 [00FF00]{0}[-]를 골드 [00FF00]{1}개[-]로 구입하시겠습니까?", m_curItem.m_name, m_curItem.m_price), () =>
            {
                SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick);

                if (PlayerDataManager.Instance.DecreaseGold(m_curItem.m_price))
                {
                    PopupManager.Instance.ClosePopup();
                    PlayerDataManager.Instance.BuyItem((int)m_curItem.m_type);
                    m_curItem.SalesCompleted();
                    SetCurItem();
                }
                else
                {
                    PopupManager.Instance.OpenPopupOk("Notice", "소지한 골드가 부족합니다.", () => { SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick); });
                }
            }, () => { SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick); });
        }
    }

    public void OnSelectItem(Item item)
    {
        //이전에 선택되어있던 아이템을 꺼준다.
        for (int i = 0; i < m_itemList.Length; i++)
        {
            if (m_itemList[i].gameObject.activeSelf)
            {
                if (m_itemList[i].IsSelected())
                {
                    m_itemList[i].Release();
                    break;
                }
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

        switch (m_curItem.m_class)
        {
            case Item.eItemClass.Weapon:
                m_stat.text = "[FF0000]공격력    : " + m_itemList[(int)type].m_stat.ToString();
                m_class.text = "[FFFF00]무기";
                break;
            case Item.eItemClass.Armor:
                m_stat.text = "[FF0000]방어횟수  : " + m_itemList[(int)type].m_stat.ToString() + "회";
                m_class.text = "[FFFF00]방어구";
                break;
            case Item.eItemClass.Acc:
                m_stat.text = "[FF0000]점수증가  : " + m_itemList[(int)type].m_stat.ToString() + "배";
                m_class.text = "[FFFF00]악세사리";
                break;
            default:
                break;
        }

        m_goldOwned.text = "보유      : [00FF00]" + PlayerDataManager.Instance.GetGold() + "[-]";
    }
    #endregion
}
