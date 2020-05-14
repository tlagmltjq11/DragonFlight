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

    PlayerController m_player;
    public enum eMonsterType
    {
        None = -1,
        White,
        Yellow,
        Pink,
        Bomb,
        Max
    }

    [SerializeField]
    GameObject m_monsterPrefab;
    GameObjectPool<MonsterController> m_monsterPool;
    
    //가장왼쪽위 몬스터의 시작좌표
    Vector2 m_startPos = new Vector2(-2.26f, 6f);
    float m_posXGap = 1.12f; //몬스터간 좌표 갭

    List<MonsterController> m_monsterList = new List<MonsterController>(); //Active되어있는 몬스터들만 들어있는 리스트
    int m_lineNumber;

    public void CreateMonsters()
    {
        bool isBomb = false;
        for (int i = 0; i < 5; i++)
        {
            var type = (eMonsterType)UnityEngine.Random.Range((int)eMonsterType.White, (int)eMonsterType.Max);

            if (type == eMonsterType.Bomb && !isBomb)
                isBomb = true;
            else if (type == eMonsterType.Bomb && isBomb)
            {
                do
                {
                    type = (eMonsterType)UnityEngine.Random.Range((int)eMonsterType.White, (int)eMonsterType.Max);
                } while (type == eMonsterType.Bomb);
            }

            var mon = m_monsterPool.Get();
            mon.SetMonster(type, m_lineNumber);
            mon.transform.position = Vector3.right * (m_startPos.x + i * m_posXGap) + Vector3.up * m_startPos.y;
            mon.gameObject.SetActive(true);

            m_monsterList.Add(mon);
        }
        m_lineNumber++;
    }

    public void RemoveMonster(MonsterController mon)
    {
        mon.gameObject.SetActive(false);
        if (m_monsterList.Remove(mon)) //이 검사를 하지 않게되면, 두 몬스터사이에 총알을 쏴서 두 몬스터가 동시에 죽게될때 정상적으로 한놈이 리무브되지않았는데 밑에서 set을 통해 풀에 다시 넣어주므로 복제가 일어날 수 있음.
        {
            m_monsterPool.Set(mon);
        }
    }

    public void RemoveMonsterAll(int lineNum)
    {
        for (int i = 0; i < m_monsterList.Count; i++)
        {
            if (m_monsterList[i].LineNum == lineNum)
            {
                m_monsterList[i].m_isAlive = false;
                m_monsterList[i].SetDie();
                m_monsterList[i].gameObject.SetActive(false);
                m_monsterPool.Set(m_monsterList[i]);
            }
        }

        //만약 몬스터컨트롤러를 위 루프안에서 지우게되면 Count값이 계속해서 바뀌므로 에러가 났을것이다. 고로 체크해두고 루프를 나와서 한번에 지우면 된다.★★★★★★★★★★★★★★★
        m_monsterList.RemoveAll(mon => !mon.m_isAlive);
    }

    public Sprite[] GetMonsterParts(eMonsterType type)
    {
        //해당 타입의 스프라이트들을 넘겨줌.
        return m_monsterParts[(int)type].m_parts;
    }

    protected override void OnStart()
    {
        m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        m_monsterPool = new GameObjectPool<MonsterController>(20, () =>
        {
            var obj = Instantiate(m_monsterPrefab) as GameObject;
            //여기서 몬스터를 생성하고 액티브를 끄지않는다. 몬스터컨트롤러에서 액티브를 꺼야함.
            obj.transform.SetParent(transform); //매니저의 자식으로 넣어서 하이어라이키 정리.
            obj.transform.localPosition = Vector3.zero; //초기화 한번 해주는 것들임.
            obj.transform.localRotation = Quaternion.identity; //초기화 한번 해주는 것들임.
            obj.transform.localScale = Vector3.one;
            var mon = obj.GetComponent<MonsterController>();
            mon.InitMonster(m_player);

            return mon;
        });

        InvokeRepeating("CreateMonsters", 3f, 2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
