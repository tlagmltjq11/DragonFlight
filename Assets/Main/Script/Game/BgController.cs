using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.SceneManagement;

public class BgController : MonoBehaviour
{
    #region Field
    [SerializeField]
    float m_speed = 0.5f;
    [SerializeField]
    Sprite[] m_bgs;
    float m_speedScale = 1f;
    SpriteRenderer m_bgRenderer;
    #endregion

    #region Public Methods
    void Start()
    {
        m_bgRenderer = GetComponent<SpriteRenderer>();

        int rand = Random.Range(0, m_bgs.Length);
        m_bgRenderer.sprite = m_bgs[rand];

        if (SoundManager.Instance != null)
        {
            string scene = SceneManager.GetActiveScene().name;
            if (scene.Equals("Title"))
            {
                SoundManager.Instance.PlayBGM(SoundManager.eAudioBGMClip.BGM01, 1f);
            }
            else if(scene.Equals("Lobby"))
            {
                SoundManager.Instance.PlayBGM(SoundManager.eAudioBGMClip.Lobby, 0.4f);
            }
            else
            {
                SoundManager.Instance.PlayBGM((SoundManager.eAudioBGMClip)Random.Range((int)SoundManager.eAudioBGMClip.BGM01, (int)SoundManager.eAudioBGMClip.Lobby), 1f);
            }
        }
    }

    void Update()
    {
        //scale값을 키워서 배경속도를 컨트롤함.
        m_bgRenderer.material.mainTextureOffset += Vector2.up * m_speed * m_speedScale * Time.deltaTime;

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.SetFlightScore((int)(m_bgRenderer.material.mainTextureOffset.y * 1000f));
        }
    }
    #endregion

    #region Public Methods
    public void SetSpeedScale(float scale)
    {
        m_speedScale = scale;
    }
    #endregion
}
