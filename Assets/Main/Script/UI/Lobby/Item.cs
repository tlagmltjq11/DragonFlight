using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    #region Field
    [SerializeField]
    UISprite m_outerGlow;
    [SerializeField]
    UISprite m_salesCompleted;
    LobbyMenu_Shop m_shop;

    public LobbyMenu_Shop.eItemType m_type;
    public string m_manual;
    public string m_name;
    public int m_price;
    public int m_damage;

    public bool m_isSaled;
    #endregion

    #region Unity Methods
    void Awake()
    {
        m_outerGlow.enabled = false;
        m_shop = GameObject.FindWithTag("Shop").GetComponent<LobbyMenu_Shop>();

        PlayerPrefs.DeleteAll();
        m_isSaled = PlayerPrefs.GetInt("IsOwned" + m_type.ToString()) == 1 ? true : false;
    }

    void Start()
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
    #endregion

    #region Public Methods
    public void OnSelect()
    {
        m_shop.OnSelectItem(this);
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
    #endregion
}
