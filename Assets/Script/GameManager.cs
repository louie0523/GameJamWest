using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject DeathCamera;
    public TextMeshProUGUI DeathTitle;
    public TextMeshProUGUI DeathInspecterText;
    public string Title;
    public string Inspecter;
    public bool Deathing = false;

    public GameObject Tutorial;

    public GunStatus CurretnGun;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadSence(int num)
    {
        SceneManager.LoadScene(num);
    }

    public void Death()
    {
        Deathing = true;
        SfxManager.Instance.Play("Death", 1);
        DeathCamera.SetActive(true);

        // �ؽ�Ʈ �ʱ�ȭ
        DeathTitle.text = "";
        DeathInspecterText.text = "";

        // ���������� Ÿ���� ȿ�� ����
        StartCoroutine(TypeTextSequence());
    }

    private IEnumerator TypeTextSequence()
    {
        yield return TypeText(DeathTitle, Title);
        yield return new WaitForSeconds(0.3f);
        yield return TypeText(DeathInspecterText, Inspecter);
    }

    private IEnumerator TypeText(TextMeshProUGUI textComponent, string fullText)
    {
        textComponent.maxVisibleCharacters = 0;
        textComponent.text = fullText;

        int totalLength = fullText.Length;

        for (int i = 0; i <= totalLength; i++)
        {
            textComponent.maxVisibleCharacters = i;
            yield return new WaitForSeconds(0.05f);  // ���� ������ �ӵ� ���� ����
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void OpenTutorial()
    {
        Tutorial.SetActive(true);
    }

    public void CloseTutorial()
    {
        Tutorial.SetActive(false);
    }
}
