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

    public float rotationSpeed = 30f; // 회전 속도 (도/초)

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
        // 현재 선택된 총기 모델 회전
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
        GunStatusText.text = $"<sprite=0> 공격력 : {GunStatusList[CurrentNum].damage} | <sprite=1> 공격 속도 : {GunStatusList[CurrentNum].AttackSpeed}초 | <sprite=2> 탄약 : {GunStatusList[CurrentNum].Range}발 | <sprite=3> 장전 시간 : {GunStatusList[CurrentNum].ReloadTime}초";


    }
}
