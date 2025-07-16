using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;

public class Move : MonoBehaviour
{
    public static Move instance;

    public float forwardSpeed = 8f;
    public float AddSpeed = 0f;
    public float strafeSpeed = 3f;
    public float jumpForce = 5f;
    public int Hp;

    private float hoofSoundTimer = 0f;
    public float hoofInterval = 0.4f; // 소리 간격 (속도에 따라 조정 가능)
    private bool hoofToggle = false;   // H1 / H2 번갈아가며


    public GameObject Bullet;
    public Transform BulletPos;
    public Camera Guner;
    public GunStatus MyGun;
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
    float lastJumpTime = -1f;   // 🆕 점프 시간 기록용


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Gunner.SetInteger("Gun", MyGun.num);
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            Debug.LogError("Rigidbody 컴포넌트가 필요합니다!");
    }

    void Update()
    {
        horizontalInput = 0f;
        if (Input.GetKey(KeyCode.A)) horizontalInput = -1f;
        if (Input.GetKey(KeyCode.D)) horizontalInput = 1f;

        if (Input.GetKeyDown(KeyCode.Mouse0)) Shut();

        if (Input.GetKey(KeyCode.LeftShift))
        {
            Run();
        }
        else
        {
            // ⬇️ 달리기 해제 시점 기록
            if (run)
                lastRunEndTime = Time.time;

            run = false;
            AddSpeed = 0f;
            GunnerCamera.AmplitudeGain = 0.25f;
        }

        MoveMent();
        HandleHoofSound();

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }

    void HandleHoofSound()
    {
        // 걷거나 뛸 때만 재생
        if (isGrounded && rb.linearVelocity.magnitude > 0.5f)
        {
            hoofSoundTimer -= Time.deltaTime;
            if (hoofSoundTimer <= 0f)
            {
                string clipName = hoofToggle ? "H1" : "H2";
                SfxManager.Instance.Play(clipName);

                hoofToggle = !hoofToggle;
                hoofSoundTimer = hoofInterval;
            }
        }
        else
        {
            // 공중에 있거나 멈췄을 때 타이머 초기화
            hoofSoundTimer = 0f;
        }
    }

    void Run()
    {
        run = true;
        AddSpeed = 4f;
        GunnerCamera.AmplitudeGain = 1.25f;
    }

    void Shut()
    {
        if (!isReloading && !isAttackDelay)
        {

            SfxManager.Instance.PlayAt("Rshot", this.transform.position, 1f);
            MyGun.Range--;

            Gunner.SetTrigger("Fire");

            foreach(ParticleSystem particleSystem in GunParticle)
            {
                particleSystem.Play();
            }

            // 1. 마우스 커서 위치 기준 Ray 쏘기
            Ray ray = Guner.ScreenPointToRay(Input.mousePosition);
            Vector3 targetPoint;

            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
                targetPoint = hit.point;
            else
                targetPoint = ray.origin + ray.direction * 40f;

            // 2. 총알 방향 계산
            Vector3 direction = (targetPoint - BulletPos.position).normalized;

            // ⬇️ 퍼짐 계산: run 중이거나, run 직후 0.5초 이내라면 높은 퍼짐
            // 퍼짐 적용 조건
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




            // 3. 총알 생성 및 방향 설정
            GameObject bullet = Instantiate(Bullet, BulletPos.position, Quaternion.LookRotation(direction));
            Bullet bulletcs = bullet.GetComponent<Bullet>();
            bulletcs.damage = MyGun.damage;

            Debug.DrawLine(BulletPos.position, targetPoint, Color.red, 2f);

            // 4. 총알 속도 적용
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            bulletRb.useGravity = false;
            bulletRb.linearVelocity = direction * 60f;


            if (MyGun.Range >= 1)
                StartCoroutine(AttackDelay(MyGun.AttackSpeed));
            else
                StartCoroutine(Reload());

            
        }
    }

    IEnumerator Reload()
    {
        Gunner.SetTrigger("Reload");
        isReloading = true;
        switch (MyGun.num)
        {
            case 0:
                yield return new WaitForSeconds(0.33333f);
                SfxManager.Instance.PlayAt("Rreload", transform.position, 1f);
                yield return new WaitForSeconds(3.3333f);
                MyGun.Range = 6;
                break;

        }
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

        lastJumpTime = Time.time;  // 🆕 점프 시간 저장
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.contacts.Length > 0)
        {
            ContactPoint contact = collision.contacts[0];
            if (Vector3.Dot(contact.normal, Vector3.up) > 0.5f)
            {
                isGrounded = true;
            }
        }
    }
}
