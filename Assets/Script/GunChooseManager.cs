using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Splines;
using UnityEngine.SceneManagement;




public class GunChooseManager : MonoBehaviour
{
    public static GunChooseManager instance;



    public List<GunStatus> GunStatusList = new List<GunStatus>();
    public int CurrentNum = 0;
    public GameObject GunModelParent;
    public TextMeshProUGUI GunName;
    public TextMeshProUGUI GunStatusText;

    public float rotationSpeed = 30f; // ȸ�� �ӵ� (��/��)

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        GunLotate();
    }

    private void Update()
    {
        // ���� ���õ� �ѱ� �� ȸ��
        if (GunModelParent.transform.childCount > CurrentNum)
        {
            Transform currentGun = GunModelParent.transform.GetChild(CurrentNum);
            if (currentGun.gameObject.activeSelf)
            {
                currentGun.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
            }
        }


        if(Input.GetKeyDown(KeyCode.A) && CurrentNum >= 1)
        {
            SfxManager.Instance.Play("Asfx");
            CurrentNum--;
            GunLotate();
        }

        if(Input.GetKeyDown(KeyCode.D) && CurrentNum < GunStatusList.Count - 1)
        {
            SfxManager.Instance.Play("Dsfx");
            CurrentNum++;
            GunLotate();
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            GoGame();
        }
    }

    public void GoGame()
    {
        GameManager.instance.CurretnGun = GunStatusList[CurrentNum];
        SceneManager.LoadScene(2);
    }

    public void GunLotate()
    {
        GameObject gun = GunModelParent.transform.GetChild(CurrentNum).gameObject;

        for (int i = 0; i < GunModelParent.transform.childCount; i++)
        {
            GameObject child = GunModelParent.transform.GetChild(i).gameObject;
            child.SetActive(i == CurrentNum);
        }

        GunName.text = GunStatusList[CurrentNum].GunName;
        GunStatusText.text = $"<sprite=0> ���ݷ� : {GunStatusList[CurrentNum].damage} | <sprite=1> ���� �ӵ� : {GunStatusList[CurrentNum].AttackSpeed}�� | <sprite=2> ź�� : {GunStatusList[CurrentNum].Range}�� | <sprite=3> ���� �ð� : {GunStatusList[CurrentNum].ReloadTime}��";


    }
}
