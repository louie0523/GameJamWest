using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NUnit.Framework;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Move : MonoBehaviour
{
    public static Move instance;


    public Transform[] AttackPoints;
    public ParticleSystem[] particles;
    public GameObject[] Gurns;

    public bool Voicing = false;

    public Transform AttackPoint;
    public float forwardSpeed = 8f;
    public float AddSpeed = 0f;
    public float strafeSpeed = 3f;
    public float jumpForce = 5f;
    public int MaxHp;
    public int Hp;
    public float MaxStamina = 100;
    public float Stamina;
    public Color StaminaRunAble;
    public Color StaminaRunNotAble;
    public Image StaminaFill;
    public Slider StaminaSlider;
    public Slider HpSlider;
    public GameObject bulletImage;
    public GameObject HorizontalLayoutGroup;
    List<GameObject> bulletIcons = new List<GameObject>();
    public bool isAlive = true;

    private float hoofSoundTimer = 0f;
    public float hoofInterval = 0.4f;
    private bool hoofToggle = false;

    public GameObject Bullet;
    public Transform BulletPos;
    public Camera Guner;

    public GunStatus MyGunSO;  // SO 직접 참조 (읽기 전용)

    // 런타임 상태 복사본
    private int currentRange;
    private int maxRange;
    private int damage;
    private float attackSpeed;

    public bool isAttackDelay = false;
    public bool isReloading = false;

    public CinemachineBasicMultiChannelPerlin GunnerCamera;
    bool run = false;

    private Rigidbody rb;
    private float horizontalInput;
    private bool isGrounded;

    public Animator animator;
    public Animator Gunner;

    public List<ParticleSystem> GunParticle = new List<ParticleSystem>();

    float lastRunEndTime = -1f;
    float lastJumpTime = -1f;
    float zeroStaminaTime = -10f;
    bool staminaWasZero = false;
    bool isRunBlocked = false;
    float runBlockEndTime = -1f;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (GameManager.instance != null)
            MyGunSO = GameManager.instance.CurretnGun;

        Hp = MaxHp;
        Stamina = MaxStamina;
        Gunner.SetInteger("Gun", MyGunSO.num);
        for(int i =0; i < Gurns.Length; i++)
        {
            if(i == MyGunSO.num)
            {
                Gurns[i].SetActive(true);
                GunParticle.Clear();
                GunParticle.Add(particles[i]);
                AttackPoint = AttackPoints[i];
            } else
            {
                Gurns[i].SetActive(false);
            }
        }
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            Debug.LogError("Rigidbody 컴포넌트가 필요합니다!");

        bulletIcons.Clear();
        int childCount = HorizontalLayoutGroup.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            bulletIcons.Add(HorizontalLayoutGroup.transform.GetChild(i).gameObject);
        }

        // MaxRange 초과 아이콘은 삭제 또는 비활성화 처리
        if (bulletIcons.Count > MyGunSO.MaxRange)
        {
            for (int i = bulletIcons.Count - 1; i >= MyGunSO.MaxRange; i--)
            {
                Destroy(bulletIcons[i]);
                bulletIcons.RemoveAt(i);
            }
        }

        // SO 데이터 복사 (런타임 중 변경은 여기서 관리)
        maxRange = MyGunSO.MaxRange;
        currentRange = maxRange;
        damage = MyGunSO.damage;
        attackSpeed = MyGunSO.AttackSpeed;

        UpdateBulletIcons();
    }

    void UpdateBulletIcons()
    {
        for (int i = 0; i < bulletIcons.Count; i++)
        {
            bulletIcons[i].SetActive(i < currentRange);
        }
    }

    void Update()
    {
        if(isAlive)
        {
            horizontalInput = 0f;
            if (Input.GetKey(KeyCode.A)) horizontalInput = -1f;
            if (Input.GetKey(KeyCode.D)) horizontalInput = 1f;

            if (Input.GetKeyDown(KeyCode.Mouse0)) Shut();

            bool isShiftHeld = Input.GetKey(KeyCode.LeftShift);
            bool runKeyHeld = isShiftHeld && !isRunBlocked;
            bool canRun = runKeyHeld && Stamina > 0f;

            if (canRun)
            {
                Run();

                Stamina -= 40f * Time.deltaTime;
                if (Stamina < 0)
                    Stamina = 0;

                if (Stamina <= 0f && !isRunBlocked)
                {
                    isRunBlocked = true;
                    runBlockEndTime = Time.time + 3f;
                    zeroStaminaTime = Time.time;
                    staminaWasZero = true;
                }
            }
            else
            {
                if (run)
                {
                    lastRunEndTime = Time.time;
                }

                run = false;
                AddSpeed = 0f;
                GunnerCamera.AmplitudeGain = 0.25f;

                if (isRunBlocked && Time.time >= runBlockEndTime)
                {
                    isRunBlocked = false;
                }

                float timeSinceZero = Time.time - zeroStaminaTime;

                if (staminaWasZero && timeSinceZero <= 5f)
                {
                    Stamina += 10f * Time.deltaTime;
                }
                else
                {
                    Stamina += 20f * Time.deltaTime;
                    staminaWasZero = false;
                }

                if (Stamina > MaxStamina)
                    Stamina = MaxStamina;
            }

            MoveMent();
            HandleHoofSound();
            SliderUpdate();

            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                Jump();
            }
        }
       
    }

    void SliderUpdate()
    {
        HpSlider.value = Hp / (float)MaxHp;
        StaminaSlider.value = Stamina / MaxStamina;
        StaminaFill.color = isRunBlocked ? StaminaRunNotAble : StaminaRunAble;
    }

    void HandleHoofSound()
    {
        if (isGrounded && rb.linearVelocity.magnitude > 0.5f && isAlive)
        {
            hoofSoundTimer -= Time.deltaTime;
            if (hoofSoundTimer <= 0f )
            {
                string clipName = hoofToggle ? "H1" : "H2";
                SfxManager.Instance.Play(clipName);

                hoofToggle = !hoofToggle;
                hoofSoundTimer = hoofInterval;
            }
        }
        else
        {
            hoofSoundTimer = 0f;
        }
    }


    void Run()
    {
        run = true;
        AddSpeed = 4f;
        GunnerCamera.AmplitudeGain = 1.25f;
    }

    public void Damage(int damage)
    {
        if (isAlive)
        {
            Hp -= damage;
            hitVoice();
            StartCoroutine(GunnerShake(0.1f));
            SfxManager.Instance.Play("hit", 1f);
            if (Hp <= 0)
            {
                Debug.Log("플레이어가 사망하였습니다!");
                GameManager.instance.Death();
                isAlive = false;
            }
        }
    }

    IEnumerator GunnerShake(float time)
    {
        GunnerCamera.AmplitudeGain += 1.5f;
        yield return new WaitForSeconds(time);
        GunnerCamera.AmplitudeGain -= 1.5f;
    }

    void hitVoice()
    {
        if(!Voicing)
        {
            SfxManager.Instance.Play("voice", 1f);
            Voicing = true;
            Invoke("voiceFalse", 3.5f);
        }
    }

    void voiceFalse()
    {
        Voicing = false;
    }





    void Shut()
    {
        if (!isReloading && !isAttackDelay && currentRange > 0 && isAlive)
        {
            switch(MyGunSO.num )
            {
                case 0:
                    SfxManager.Instance.PlayAt("Rshot", this.transform.position, 1f);
                    break;
                case 1:
                    SfxManager.Instance.PlayAt("mauser", this.transform.position, 1f);
                    break;
            }
           
            currentRange--;

            UpdateBulletIcons();

            Gunner.SetTrigger("Fire");

            foreach (ParticleSystem particleSystem in GunParticle)
            {
                particleSystem.Play();
            }

            Ray ray = Guner.ScreenPointToRay(Input.mousePosition);
            Vector3 targetPoint;

            if (Physics.Raycast(ray, out RaycastHit hit, MyGunSO.MaxRange))
                targetPoint = hit.point;
            else
                targetPoint = ray.origin + ray.direction * 40f;

            Vector3 direction = (targetPoint - BulletPos.position).normalized;

            bool shouldApplyRunSpread = run || (Time.time - lastRunEndTime <= 0.5f);
            bool shouldApplyJumpSpread = (Time.time - lastJumpTime <= 0.5f);
            bool isUnstable = shouldApplyRunSpread || shouldApplyJumpSpread;

            float baseSpread = 0.01f;
            float runSpread = 0.25f;
            float currentSpread = isUnstable ? runSpread : baseSpread;

            direction += new Vector3(
                Random.Range(-currentSpread, currentSpread),
                Random.Range(-currentSpread, currentSpread),
                Random.Range(-currentSpread, currentSpread)
            );
            direction.Normalize();

            GameObject bullet = Instantiate(Bullet, BulletPos.position, Quaternion.LookRotation(direction));
            Bullet bulletcs = bullet.GetComponent<Bullet>();
            bulletcs.damage = damage;

            Debug.DrawLine(BulletPos.position, targetPoint, Color.red, 2f);

            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            bulletRb.useGravity = false;
            bulletRb.linearVelocity = direction * 60f;

            if (currentRange >= 1)
                StartCoroutine(AttackDelay(attackSpeed));
            else
                StartCoroutine(Reload());
        }
    }

    IEnumerator Reload()
    {
        Gunner.SetTrigger("Reload");
        isReloading = true;

        switch (MyGunSO.num)
        {
            case 0:
                yield return new WaitForSeconds(0.33333f);
                SfxManager.Instance.Play("Rreload", 1f);
                yield return new WaitForSeconds(3.3333f);
                break;
            case 1:
                SfxManager.Instance.Play("Mreload", 1f);
                yield return new WaitForSeconds(8.25f);
                break;
        }

        currentRange = maxRange;
        UpdateBulletIcons();
        isReloading = false;
    }

    IEnumerator AttackDelay(float time)
    {
        isAttackDelay = true;
        yield return new WaitForSeconds(time);
        isAttackDelay = false;
    }

    void MoveMent()
    {
        Vector3 forwardVelocity = transform.forward * (forwardSpeed + AddSpeed);
        Vector3 strafeVelocity = transform.right * horizontalInput * strafeSpeed;
        Vector3 finalVelocity = new Vector3(strafeVelocity.x, rb.linearVelocity.y, forwardVelocity.z);
        animator.SetBool("Run", run);

        rb.linearVelocity = finalVelocity;
    }

    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        animator.SetTrigger("Jump");
        isGrounded = false;

        lastJumpTime = Time.time;
    }



    private void OnCollisionEnter(Collision collision)
    {
        if (collision.contacts.Length > 0)
        {
            ContactPoint contact = collision.contacts[0];
            if (Vector3.Dot(contact.normal, Vector3.up) > 0.5f)
            {
                SfxManager.Instance.PlayAt("land", this.transform.position, 1f);
                isGrounded = true;
            }
        }
    }
}
