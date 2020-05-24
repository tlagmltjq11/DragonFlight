using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    [SerializeField]
    int m_hp;
    [SerializeField]
    public bool m_isAlive;
    public int LineNum { get; set; }
    Vector3 m_dir = Vector3.down;
    float m_speed = 4f;
    float m_speedScale = 0.8f;
    float m_prevSpeedScale = 0f;
    MonsterManager.eMonsterType m_type;
    SpriteRenderer[] m_sprRenderers;
    Animator m_animator;

    //플레이어와의 방향을 알아야 아이템을 해당방향으로 떨어뜨리기 때문에 플레이어위치를 알아야함. 
    //대신 몬스터마다 계속해서 찾을바에 몬스터매니저에서 한번에 찾아서 보내주는것이 낫다.
    //기존엔 몬스터 한마리를 생성할때마다 파인드를 해줘야하지만, 매니저에서 한번만 찾아서 넘겨주는게 낫다는것.
    PlayerController m_player;

    public void InitMonster(PlayerController player)
    {
        m_player = player;
    }

    public void SetMonster(MonsterManager.eMonsterType type, int line, float scale)
    {
        m_type = type;
        m_hp = ((int)m_type + 1) * 2;//타입에 따라서 hp를 다르게 설정함.
        ChangeParts();
        m_isAlive = true;
        LineNum = line;
        m_speedScale = scale;
    }

    //원래대로 돌아올 속도를 알고있어야 하기 떄문에 prev 변수를 사용.
    public void SetSpeedScale(float scale)
    {
        m_prevSpeedScale = m_speedScale;
        m_speedScale = scale;

        if(scale.Equals(1f))
        {
            SetDefaultSpeedScale();
        }
    }

    public void SetDefaultSpeedScale()
    {
        m_speedScale = m_prevSpeedScale;
        m_prevSpeedScale = 0f;
    }

    public void SetDie()
    {
        ScoreManager.Instance.SetHuntScore(((int)m_type + 1) * 50);
        SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.MonDie);
        SfxManager.Instance.CreateSfx(SfxManager.eSfxType.Dust, transform.position);
        ItemManager.Instance.CreateItem(transform.position, (m_player.transform.position - transform.position));
    }

    public void SetDamage(int dmg)
    {
        if (!m_isAlive) return;

        m_animator.Play("Hit", 0, 0f);        //즉시 해당 애니메이션을 바로 재생하게끔 하는 방식.
        // 이방식의 문제는 히트중 또 히트가 들어오면 두번째로 들어온 히트를 무시함. 즉 똑같으니까 하던 애니메이션을 계속 진행함.
        // 고로 시간을 초기화시켜줘야함.

        m_hp -= dmg;
        if (m_hp <= 0)
        {
            m_hp = 0;
            if (m_type == MonsterManager.eMonsterType.Bomb)
            {
                MonsterManager.Instance.RemoveMonsterAll(LineNum);
            }
            else
            {
                m_isAlive = false;
                MonsterManager.Instance.RemoveMonster(this);
                SetDie();
            }

        }
    }

    void ChangeParts()
    {
        var parts = MonsterManager.Instance.GetMonsterParts(m_type);
        //body
        m_sprRenderers[0].sprite = parts[0];
        m_sprRenderers[1].sprite = m_sprRenderers[2].sprite = parts[1];
        m_sprRenderers[3].sprite = m_sprRenderers[4].sprite = parts[3];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("BgColliderBottom"))
        {
            MonsterManager.Instance.RemoveMonster(this);
        }
        if(collision.tag.Equals("Invincible"))
        {
            m_hp = 0;
            SetDamage(0);
        }
        if (collision.tag.Equals("ShockWave"))
        {
            //아이템 생성없이 삭제
            m_hp = 0;
            m_isAlive = false;
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
        transform.position += m_dir * m_speed * m_speedScale * Time.deltaTime;
    }
}