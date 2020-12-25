using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyMenu_Inventory : MonoBehaviour, ILobbyMenu
{
    #region Field
    [SerializeField]
    LobbyMenu_Character m_charMenu;
    [SerializeField]
    UI2DSprite m_charSpr;
    [SerializeField]
    LobbyController m_lobby;
    [SerializeField]
    UIGrid m_uiGrid;
    [SerializeField]
    UIButton m_equipBtn;
    [SerializeField]
    UILabel m_manual;
    [SerializeField]
    UILabel m_name;
    [SerializeField]
    UILabel m_stat;
    public UISprite[] m_equipItems;
    TweenPosition m_charTween;
    Dictionary<string, Item> m_itemDict = new Dictionary<string, Item>();
    Item m_curItem;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        m_charTween = m_charSpr.gameObject.GetComponent<TweenPosition>();
    }
    #endregion

    #region Public Methods
    public eLobbyMenuType m_type { get { return eLobbyMenuType.Inventory; } }

    public GameObject gObj { get { return gameObject; } }

    public void SetUI()
    {
        gameObject.SetActive(true);
        LoadInventoryInfo();
    }

    public void CloseUI()
    {
        gameObject.SetActive(false);
    }
    public void OnPressBack()
    {
        SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick);
        m_lobby.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    public void OnEquipItem()
    {
        if (m_curItem != null)
        {
            SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick);

            PopupManager.Instance.OpenPopupOkCancel("Notice", string.Format("아이템 [00FF00]{0}[-]를 장착하시겠습니까?", m_curItem.m_name), () =>
            {
                SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick);
                PopupManager.Instance.ClosePopup();

                PlayerDataManager.Instance.SetCurEquipItem(m_curItem.m_class, (int)m_curItem.m_type);
                LoadEquipItemInfo(m_curItem);
                m_curItem.Equipped();

            }, () => { SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick); });
        }
    }

    public void OnSelectItem(Item item)
    {
        foreach(Item prevItem in m_itemDict.Values)
        {
            if(prevItem.IsSelected())
            {
                prevItem.Release();
                break;
            }
        }

        m_curItem = item;

        //현재 아이템의 선택효과를 켜준다.
        item.Selected();
        //해당 아이템의 설명을 띄워준다.
        LoadItemsManual(m_curItem);

        if (item.m_isEquipped)
        {
            m_equipBtn.isEnabled = false;
        }
        else
        {
            m_equipBtn.isEnabled = true;
        }
    }
    #endregion

    #region Private Methods
    void LoadInventoryInfo()
    {
        m_charMenu = m_charMenu.gObj.GetComponent<LobbyMenu_Character>();
        var spr = m_charMenu.GetCharSprite();
        var pos = m_charMenu.GetCharSprPosition(PlayerDataManager.Instance.GetCurHero() - 1);

        //로비의 캐릭터 이미지를 현재 선택된 것으로 변경
        m_charSpr.sprite2D = spr;
        m_charSpr.MakePixelPerfect();
        m_charSpr.transform.localPosition = pos + Vector3.up * 400f;

        m_charTween.from = m_charSpr.transform.localPosition;
        m_charTween.to = m_charTween.from + Vector3.down * 20f;
        m_charTween.ResetToBeginning();
        m_charTween.PlayForward();


        for(int i=0; i<PlayerDataManager.Instance.GetEquipmentsNums(); i++)
        {
            if(PlayerDataManager.Instance.IsOwnedItem(i))
            {
                string target = string.Format("Equipment/Equipment_{0:00}", i + 1);

                if (!m_itemDict.ContainsKey(target))
                {
                    GameObject equips = Resources.Load<GameObject>(target);
                    
                    var equip = Instantiate(equips) as GameObject;
                    Item item = equip.GetComponent<Item>();

                    m_itemDict.Add(target, item);

                    equip.transform.SetParent(m_uiGrid.transform);
                    equip.transform.localPosition = Vector3.zero;
                    equip.transform.localScale = Vector3.one;
                    
                    //장착되어있던 장비들 처리.
                    if(PlayerDataManager.Instance.IsEquippedItem(item.m_class, (int)item.m_type))
                    {
                        switch (item.m_class)
                        {
                            case Item.eItemClass.Weapon:
                                m_equipItems[0].spriteName = item.m_icon.spriteName;
                                break;

                            case Item.eItemClass.Armor:
                                m_equipItems[1].spriteName = item.m_icon.spriteName;
                                break;

                            case Item.eItemClass.Acc:
                                m_equipItems[2].spriteName = item.m_icon.spriteName;
                                break;

                            default:
                                break;
                        }
                    }
                }
            }
        }

        m_uiGrid.Reposition(); //다시 자식들을 정렬하게함.
    }

    void LoadItemsManual(Item item)
    {
        m_name.text = item.m_name;
        m_manual.text = item.m_manual;

        switch (m_curItem.m_class)
        {
            case Item.eItemClass.Weapon:
                m_stat.text = "[FF0000]공격력    : " + item.m_stat.ToString();
                break;
            case Item.eItemClass.Armor:
                m_stat.text = "[FF0000]방어횟수  : " + item.m_stat.ToString() + "회";
                break;
            case Item.eItemClass.Acc:
                m_stat.text = "[FF0000]점수증가  : " + item.m_stat.ToString() + "배";
                break;
            default:
                break;
        }
    }

    void LoadEquipItemInfo(Item item)
    {
        switch (item.m_class)
        {
            case Item.eItemClass.Weapon:

                foreach (Item prevItem in m_itemDict.Values)
                {
                    if (prevItem.m_isEquipped && prevItem.m_class == Item.eItemClass.Weapon)
                    {
                        prevItem.UnEquipped();
                        break;
                    }
                }

                m_equipItems[0].spriteName = item.m_icon.spriteName;
                break;

            case Item.eItemClass.Armor:
                foreach (Item prevItem in m_itemDict.Values)
                {
                    if (prevItem.m_isEquipped && prevItem.m_class == Item.eItemClass.Armor)
                    {
                        prevItem.UnEquipped();
                        break;
                    }
                }

                m_equipItems[1].spriteName = item.m_icon.spriteName;
                break;

            case Item.eItemClass.Acc:
                foreach (Item prevItem in m_itemDict.Values)
                {
                    if (prevItem.m_isEquipped && prevItem.m_class == Item.eItemClass.Acc)
                    {
                        prevItem.UnEquipped();
                        break;
                    }
                }

                m_equipItems[2].spriteName = item.m_icon.spriteName;
                break;

            default:
                break;
        }
    }
    #endregion
}
