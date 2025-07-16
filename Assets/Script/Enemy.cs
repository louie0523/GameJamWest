using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    public float Speed;
    public int Hp = 10;
    public bool isAlive = true;

    private NavMeshAgent agent;
    private Rigidbody rb;

    public List<ParticleSystem> particleSystems = new List<ParticleSystem>();
    public Animator animator;

    public Transform foot;
    public float fenceCheckDistance = 2.8f;
    public float fenceJumpForce = 7f;
    public LayerMask fenceLayer;

    private bool isJumpingOverFence = false;

    // 총 발사 관련
    public LineRenderer lineRenderer;
    private float lockedPlayerX = 0f;
    private bool isTrackingLine = false;
    private float shootTimer = 0f;
    public float shootInterval = 8f;

    private void Start()
    {
        foot = transform.GetChild(1).transform;
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        agent.speed = Speed;
        rb.isKinematic = true;

        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.enabled = false;
        }
    }

    private void Update()
    {
        if (!isAlive) return;

        FenceCheck();

        if (!isJumpingOverFence && Move.instance != null)
        {
            agent.SetDestination(Move.instance.transform.position);
        }

        shootTimer += Time.deltaTime;
        if (shootTimer >= shootInterval)
        {
            shootTimer = 0f;
            StartCoroutine(DelayedFire());
        }

        if (isTrackingLine)
        {
            UpdateBulletTrajectoryLine();
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
        if (Hp <= 0)
        {
            isAlive = false;
            Debug.Log($"{gameObject.name} 사망");

            agent.enabled = false;
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y + 180f, 0);

            StartCoroutine(RunAwayAndDestroy());
        }
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


    void ShootActualBullet()
    {
        // 실제 총알 발사 시점
        Debug.Log("🔫 Enemy fired bullet at locked X: " + lockedPlayerX);

        // 라인 꺼도 됨
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }

        isTrackingLine = false;

        // TODO: Instantiate bullet prefab here if needed
    }

    IEnumerator DelayedFire()
    {
        if (Move.instance == null) yield break;

        lockedPlayerX = Move.instance.transform.position.x;
        Debug.Log($"DelayedFire: lockedPlayerX = {lockedPlayerX}");

        if (lineRenderer != null)
        {
            lineRenderer.enabled = true;
            lineRenderer.positionCount = 2;
        }

        isTrackingLine = true;
        yield return new WaitForSeconds(2f);
        ShootActualBullet();
    }

    void UpdateBulletTrajectoryLine()
    {
        if (lineRenderer == null)
        {
            Debug.LogWarning("❌ LineRenderer is null");
            return;
        }

        if (!lineRenderer.enabled)
        {
            Debug.LogWarning("❌ LineRenderer is DISABLED");
            return;
        }

        Vector3 start = lineRenderer.gameObject.transform.position;
        Vector3 cur = Move.instance.transform.position;
        Vector3 end = new Vector3(lockedPlayerX, cur.y, cur.z);

        Debug.Log($"✔️ Drawing line from {cur} to {end}");

        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }

}
