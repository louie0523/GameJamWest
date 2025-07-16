using UnityEngine;

public class ObjDamage : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<Move>() != null)
        {
            collision.gameObject.GetComponent<Move>().Hp -= 1;
        }
    }
}
