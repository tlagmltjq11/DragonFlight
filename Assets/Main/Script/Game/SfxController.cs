using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxController : MonoBehaviour
{
    #region Field
    public SfxManager.eSfxType m_type;
    ParticleSystem[] m_particles;
    #endregion

    #region Unity Methods
    void Awake()
    {
        m_particles = GetComponentsInChildren<ParticleSystem>();
    }
    #endregion

    #region Public Methods
    public void InitSfx(SfxManager.eSfxType type)
    {
        m_type = type;
        gameObject.SetActive(false);
    }

    public void PlaySfx()
    {
        for(int i=0; i<m_particles.Length; i++)
        {
            m_particles[i].Play();
        }
    }
    #endregion
}
