using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgController : MonoBehaviour
{
    [SerializeField]
    float m_speed = 0.5f;

    SpriteRenderer m_bgRenderer;
    // Start is called before the first frame update
    void Start()
    {
        m_bgRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        m_bgRenderer.material.mainTextureOffset += Vector2.up * m_speed * Time.deltaTime; 
    }

    public void Hello()
    {
        Debug.Log("hh");
    }
}
