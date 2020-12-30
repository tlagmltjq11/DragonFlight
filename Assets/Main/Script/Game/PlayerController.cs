using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Field
    Vector3 m_dir;
    float m_speed = 8f;

    float m_moveScale;
    [SerializeField]
    Collider2D m_collider;

    [SerializeField]
    GameObject m_bulletPrefab;
    [SerializeField]
    GameObject m_firePos;
    [SerializeField]
    SpriteRenderer[] m_partsRenderer;

    GameObject m_sfxMagnetObj;
    GameObject m_sfxShockWaveObj;
    GameObject m_sfxShieldObj;

    GameObjectPool<ProjectileController> m_bulletPool;
    Animator m_animator;
    bool m_isDrag;
    Vector3 m_startPos;

    Item m_armor = null;
    int m_armorChance = 0;
    bool m_canHit = true;
    #endregion

    #region Private Methods
    void KeyProcess()
    {
        m_dir.x = Input.GetAxis("Horizontal");
    }

    void CreateBullet()
    {
        var bullet = m_bulletPool.Get();
        bullet.SetProjectile(m_firePos.transform.position);
        bullet.gameObject.SetActive(true);
    }

    void LoadCharacterSprite()
    {
        var sprites = Resources.LoadAll<Sprite>(string.Format("Heroes/sunny_{0:00}", PlayerDataManager.Instance.GetCurHero()));
        m_partsRenderer[0].sprite = sprites[0]; //몸체
        m_partsRenderer[1].sprite = m_partsRenderer[2].sprite = sprites[1]; //양 날개
    }

    void StartFire()
    {
        InvokeRepeating("CreateBullet", 0.1f, 0.1f);
    }

    void StopFire()
    {
        CancelInvoke("CreateBullet");
    }
    #endregion

    #region Public Methods
    public void SetDie()
    {
        CancelInvoke("CreateBullet");
        gameObject.SetActive(false);
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

    public void ShieldOn()
    {
        Debug.Log("실드온");
        SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.Shield);
        m_sfxShieldObj.SetActive(true);
        StartCoroutine("Shield");
        m_canHit = false;
    }

    public void ShieldOff()
    {
        Debug.Log("실드오프");
        m_sfxShieldObj.SetActive(false);
        //StopCoroutine("Shield"); //굳이없어도될듯.
        m_canHit = true;
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
    #endregion

    #region Unity Methods
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (m_canHit)
        {
            if (collision.tag.Equals("Monster") || collision.tag.Equals("Meteo"))
            {
                if (GameManager.Instance.GetState() != GameManager.eGameState.Invincible)
                {
                    if (m_armorChance == 0)
                    {
                        SoundManager.Instance.PlaySfx(SoundManager.eAudioSFXClip.PlayerDie);
                        GameManager.Instance.SetState(GameManager.eGameState.Result);
                    }
                    else
                    {
                        BuffManager.Instance.SetBuff(BuffManager.eBuffType.Shield);
                        m_armorChance--;
                    }
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_animator = GetComponent<Animator>();
        //시작한뒤 3초뒤에 0.1초 간격으로 계속 총알을 발사시킴.
        InvokeRepeating("CreateBullet", 3f, 0.1f);

        m_bulletPool = new GameObjectPool<ProjectileController>(20, () =>
        {
            var obj = Instantiate(m_bulletPrefab) as GameObject;
            var bullet = obj.GetComponent<ProjectileController>();

            Item item = PlayerDataManager.Instance.GetCurEquipItem(Item.eItemClass.Weapon);

            if (item != null)
            {
                bullet.gameObject.GetComponent<SpriteRenderer>().sprite = item.m_sprite;
                bullet.SetPower(item.m_stat);
            }

            return bullet; 
        });

        m_armor = PlayerDataManager.Instance.GetCurEquipItem(Item.eItemClass.Armor);

        //장착한 방어구가있다면
        if(m_armor != null)
        {
            m_armorChance = m_armor.m_stat;
        }

        m_sfxMagnetObj = Util.FindChildObject(gameObject, "Magnet");
        m_sfxShockWaveObj = Util.FindChildObject(gameObject, "ShockWave");
        m_sfxShieldObj = Util.FindChildObject(gameObject, "Shield");

        m_sfxMagnetObj.SetActive(false);
        SetShockWave(false);
        m_sfxShieldObj.SetActive(false);

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
    #endregion

    #region Coroutine
    IEnumerator Shield()
    {
        float time = 0f;
        bool temp = false;

        while(time < 3f)
        {
            if (!temp)
            {
                m_partsRenderer[0].enabled = false;
                m_partsRenderer[1].enabled = false;
                m_partsRenderer[2].enabled = false;

                temp = true;
            }
            else
            {
                m_partsRenderer[0].enabled = true;
                m_partsRenderer[1].enabled = true;
                m_partsRenderer[2].enabled = true;

                temp = false;
            }

            time += 0.25f;
            yield return new WaitForSeconds(0.25f);
        }

        m_partsRenderer[0].enabled = true;
        m_partsRenderer[1].enabled = true;
        m_partsRenderer[2].enabled = true;
    }
    #endregion
}
