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
        // 시작할 때 첫 번째 BGM 재생
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
            audioSource.Stop(); // 현재 음악 멈추고
            audioSource.clip = bgms[index]; // 새 클립으로 바꾼 뒤
            audioSource.Play(); // 재생
        }
        else
        {
            Debug.LogWarning("Bgm 인덱스 범위 초과: " + index);
        }
    }
}
