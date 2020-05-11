using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    int m_hp;

    Vector3 m_dir = Vector3.down;
    float m_speed = 2f;
    MonsterManager.eMonsterType m_type;

    SpriteRenderer[] m_sprRenderers;

    Animator m_animator;

    public void SetMonster(MonsterManager.eMonsterType type)
    {
        m_type = type;
        m_hp = (int)m_type + 1; //타입에 따라서 hp를 다르게 설정함.
        ChangeParts();
    }

    public void SetDamage(int dmg)
    {
        m_animator.Play("Hit", 0, 0f); //즉시 해당 애니메이션을 바로 재생하게끔 하는 방식.
        // 이방식의 문제는 히트중 또 히트가 들어오면 두번째로 들어온 히트를 무시함. 즉 똑같으니까 하던 애니메이션을 계속 진행함.
        // 고로 시간을 초기화시켜줘야함.

        m_hp -= dmg;

        if(m_hp <= 0)
        {
            SetDie();
        }
    }

    void SetDie()
    {
        MonsterManager.Instance.RemoveMonster(this);
    }

    void ChangeParts()
    {
        var parts = MonsterManager.Instance.GetMonsterParts(m_type);

        m_sprRenderers[0].sprite = parts[0]; //body
        m_sprRenderers[1].sprite = parts[1]; //eye
        m_sprRenderers[2].sprite = parts[1];
        m_sprRenderers[3].sprite = parts[3]; //wing
        m_sprRenderers[4].sprite = parts[3];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag.Equals("BgColliderBottom"))
        {
            MonsterManager.Instance.RemoveMonster(this);
        }
    }


    // Start is called before the first frame update
    void Awake()
    {
        m_sprRenderers = GetComponentsInChildren<SpriteRenderer>();
        m_animator = GetComponent<Animator>();
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += m_dir * m_speed * Time.deltaTime;
    }
}
