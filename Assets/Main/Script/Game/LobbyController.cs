using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyController : MonoBehaviour
{
    [SerializeField]
    GameObject m_menuBtnObj;
    [SerializeField]
    GameObject m_menuObj;
    Transform[] m_menu;
    UIButton[] m_menuBtns;

    public void OpenMenu(UIButton button)
    {
        gameObject.SetActive(false);
        var index = int.Parse(button.name.Substring(0, 2));
        m_menu[index - 1].gameObject.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_menuBtns = m_menuBtnObj.GetComponentsInChildren<UIButton>();
        var results = m_menuObj.GetComponentsInChildren<Transform>();
        m_menu = new Transform[results.Length - 1];
        for(int i=0; i<m_menu.Length; i++)
        {
            //부모까지 딸려온거 빼주는것
            m_menu[i] = results[i + 1];
            m_menu[i].gameObject.SetActive(false);
        }

        for(int i=0; i<m_menuBtns.Length; i++)
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
