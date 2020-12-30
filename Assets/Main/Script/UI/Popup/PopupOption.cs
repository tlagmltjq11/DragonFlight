using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupOption : MonoBehaviour
{
    #region Field
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
    #endregion

    #region Unity Methods
    void Start()
    {
        var onBGM = PlayerPrefs.GetInt("OPTION_BGM", 1) == 1 ? false : true;
        var onSFX = PlayerPrefs.GetInt("OPTION_SFX", 1) == 1 ? false : true;
        m_curSpeed = PlayerPrefs.GetInt("OPTION_SPEED", 1);

        m_speedIcon.spriteName = string.Format("option_{0}x", m_curSpeed);

        m_bgmToggle.value = !onBGM;
        m_sfxToggle.value = !onSFX;
    }
    #endregion

    #region Public Methods
    //파라메터로 안주면 ok, cancel값으로 들어간다는 의미. 또한 생략할 수 있게끔 맨뒤에 위치시켜야 한다.
    public void SetPopup(PopupButtonDelegate okBtnDel)
    {
        m_popupTween.ResetToBeginning();
        m_popupTween.PlayForward();
        m_okBtnDel = okBtnDel;
    }

    public void SetBGM()
    {
        SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick);

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
        SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.ButtonClick);

        m_curSpeed++;
        if(m_curSpeed > 3)
        {
            m_curSpeed = 1;
        }

        m_speedIcon.spriteName = string.Format("option_{0}x", m_curSpeed);

        PlayerPrefs.SetInt("OPTION_SPEED", m_curSpeed);
        PlayerPrefs.Save();
    }

    public void OnPressOk()
    {
        m_okBtnDel();
        PopupManager.Instance.ClosePopup();
    }
    #endregion
}
