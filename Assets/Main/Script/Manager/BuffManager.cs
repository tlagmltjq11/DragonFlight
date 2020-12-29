using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : SingletonMonoBehaviour<BuffManager>
{
    #region Field
    public enum eBuffType
    {
        PowerShot,
        Invincible,
        Magnet,
        Shield,
        Max
    }

    public class Buff
    {
        public float m_lifeTime;
        public eBuffType m_buffType;
    }

    PlayerController m_player;
    Dictionary<eBuffType, Buff> m_buffList = new Dictionary<eBuffType, Buff>();
    float[] durations = new float[] { 10f, 2.55f, 5f, 3f };
    CameraShake m_camShake;
    #endregion

    #region Unity Methods
    protected override void OnStart()
    {
        m_camShake = Camera.main.GetComponent<CameraShake>();
        m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void Update()
    {
        //딕셔너리는 foreach가 훨씬 편함. for문으로 하면 아래처럼됨.
        for (int i = 0; i < m_buffList.Count; i++)
        {
            var data = m_buffList.GetEnumerator();
            data.MoveNext();
            var valuePair = data.Current;
            var buff = m_buffList[valuePair.Key];
            buff.m_lifeTime -= Time.deltaTime;

            if (buff.m_lifeTime <= 0)
            {
                switch (buff.m_buffType)
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
                m_buffList.Remove(buff.m_buffType);
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

        
        if (!m_buffList.ContainsKey(buff))
        {
            m_buffList.Add(buff, new Buff() { m_lifeTime = durations[(int)buff], m_buffType = buff });
            switch(buff)
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
            var findBuff = m_buffList[buff];
            //똑같은 버프는 중복이아니라 시간리셋으로 구현하는게 맞음.
            findBuff.m_lifeTime = durations[(int)buff];
        }
    }
    #endregion
}
