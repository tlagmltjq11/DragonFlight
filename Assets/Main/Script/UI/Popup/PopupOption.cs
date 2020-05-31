using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupOption : MonoBehaviour
{
    //모든 트윈은 아래로 받아오기 가능.
    [SerializeField]
    UITweener m_popupTween;
    PopupButtonDelegate m_okBtnDel;

    [SerializeField]
    UIToggle m_bgmToggle;
    [SerializeField]
    UIToggle m_sfxToggle;

    [SerializeField]
    UISprite m_speedIcon;

    int m_curSpeed;

    //파라메터로 안주면 ok, cancel값으로 들어간다는 의미. 또한 생략할 수 있게끔 맨뒤에 위치시켜야 한다.
    public void SetPopup(PopupButtonDelegate okBtnDel)
    {
        m_popupTween.ResetToBeginning();
        m_popupTween.PlayForward();
        m_okBtnDel = okBtnDel;
    }

    public void SetBGM()
    {
        var isOn = m_bgmToggle.value;
        SoundManager.Instance.MuteBGM(!isOn);

        if(isOn)
        {
            SoundManager.Instance.PlayBGM();
        }

        PlayerPrefs.SetInt("OPTION_BGM", isOn == true ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetSFX()
    {
        var isOn = m_sfxToggle.value;
        SoundManager.Instance.MuteSFX(!isOn);

        if (isOn)
        {
            SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.GetItem);
        }

        PlayerPrefs.SetInt("OPTION_SFX", isOn == true ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetSpeed()
    {
        m_curSpeed++;
        if(m_curSpeed > 2)
        {
            m_curSpeed = 0;
        }

        m_speedIcon.spriteName = string.Format("option_{0}x", m_curSpeed + 1);

        PlayerPrefs.SetInt("OPTION_SPEED", m_curSpeed);
        PlayerPrefs.Save();
    }

    public void OnPressOk()
    {
        if (m_okBtnDel != null)
        {
            m_okBtnDel();
        }
        else
        {
            PopupManager.Instance.ClosePopup();
        }
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
