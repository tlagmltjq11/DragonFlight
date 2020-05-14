using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    float m_speed = 10f;
    PlayerController m_player;

    int m_power = 1;
    Vector3 m_prevPos;
    BoxCollider2D m_collider;

    public void SetProjectile(Vector3 pos)
    {
        transform.position = pos;
    }

    //이부분 나중에 다시 수정한다하심. 원하는 결과가 안나옴.
    void CheckCollison()
    {
        var hit = Physics2D.Raycast(transform.position, Vector3.up, (transform.position - m_prevPos).magnitude + m_collider.size.y / 2f, 1 << LayerMask.NameToLayer("Monster"));
        Debug.DrawRay(transform.position, Vector3.up * ((transform.position - m_prevPos).magnitude + m_collider.size.y / 2f), Color.green);
        if(hit.collider != null)
        {
            if(hit.collider.tag.Equals("Monster"))
            {
                var mon = hit.collider.gameObject.GetComponent<MonsterController>();
                mon.SetDamage(m_power);

                transform.position = hit.point;
                SfxManager.Instance.CreateSfx(SfxManager.eSfxType.Hit, hit.point);
                m_player.RemoveBullet(this);
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("BgColliderTop"))
        {
            m_player.RemoveBullet(this);
        }

        /*else if(collision.tag.Equals("Monster"))
        {
            var mon = collision.gameObject.GetComponent<MonsterController>();

            SfxManager.Instance.CreateSfx(SfxManager.eSfxType.Hit, transform.position);
            m_player.RemoveBullet(this);

            mon.SetDamage(m_power);
        }
        */
    }

    private void Awake()
    {
        m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //풀링으로 미리 만들어놓은것들을 Active를 꺼놓지않으면 전부 발사가 되어버릴것임.
        m_collider = GetComponent<BoxCollider2D>();
        gameObject.SetActive(false);
        //Invoke("RemoveProjectile", m_lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.up * m_speed * Time.deltaTime;
        CheckCollison();
    }

    //update이후에 호출됨.
    //업데이트에서 이동처리가 끝나면 이전좌표를 갱신하는것.
    private void LateUpdate()
    {
        m_prevPos = transform.position;
    }
}
