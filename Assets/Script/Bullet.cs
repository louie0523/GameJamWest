using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public float lifeTime = 1f;



    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && this.CompareTag("Pb")) 
        {
            Debug.Log("�� ����");
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            enemy.Damage(damage);
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Player") && this.CompareTag("Eb"))
        {
            Debug.Log("�÷��̾� ����");
            Move.instance.Damage(damage);
        }
    }


    private void Start()
    {
        Invoke("DestoryObj", lifeTime);
    }

    void DestoryObj()
    {
        Destroy(gameObject);
    }
}
