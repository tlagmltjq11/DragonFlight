# DragonFlight
프로젝트 설명은 아래 [링크](#1)를 통해 영상으로 확인할 수 있고, 코드와 같은 부가설명은 [About Dev](#2) 부분을 참고해주세요.<br>
<br>

### About Project.:two_men_holding_hands:
라인게임즈에서 개발한 드래곤플라이트를 모작한 프로젝트입니다.<br>
<br>

### Video.:video_camera: <div id="1">이미지를 클릭해주세요.</div>
[![시연영상](https://img.youtube.com/vi/TNQ0OKnjaWw/0.jpg)](https://www.youtube.com/watch?v=TNQ0OKnjaWw)
<br>
<br>

### About Dev.:nut_and_bolt: <div id="2"></div>
<br>

<details>
<summary>무기관련 Code 접기/펼치기</summary>
<div markdown="1">

<br>

<details>
<summary>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Weapon code 접기/펼치기</summary>
<div markdown="1">
	
```c#
public abstract class Weapon : MonoBehaviour
{
	#region Field
	#region References
	// References
	public Transform m_shootPoint; //레이(총알) 발사지점
	public ParticleSystem muzzleFlash; //총기화염
	public Player_StateManager m_stateManager; //플레이어 상태매니저
	public CameraRotate m_cameraRotate;
	public GameObject m_horizonCamRecoil; //수평반동
	public GameObject m_verticalCamRecoil; //수직반동
	public Transform m_casingPoint; //탄피 생성 포인트
	public Camera m_camera; //플레이어 카메라
	public GameObject[] m_sights; //sight 파츠
	public Animator m_anim;
	public GameObject m_player;
	#endregion
	
	#region Weapon info
	// Weapon Specification
	protected float m_range; //사정거리
	protected float m_accuracy; //현재 정확도
	protected float m_power; //데미지
	protected float m_originAccuracy; //원래 정확도
	public string m_weaponName; //무기 이름
	public int m_bulletsPerMag; //탄창 당 탄약
	public int m_bulletsRemain; //남은 전체 탄약
	public int m_totalMag; //총 탄창
	public int m_currentBullets; //현재 탄약
	public float m_fireRate; //연사력
	
	// 현재 장착한 sight 파츠에 따라 정조준의 최종 포지션이 다름.
	public Vector3 m_aimPosition;
	public Vector3 m_dotSightPosition;
	public Vector3 m_acogSightPosition;
	public Vector3 m_originalPosition;
	
	// 반동 만들때 사용
	protected Vector3 m_recoilKickBack; //총기가 뒤로 밀리는 위치
	protected float m_recoilAmount; //반동의 세기를 조절
	protected float m_recoilVert; //수직반동
	protected float m_recoiltHoriz; //수평반동
	#endregion
	
	#region State Check vars
	// 각종 상태체크
	protected AnimatorStateInfo m_info;
	protected bool m_isAimOutOver;
	public bool m_isReloading;
	public bool m_isDrawing;
	public bool m_isAiming;
	public bool m_isFiring;
	public float m_fireTimer; //발사간격 
    	#endregion
    	#endregion

    	#region Abstract Methods
    	public abstract void Fire();
	public abstract void StopFiring(); //연사를 멈출 경우 반동 회복
	public abstract void ChangeSight(); //파츠 변경
	public abstract void Reload(); //재장전
	#endregion

	#region Protected Methods
	protected void Recoil() //반동
	{
		Vector3 HorizonCamRecoil = new Vector3(0f, Random.Range(-m_recoiltHoriz, m_recoiltHoriz), 0f);
		Vector3 VerticalCamRecoil = new Vector3(-m_recoilVert, 0f, 0f);

		if (!m_isAiming)
		{
			Vector3 gunRecoil = new Vector3(Random.Range(-m_recoilKickBack.x, m_recoilKickBack.x), m_recoilKickBack.y, m_recoilKickBack.z);
			//총기가 뒤로 밀리는 반동
			transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition + gunRecoil, m_recoilAmount);
			//수평반동
			m_horizonCamRecoil.transform.localRotation = Quaternion.Slerp(m_horizonCamRecoil.transform.localRotation, Quaternion.Euler(m_horizonCamRecoil.transform.localEulerAngles + HorizonCamRecoil), m_recoilAmount);
			//수직반동
			m_cameraRotate.VerticalCamRotate(-VerticalCamRecoil.x);
		}
		else
		{
			Vector3 gunRecoil = new Vector3(Random.Range(-m_recoilKickBack.x, m_recoilKickBack.x) / 2f, 0, m_recoilKickBack.z);
			//총기가 뒤로 밀리는 반동
			transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition + gunRecoil, m_recoilAmount);

			//수평반동
			m_horizonCamRecoil.transform.localRotation = Quaternion.Slerp(m_horizonCamRecoil.transform.localRotation, Quaternion.Euler(m_horizonCamRecoil.transform.localEulerAngles + HorizonCamRecoil / 1.5f), m_recoilAmount);
			//수직반동
			m_cameraRotate.VerticalCamRotate(-VerticalCamRecoil.x / 2f);
		}
	}

	protected void RecoilBack() //수평반동 회복 -> Update문에서 호출
	{
		m_horizonCamRecoil.transform.localRotation = Quaternion.Slerp(m_horizonCamRecoil.transform.localRotation, Quaternion.Euler(0f, 0f, 0f), Time.deltaTime * 3f);
	}

	protected void CasingEffect() //탄피이펙트
	{
		//매번 랜덤한 각도로 튕겨져 나감.
		Quaternion randomQuaternion = new Quaternion(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f), 1);
		var casing = ObjPool.Instance.m_casingPool.Get(); //풀에서 탄피를 꺼냄.

		if (casing != null)
		{
			casing.transform.SetParent(m_casingPoint);
			casing.transform.localPosition = new Vector3(-1f, -3f, 0f);
			casing.transform.localScale = new Vector3(23, 23, 23);
			casing.transform.localRotation = Quaternion.identity;

			var rigid = casing.gameObject.GetComponent<Rigidbody>();

			rigid.isKinematic = false; //물리힘을 가하기 위해.
			casing.gameObject.SetActive(true);
			//매번 랜덤한 힘을 가해준다.
			rigid.AddRelativeForce(new Vector3(Random.Range(50f, 100f), Random.Range(50f, 100f), Random.Range(-10f, 20f)));
			rigid.MoveRotation(randomQuaternion.normalized);
		}
	}
	#endregion
	
	#region Public Methods
	public void AimIn() //정조준
	{
		m_anim.SetBool("ISAIM", true); //IDLE 애니메이션이 재생되지 않도록 ISAIM으로 변경
		m_isAiming = true;

		m_accuracy = m_accuracy / 4f; //정확도를 높여줌.

		if (UIManager.Instance != null)
		{
			UIManager.Instance.CrossHairOnOff(false); //크로스헤어를 비활성화 시킨다.
		}
		SoundManager.Instance.Play2DSound(SoundManager.eAudioClip.AIM_IN, 3.5f);
	}

	public void AimOut() //정조준 해제
	{
		m_isAiming = false;
		m_anim.SetBool("ISAIM", false);

		//정확도를 다시 낮춰준다
		if (m_stateManager.m_isCrouching)
		{
			m_accuracy = m_originAccuracy / 2f;
		}
		else if (!m_stateManager.m_isGrounded)
		{
			m_accuracy = m_originAccuracy * 5f;
		}
		else
		{
			m_accuracy = m_originAccuracy;
		}

		if (UIManager.Instance != null)
		{
			UIManager.Instance.CrossHairOnOff(true); //크로스헤어 활성화
		}
	}

	public void JumpAccuracy(bool j) //점프했을때 정확도 조정
	{
		if (j)
		{
			m_accuracy = m_accuracy * 5f;
		}
		else
		{
			if (m_isAiming)
			{
				m_accuracy = m_originAccuracy / 4f;
			}
			else
			{
				m_accuracy = m_originAccuracy;
			}
		}
	}

	public void CrouchAccuracy(bool c) //앉았을때 정확도 조정
	{
		if (c)
		{
			m_accuracy = m_accuracy / 2f;
		}
		else
		{
			if (m_isAiming)
			{
				m_accuracy = m_originAccuracy / 4f;
			}
			else
			{
				m_accuracy = m_originAccuracy;
			}
		}
	}
	#endregion
}
```
</div>
</details>

<details>
<summary>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Weapon_AKM code 접기/펼치기</summary>
<div markdown="1">
	
```c#
public class Weapon_AKM : Weapon
{
    //Unity Methods 생략..
    
    #region Abstract Methods Implement
    public override void Fire() //총 발사
    {
	if (m_fireTimer < m_fireRate) //연사력을 시간으로 구현
	{
		return;
	}

	SoundManager.Instance.Play2DSound(SoundManager.eAudioClip.AKM_SHOOT, 0.8f); //발포음 재생
 
	if (m_isFiring) //연사중이라면 반동을 지속해서 키워줌
        { 
		m_recoilVert += 0.15f;
		m_recoilVert = Mathf.Clamp(m_recoilVert, 1.2f, 3f);
		m_recoiltHoriz += 0.05f;
		m_recoiltHoriz = Mathf.Clamp(m_recoiltHoriz, 0.4f, 0.8f);
        }

	RaycastHit hit;

	int layerMask = ((1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("Sfx")) | (1 << LayerMask.NameToLayer("Interactable")) | (1 << LayerMask.NameToLayer("Player_Throw")) | (1 << LayerMask.NameToLayer("Enemy_ExplosionHitCol")));
	layerMask = ~layerMask;
		
	//레이캐스트 발사 == 총알
	if (Physics.Raycast(m_shootPoint.position, m_shootPoint.transform.forward + Random.onUnitSphere * m_accuracy, out hit, m_range, layerMask))
	{
		if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy")) //적이 맞았을 경우
		{
			var blood = ObjPool.Instance.m_bloodPool.Get(); //블러드 이펙트를 풀에서 꺼냄

			if (blood != null)
			{
				blood.gameObject.transform.position = hit.point; //hit 포인트로 이동
				//법선벡터를 이용해서 잘보이게 회전시킴.
				blood.gameObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
				blood.gameObject.SetActive(true); //활성화 시켜서 재생시킨다.
			}
				
			//총을 맞은 적
			Enemy_StateManager enemy = hit.transform.GetComponentInParent<Enemy_StateManager>();

			if (enemy)
			{
				//Tag비교는 compareTag를 이용!
				if (hit.collider.gameObject.CompareTag("HeadShot")) //헤드샷 판별
				{
					//헤드샷 사운드 재생
					SoundManager.Instance.Play2DSound(SoundManager.eAudioClip.HEADSHOT, 1.5f);
					enemy.Damaged(m_power * 100f); //바로 죽이기 위해 x 100
				}
				else
				{
					//Hit 사운드 재생
					SoundManager.Instance.Play2DSound(SoundManager.eAudioClip.HITSOUND, 1.5f);
					enemy.Damaged(m_power);
				}
			}
		}
		else //적이 아닐 경우
		{
			var hitHole = ObjPool.Instance.m_hitHoleObjPool.Get(); //탄흔 이펙트를 풀에서 꺼냄.

			if (hitHole != null)
			{
				hitHole.gameObject.transform.position = hit.point;
				//법선벡터를 이용해서 잘보이게 회전시킴.
				hitHole.gameObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
				// 탄흔이 오브젝트를 따라가게끔 유도하기 위해 리턴되기 전까지만 부모로 지정
				hitHole.transform.SetParent(hit.transform); 
				hitHole.gameObject.SetActive(true);
			}

			var hitSpark = ObjPool.Instance.m_hitSparkPool.Get(); //사격으로 인한 스파크이펙트를 풀에서 꺼냄.

			if (hitSpark != null)
			{
				hitSpark.gameObject.transform.position = hit.point;
				//법선벡터를 이용해서 잘보이게 회전시킴.
				hitSpark.gameObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
				hitSpark.gameObject.SetActive(true); //활성화시켜 재생시킴.
			}

			//Movalble은 리지드바디를 가진 오브젝트들의 레이어임.
			if (hit.transform.gameObject.layer.Equals(LayerMask.NameToLayer("Movable")))
			{
				Rigidbody rig = hit.transform.GetComponent<Rigidbody>();

				if (rig)
				{
					//피격된 지점에서 물리힘을 가해줌.
					rig.AddForceAtPosition(m_shootPoint.forward * m_power * 70f, m_shootPoint.position);
				}
			}
		}
	}

	m_currentBullets--; //탄약 --
	m_fireTimer = 0.0f;
	m_anim.CrossFadeInFixedTime("FIRE", 0.01f); //애니메이션을 즉시 FIRE로 바꿔줌.

	muzzleFlash.Play(); //총기화염 play
	Recoil(); //반동
	CasingEffect(); //탄피 이펙트 생성
    }

    public override void StopFiring() //연사를 멈출 경우 반동 회복
    {
	m_recoilVert = 1.2f;
	m_recoiltHoriz = 0.65f;
    }

    public override void Reload() //재장전
    {
    	//탄창이 꽉차있거나, 남은 탄약이 없다면 return
	if (m_currentBullets == m_bulletsPerMag || m_bulletsRemain == 0)
	{
		return;
	}

	//재장전 
	SoundManager.Instance.Play2DSound_Play((int)SoundManager.eAudioClip.AKM_RELOAD, 1f);
	m_anim.CrossFadeInFixedTime("RELOAD", 0.01f); //애니메이션을 즉시 RELOAD로 바꿔줌
	//-> UI와 실제 남은 탄약을 바꿔주는 부분은 애니메이션 Event로 ReloadComplete() 메소드를 호출시켜줌.
    }

    public override void ChangeSight() //Sight 파츠변경
    {
	bool check = false;
	int index = 0;

	for(int i=0; i<m_sights.Length; i++)
        {
	    if(m_sights[i].activeSelf)
            {
		check = true;
		index = i;
		break;
            }
        }
		
	if(check)
        {
	    m_sights[index].SetActive(false);
			
	    if(index + 1 < m_sights.Length)
            {
		m_sights[index + 1].SetActive(true);
	    }
        }
	else
        {
	    m_sights[index].SetActive(true);
        }
    }
    #endregion

    #region Public Methods
    public void ReloadComplete() //재장전 애니메이션에서 이벤트로 호출시켜준다.
    {
	int temp = 0;

	temp = m_bulletsPerMag - m_currentBullets; //장전되어야 할 총알의 수

	if(temp >= m_bulletsRemain)
        {
		m_currentBullets += m_bulletsRemain;
		m_bulletsRemain = 0;
        }
	else
        {
		m_currentBullets += temp;
		m_bulletsRemain -= temp;
        }

	UIManager.Instance.Update_Bullet(m_currentBullets, m_bulletsRemain);
    }
    #endregion
}
```

</div>
</details>

<br>

<details>
<summary>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ATW code 접기/펼치기</summary>
<div markdown="1">
	
```c#
public abstract class ATW : MonoBehaviour //Ahead Thrown Weapon 투척무기
{
    #region Field
    public Rigidbody m_rigid;
    public GameObject m_effectObj; //폭발 이펙트
    public GameObject m_meshObj; //몸체 Mesh -> 폭발 시 비활성화
    public int m_remainNum; //남은 갯수
    public float m_timeToOper; //동작하기까지 걸리는 시간
    public string m_name;
    protected float m_power; //데미지
    protected float m_explosionRadius; //폭발범위
    #endregion

    #region Public Methods
    public void Starter() //스타터
    {
        Invoke("Operation", m_timeToOper);
    }
    #endregion

    #region Abstract Methods
    public abstract void Operation(); //실제 동작
    #endregion
}
```

</div>
</details>

<details>
<summary>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ATW_Grenade code 접기/펼치기</summary>
<div markdown="1">
	
```c#
public class ATW_Grenade : ATW
{
    #region Unity Methods
    void Start()
    {
        m_rigid = GetComponent<Rigidbody>();
        m_remainNum = 2; //디폴트갯수 2개
        m_timeToOper = 3f; //3초뒤 폭발
        m_name = "Grenade";
        m_power = 6.5f;
        m_explosionRadius = 5f; //폭발 범위 반지름
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer.Equals(LayerMask.NameToLayer("Ground"))) //땅이나 벽에 부딪힐 경우
        {	
	    //ImpactGround 클립 재생
            SoundManager.Instance.Play3DSound(SoundManager.eAudioClip.ATW_IMPACTONTGROUND, gameObject.transform.position, 20f, 1.5f);
        }
    }
    #endregion

    #region Abstract Methods Implement
    public override void Operation() //실제 동작
    {
        //파티클시스템을 작동시키기 위해서, 수류탄의 몸체를 정지시킨 후 똑바로 세워놓음.
        m_rigid.velocity = Vector3.zero;
        m_rigid.angularVelocity = Vector3.zero;
        gameObject.transform.rotation = Quaternion.Euler(Vector3.zero);

        //수류탄의 몸체를 보이지않게함.
        m_meshObj.SetActive(false);
        //수류탄 파티클시스템을 작동시킴.
        m_effectObj.SetActive(true);

        //랜덤으로 폭발음을 재생
        SoundManager.Instance.Play3DSound(Random.Range((int)SoundManager.eAudioClip.ATW_EXPLOSION1, 
			                  (int)SoundManager.eAudioClip.ATW_IMPACTONTGROUND), gameObject.transform.position, 40f, 2f);

        //대상 레이어만 지정
        int layerMask = ((1 << LayerMask.NameToLayer("Enemy_ExplosionHitCol")) | (1 << LayerMask.NameToLayer("Movable")));
        //SphereCastAll을 통해서 수류탄의 폭발지점 반경내에있는 대상 레이어의 모든 hit정보를 받아온다.
        RaycastHit[] hits = Physics.SphereCastAll(gameObject.transform.position, m_explosionRadius, Vector3.up, 0, layerMask,
						   QueryTriggerInteraction.UseGlobal);

	//모든 hit 검사
        foreach(RaycastHit hit in hits)
        {
            //수류탄과의 거리별 데미지를 계산한다.
            
            //혹시 거리가 0이 나오는 경우를 대비
            var distance = Vector3.Distance(hit.transform.position, gameObject.transform.position);
            if(distance == 0)
            {
                distance = 1;
            }

            //대상이 적군일 경우
            if(hit.transform.root.gameObject.layer.Equals(LayerMask.NameToLayer("Enemy")))
            {
                //적군과 수류탄사이에 장애물이 가로막고있는지 Raycast로 판별 후, 가로막혀있다면 데미지를 주지않는다.
                RaycastHit check;
		//방향
                var dir = hit.transform.position - gameObject.transform.position;
                int layer = (1 << LayerMask.NameToLayer("Ground")) | (1 << LayerMask.NameToLayer("Interactable")) |
		              (1 << LayerMask.NameToLayer("Movable"));
			      
		//적과 수류탄사이에 다른 무엇인가 존재한다면 장애물로 판단하고 데미지를 주지 않은채 다음 hit 검사로 넘어간다. 
                if(Physics.Raycast(gameObject.transform.position, dir.normalized, out check, m_explosionRadius, layer))
                {
                    continue;
                }

                var enemy = hit.transform.GetComponentInParent<Enemy_StateManager>(); //데미지를 주기위해 적 FSM매니저를 가져옴.

                //사이에 장애물이 없으므로 거리별 데미지를 준다.
                enemy.Damaged(100 * (m_power / distance));

                if (enemy.m_hp <= 0) //대상이 죽은 상태라면
                {
                    //대상 적군의 리지드바디를 받아온다.
                    var rigs = enemy.transform.root.gameObject.GetComponentsInChildren<Rigidbody>();

                    foreach (Rigidbody rig in rigs)
                    {
                        //물리힘을 가하여 폭발에 날아가는 효과를 준다.
                        rig.AddExplosionForce(500 * (m_power / distance), gameObject.transform.position, m_explosionRadius, 8f);
                    }
                }
            }
	    //적군이 아닌 크래쉬박스와 같은 Movalble 오브젝트일 경우
            else if(hit.transform.gameObject.layer.Equals(LayerMask.NameToLayer("Movable")))
            {
                Crash_Box cb = hit.transform.gameObject.GetComponent<Crash_Box>();
                cb.Crash(); //박스가 여러 조각의 오브젝트들로 대체되는 메소드 -> 부서지는 현상을 보여주는것

                var rigs = hit.transform.GetComponentsInChildren<Rigidbody>();

                foreach (Rigidbody rig in rigs)
                {
		    //각 박스조각 오브젝트들에게 물리힘을 가해 날려준다.
                    rig.AddExplosionForce(100 * (m_power / distance), gameObject.transform.position, m_explosionRadius, 5f);
                }
            }
        }
	
	//2초뒤 삭제
        Destroy(gameObject, 2f);
    }
    #endregion
}
```

</div>
</details>

<br>

**Explanation**:gun:<br>
(구현설명은 주석으로 간단하게 처리했습니다!)<br>
공통된 내용(필드나 메소드)들을 추출하고, 통일된 내용으로 작성하도록 상위 클래스인 Weapon, ATW 추상클래스를 구현했습니다. 모든 총기 및 투척무기 클래스는 해당 추상클래스를 상속받아, 
각자 필요한 메소드나 필드만 추가로 정의하고, 추상 메소드를 오버라이딩하여 클래스마다 다르게 실행될 로직을 작성해 주면 됩니다. 이러한 구성을 통해서, 코드들을 규격화 할 수 있었고 
아래와 같이 다형성 사용, 느슨한 결합 등을 이룰 수 있었습니다.

```c#
//플레이어 컨트롤 스크립트(간략화)
public Weapon m_currentWeapon; //현재 총기
public ATW m_currentATW; //현재 투척무기
...
    if (Input.GetButton("Fire1"))
    {
        m_currentWeapon.Fire(); //다형성
    }
...
    void SetCurrentWeapon(Weapon weapon) //느슨한 결합
    {
        m_currentWeapon = weapon;
    }
...
    if (Input.GetKeyDown(KeyCode.G))
    {
        if(m_currentATW.m_remainNum > 0)
        {
            scr.m_rigid.AddForce(grenade.transform.up * 5f + grenade.transform.forward * 15f, ForceMode.Impulse);
            scr.Starter(); //다형성
        }
    }
...
    void SetCurrentATW(ATW atw) //느슨한 결합
    {
        m_currentATW = atw;
    }
...
```
</div>
</details>

<br>

<details>
<summary>FSM Code 접기/펼치기</summary>
<div markdown="1">
	
<br>	
	
<details>
<summary>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;적 FSM Code 접기/펼치기</summary>
<div markdown="1">

<br>

<details>
<summary>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;FSM Class 접기/펼치기</summary>
<div markdown="1">

```c#
public class FSM <T>  : MonoBehaviour //상태 매니저는 해당 FSM 클래스를 상속받아서 모든 기능들을 이용함.
{
	private T m_owner; //상태 매니저를 나타냄.
	private IFSMState<T> m_currentState = null; //현재 상태
	private IFSMState<T> m_previousState = null; //이전 상태

	public IFSMState<T> CurrentState{ get {return m_currentState;} } //현재 상태 프로퍼티
	public IFSMState<T> PreviousState{ get {return m_previousState;} } //이전 상태 프로퍼티

	//	초기 상태 설정..
	protected void InitState(T owner, IFSMState<T> initialState)
	{
		this.m_owner = owner;
		ChangeState(initialState);
	}

	//	각 상태의 Execute 처리..
	protected void  FSMUpdate() 
	{ 
		if (m_currentState != null)
		{
			m_currentState.Execute(owner);
		}
	}

	//	상태 변경..
	public void  ChangeState(IFSMState<T> newState)
	{
		m_previousState = m_currentState;
 
		if (m_currentState != null)
		{
			m_currentState.Exit(owner); //상태전환 이전에 Exit 호출
		}
 
		m_currentState = newState;
 
		if (m_currentState != null)
		{
			m_currentState.Enter(owner); //상태전환과 동시에 Enter 호출
		}
	}

	//	이전 상태로 전환..
	public void  RevertState()
	{
		if (m_previousState != null)
		{
			ChangeState(m_previousState);
		}
	}

	public override string ToString() 
	{ 
		return m_currentState.ToString(); //현재상태 string 반환
	}
}
```

</div>
</details>

<details>
<summary>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;IFSM Interface 접기/펼치기</summary>
<div markdown="1">

```c#
public interface IFSMState<T> //각 상태들이 포함해야할 메소드를 정의한 인터페이스
{	
    //  상태 진입..
    void Enter(T e);

    //  상태 진행..
    void Execute(T e);

    //  상태 종료..
    void Exit(T e);
}
```

</div>
</details>

<details>
<summary>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;FSM State Manager 접기/펼치기</summary>
<div markdown="1">

```c#
public class Enemy_StateManager : FSM<Enemy_StateManager> //상태매니저
{
    //상태들의 변수를 전부 매니저에 선언하는 이유는 상태들이 싱글턴이기 때문에 전역으로 관리되는데, 거기서 변수를 관리해버리면
    //해당 상태를 사용하는 모든 유닛들의 변수값이 같이 바뀌게 되기 때문이다. 각자 유닛들의 매니저에서 변수를 관리해야하는것.
    #region Field
    public Animator m_anim;
    public GameObject m_player; //플레이어
    public NavMeshAgent m_navAgent;
    public GameObject m_shootPoint; //레이(총알) 발사 위치
    public LineRenderer m_lineRenderer; //총알의 발사 경로를 그려줄 라인렌더러
    public AnimatorStateInfo m_info; //애니메이터 상태 정보
    public ParticleSystem muzzleFlash; //총구화염 이펙트
    public Rigidbody m_bodyRig;

    public float m_idleTime; //Idle 필수 지속시간
    public float m_dieTime; //적이 죽고 난 후 5초 뒤 비활성화해주기 위한 변수
    public float m_detectSight = 30f; //플레이어 감지거리
    public float m_attackSight = 15f; //공격가능거리
    public int m_check; //플레이어를 발견했는지 검사.
    public float m_footstepTimer; //발소리 m_footstepCycle 마다 발소리를 재생시키기 위해 시간초를 카운팅하는 변수.
    public float m_footstepCycle; //발소리 재생 간격
    public float m_hp; //체력
    public float m_bullets; //탄약
    public int m_footstepTurn = 0; //왼쪽 발소리, 오른쪽 발소리 차례대로 한번씩 재생시키기 위한 변수.

    public Transform m_wayPointObj; //웨이포인트를 자식으로 갖고있는 상위오브젝트
    public Transform[] m_wayPoints; //웨이포인트들.
    public bool m_isStayPos; //True라면 현재 포지션을 지키는 AI, False라면 Patrol하는 AI
    #endregion

    #region Unity Methods
    void OnEnable()
    {
        Init(); //초기화
        RagdollOnOff(true); //살아있는 상태에는 isKinematic을 true로 해줌. -> false로 할 시 RayCast에 감지되지않는 문제가 생김.
        InitState(this, Enemy_IDLE.Instance); //상태를 Idle로 초기화
    }

    void OnDisable()
    {
        RagdollOnOff(true);
    }

    void Start()
    {
        if(m_wayPointObj != null)
        {
            m_wayPoints = m_wayPointObj.GetComponentsInChildren<Transform>(); //웨이포인트들을 가져온다.
        }
    }

    void Update()
    {
        m_info = m_anim.GetCurrentAnimatorStateInfo(0); //현재 동작중인 애니메이션 정보를 받아옴.

        FSMUpdate(); //현재 상태의 Execute() 실행
    }
    #endregion

    #region Private Methods
    void Init() //초기화 메소드
    {
        m_anim = GetComponent<Animator>();
        m_navAgent = GetComponent<NavMeshAgent>();

	//라인렌더러 설정
        m_lineRenderer = GetComponent<LineRenderer>();
        m_lineRenderer.startWidth = 0.01f;
        m_lineRenderer.endWidth = 0.01f;
        m_lineRenderer.startColor = Color.white;
        m_lineRenderer.endColor = Color.white;
        m_lineRenderer.enabled = false;
        
        m_idleTime = 0f;
        m_dieTime = 0f;
        m_check = 0;
        m_hp = 100f;
        m_bullets = 5f;

        m_player = GameObject.Find("Player");
        m_anim.Rebind();
    }
    #endregion

    #region Public Methods
    public bool SearchTarget() //플레이어를 감지하는 메소드
    {
        var dir = (m_player.transform.position + Vector3.up * 0.4f) - (gameObject.transform.position + Vector3.up * 1.3f); //눈높이를 맞춰서 방향을 구함.

        m_check = 0;

        RaycastHit m_hit;

        int layerMask = ((1 << LayerMask.NameToLayer("Enemy")) | (1 << LayerMask.NameToLayer("Sfx")) | 
									(1 << LayerMask.NameToLayer("Interactable")));
        layerMask = ~layerMask; //위 레이어들을 제외한 나머지
	
	//동일선상 눈높이에서 플레이어의 방향으로 감지거리만큼 Ray를 발사.
        if (Physics.Raycast(gameObject.transform.position + Vector3.up * 1.3f, dir.normalized, out m_hit, m_detectSight, layerMask))
        {
            if (m_hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                m_check++;
            }
        }

        if (m_check == 1)
        {
            return true; //발견
        }
        else
        {
            return false; //미발견
        }
    }

    public bool canAttack() //공격 가능 거리인지 판단
    {
        var distance = Vector3.Distance(gameObject.transform.position, m_player.transform.position); //플레이어와의 거리를 계산

        if (distance <= m_attackSight) //공격 가능 거리 일 경우
        {
            return true;
        }

        return false; //아닐 경우
    }

    public void Fire() //Ray(총알) 발사
    {
        RaycastHit hit;

        int layerMask = ((1 << LayerMask.NameToLayer("Enemy")) | (1 << LayerMask.NameToLayer("Sfx")) | 
				(1 << LayerMask.NameToLayer("Interactable")) | (1 << LayerMask.NameToLayer("Enemy_ExplosionHitCol")));
				
        layerMask = ~layerMask; //위 레이어를 제외한 나머지 레이어

	//방향을 구함.
        Vector3 dir = new Vector3(m_player.transform.position.x, m_player.transform.position.y + 0.2f, m_player.transform.position.z) - m_shootPoint.transform.position;
        m_shootPoint.transform.forward = dir.normalized; //총구의 방향을 플레이어의 위치가있는 방향으로 돌려줌.

	// Random.onUnitSphere를 이용해서 정확도를 분산시켜주며, 총알 발사.
        if (Physics.Raycast(m_shootPoint.transform.position, m_shootPoint.transform.forward + Random.onUnitSphere * 0.05f, out hit, 100f, layerMask))
        {
            m_lineRenderer.SetPosition(0, m_shootPoint.transform.position);
            m_lineRenderer.SetPosition(1, hit.point); //라인렌더러로 BulletLine을 그려줌.
            StartCoroutine(ShowBulletLine()); //해당 라인을 몇초간 지속 후 지워주기 위함.

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player")) //플레이어가 맞았을 경우
            {
                if(hit.collider.gameObject.name.Equals("Player")) //몸에 맞았을 경우 5데미지
                {
                    Player_StateManager player = hit.collider.gameObject.GetComponent<Player_StateManager>();

                    if (player != null)
                    {
                        player.Damaged(5f);
                    }
                }
                else if(hit.collider.gameObject.name.Equals("UpperBodyLean")) //상체에 맞았을 경우 10데미지
                {
                    Player_StateManager player = hit.collider.gameObject.GetComponentInParent<Player_StateManager>();

                    if (player != null)
                    {
                        player.Damaged(10f);
                    }
                }
            }
            else //맞은 대상이 플레이어가 아닐 경우
            {
                var hitSpark = ObjPool.Instance.m_hitSparkPool.Get(); //총알 스파크이펙트를 풀에서 꺼냄.

                if (hitSpark != null)
                {
                    hitSpark.gameObject.transform.position = hit.point; //힛 포인트에 위치시킴.
                    hitSpark.gameObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal); //법선벡터를 이용해 잘보이게함.
                    hitSpark.gameObject.SetActive(true); //파티클시스템 재생
                }
            }
        }

	//발포음 재생
        SoundManager.Instance.Play3DSound(SoundManager.eAudioClip.M4_SHOOT, gameObject.transform.position, 30f, 0.7f);
        muzzleFlash.Play(); //총구화염 재생
        m_bullets--; //탄약--

        if(m_bullets == 0) //현재 탄약이 다 떨어지면 재장전
        {
            m_anim.SetBool("ISATTACK", false);
            m_anim.CrossFadeInFixedTime("RELOAD", 0.01f); //즉시 재장전 애니메이션 작동
            SoundManager.Instance.Play3DSound(SoundManager.eAudioClip.M4_RELOAD, gameObject.transform.position, 20f, 0.5f); //재장정 클립 재생
            m_bullets = 5f; 
        }
    }

    public void Damaged(float dmg) //적이 데미지를 입었을 경우
    {
        if (m_hp <= 0)
        {
            return;
        }

	//데미지로인해 죽을 경우
        if (m_hp - dmg <= 0f)
        {
            m_hp = 0f;
	    //죽을때 사운드클립을 랜덤하게 선택해 재생.
            SoundManager.Instance.Play3DSound(Random.Range((int)SoundManager.eAudioClip.ENEMY_DEATH1, (int)SoundManager.eAudioClip.HITSOUND), gameObject.transform.position, 20f, 1.2f);
            ChangeState(Enemy_DIE.Instance); //현재상태를 Die로 변경.
        }
        else 
        {
            m_hp = m_hp - dmg; //데미지만큼 체력 차감.
        }
    }

    public void RagdollOnOff(bool OnOff) //래그돌로인한 Rigidbody들의 isKinematic을 OnOff하는 메소드
    {
        m_anim.enabled = OnOff; //죽을경우 애니메이터도 비활성화 시켜줘야 래그돌로인해 자연스럽게 죽게됨.
        Rigidbody[] rigs = gameObject.GetComponentsInChildren<Rigidbody>();

        for (int i = 0; i < rigs.Length; i++)
        {
            rigs[i].isKinematic = OnOff;
        }
    }
    #endregion

    #region Coroutine
    IEnumerator ShowBulletLine()
    {
        m_lineRenderer.enabled = true;
        yield return new WaitForSeconds(0.1f); //0.1초 후 라인을 지우기 위해 비활성화
        m_lineRenderer.enabled = false;
    }
    #endregion
}
```

</div>
</details>

<details>
<summary>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;FSM State Idle 접기/펼치기</summary>
<div markdown="1">

```c#
public class Enemy_IDLE : FSMSingleton<Enemy_IDLE>, IFSMState<Enemy_StateManager> //IDLE상태 클래스 - 싱글턴 및 IFSMState 인터페이스 상속
{
    //  상태 진입..
    public void Enter(Enemy_StateManager e)
    {
        e.m_anim.Rebind(); //애니메이터 초기화
        e.m_navAgent.ResetPath(); //navAgent path 초기화
    }

    //  상태 진행..
    public void Execute(Enemy_StateManager e)
    {
        e.m_idleTime += Time.deltaTime; //IDLE 필수 지속시간을위한 체크

        if (e.m_idleTime >= 1.5f) //필수 지속시간을 넘어섰다면, 상태변경 가능.
        {
            if (e.SearchTarget()) //플레이어를 감지했을 경우
            {
                if (e.canAttack()) //공격 가능 거리내에 플레이어가 있을 경우
                {
                    e.ChangeState(Enemy_ATTACK.Instance); //공격 상태로 변경
                }
                else if(!e.m_isStayPos) //공격 가능 거리내에 플레이어가 없고, 자리를 지키는 AI가 아니라면
                {
                    e.ChangeState(Enemy_RUN.Instance); //질주 상태로 변경
                }
            }
            else if(!e.m_isStayPos) //플레이어를 찾지 못했고, 자리를 지키는 AI가 아니라면
            {
                e.ChangeState(Enemy_PATROL.Instance); //패트롤로 상태 변경
            }
        }
    }

    //  상태 종료..
    public void Exit(Enemy_StateManager e)
    {
        e.m_idleTime = 0f; //
    }
}
```

</div>
</details>

<details>
<summary>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;FSM State Patrol 접기/펼치기</summary>
<div markdown="1">

```c#
public class Enemy_PATROL : FSMSingleton<Enemy_PATROL>, IFSMState<Enemy_StateManager> //PATROL상태 클래스 - 싱글턴 및 IFSMState 인터페이스 상속
{
    //  상태 진입..
    public void Enter(Enemy_StateManager e)
    {
        e.m_navAgent.stoppingDistance = 0.5f;
        e.m_navAgent.speed = 1f; //패트롤 시 속도
        e.m_footstepCycle = 0.8f; //패트롤 속도에 맞춰 발걸음 재생 간격을 정해줌.
    }

    //  상태 진행..
    public void Execute(Enemy_StateManager e)
    {
        if (!e.SearchTarget()) //플레이어를 감지하지 못했을 경우
        {
            if(e.m_isStayPos)
            {
                return;
            }

            if (!e.m_navAgent.hasPath) //이미 path를 갖고있지 않을 경우
            {
                if(e.m_wayPointObj != null) //웨이포인트가 정해진 AI일 경우
                {
                    e.m_anim.SetBool("ISWALK", true); //걷는 애니메이션 작동
		    //웨이포인트 순서대로 이동시켜준다.
                    e.m_navAgent.SetDestination(e.m_wayPoints[Random.Range(1, e.m_wayPoints.Length)].position);
                }
                else //랜덤패트롤링 AI일 경우
                {
                    NavMeshHit hit;
                    Vector3 finalPosition = Vector3.zero;
                    Vector3 randomDirection = Random.insideUnitSphere * 5f; //랜덤한 방향
                    randomDirection.y = 0f;
                    randomDirection += e.gameObject.transform.position;

                    //randomDirection위치에 navMesh가 존재하여 갈 수 있는지 체크
                    if (NavMesh.SamplePosition(randomDirection, out hit, 1f, 1))
                    {
                        finalPosition = hit.position;
                    }

                    e.m_anim.SetBool("ISWALK", true); //걷는 애니메이션 작동
                    e.m_navAgent.SetDestination(finalPosition); //랜덤한 위치로 이동시켜준다.
                }
            }
            else //이미 path가 있을 경우
            {
                #region Footstep
                e.m_footstepTimer += Time.deltaTime; //발소리 재생간격 시간초 카운트

                if (e.m_footstepTimer > e.m_footstepCycle)
                {
                    if (e.m_footstepTurn == 0) //왼쪽
                    {
                        SoundManager.Instance.Play3DSound(SoundManager.eAudioClip.FOOTSTEP3, e.gameObject.transform.position, 8f, 1f);
                        e.m_footstepTurn = 1;
                    }
                    else if (e.m_footstepTurn == 1) //오른쪽 발소리 재생
                    {
                        SoundManager.Instance.Play3DSound(SoundManager.eAudioClip.FOOTSTEP4, e.gameObject.transform.position, 8f, 1f);
                        e.m_footstepTurn = 0;
                    }

                    e.m_footstepTimer = 0f; //초기화
                }
                #endregion

		//목적지에 거의 다가왔다면 멈춰준 후, 다시 IDLE 상태로 변경해준다.
                if (e.m_navAgent.remainingDistance <= e.m_navAgent.stoppingDistance)
                {
                    e.m_navAgent.ResetPath();
                    e.ChangeState(Enemy_IDLE.Instance);
                }
            }
        }
        else //플레이어를 감지했을 경우
        {
            e.ChangeState(Enemy_IDLE.Instance); //플레이어를 감지했으면 IDLE을 거쳐 RUN 혹은 ATTACK 상태로 변경되게 될 것임.
        }
    }

    //  상태 종료..
    public void Exit(Enemy_StateManager e)
    {
        e.m_navAgent.ResetPath(); //path 초기화
        e.m_anim.SetBool("ISWALK", false);
        e.m_footstepCycle = 0f;
    }
}
```

</div>
</details>

<details>
<summary>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;FSM State Attack 접기/펼치기</summary>
<div markdown="1">

```c#
public class Enemy_ATTACK : FSMSingleton<Enemy_ATTACK>, IFSMState<Enemy_StateManager> //ATTACK상태 클래스 - 싱글턴 및 IFSMState 인터페이스 상속
{
    //  상태 진입..
    public void Enter(Enemy_StateManager e)
    {
    
    }

    //  상태 진행..
    public void Execute(Enemy_StateManager e)
    {
        if(e.SearchTarget()) //플레이어를 감지했을 경우
        {
            Vector3 dir = e.m_player.transform.position - e.gameObject.transform.position; //플레이어의 위치 방향을 구한다.
	    //보간을 이용해 부드럽게 플레이어를 바라보게 한다.
            e.gameObject.transform.forward = Vector3.Lerp(e.gameObject.transform.forward, new Vector3(dir.x, 0f, dir.z), Time.deltaTime * 3f);

            if (!e.m_info.IsName("RELOAD")) //재장전 중이 아닐 경우
            {
                if (e.canAttack()) //플레이어가 공격 가능 거리내에 있을 경우
                {
                    e.m_anim.SetBool("ISATTACK", true); //공격 애니메이션 작동 -> 애니메이션 Event로 Enemy_StateManager의 Fire() 메소드를 호출함.
                }
                else
                {
                    e.m_idleTime = 1f;//IDLE로 넘어갔다가, 빠르게 재추적(RUN)하도록 시간을 1로 세팅해둠.
                    e.ChangeState(Enemy_IDLE.Instance); //IDLE로 상태변경
                }
            }
        }
        else //플레이어를 감지하지 못했을 경우
        {
            e.ChangeState(Enemy_IDLE.Instance); //IDLE로 상태변경
        }
    }

    //  상태 종료..
    public void Exit(Enemy_StateManager e)
    {
        e.m_anim.SetBool("ISATTACK", false);
    }
}
```

</div>
</details>

<details>
<summary>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;FSM State Run 접기/펼치기</summary>
<div markdown="1">

```c#
public class Enemy_RUN : FSMSingleton<Enemy_RUN>, IFSMState<Enemy_StateManager> //RUN상태 클래스 - 싱글턴 및 IFSMState 인터페이스 상속
{
    //  상태 진입..
    public void Enter(Enemy_StateManager e)
    {
        e.m_navAgent.speed = 2.5f; //속도를 RUN에 맞게 설정
        e.m_footstepCycle = 0.5f; //속도에 맞게 발소리 재생간격 설정
    }

    //  상태 진행..
    public void Execute(Enemy_StateManager e)
    {
        if (e.SearchTarget()) //플레이어를 감지했을 경우
        {
            e.m_anim.SetBool("ISRUN", true); //달리기 애니메이션 작동
            e.m_navAgent.SetDestination(e.m_player.transform.position); //플레이어 위치로 이동시켜준다.

            #region Footstep
            e.m_footstepTimer += Time.deltaTime; //발소리 재생간격 시간초 카운트

            if (e.m_footstepTimer > e.m_footstepCycle)
            {
                if (e.m_footstepTurn == 0) //왼발
                {
                    SoundManager.Instance.Play3DSound(SoundManager.eAudioClip.FOOTSTEP3, e.gameObject.transform.position, 20f, 1f);
                    e.m_footstepTurn = 1;
                }
                else if (e.m_footstepTurn == 1) //오른발 발소리 클립 재생
                {
                    SoundManager.Instance.Play3DSound(SoundManager.eAudioClip.FOOTSTEP4, e.gameObject.transform.position, 20f, 1f);
                    e.m_footstepTurn = 0;
                }

                e.m_footstepTimer = 0f;
            }
            #endregion

            if (e.canAttack()) //플레이어가 공격 가능 거리내에 있을 경우
            {
                e.ChangeState(Enemy_IDLE.Instance); //IDLE로 상태 변경
            }
        }
        else //플레이어를 감지하지 못했을 
        {
            e.ChangeState(Enemy_IDLE.Instance); //IDLE로 상태 변경
        }
    }

    //  상태 종료..
    public void Exit(Enemy_StateManager e)
    {
        e.m_navAgent.ResetPath();
        e.m_anim.SetBool("ISRUN", false);
        e.m_footstepCycle = 0f;
        e.m_idleTime = 1.5f; //대기시간없이 바로 attack 상태로 전이되게끔 유도.
    }
}
```

</div>
</details>

<details>
<summary>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;FSM State Die 접기/펼치기</summary>
<div markdown="1">

```c#
public class Enemy_DIE : FSMSingleton<Enemy_DIE>, IFSMState<Enemy_StateManager> //DIE상태 클래스 - 싱글턴 및 IFSMState 인터페이스 상속
{
    //  상태 진입..
    public void Enter(Enemy_StateManager e)
    {
        e.RagdollOnOff(false); //래그돌을 동작시키기 위해서, 모든 리지드바디들의 isKinematic을 false로 설정. 
        GameManager.Instance.AddScore(50); //플레이어가 적을 죽였으니 50점 추가.
    }

    //  상태 진행..
    public void Execute(Enemy_StateManager e)
    {
    	//5초뒤에 비활성화시켜줌.
        e.m_dieTime += Time.deltaTime; 

        if(e.m_dieTime >= 5f)
        {
            e.gameObject.SetActive(false);
        }
    }

    //  상태 종료..
    public void Exit(Enemy_StateManager e)
    {
        
    }
}
```

</div>
</details>

</div>
</details>

<br>

<details>
<summary>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;인질 FSM Code 접기/펼치기</summary>
<div markdown="1">

```c#
public class Hostage_Controller : MonoBehaviour
{
    #region Field
    public enum eState //인질이 가질 수 있는 상태들을 열거형으로 정리.
    {
        TIED,
        RESCUE,
        IDLE,
        RUN,
        MAX
    }

    eState m_state; //현재 상태
    Animator m_anim; //애니메이터
    NavMeshAgent m_nav;
    AnimatorStateInfo m_info; //현재 동작중인 애니메이션 정보
    GameObject m_player; //플레이어
    float m_disFromPlayer; //플레이어와의 유지거리
    float m_footstepTimer; //발소리 m_footstepCycle 마다 발소리를 재생시키기 위해 시간초를 카운팅하는 변수. 
    float m_footstepCycle; //발소리 재생 간격
    int m_footstepTurn; //왼발, 오른발 번갈아가며 재생하기 위함.
    #endregion

    #region Unity Methods
    void Start()
    {
        m_anim = GetComponent<Animator>();
        m_nav = GetComponent<NavMeshAgent>();
        m_disFromPlayer = 3f; //유지거리 설정
        m_state = eState.TIED; //묶여있는 상태로 초기화
        m_footstepTimer = 0f;
        m_footstepCycle = 0.5f; //인질의 이동속도에 맞춰 재생간격 설정
        m_footstepTurn = 0;
    }

    void Update()
    {
        if(m_player != null && m_state == eState.TIED) //인질이 묶여있는 상태이고, 플레이어가 감지되었을 경우
        {
            if (Input.GetKey(KeyCode.F)) //F키를 누르고 있을 경우
            {
                if (UIManager.Instance.ProgressBarFill(0.2f)) //진행바의 이미지를 서서히 채워준다. 만약 진행바가 전부 채워졌다면 내부로 진입
                {
                    m_state = eState.RESCUE; //구출된 상태로 변경
                    UIManager.Instance.ProgressObjOnOff(false); //진행바를 비활성화
                    SoundManager.Instance.Play2DSound(SoundManager.eAudioClip.UNTIE_HOSTAGE, 1f); //밧줄을 풀어주는 사운드 재생.
                }
            }
            else //F키에서 손을 뗐을 경우
            {
                UIManager.Instance.ProgressBarFill(-0.2f); //진행바의 이미지를 서서히 비워준다.
            }
        }

        switch(m_state) //FSM
        {
            case eState.TIED: //묶여있는 상태
                break;

            case eState.RESCUE: //구출된 직후 상태
                m_anim.SetTrigger("ISRESCUE"); //구출 애니메이션 작동

                m_info = m_anim.GetCurrentAnimatorStateInfo(0); //현재 진행중인 애니메이션 정보를 받아옴.
                if (m_info.IsName("Hostage_IDLE")) //만약 구출 애니메이션 작동이 끝나고, IDLE 애니메이션으로 넘어간 상태일 경우
                {
                    GameManager.Instance.AddScore(500); //500점 추가.
                    GameManager.Instance.ActiveNextWave(); //탈출 시 만나게될 새로운 적들을 활성화시키고, SafeZone 파티클시스템을 활성화시켜줌.
                    m_state = eState.IDLE; //IDLE상태로 변경
                }
                break;

            case eState.IDLE: //Idle 상태
                m_nav.ResetPath(); //path 초기화
                Vector3 dir = m_player.transform.position - gameObject.transform.position; //플레이어의 위치 방향을 구함.
		//보간을 이용해 부드럽게 플레이어를 바라보도록 함.
                gameObject.transform.forward = Vector3.Lerp(gameObject.transform.forward, new Vector3(dir.x, 0f, dir.z), Time.deltaTime * 3f);

		//플레이어와의 거리가 유지거리 이상으로 멀어질 경우
                if (Vector3.Distance(gameObject.transform.position, m_player.transform.position) > m_disFromPlayer)
                {
                    m_state = eState.RUN; //RUN 상태로 변경한다.
                }
                break;

            case eState.RUN: //달리기 상태
                m_anim.SetBool("ISRUN", true); //달리기 애니메이션 작동
                m_nav.SetDestination(m_player.transform.position); //플레이어의 위치로 이동시켜줌.

                #region Footstep
                m_footstepTimer += Time.deltaTime; 

                if (m_footstepTimer > m_footstepCycle) //발소리 재생간격 시간초 카운트
                {
                    if (m_footstepTurn == 0) //왼발
                    {
                        SoundManager.Instance.Play3DSound(SoundManager.eAudioClip.FOOTSTEP3, gameObject.transform.position, 10f, 1f);
                        m_footstepTurn = 1;
                    }
                    else if (m_footstepTurn == 1) //오른발 클립 재생
                    {
                        SoundManager.Instance.Play3DSound(SoundManager.eAudioClip.FOOTSTEP4, gameObject.transform.position, 10f, 1f);
                        m_footstepTurn = 0;
                    }

                    m_footstepTimer = 0f;
                }
                #endregion

		//플레이어와의 거리가 유지거리 내일 경우
                if (Vector3.Distance(gameObject.transform.position, m_player.transform.position) <= m_disFromPlayer)
                {
                    m_anim.SetBool("ISRUN", false); //달리기 애니메이션 중지
                    m_state = eState.IDLE; //Idle 상태로 변경
                }
                break;

            default:
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
    	//인질이 묶여있는 상태이며, 플레이어가 인질의 trigger 콜라이더 내에 진입했을 경우
        if ((other.gameObject.layer.Equals(LayerMask.NameToLayer("Player")) || other.gameObject.name.Equals("Player")) && m_state == eState.TIED )
        {
            m_player = other.transform.root.gameObject; //m_player가 구출가능 거리로 다가왔음을 알리기 위함.
            UIManager.Instance.ProgressObjOnOff(true); //구출 진행바를 활성화
        }
    }

    private void OnTriggerExit(Collider other)
    {
    	//인질이 묶여있는 상태이며, 플레이어가 인질의 trigger 콜라이더 바깥으로 나갔을 경우
        if ((other.gameObject.layer.Equals(LayerMask.NameToLayer("Player")) || other.gameObject.name.Equals("Player")) && m_state == eState.TIED)
        {
	    m_player = null; //m_player가 구출가능 거리가 아니라는것을 알리기 위함.
            UIManager.Instance.ProgressObjOnOff(false); //구출 진행바를 비활성화
        }
    }
    #endregion
}
```

</div>
</details>

<br>

**Explanation**:wrench:<br>
(구현설명은 주석으로 간단하게 처리했습니다!)<br>
적 AI 같은 경우, 상태들을 클래스로 관리하는 FSM으로 구현했습니다. 각 상태들은 싱글턴 패턴을 적용해 상태변환 시마다 new, delete가 난무하는 것을 예방함으로써, 오버헤드와 메모리 낭비를 방지하고자 했습니다. 또한 각 상태가 동일한 메소드(동작)를 포함하도록 강제하고, 다중상속 문제를 해결하며 다형성을 이용하기 위해 IFSM 인터페이스를 구현하도록 했습니다. 이러한 구조를 이용하니 클래스 간 느슨한 결합도를 유지할 수 있어서 Open-Closed Principle(확장에는 열리게 하고, 수정에는 닫히게 해야 한다는 객체지향 원칙)을 지킬 수 있었습니다. 또한, 구조가 심플하다 보니 개발하는 데 있어서 좀 더 수월해지는 이점도 존재했습니다.

인질 같은 경우 정말 간단한 상태만을 갖는 AI로 구현하면 됐기 때문에, 위처럼 개발하는 것이 오히려 낭비라고 생각하여서 한 class내에서 switch-case문으로 간단하게 구현하였습니다.

```c#
//FSM
protected void  FSMUpdate() 
{ 
	if (m_currentState != null)
	{
		m_currentState.Execute(owner); //Interface로 인한 다형성
	}
}

...

//상태매니저
void Update()
{
	FSMUpdate(); //현재 상태의 Execute() 실행
}
```

</div>
</details>

<br>

<details>
<summary>Managers Code 접기/펼치기</summary>
<div markdown="1">

<br>

<details>
<summary>&nbsp;&nbsp;&nbsp;&nbsp;SoundManager 접기/펼치기</summary>
<div markdown="1">

```c#
public class SoundManager : SingletonMonoBehaviour<SoundManager> //싱글턴패턴 
{
    #region Field
    public enum eAudioClip
    {
        FOOTSTEP1,
        FOOTSTEP2,
        FOOTSTEP3,
        FOOTSTEP4,
        JUMP,
        LAND,
        AKM_SHOOT,
        M4_SHOOT,
        GLOCK_SHOOT,
        AKM_DRAW,
        GLOCK_DRAW,
        AKM_RELOAD,
        GLOCK_RELOAD,
        AIM_IN,
        GRUNT1,
        GRUNT2,
        GRUNT3,
        GRUNT4,
        ENEMY_DEATH1,
        ENEMY_DEATH2,
        ENEMY_DEATH3,
        HITSOUND,
        HEADSHOT,
        SUPPLY,
        PLAYER_DEATH,
        M4_RELOAD,
        ATW_EXPLOSION1,
        ATW_EXPLOSION2,
        ATW_IMPACTONTGROUND,
        ATW_THROW,
        SUPPLYBOX_OPEN,
        SUPPLYBOX_CLOSE,
        ATW_CHANGE,
        HEALTH_SUPPLY,
        UNTIE_HOSTAGE,
        FLASH_ON,
        FLASH_OFF,
        BUTTON,
        MAX
    }

    [SerializeField]
    AudioClip[] m_clips; //오디오 클립들
    [SerializeField]
    AudioSource m_2DSoundSource; //PlayOneShot Method 전용
    [SerializeField] 
    AudioSource m_2DSoundSource_Play; //Play Method 전용
    [SerializeField]
    AudioSource m_BGMSource;
    [SerializeField]
    GameObject m_objPoolManager; //오브젝트풀 매니저 하위에 모든 풀링 오브젝트들이 들어가있음.
    [SerializeField]
    AudioMixer m_audioMixer; //볼륨관리를 위한 오디오믹서
    List<AudioSource> m_pausedAudios = new List<AudioSource>(); //일시중지된 오디오소스들을 담아놓는 리스트.
    #endregion

    #region Unity Methods
    void Start()
    {
        BGMAudioControl(PlayerPrefs.GetFloat("BGMVolume")); //PlayerPrefs에 저장된 볼륨크기로 초기화
        EffectAudioControl(PlayerPrefs.GetFloat("SFXVolume")); //PlayerPrefs에 저장된 볼륨크기로 초기화
    }
    #endregion

    #region Public Methods
    public void EffectAudioControl(float volume) //UI슬라이더에 연결된 메소드 -> Effect 볼륨조절
    {
        if(volume == -40f)
        {
            m_audioMixer.SetFloat("SFXVolume", -80); //Mute 해주기위함
        }
        else
        {
            m_audioMixer.SetFloat("SFXVolume", volume);
        }

        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }

    public void BGMAudioControl(float volume) //UI슬라이더에 연결된 메소드 -> BGM 볼륨조절
    {
        if (volume == -40f)
        {
            m_audioMixer.SetFloat("BGMVolume", -80); //Mute 해주기위함
        }
        else
        {
            m_audioMixer.SetFloat("BGMVolume", volume);
        }

        PlayerPrefs.SetFloat("BGMVolume", volume);
        PlayerPrefs.Save();
    }
    
    //오버로딩 eAudioClip
    public void Play2DSound(eAudioClip clip, float volume)
    {
        m_2DSoundSource.PlayOneShot(m_clips[(int)clip], volume);
    }

    //오버로딩 int
    public void Play2DSound(int clip, float volume)
    {
        m_2DSoundSource.PlayOneShot(m_clips[clip], volume);
    }

    //Play Method 전용 -> Reload, Draw 를 연달아서 실행할 경우 사운드가 끊기게 하기 위함.(중복재생방지)
    public void Play2DSound_Play(int clip, float volume)
    {
        m_2DSoundSource_Play.clip = m_clips[clip];
        m_2DSoundSource_Play.volume = volume;
        m_2DSoundSource_Play.Play();
    }

    //3D 사운드 재생
    public void Play3DSound(eAudioClip clip, Vector3 pos, float maxDistance, float volume)
    {
        var obj = ObjPool.Instance.m_audioPool.Get(); //오브젝트 풀에서 3D오디오소스가 부착된 게임오브젝트를 꺼냄.

        if (obj != null)
        {
            AudioSource audio = obj.gameObject.GetComponent<AudioSource>(); //오디오소스 Get
            audio.priority = 128;
            audio.clip = m_clips[(int)clip]; //클립 지정

            obj.transform.position = pos; //재생시킬 위치 지정
            audio.maxDistance = maxDistance; //최대거리 

            obj.gameObject.SetActive(true); //오브젝트를 활성화
            audio.Play(); //재생
            obj.ReturnInvoke(m_clips[(int)clip].length); //오디오 클립의 길이만큼 대기 후 풀에 반환.
        }
    }

    //오버로딩 int
    public void Play3DSound(int clip, Vector3 pos, float maxDistance, float volume)
    {
        var obj = ObjPool.Instance.m_audioPool.Get(); //오브젝트 풀에서 3D오디오소스가 부착된 게임오브젝트를 꺼냄

        if (obj != null)
        {
            AudioSource audio = obj.gameObject.GetComponent<AudioSource>(); //오디오소스 Get
            audio.priority = 128;
            audio.clip = m_clips[clip];  //클립 지정

            obj.transform.position = pos; //재생시킬 위치 지정
            audio.maxDistance = maxDistance; //최대거리

            obj.gameObject.SetActive(true); //오브젝트를 활성화
            audio.Play(); //재생
            obj.ReturnInvoke(m_clips[clip].length); //오디오 클립의 길이만큼 대기 후 풀에 반환.
        }
    }
    
    public void StopSound() //TimeScale == 0 일 경우 재생중이던 모든 오디오 중지.
    {
        if (m_2DSoundSource.isPlaying) //재생중이라면
        {
            m_2DSoundSource.Pause(); //재생 중지
            m_pausedAudios.Add(m_2DSoundSource); //일시중지된 오디오소스 리스트에 추가
        }
        if (m_2DSoundSource_Play.isPlaying) //재생중이라면
        {
            m_2DSoundSource_Play.Pause(); //재생 중지
            m_pausedAudios.Add(m_2DSoundSource_Play); //일시중지된 오디오소스 리스트에 추가
        }
        if (m_BGMSource.isPlaying)  //재생중이라면
        {
            m_BGMSource.Pause(); //재생 중지
            m_pausedAudios.Add(m_BGMSource); //일시중지된 오디오소스 리스트에 추가
        }
	
        //오브젝트풀 매니저 하위에 들어가 있는 3D오디오소스 오브젝트 중 활성화된, 즉 재생 중이던 것들만 가져옴.
        var sources = m_objPoolManager.GetComponentsInChildren<AudioSource>();

        foreach(AudioSource audio in sources)
        {
            audio.Pause(); //일시중지
	    m_pausedAudios.Add(audio); //일시중지된 오디오소스 리스트에 추가
        }
    }

    public void ReStartSound() //TimeScale == 1 일 경우 일시중지되었던 모든 오디오를 다시 재생시켜준다.
    {
        //퍼지된 오디오소스만 리스트에 저장해두었다가 재생시킴.
        for (int i=0; i<m_pausedAudios.Count; i++)
        {
            m_pausedAudios[i].Play();
        }
	
        m_pausedAudios.Clear(); //클리어
    }
    #endregion
}
```

</div>
</details>

<br>

<details>
<summary>&nbsp;&nbsp;&nbsp;&nbsp;GameManager 접기/펼치기</summary>
<div markdown="1">

```c#
public class GameManager : SingletonMonoBehaviour<GameManager>
{
    #region Field
    public enum eGameState //게임의 현재상태들.
    {
        Normal, //일반
        Pause, //일시중지
        PlayerDead, //플레이어사망
        Success, //미션성공
        Max
    }

    eGameState m_state; //게임의 현재상태
    [SerializeField]
    GameObject m_player; //플레이어
    [SerializeField]
    GameObject m_camera; //플레이어 카메라
    [SerializeField]
    GameObject m_WaitBox; //게임시작 전 대기장소
    GameObject m_failViewCam; //플레이어 사망 시 비춰줄 카메라
    Vector3 FailViewPosition;
    Player_StateManager m_playScr; //플레이어 컨트롤러
    CameraRotate m_camScr; //화면 컨트롤러
    WeaponSway m_sway; //총의 흔들림 즉 Sway 스크립트
    bool m_isStart; //게임이 시작된 상태인지

    int m_time; //타이머
    int m_score; //점수
    public GameObject[] m_wave2Enemys; //인질 구출 후 새로 활성화 될 적들
    public GameObject m_RescuePoint; //인질 구출 지점
    #endregion

    #region Unity Methods
    protected override void OnStart()
    {
        m_isStart = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; //커서를 감춘다.

        m_playScr = m_player.GetComponent<Player_StateManager>();
        m_camScr = m_camera.GetComponent<CameraRotate>();
        m_sway = m_player.GetComponentInChildren<WeaponSway>();

        m_time = 0;
        m_score = 0;

        Time.timeScale = 1;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && m_isStart && m_state != eGameState.PlayerDead) //게임도중 ESC를 눌렀을 시 timescale을 이용해 일시정지 해준다.
        {
            if (Time.timeScale == 0)
            {
                SetState(eGameState.Normal);
                Time.timeScale = 1;
            }
            else
            {
                SetState(eGameState.Pause);
                Time.timeScale = 0;
            }
        }
    }
    #endregion

    #region Public Methods

    public void HoldPlayer() //플레이어 행동 불가
    {
        m_playScr.enabled = false; //움직이지 못하고
        m_camScr.enabled = false; //화면을 돌리지 못하며
        m_sway.enabled = false; //총기도 움직이지 않는다.
    }

    public void ReleasePlayer() //플레이어 행동 가능
    {
        m_playScr.enabled = true;
        m_camScr.enabled = true;
        m_sway.enabled = true;
    }

    public void ActiveNextWave() //인질 구출 시 다음 wave 적들을 활성화 시켜주고, 구출포인트를 활성화
    {
        foreach (GameObject obj in m_wave2Enemys)
        {
            obj.SetActive(true); //적 활성화
        }

        m_RescuePoint.SetActive(true); //구출지점 활성화
    }

    public void GameStart() //게임 시작
    {
        m_isStart = true;
        m_WaitBox.SetActive(false);
        StartCoroutine("Timer"); //타이머 start
    }
    
    public bool GetIsStart() //게임이 시작된 상태인지 반환
    {
        return m_isStart;
    }

    public void AddScore(int score) //해당 게임의 점수를 더해준다.
    {
        m_score += score;
    }

    public eGameState GetState() //현재 게임 상태 반환
    {
        return m_state;
    }

    public void SetState(eGameState state) //현재 게임 상태 지정
    {
        if (m_state == state) //같은 상태로 지정한다면 return
        {
            return;
        }

        m_state = state;

        switch (m_state) //지정된 상태에 맞게 처리
        {
            case eGameState.Normal: //일반 상태
                Cursor.lockState = CursorLockMode.Locked; //커서를 화면중앙에 위치시키고 고정
                Cursor.visible = false; //커서 감추기
                ReleasePlayer(); //플레이어 행동 가능
                SoundManager.Instance.ReStartSound(); //중지되었던 사운드들 다시 재생
                UIManager.Instance.CloseMenu(); //메뉴 닫아주기
                break;

            case eGameState.Pause: //일시중지 상태
                Cursor.lockState = CursorLockMode.None; //잠금해제
                Cursor.visible = true; //커서 보여주기
                HoldPlayer(); //플레이어 행동 불가
                SoundManager.Instance.StopSound(); //재생중이던 사운드들 일시 중지
                UIManager.Instance.OpenMenu(); //메뉴 열어주기
                break;

            case eGameState.PlayerDead: //플레이어 사망 상태
                StopCoroutine("Timer"); //타이머 중단
                UIManager.Instance.GameResult(false, m_time, m_score); //게임 결과 화면 활성화 -> 시간과 점수를 넘겨줌
                StartCoroutine("FailView"); //플레이어 사망 시 카메라 뷰 
                m_player.gameObject.SetActive(false); //플레이어를 비활성화
                break;

            case eGameState.Success: //미션 성공 상태
                StopCoroutine("Timer"); //타이머 중단
                UIManager.Instance.CloseMenu(); //혹시 켜져있을 메뉴 닫아주기
                UIManager.Instance.GameResult(true, m_time, m_score); //게임 결과 화면 활성화 -> 시간과 점수를 넘겨줌
                Cursor.lockState = CursorLockMode.None; //커서 잠금해제
                Cursor.visible = true; //커서 보여주기
		HoldPlayer(); //플레이어 행동 불가
                m_player.layer = LayerMask.NameToLayer("Default"); //적들이 공격하지 못하도록 레이어를 일시적으로 변경
                break;
        }
    }
    #endregion

    #region Coroutine
    IEnumerator FailView() //플레이어 사망 시 카메라 View
    {
        m_failViewCam = new GameObject("FailViewCam"); //게임오브젝트 생성
        m_failViewCam.gameObject.AddComponent<Camera>(); //카메라 컴포넌트 부착
        m_failViewCam.gameObject.AddComponent<AudioListener>(); //오디오리스너 부착 -> 플레이어가 사망 할 시 비활성화 되기 때문에 오디오리스너가 존재하지않게됨.

        m_failViewCam.gameObject.transform.position = m_camera.transform.position; //플레이어 카메라 위치로 이동
        m_failViewCam.gameObject.transform.eulerAngles = new Vector3(0f, m_camera.transform.eulerAngles.y, 0f); //플레이어가 바라보던 각도로 설정

	//최종포지션
        FailViewPosition = new Vector3(m_failViewCam.transform.position.x, m_failViewCam.transform.position.y + 2f, m_failViewCam.transform.position.z);

        while (true)
        {
	    //보간을 이용해서 부드럽게 플레이어의 몸체를 바라보도록 회전
            m_failViewCam.transform.rotation = Quaternion.Slerp(m_failViewCam.transform.rotation, Quaternion.Euler(90f, m_failViewCam.transform.eulerAngles.y, 0f), Time.deltaTime * 3f);
	    //몸체를 위에서 내려다 볼 수 있게 포지션 이동
            m_failViewCam.transform.position = Vector3.Lerp(m_failViewCam.transform.position, FailViewPosition, Time.deltaTime * 2f);
            yield return null;
		
	    //최종포지션에 도착했다면 코루틴 중지
            if(m_failViewCam.transform.position == FailViewPosition)
            {
                yield break;
            }
        }
    }

    IEnumerator Timer() //타이머
    {
        while(true)
        {
            yield return new WaitForSeconds(1f);

            m_time++;
        }
    }

    #endregion
}
```

</div>
</details>

<br>

**Explanation**:mortar_board:<br>
(구현설명은 주석으로 간단하게 처리했습니다!)<br>
<br>
*SoundManager*<br>
싱글턴패턴을 적용한 SoundManager 같은 경우 모든 사운드클립 및 오디오소스를 관리하며 재생 및 중단을 수행하게끔 정리했습니다. 이를 통해 사운드 재생이 필요한 곳에서 쉽게 참조하여 간편하게 사용할 수 있었으며, 난무하는 오디오소스 컴포넌트를 방지할 수 있었습니다. 특히 3D사운드 재생에 관련해서는 생성, 삭제 과정의 오버헤드를 방지하기 위해 3D오디오소스를 부착한 오브젝트를 Object Pooling으로 관리하면서 사용합니다. 또한 오디오 과부하를 방지하기 위해 Object Pool에서 최대로 생성될 수 있는 갯수를 제한해, 동시에 재생될 수 있는 3D오디오소스 수를 제한했습니다.<br>

*GameManager*<br>
싱글턴패턴을 적용한 GameManager 같은 경우 현재 게임이 가질 수 있는 모든 상태를 열거형으로 선언한 후, FSM 방식으로 한 번에 하나의 상태만을 유지하며 해당 상태에서 처리해야 할 작업을
수행하게끔 구현했습니다.

<br>

</div>
</details>

<br>

<details>
<summary>Optimization 접기/펼치기</summary>
<div markdown="1">

<br>

**Explanation**:scissors:<br>
* LightMap을 사용해, 실시간 조명연산을 최소화 시키고자 했습니다.
* Occlusion Culling을 이용하여, 렌더링 작업을 최적화 시키고자 했습니다.
* Object Pooling 기법을 사용하여, 반복되는 생성-삭제 작업으로인한 GC의 잦은 호출을 방지했습니다.
* Atlas를 사용하여 드로우콜을 낮추고자 했습니다.

<br>

</div>
</details>

<br>

### Difficult Point.:sweat_smile:
* Unity를 이용해 게임 프로젝트를 처음 진행하다 보니, 설계에 있어 어려움을 겪었습니다. 이런저런 자료들을 많이 참고해봤지만, 직접 경험해보지 않아서인지
와닿지 않았습니다. 그래서 이번 프로젝트만큼은 주먹구구식으로 덤벼들었던 것 같습니다. 그 결과 프로젝트를 검토해보니 중복되는 코드, 비효율적인 구조 등을
발견하게 되었습니다. 하지만, 이번 프로젝트를 경험으로 적어도 Class들의 설계를 어떤 식으로 진행해야 할지 약간의 감을 잡을 수 있었고, 설계의 중요성을 다시 한번 느낄 수 있었습니다.

* 앞서 말씀드렸다시피, 처음이었기 때문에 Unity 엔진의 기능들, MonoBehaviour 클래스에서 제공하는 기능들을 배우는 시간이 개발하는 시간보다 배로 많았던 점이 힘들었습니다. 하지만, 많은 기능을 알게 되었고 직접 사용해 볼 수 있었던 뜻깊은 경험이 되었으며, 게임 개발에 있어 자신감을 불어 넣어준 계기가 되기도 했습니다.

<br>
<br>



메인화면 이미지 출처 https://1freewallpapers.com/point-blank-swat-game/ko
