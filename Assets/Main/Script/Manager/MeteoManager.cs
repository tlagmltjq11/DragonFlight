using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteoManager : SingletonMonoBehaviour<MeteoManager>
{
    #region Field
    [SerializeField]
    GameObject m_meteoPrefab;
    GameObjectPool<MeteoController> m_meteoPool;

    //가장왼쪽 메테오 시작좌표
    Vector2 m_startPos = new Vector2(-2.1f, 0f);
    float m_posXGap = 1.4f; //메테오간 좌표 갭
    PlayerController m_player;
    float m_timer;
    bool[] m_meteoSpace;
    #endregion

    #region Unity Methods
    void Start()
    {
        m_timer = 0;
        m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        m_meteoSpace = new bool[4];

        m_meteoPool = new GameObjectPool<MeteoController>(10, () =>
        {
            var obj = Instantiate(m_meteoPrefab) as GameObject;
            obj.transform.SetParent(transform);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;
            var mon = obj.GetComponent<MeteoController>();
            mon.InitMeteo(m_player);

            return mon;
        });

        InvokeRepeating("CreateMeteo", 3f, 8f);
    }

    void Update()
    {
        m_timer += Time.deltaTime;
    }
    #endregion

    #region Public Methods
    public void RemoveMeteo(MeteoController mon)
    {
        mon.gameObject.SetActive(false);
        m_meteoPool.Set(mon);
    }

    public void StopCreateMeteo()
    {
        CancelInvoke("CreateMeteo");
    }

    public void CreateMeteo()
    {
        int nums = Random.Range(1, 4);

        for(int i=0; i<nums; i++)
        {
            int space = Random.Range(0, 4);

            if(m_meteoSpace[space] == true)
            {
                do
                {
                    space = Random.Range(0, 4);
                } while (m_meteoSpace[space] == true);
            }

            m_meteoSpace[space] = true;

            var meteo = m_meteoPool.Get();
            meteo.setMeteo(m_startPos + new Vector2(m_posXGap * space, 0f), space);
            meteo.gameObject.SetActive(true);
        }
    }

    public void SetSpace(int space)
    {
        m_meteoSpace[space] = false;
    }
    #endregion
}
