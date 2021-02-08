using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupOkCancel : MonoBehaviour
{
    #region Field
    [SerializeField]
    UILabel m_subjectLabel;
    [SerializeField]
    UILabel m_bodyLabel;
    [SerializeField]
    UILabel m_okBtnLabel;
    [SerializeField]
    UILabel m_canceBtnlLabel;
    
    //모든 트윈은 아래로 받아오기 가능.
    [SerializeField]
    UITweener m_popupTween;

    PopupButtonDelegate m_okBtnDel;
    PopupButtonDelegate m_cancelBtnDel;
    #endregion

    #region Public Methods
    //파라메터로 안주면 ok, cancel값으로 들어간다는 의미. 또한 생략할 수 있게끔 맨뒤에 위치시켜야 한다.
    public void SetPopup(string subject, string body, PopupButtonDelegate  okBtnDel, PopupButtonDelegate cancelBtnDel, string okBtnText = "OK", string cancelBtnText = "Cancel")
    {
        m_popupTween.ResetToBeginning();
        m_popupTween.PlayForward();

        m_subjectLabel.text = subject;
        m_bodyLabel.text = body;
        m_okBtnLabel.text = okBtnText;
        m_canceBtnlLabel.text = cancelBtnText;
        m_okBtnDel = okBtnDel;
        m_cancelBtnDel = cancelBtnDel;
    }

    public void OnPressOk()
    {
        if(m_okBtnDel != null)
        {
            m_okBtnDel();
        }
        else
        {
            PopupManager.Instance.ClosePopup();
        }
    }

    public void OnPressCancel()
    {
        if(m_cancelBtnDel != null)
        {
            m_cancelBtnDel();
        }

        PopupManager.Instance.ClosePopup();
    }
    #endregion
}
