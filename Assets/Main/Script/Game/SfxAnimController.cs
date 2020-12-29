using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxAnimController : MonoBehaviour
{
    #region Field
    Animation m_animation;
    #endregion

    #region Unity Methods
    void Start()
    {
        m_animation = GetComponent<Animation>();
    }

    void Update()
    {
        if(!m_animation.isPlaying)
        {
            gameObject.SetActive(false);
        }
    }
    #endregion
}
