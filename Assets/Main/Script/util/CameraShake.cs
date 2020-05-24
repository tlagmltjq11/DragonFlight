using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField]
    float m_duration;
    [SerializeField]
    float m_power;
    float m_time;
    bool m_isStart;

    Vector3 m_orgPos;

    public void ShakeCamera()
    {
        m_isStart = true;
        m_time = 0f;

    }

    public void Stop()
    {
        m_isStart = false;
        m_time = 0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_orgPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isStart)
        {
            m_time += Time.deltaTime;

            if (m_time > m_duration)
            {
                m_time = 0;
                m_isStart = false;
                transform.position = m_orgPos;
                return;
            }
            var dir = Random.insideUnitCircle;
            dir = dir.normalized * m_power * Time.deltaTime;
            transform.position = new Vector3(dir.x, dir.y, m_orgPos.z);
        }
    }
}
