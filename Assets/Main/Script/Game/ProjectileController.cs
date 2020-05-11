using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    float m_speed = 10f;
    PlayerController m_player;

    int m_power = 1;

    public void SetProjectile(Vector3 pos)
    {
        transform.position = pos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("BgColliderTop"))
        {
            m_player.RemoveBullet(this);
        }
        else if(collision.tag.Equals("Monster"))
        {
            SfxManager.Instance.CreateSfx(SfxManager.eSfxType.Hit, transform.position);
            m_player.RemoveBullet(this);
            collision.gameObject.GetComponent<MonsterController>().SetDamage(m_power);
        }
    }

    private void Awake()
    {
        m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //풀링으로 미리 만들어놓은것들을 Active를 꺼놓지않으면 전부 발사가 되어버릴것임.
        gameObject.SetActive(false);
        //Invoke("RemoveProjectile", m_lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.up * m_speed * Time.deltaTime;
    }
}
