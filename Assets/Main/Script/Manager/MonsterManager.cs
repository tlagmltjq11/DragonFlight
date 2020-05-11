using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : SingletonMonoBehaviour<MonsterManager>
{
    [Serializable]
    public class MonsterPartsSprite
    {
        public Sprite[] m_parts;
    }

    [SerializeField]
    MonsterPartsSprite[] m_monsterParts;

    public enum eMonsterType
    {
        None = -1,
        White,
        Yellow,
        Pink,
        Max
    }

    [SerializeField]
    GameObject m_monsterPrefab;
    GameObjectPool<MonsterController> m_monsterPool;
    
    //가장왼쪽위 몬스터의 시작좌표
    Vector2 m_startPos = new Vector2(-2.26f, 6f);
    float m_posXGap = 1.12f; //몬스터간 좌표 갭

    public void CreateMonsters()
    {
        for(int i=0; i<5; i++)
        {
            var type = (eMonsterType)UnityEngine.Random.Range((int)eMonsterType.White, (int)eMonsterType.Max);

            var mon = m_monsterPool.Get();
            mon.SetMonster(type);

            mon.transform.position = Vector3.right * (m_startPos.x + i * m_posXGap) + Vector3.up * m_startPos.y;
            mon.gameObject.SetActive(true);
        }     
    }

    public void RemoveMonster(MonsterController mon)
    {
        mon.gameObject.SetActive(false);
        m_monsterPool.Set(mon);
    }

    public Sprite[] GetMonsterParts(eMonsterType type)
    {
        //해당 타입의 스프라이트들을 넘겨줌.
        return m_monsterParts[(int)type].m_parts;
    }

    protected override void OnStart()
    {
        m_monsterPool = new GameObjectPool<MonsterController>(20, () =>
        {
            var obj = Instantiate(m_monsterPrefab) as GameObject;
            //여기서 몬스터를 생성하고 액티브를 끄지않는다. 몬스터컨트롤러에서 액티브를 꺼야함.
            obj.transform.SetParent(transform); //매니저의 자식으로 넣어서 하이어라이키 정리.
            obj.transform.localPosition = Vector3.zero; //초기화 한번 해주는 것들임.
            obj.transform.localRotation = Quaternion.identity; //초기화 한번 해주는 것들임.
            var mon = obj.GetComponent<MonsterController>();

            return mon;
        });

        InvokeRepeating("CreateMonsters", 3f, 2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
