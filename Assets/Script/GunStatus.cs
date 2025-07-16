using UnityEngine;


[CreateAssetMenu(fileName = "NewGunStatus", menuName = "Gun/Gun Status")]
public class GunStatus : ScriptableObject
{
    public string GunName;
    public int num = 0;
    public GameObject GunModel;
    public int damage;
    public float AttackSpeed;
    public float Range;
    public float ReloadTime;
}