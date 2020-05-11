using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxController : MonoBehaviour
{
    public SfxManager.eSfxType m_type;
    ParticleSystem[] m_particles;
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

    // Start is called before the first frame update
    void Awake()
    {
        m_particles = GetComponentsInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
