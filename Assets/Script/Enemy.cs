using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public enum EnemyType { Move, Sniper }

public class Enemy : MonoBehaviour
{
    public EnemyType enemyType;

    public bool AttackUse = true;
    public float Speed;
    public float downSpeed;
    public int Hp = 10;
    public bool isAlive = true;

    private NavMeshAgent agent;
    private Rigidbody rb;

    public int damage = 3;
    public GameObject bullet;
    public string bulletSound;
    public List<ParticleSystem> particleSystems = new List<ParticleSystem>();
    public Animator animator;

    public Transform foot;
    public float fenceCheckDistance = 2.8f;
    public float fenceJumpForce = 7f;
    public LayerMask fenceLayer;

    private bool isJumpingOverFence = false;

    public LineRenderer lineRenderer;
    private float lockedPlayerX = 0f;
    private bool isTrackingLine = false;
    private float shootTimer = 0f;
    public float shootInterval = 8f;
    float originalShootInterval;

    private float burstCooldownTimer = 0f;
    private float burstCooldown = 3f;

    private void Start()
    {
        originalShootInterval = shootInterval;
        if (enemyType == EnemyType.Move)
        {
            foot = transform.GetChild(1).transform;
            agent = GetComponent<NavMeshAgent>();
            rb = GetComponent<Rigidbody>();
            agent.speed = Speed;
            rb.isKinematic = true;
        }

        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.enabled = false;
        }
    }

    private void Update()
    {
        if (!isAlive || Move.instance == null) return;

        if (enemyType == EnemyType.Move)
        {
            agent.speed = Speed - downSpeed;
            FenceCheck();
            if (!isJumpingOverFence)
            {
                agent.SetDestination(Move.instance.transform.position);
            }
        }

        shootTimer += Time.deltaTime;
        if (shootTimer >= shootInterval && AttackUse)
        {
            shootTimer = 0f;
            StartCoroutine(DelayedFire());
        }

        burstCooldownTimer += Time.deltaTime;
        float distance = Vector3.Distance(transform.position, Move.instance.transform.position);
        if (distance <= 10f && burstCooldownTimer >= burstCooldown && enemyType != EnemyType.Sniper)
        {
            burstCooldownTimer = 0f;
            StartCoroutine(CloseRangeBurst());
        }

        if (isTrackingLine)
        {
            UpdateBulletTrajectoryLine();
        }

        if(GameManager.instance.Deathing)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator CloseRangeBurst()
    {
        if (Move.instance == null || !isAlive || !AttackUse) yield break;

        int burstCount = 3; // 무조건 3발
        for (int i = 0; i < burstCount; i++)
        {
            if (Move.instance == null) yield break;
            lockedPlayerX = Move.instance.transform.position.x;
            ShootActualBullet(damage - 2); // 대미지 2 감소
            yield return new WaitForSeconds(0.15f);
        }
    }

    void FenceCheck()
    {
        if (isJumpingOverFence) return;

        Ray ray = new Ray(foot.position, foot.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, fenceCheckDistance, fenceLayer))
        {
            if (hit.collider.CompareTag("Fence"))
            {
                StartCoroutine(JumpOverFence());
            }
        }

        Debug.DrawRay(foot.position, foot.forward * fenceCheckDistance, Color.cyan);
    }

    IEnumerator JumpOverFence()
    {
        isJumpingOverFence = true;
        agent.enabled = false;
        rb.isKinematic = false;

        Vector3 jumpDirection = transform.forward + Vector3.up;
        jumpDirection.Normalize();

        rb.linearVelocity = Vector3.zero;
        rb.AddForce(jumpDirection * fenceJumpForce, ForceMode.VelocityChange);

        yield return new WaitForSeconds(1.6f);

        SfxManager.Instance.PlayAt("land", this.transform.position, 1f);
        rb.isKinematic = true;
        agent.enabled = true;
        isJumpingOverFence = false;
    }

    public void Damage(int damage)
    {
        if (!isAlive) return;

        int rand = Random.Range(1, 4);
        SfxManager.Instance.PlayAt("Erun" + rand, transform.position, 1.5f);

        Hp -= damage;
        float randf = Random.Range(1f, 3f);
        StartCoroutine(DownSpeeds(randf));
        if (Hp <= 0)
        {
            isAlive = false;
            Debug.Log($"{gameObject.name} 사망");
            agent.enabled = false;
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y + 180f, 0);
            StartCoroutine(RunAwayAndDestroy());
        }
    }

    IEnumerator DownSpeeds(float time)
    {
        if (downSpeed != 0)
            yield return null;

        downSpeed = 2f;
        yield return new WaitForSeconds(time);
        downSpeed = 0f;


    }

    IEnumerator RunAwayAndDestroy()
    {
        float runTime = 5f;
        float timer = 0f;
        float runSpeed = 4f;

        while (timer < runTime)
        {
            transform.Translate(Vector3.forward * runSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    void ShootActualBullet() => ShootActualBullet(damage);

    void ShootActualBullet(int customDamage)
    {
        if (Move.instance == null || bullet == null || !isAlive || !AttackUse) return;

        if (lineRenderer != null) lineRenderer.enabled = false;
        isTrackingLine = false;

        Vector3 start = lineRenderer != null ? lineRenderer.gameObject.transform.position : transform.position;
        Vector3 cur = Move.instance.AttackPoint.position;
        Vector3 target = new Vector3(lockedPlayerX, cur.y, cur.z);
        Vector3 dir = (target - start).normalized;

        GameObject bulletObj = Instantiate(bullet, start, Quaternion.LookRotation(dir));
        Rigidbody bulletRb = bulletObj.GetComponent<Rigidbody>();
        Bullet bulletcs = bulletRb.GetComponent<Bullet>();
        bulletcs.damage = customDamage;

        SfxManager.Instance.Play(bulletSound, 1f);
        foreach (ParticleSystem particle in particleSystems)
        {
            particle.Play();
        }

        if (bulletRb != null)
        {
            bulletRb.useGravity = false;
            bulletRb.linearVelocity = dir * 40f;
        }

        Debug.DrawLine(start, target, Color.red, 2f);
    }

    IEnumerator DelayedFire()
    {
        if (Move.instance == null || !AttackUse || !isAlive) yield break;

        float distance = Vector3.Distance(transform.position, Move.instance.transform.position);
        if (distance <= 10f && enemyType != EnemyType.Sniper)
        {
            int burstCount = Random.Range(2, 4);
            for (int i = 0; i < burstCount; i++)
            {
                if (Move.instance == null) yield break;
                lockedPlayerX = Move.instance.transform.position.x;
                ShootActualBullet();
                yield return new WaitForSeconds(0.15f);
            }

            shootInterval = originalShootInterval;
            yield break;
        }

        lockedPlayerX = Move.instance.transform.position.x;

        if (lineRenderer != null)
        {
            lineRenderer.enabled = true;
            lineRenderer.positionCount = 2;
        }

        isTrackingLine = true;

        if (enemyType == EnemyType.Sniper)
            SfxManager.Instance.Play("Eb3Ready", 0.9f);

        yield return new WaitForSeconds(2f);

        if (Move.instance == null) yield break;
        ShootActualBullet();

        shootInterval = originalShootInterval;
    }

    void UpdateBulletTrajectoryLine()
    {
        if (lineRenderer == null || !lineRenderer.enabled) return;

        Vector3 start = lineRenderer.gameObject.transform.position;
        Vector3 cur = Move.instance.AttackPoint.position;
        Vector3 end = new Vector3(lockedPlayerX, cur.y, cur.z);

        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }
}
