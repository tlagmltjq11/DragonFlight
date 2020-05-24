using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : SingletonMonoBehaviour<ItemManager>
{

    public enum eItemType
    {
        Coin,
        Gem_Red,
        Gem_Green,
        Gem_Blue,
        Invincible,
        Magnet,
        Max
    }

    [SerializeField]
    Sprite[] m_itemSprites;
    [SerializeField]
    GameObject m_itemPrefab;
    GameObjectPool<GameItem> m_gameItemPool;
    List<GameItem> m_itemList = new List<GameItem>();

    [SerializeField]
    PlayerController m_player;

    int[] m_itemTable = new int[(int)eItemType.Max] {85, 3, 2, 1, 4, 5}; //각 아이템들의 생성확률을 100을 기준으로 정해줌 

    public Sprite GetItemSprite(eItemType type)
    {
        //로드순서가 같아야 가능함.
        return m_itemSprites[(int)type];
    }

    public void CreateItem(Vector3 pos, Vector3 dir)
    {
        int type = 0;
        do
        {
            type = Util.GetPriorities(m_itemTable);//(eItemType)Random.Range((int)eItemType.Coin, (int)eItemType.Max);       

        } while (GameManager.Instance.GetState() == GameManager.eGameState.Invincible && (eItemType)type == eItemType.Invincible);

        var item = m_gameItemPool.Get();
        item.SetItem((eItemType)type, pos, dir);
        m_itemList.Add(item);
    }
    
    public void RemoveItem(GameItem item)
    {
        item.gameObject.SetActive(false);
        if(m_itemList.Remove(item))
            m_gameItemPool.Set(item);
    }

    // Start is called before the first frame update
    protected override void OnStart()
    {
        //동적로드 여러개 동시에 하는 방식.
        m_itemSprites = Resources.LoadAll<Sprite>("Item/");

        m_gameItemPool = new GameObjectPool<GameItem>(10, () =>
        {
            var obj = Instantiate(m_itemPrefab) as GameObject;
            obj.transform.SetParent(transform);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;

            var item = obj.GetComponent<GameItem>();

            return item;
        });
    }

    // Update is called once per frame
    void Update()
    {
        for(int i=0; i<m_itemList.Count; i++)
        {
            if(m_itemList[i].IsMagnet)
            {
                //플레이어와의 위치를 빼서 방향을 알아내고 노말라이즈한다. 그 후 속도를 곱함.
                m_itemList[i].transform.position += (m_player.transform.position - m_itemList[i].transform.position).normalized * 10f * Time.deltaTime;
            }
        }
    }
}
