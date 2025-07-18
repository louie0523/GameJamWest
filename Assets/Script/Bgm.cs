using UnityEngine;
using System.Collections.Generic;

public class Bgm : MonoBehaviour
{
    public static Bgm instance;
    public AudioSource audioSource;
    public List<AudioClip> bgms = new List<AudioClip>();

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // ������ �� ù ��° BGM ���
        if (bgms.Count > 0 && audioSource != null)
        {
            audioSource.clip = bgms[0];
            audioSource.loop = true;
            audioSource.Play();
        }
    }


    public void PlayBgm(int index)
    {
        if (index >= 0 && index < bgms.Count)
        {
            audioSource.Stop(); // ���� ���� ���߰�
            audioSource.clip = bgms[index]; // �� Ŭ������ �ٲ� ��
            audioSource.Play(); // ���
        }
        else
        {
            Debug.LogWarning("Bgm �ε��� ���� �ʰ�: " + index);
        }
    }
}
