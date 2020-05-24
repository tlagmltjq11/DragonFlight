using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxAnimController : MonoBehaviour
{
    Animation m_animation;

    // Start is called before the first frame update
    void Start()
    {
        m_animation = GetComponent<Animation>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!m_animation.isPlaying)
        {
            gameObject.SetActive(false);
        }
    }
}
