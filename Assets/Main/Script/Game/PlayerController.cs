using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Vector3 m_dir;
    float m_speed = 4f;

    float m_moveScale;
    BoxCollider2D m_collider;

    [SerializeField]
    GameObject m_bulletPrefab;
    [SerializeField]
    GameObject m_firePos;

    GameObjectPool<ProjectileController> m_bulletPool;

    void KeyProcess()
    {
        m_dir.x = Input.GetAxis("Horizontal");
    }

    void CreateBullet()
    {
        //var obj = Instantiate(m_bulletPrefab) as GameObject;
        //강사님은 그냥 obj.transform.position = m_firepos.transform.position;으로 하신듯.
        //var script = obj.GetComponent<Projectile>();
        //script.SetProjectile(m_firePos.transform.position);

        //pool에서 총알을 하나꺼내서
        var bullet = m_bulletPool.Get();
        //위치를 잡아주고
        bullet.transform.position = m_firePos.transform.position;
        //꺼져있는 Active를 켜주면 날아가게된다.
        bullet.gameObject.SetActive(true);
    }

    public void RemoveBullet(ProjectileController bullet)
    {
        bullet.gameObject.SetActive(false);
        m_bulletPool.Set(bullet);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_collider = GetComponent<BoxCollider2D>();
        
        m_bulletPool = new GameObjectPool<ProjectileController>(20, () => 
        {
            var obj = Instantiate(m_bulletPrefab) as GameObject;
            var bullet = obj.GetComponent<ProjectileController>();
            
            return bullet; 
        });

        //시작한뒤 3초뒤에 0.1초 간격으로 계속 총알을 발사시킴.
        InvokeRepeating("CreateBullet", 3f, 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        KeyProcess();

        if(m_dir != Vector3.zero)
        {
            m_moveScale = m_speed * Time.deltaTime;
            float checkMove = m_moveScale;
            var amount = m_collider.size.x / 2;

            if (m_moveScale < amount)
            {
                checkMove = amount;
            }

            //어느 위치에서, 어느 방향으로, 얼마만큼 레이를 쏘겠느냐.
            var hitInfo = Physics2D.Raycast(transform.position, m_dir, checkMove, 1 << LayerMask.NameToLayer("Collider"));

            //충돌했다면
            if (hitInfo.collider != null)
            {
                checkMove = hitInfo.distance - amount;
            }
            else
            {
                checkMove = m_moveScale;
            }

            transform.position += m_dir * checkMove;
        }

    }
}
