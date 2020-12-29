using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : SingletonMonoBehaviour<ItemManager>
{
    #region Field
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
    [SerializeField]
    PlayerController m_player;
    GameObjectPool<GameItem> m_gameItemPool;
    List<GameItem> m_itemList = new List<GameItem>();

    int[] m_itemTable = new int[(int)eItemType.Max] {85, 3, 2, 1, 4, 5}; //각 아이템들의 생성확률을 100을 기준으로 정해줌 
    #endregion

    #region Public Methods
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

        } while (GameManager.Instance.GetState() == GameManager.eGameState.Invincible && (eItemType)type == eItemType.Invincible); //Invincible 상태에서도 Invincible이 등장하는것은 밸런스 붕괴.

        var item = m_gameItemPool.Get();
        item.SetItem((eItemType)type, pos, dir);
        m_itemList.Add(item);
    }
    
    public void RemoveItem(GameItem item)
    {
        item.gameObject.SetActive(false);

        if (m_itemList.Remove(item))
        {
            //반환
            m_gameItemPool.Set(item);
        }
    }
    #endregion

    #region Unity Methods
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

    void Update()
    {
        for(int i=0; i<m_itemList.Count; i++)
        {
            if(m_itemList[i].IsMagnet)
            {
                //마그넷효과를 받은 아이템들을 플레이어쪽으로 이동시켜준다.
                m_itemList[i].transform.position += (m_player.transform.position - m_itemList[i].transform.position).normalized * 10f * Time.deltaTime;
            }
        }
    }
    #endregion
}
