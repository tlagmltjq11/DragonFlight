using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class BgController : MonoBehaviour
{
    #region Field
    [SerializeField]
    float m_speed = 0.5f;
    float m_speedScale = 1f;
    SpriteRenderer m_bgRenderer;
    #endregion

    #region Public Methods
    void Start()
    {
        m_bgRenderer = GetComponent<SpriteRenderer>();

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayBGM(SoundManager.eAudioBGMClip.BGM01);
        }
    }

    void Update()
    {
        //scale값을 키워서 배경속도를 컨트롤함.
        m_bgRenderer.material.mainTextureOffset += Vector2.up * m_speed * m_speedScale * Time.deltaTime;
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.SetFlightScore((int)(m_bgRenderer.material.mainTextureOffset.y * 1000f));
    }
    #endregion

    #region Public Methods
    public void SetSpeedScale(float scale)
    {
        m_speedScale = scale;
    }
    #endregion
}
