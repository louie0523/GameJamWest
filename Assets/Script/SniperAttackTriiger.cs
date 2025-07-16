using UnityEngine;

public class SniperAttackTriiger : MonoBehaviour
{

    public bool Act = false;
    public Enemy Sniper;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player") && !Act)
        {
            Sniper.AttackUse = true;
            Act = true;
        }
    }
}
