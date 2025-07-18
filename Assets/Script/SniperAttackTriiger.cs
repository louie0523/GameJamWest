using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum Unit
{
    Sniper,
    Mover,
}
public class SniperAttackTriiger : MonoBehaviour
{
    public Unit unitType;
    public bool Act = false;
    public bool AttackUse = true;
    public Enemy Sniper;
    public GameObject Mover;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player") && !Act)
        {
            Act = true;
            if (unitType == Unit.Sniper)
            {
                Sniper.AttackUse = AttackUse;
            } else if(unitType == Unit.Mover)
            {
                Mover.SetActive(true);
                int rand = Random.Range(1, 4);
                SfxManager.Instance.Play("Erun" + rand);
                StartCoroutine(speedUp());
                
            }

        }
    }

    IEnumerator speedUp()
    {
        Enemy enemy = Mover.GetComponent<Enemy>();
        enemy.Speed += 10f;
        yield return new WaitForSeconds(3f);
        enemy.Speed -= 10f;

    }
}
