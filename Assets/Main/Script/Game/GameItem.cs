using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameItem : MonoBehaviour
{
    SpriteRenderer m_iconSprRen;
    Rigidbody2D m_rigid;
    ItemManager.eItemType m_type;
    public void SetItem(ItemManager.eItemType type, Vector3 pos, Vector3 dir)
    {
        gameObject.SetActive(true);

        m_type = type;
        m_iconSprRen.sprite = ItemManager.Instance.GetItemSprite(type);
        transform.position = pos;
        //이미 돌아가있는 아이템이 있을 수 있으니 회전값을 초기화
        transform.rotation = Quaternion.identity;
        //메모리풀링을 사용할건데 active를 꺼놓는다고해서 물리연산을 위해 가했던 힘들은 사라지지 않는다 고로 이전의 힘값들은 초기화해주어야한다.
        m_rigid.velocity = Vector2.zero;
        m_rigid.angularVelocity = 0f;

        //dir 방향대로 위로 힘을 가해줌.
        m_rigid.AddForce(Vector2.up * 2f + (Vector2)dir.normalized, ForceMode2D.Impulse);

        if(type >= ItemManager.eItemType.Gem_Red && type <= ItemManager.eItemType.Gem_Blue)
        {
            //회전하는 힘을 가해줄때 사용하는 메소드
            //+면 시계방향 -면 반시계방향
            m_rigid.AddTorque(dir.x < 0f ? -1 : 1 * 1f, ForceMode2D.Impulse);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag.Equals("BgColliderBottom"))
        {
            ItemManager.Instance.RemoveItem(this);
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        m_iconSprRen = GetComponent<SpriteRenderer>();
        m_rigid = GetComponent<Rigidbody2D>();
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
