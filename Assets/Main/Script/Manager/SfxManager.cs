using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxManager : SingletonMonoBehaviour<SfxManager>
{
    public enum eSfxType
    {
        Hit,
        Dust,
        Max
    }

    //이펙트는 게임에 여러개가 존재할테니 풀을 여러개 만들어야 한다.
    [SerializeField]
    GameObject[] m_sfxPrefab;
    //문자열과 풀을 딕셔너리로 선언함.
    Dictionary<eSfxType, GameObjectPool<SfxController>> m_sfxPool = new Dictionary<eSfxType, GameObjectPool<SfxController>>();

    public void CreateSfx(eSfxType type, Vector3 pos)
    {
        var pool = m_sfxPool[type];
        var sfx = pool.Get();
        sfx.gameObject.SetActive(true);
        sfx.transform.position = pos;
        sfx.PlaySfx();
    }

    public void RemoveSfx(SfxController sfx)
    {
        sfx.gameObject.SetActive(false);
        //해당 풀로 반환
        var pool = m_sfxPool[sfx.m_type];
        pool.Set(sfx);
    }


    protected override void OnStart()
    {
        foreach(GameObject prefab in m_sfxPrefab)
        {
            GameObjectPool<SfxController> pool = new GameObjectPool<SfxController>(5, () =>
            {
                //이 구문을 for문으로 구현하면 안되는 이유는 Pool에서 만들어둔 10개를 다써서, 새로 함수를 호출할때 지역변수인 i값이 없기 때문이다.
                //foreach는 주소값을 갖고있음.
                //var obj = Instantiate(m_sfxPrefab[i]) as GameObject;
                var obj = Instantiate(prefab) as GameObject;
                obj.transform.SetParent(transform);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localScale = Vector3.one;
                obj.transform.localRotation = Quaternion.identity;

                var sfx = obj.GetComponent<SfxController>();
                //프리팹 이름에서 Sfx_ 다음 부터의 스트링을 가져옴
                var name = prefab.name.Substring(4);
                for(int i=0; i<(int)eSfxType.Max; i++)
                {
                    if(((eSfxType)i).ToString().Equals(name))
                    {
                        sfx.InitSfx((eSfxType)i);
                        
                    }
                }
                return sfx;
            });

            m_sfxPool.Add(pool.Peek().m_type, pool);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
