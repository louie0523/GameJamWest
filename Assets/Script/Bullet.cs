using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public float lifeTime = 1f;


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Àû ¸ÂÀ½");
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            enemy.Damage(damage);
            Destroy(gameObject);
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
