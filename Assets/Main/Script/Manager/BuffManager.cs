using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BuffManager : SingletonMonoBehaviour<BuffManager>
{
    #region Field
    public enum eBuffType //버프타입
    {
        PowerShot,
        Invincible,
        Magnet,
        Shield,
        Max
    }

    Dictionary<eBuffType, float> m_buffList = new Dictionary<eBuffType, float>(); //각 버프마다 지속시간 체크
    float[] durations = new float[] { 10f, 2.55f, 5f, 3f };//각 버프별 지속시간
    CameraShake m_camShake;

    PlayerController m_player;
    #endregion

    #region Unity Methods
    protected override void OnStart()
    {
        m_camShake = Camera.main.GetComponent<CameraShake>();
        m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        for(int i=0; i< (int)eBuffType.Max; i++)
        {
            m_buffList.Add((eBuffType)i, 0);
        }
    }

    void Update()
    {
        foreach (var kvp in m_buffList.ToList())
        {
            if (kvp.Value != 0)
            {
                float newLen = kvp.Value - Time.deltaTime;

                if (newLen > 0f)
                {
                    m_buffList[kvp.Key] = newLen;
                }
                else
                {
                    m_buffList[kvp.Key] = 0f;

                    switch (kvp.Key)
                    {
                        case eBuffType.Magnet:
                            m_player.SetMagnet(false);
                            break;
                        case eBuffType.Invincible:
                            GameManager.Instance.SetState(GameManager.eGameState.Normal);
                            m_player.SetShockWave(true);
                            break;
                        case eBuffType.Shield:
                            m_player.ShieldOff();
                            break;
                    }
                }
            }
        }
    }
    #endregion

    #region Public Methods
    public void SetBuff(eBuffType buff)
    {
        if(buff == eBuffType.Invincible)
        {
            m_camShake.ShakeCamera();
        }

        if (m_buffList[buff] == 0f)
        {
            m_buffList[buff] = durations[(int)buff]; //버프시간 갱신으로 추가해줌.
            switch (buff)
            {
                case eBuffType.Magnet:
                    m_player.SetMagnet(true);
                    break;
                case eBuffType.Invincible:
                    GameManager.Instance.SetState(GameManager.eGameState.Invincible);
                    break;
                case eBuffType.Shield:
                    m_player.ShieldOn();
                    break;
            }
        }
        else
        {
            m_buffList[buff] = durations[(int)buff]; //시간리셋
        }
    }
    #endregion
}
