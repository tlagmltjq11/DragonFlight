using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    #region Field
    public enum eItemClass
    {
        Weapon,
        Armor,
        Acc,
        Max
    }

    [SerializeField]
    UISprite m_outerGlow;
    [SerializeField]
    UISprite m_salesCompleted;
    LobbyMenu_Shop m_shop;
    LobbyMenu_Inventory m_inven;

    public LobbyMenu_Shop.eItemType m_type;
    public eItemClass m_class;
    public string m_manual;
    public string m_name;
    public int m_price;
    public int m_stat;

    public bool m_isSaled;
    public bool m_isEquipped;

    public UISprite m_icon;
    public Sprite m_sprite;
    #endregion

    #region Unity Methods
    void Awake()
    {
        m_outerGlow.enabled = false;
    }

    void Start()
    {
        ILobbyMenu parent = gameObject.GetComponentInParent<ILobbyMenu>();

        if (parent.gObj.name.Equals("Menu_Shop"))
        {
            m_shop = parent.gObj.GetComponent<LobbyMenu_Shop>();

            m_isSaled = PlayerDataManager.Instance.IsOwnedItem((int)m_type);
        }
        else if (parent.gObj.name.Equals("Menu_Inventory"))
        {
            m_inven = parent.gObj.GetComponent<LobbyMenu_Inventory>();

            m_isEquipped = PlayerDataManager.Instance.IsEquippedItem(m_class ,(int)m_type);
        }


        if (m_shop != null)
        {
            if (m_isSaled)
            {
                m_salesCompleted.enabled = true;
            }
            else
            {
                m_salesCompleted.enabled = false;
            }
        }
        else
        {
            if(m_isEquipped)
            {
                m_salesCompleted.enabled = true;
            }
            else
            {
                m_salesCompleted.enabled = false;
            }
        }
    }
    #endregion

    #region Public Methods
    public void OnSelect()
    {
        if(!IsSelected())
        {
            //중복재생을 막기 위함.
            SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick);
        }

        if(m_shop != null)
        {
            m_shop.OnSelectItem(this);
        }
        else
        {
            //인벤 셀렉트
            m_inven.OnSelectItem(this);
        }
    }

    public bool IsSelected()
    {
        //선택되어있으면 true를 반환할것임. 반대도 마찬가지
        return m_outerGlow.enabled;
    }

    public void Selected()
    {
        m_outerGlow.enabled = true;
    }

    public void Release()
    {
        m_outerGlow.enabled = false;
    }

    //이미 판매완료된 아이템을 의미.
    public void SalesCompleted()
    {
        m_salesCompleted.enabled = true;
        m_isSaled = true;
    }

    public void Equipped()
    {
        m_salesCompleted.enabled = true;
        m_isEquipped = true;
    }

    public void UnEquipped()
    {
        m_salesCompleted.enabled = false;
        m_isEquipped = false;
    }
    #endregion
}
