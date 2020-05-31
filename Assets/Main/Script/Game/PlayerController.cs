using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Vector3 m_dir;
    float m_speed = 8f;

    float m_moveScale;
    Collider2D m_collider;

    [SerializeField]
    GameObject m_bulletPrefab;
    [SerializeField]
    GameObject m_firePos;
    [SerializeField]
    SpriteRenderer[] m_partsRenderer;

    GameObject m_sfxMagnetObj;
    GameObject m_sfxShockWaveObj;

    GameObjectPool<ProjectileController> m_bulletPool;
    Animator m_animator;
    bool m_isDrag;
    Vector3 m_startPos;

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

    void LoadCharacterSprite()
    {
        var sprites = Resources.LoadAll<Sprite>(string.Format("Heroes/sunny_{0:00}", PlayerDataManager.Instance.GetCurHero()));
        m_partsRenderer[0].sprite = sprites[0];
        m_partsRenderer[1].sprite = m_partsRenderer[2].sprite = sprites[1];
    }

    public void SetDie()
    {
        CancelInvoke("CreateBullet");
        gameObject.SetActive(false);
    }

    void StartFire()
    {
        InvokeRepeating("CreateBullet", 0.1f, 0.1f);
    }

    void StopFire()
    {
        CancelInvoke("CreateBullet");
    }

    public void RemoveBullet(ProjectileController bullet)
    {
        bullet.gameObject.SetActive(false);
        m_bulletPool.Set(bullet);
    }

    public void SetShockWave(bool isOn)
    {
        m_sfxShockWaveObj.SetActive(isOn);
    }

    public void SetMagnet(bool isOn)
    {
        m_sfxMagnetObj.SetActive(isOn);
    }

    public void SetInvincible()
    {
        //총쏘는것을 멈추게함.
        StopFire();
        m_animator.Play("Invincible");
    }

    public void EndInvincible()
    {
        StartFire();
        m_animator.Play("fly");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag.Equals("Monster"))
        {
            if (GameManager.Instance.GetState() != GameManager.eGameState.Invincible)
            {
                GameManager.Instance.SetState(GameManager.eGameState.Result);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_collider = GetComponent<Collider2D>();
        //시작한뒤 3초뒤에 0.1초 간격으로 계속 총알을 발사시킴.
        InvokeRepeating("CreateBullet", 3f, 0.1f);

        m_bulletPool = new GameObjectPool<ProjectileController>(20, () => 
        {
            var obj = Instantiate(m_bulletPrefab) as GameObject;
            var bullet = obj.GetComponent<ProjectileController>();
            
            return bullet; 
        });

        m_sfxMagnetObj = Util.FindChildObject(gameObject, "Magnet");
        m_sfxShockWaveObj = Util.FindChildObject(gameObject, "ShockWave");
        m_sfxMagnetObj.SetActive(false);
        SetShockWave(false);

        LoadCharacterSprite();
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            m_isDrag = true;
            m_startPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            m_isDrag = false;
        }

        if (m_isDrag == true)
        {
            //바로 마우스가 어디로갔는지 체크한다.
            var movePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var dir = movePos - m_startPos;


            //KeyProcess();

            m_dir = new Vector3(dir.x, 0, 0).normalized;

            if (m_dir != Vector3.zero)
            {
                //m_moveScale = m_speed * Time.deltaTime;
                m_moveScale = Mathf.Abs(dir.x);
                float checkMove = m_moveScale;
                var amount = m_collider.bounds.size.x / 2;

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

            m_startPos = movePos;

        }
    }
}
