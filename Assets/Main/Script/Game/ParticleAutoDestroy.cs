using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAutoDestroy : MonoBehaviour
{

    //모든 파티클시스템을 가져와야함.
    ParticleSystem[] m_particles;
    SfxController m_sfxController;
    // Start is called before the first frame update
    void Start()
    {
        m_sfxController = GetComponent<SfxController>();
        m_particles = GetComponentsInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        bool isPlay = false;

        for(int i=0; i<m_particles.Length; i++)
        {
            if(m_particles[i].isPlaying)
            {
                //하나라도 재생중이면 더 볼것이 없음.
                isPlay = true;
                break;
            }
        }

        //모든 파티클시스템의 재생이 끝났다면
        if(!isPlay)
        {
            SfxManager.Instance.RemoveSfx(m_sfxController);
        }
    }
}
