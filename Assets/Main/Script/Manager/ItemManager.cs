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

    public Sprite GetItemSprite(eItemType type)
    {
        //로드순서가 같아야 가능함.
        return m_itemSprites[(int)type];
    }

    public void CreateItem(Vector3 pos, Vector3 dir)
    {
        var type = (eItemType)Random.Range((int)eItemType.Coin, (int)eItemType.Max);

        var item = m_gameItemPool.Get();
        item.SetItem(type, pos, dir);
    }
    
    public void RemoveItem(GameItem item)
    {
        item.gameObject.SetActive(false);
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
        
    }
}
