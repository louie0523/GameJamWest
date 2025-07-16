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

    public List<ParticleSystem> particleSystems = new List<ParticleSystem>();

    public Animator animator;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = Speed;
    }

    private void Update()
    {
        if (isAlive && Move.instance != null)
        {
            agent.SetDestination(Move.instance.transform.position);
        }
    }

    public void Damage(int damage)
    {
        if (isAlive)
        {
            Hp -= damage;
            if (Hp <= 0)
            {
                isAlive = false;
                Debug.Log($"{gameObject.name}가 사망");

                int rand = Random.Range(1, 4);

                SfxManager.Instance.PlayAt("Erun" + rand, this.transform.position, 1.5f);

                agent.enabled = false; // NavMeshAgent 비활성화

                // 뒤로 돌기 (Y축 기준 180도 회전)
                transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y + 180f, 0);

                // 코루틴으로 뒤로 달리기 시작
                StartCoroutine(RunAwayAndDestroy());
            }
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


}
