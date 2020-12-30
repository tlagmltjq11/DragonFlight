using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteoController : MonoBehaviour
{
    #region Field
    [SerializeField]
    GameObject m_meteoTrans;
    [SerializeField]
    GameObject m_meteoRotate;
    [SerializeField]
    GameObject m_warnLine;
    [SerializeField]
    GameObject m_warnMark;
    [SerializeField]
    PlayerController m_player;
    [SerializeField]
    ParticleSystem m_smoke;
    [SerializeField]
    float m_speed = 10f;
    float m_followTime;
    int m_space;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        m_meteoTrans.transform.localPosition = new Vector3(0f, 6f, 0f);
        m_warnLine.SetActive(true);
        m_warnMark.SetActive(true);
        m_followTime = 3f;
        m_speed = 10f;

        StartCoroutine("Alert");
        m_smoke.Play();
    }

    void Update()
    {
        if (m_followTime >= 0f)
        {
            m_followTime -= Time.deltaTime;
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, new Vector3(m_player.gameObject.transform.position.x, 0f, 0f), Time.deltaTime / 5f);
        }
        else
        {
            m_meteoTrans.transform.position += Vector3.down * m_speed * Time.deltaTime;
            m_meteoRotate.transform.Rotate(Vector3.forward * 3f, Space.Self);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("BgColliderBottom") || collision.tag.Equals("Invincible") || collision.tag.Equals("ShockWave") || collision.tag.Equals("Player"))
        {
            m_smoke.Stop();
            SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.MeteoExplosion);
            MeteoManager.Instance.SetSpace(m_space);
            MeteoManager.Instance.RemoveMeteo(this);
        }
    }
    #endregion

    #region Public Methods
    public void setMeteo(Vector2 transform, int space)
    {
        gameObject.transform.position = transform;
        m_space = space;
    }

    public void InitMeteo(PlayerController player)
    {
        m_player = player;
    }
    #endregion

    #region Coroutine
    IEnumerator Alert()
    {
        bool temp = false;

        while (m_followTime >= 0.5f)
        {
            if (!temp)
            {
                m_warnMark.SetActive(false);

                temp = true;
            }
            else
            {
                m_warnMark.SetActive(true);

                temp = false;
            }
            yield return new WaitForSeconds(m_followTime / 5f);
        }

        m_warnMark.SetActive(true);
        yield return new WaitForSeconds(0.45f);

        SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.MeteoAlert);
        m_warnLine.SetActive(false);
        m_warnMark.SetActive(false);
    }
    #endregion
}
