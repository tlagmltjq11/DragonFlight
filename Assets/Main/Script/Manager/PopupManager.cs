using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public delegate void PopupButtonDelegate();

public class PopupManager : DonDestroy<PopupManager>
{
    #region Field
    [SerializeField]
    GameObject m_popupOkCancelPrefab; //동적로드 방식을 사용할 것임.
    [SerializeField]
    GameObject m_popupOkPrefab;
    [SerializeField]
    GameObject m_popupOptionPrefab;

    int m_popupDepth = 1000;
    int m_depthGap = 10;

    List<GameObject> m_popupList = new List<GameObject>();
    #endregion

    #region Unity Methods
    protected override void OnStart()
    {
        m_popupOkCancelPrefab = Resources.Load("Popup/PopupOkCancel") as GameObject;
        m_popupOkPrefab = Resources.Load("Popup/PopupOk") as GameObject;
        m_popupOptionPrefab = Resources.Load("Popup/PopupOption") as GameObject;
        base.OnStart();
    }
    #endregion

    #region Public Methods
    public void OpenPopupOkCancel(string subject, string body, PopupButtonDelegate okBtnDel, PopupButtonDelegate cancelBtnDel, string okBtnStr = "OK", string cancelBtnStr = "Cancel")
    {
        var obj = Instantiate(m_popupOkCancelPrefab) as GameObject;
        
        //위에서부터 순서대로 찾아오기 때문에 가장먼저 온 패널이 가장 상위 패널임.
        var panels = obj.GetComponentsInChildren<UIPanel>();
        
        for(int i=0; i<panels.Length; i++)
        {
            //시작점을 현재 팝업의 갯수 * 갭을 해주며 + i 를 해주면서 내부 패널들의 뎁스를 1씩 늘려 맞춰준다.
            panels[i].depth = m_popupDepth + (m_popupList.Count * m_depthGap + i);
        }

        //모든 팝업들을 팝업매니저의 자식으로 만들어 관리할것임.
        obj.transform.SetParent(transform);

        //초기화
        obj.transform.localPosition = Vector3.zero;

        var popup = obj.GetComponent<PopupOkCancel>();

        //만들어진 팝업에 넘겨줌.
        popup.SetPopup(subject, body, okBtnDel, cancelBtnDel, okBtnStr, cancelBtnStr);

        m_popupList.Add(obj);
    }

    public void OpenPopupOk(string subject, string body, PopupButtonDelegate okBtnDel, string okBtnStr = "OK")
    {
        var obj = Instantiate(m_popupOkPrefab) as GameObject;

        //위에서부터 순서대로 찾아오기 때문에 가장먼저 온 패널이 가장 상위 패널임.
        var panels = obj.GetComponentsInChildren<UIPanel>();

        for (int i = 0; i < panels.Length; i++)
        {
            //시작점을 현재 팝업의 갯수 * 갭을 해주며 + i 를 해주면서 내부 패널들의 뎁스를 1씩 늘려 맞춰준다.
            panels[i].depth = m_popupDepth + (m_popupList.Count * m_depthGap + i);
        }

        //모든 팝업들을 팝업매니저의 자식으로 만들어 관리할것임.
        obj.transform.SetParent(transform);

        //초기화
        obj.transform.localPosition = Vector3.zero;

        var popup = obj.GetComponent<PopupOk>();

        //만들어진 팝업에 넘겨줌.
        popup.SetPopup(subject, body, okBtnDel, okBtnStr);

        m_popupList.Add(obj);
    }

    public void OpenPopupOption(PopupButtonDelegate okBtnDel)
    {
        var obj = Instantiate(m_popupOptionPrefab) as GameObject;

        //위에서부터 순서대로 찾아오기 때문에 가장먼저 온 패널이 가장 상위 패널임.
        var panels = obj.GetComponentsInChildren<UIPanel>();

        for (int i = 0; i < panels.Length; i++)
        {
            //시작점을 현재 팝업의 갯수 * 갭을 해주며 + i 를 해주면서 내부 패널들의 뎁스를 1씩 늘려 맞춰준다.
            panels[i].depth = m_popupDepth + (m_popupList.Count * m_depthGap + i);
        }

        //모든 팝업들을 팝업매니저의 자식으로 만들어 관리할것임.
        obj.transform.SetParent(transform);

        //초기화
        obj.transform.localPosition = Vector3.zero;

        var popup = obj.GetComponent<PopupOption>();
        popup.SetPopup(okBtnDel);
        m_popupList.Add(obj);
    }

    public void ClosePopup()
    {
        if(m_popupList.Count > 0)
        {
            Destroy(m_popupList[m_popupList.Count - 1].gameObject);
            //마지막에 생성된 팝업부터 없어질거라는 것을 아니까.
            m_popupList.Remove(m_popupList[m_popupList.Count-1]);
        }
    }

    public bool CanClosePopup(KeyCode key)
    {
        if(key == KeyCode.Escape)
        {
            if(m_popupList.Count > 0)
            {
                ClosePopup();
                return true;
            }
        }
        return false;
    }
    #endregion
}
