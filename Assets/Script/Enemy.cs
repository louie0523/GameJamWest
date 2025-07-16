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
                Debug.Log($"{gameObject.name}�� ���");

                int rand = Random.Range(1, 4);

                SfxManager.Instance.PlayAt("Erun" + rand, this.transform.position, 1.5f);

                agent.enabled = false; // NavMeshAgent ��Ȱ��ȭ

                // �ڷ� ���� (Y�� ���� 180�� ȸ��)
                transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y + 180f, 0);

                // �ڷ�ƾ���� �ڷ� �޸��� ����
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
