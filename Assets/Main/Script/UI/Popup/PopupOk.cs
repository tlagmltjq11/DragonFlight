using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupOk : MonoBehaviour
{
    [SerializeField]
    UILabel m_subjectLabel;
    [SerializeField]
    UILabel m_bodyLabel;
    [SerializeField]
    UILabel m_okBtnLabel;

    //모든 트윈은 아래로 받아오기 가능.
    [SerializeField]
    UITweener m_popupTween;

    PopupButtonDelegate m_okBtnDel;


    //파라메터로 안주면 ok, cancel값으로 들어간다는 의미. 또한 생략할 수 있게끔 맨뒤에 위치시켜야 한다.
    public void SetPopup(string subject, string body, PopupButtonDelegate okBtnDel, string okBtnText = "OK")
    {
        m_popupTween.ResetToBeginning();
        m_popupTween.PlayForward();

        m_subjectLabel.text = subject;
        m_bodyLabel.text = body;
        m_okBtnLabel.text = okBtnText;
        m_okBtnDel = okBtnDel;
    }

    public void OnPressOk()
    {

        m_okBtnDel();
        PopupManager.Instance.ClosePopup();

        /*
        if (m_okBtnDel != null)
        {
            m_okBtnDel();
        }
        else
        {
            PopupManager.Instance.ClosePopup();
        }
        */
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
