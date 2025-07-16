using System.Collections.Generic;
using UnityEngine;

public class SfxManager : MonoBehaviour
{
    public static SfxManager Instance;

    [System.Serializable]
    public class SfxData
    {
        public string name;
        public AudioClip clip;
    }

    [Header("사운드 리스트")]
    public List<SfxData> sfxList = new List<SfxData>();

    private Dictionary<string, AudioClip> sfxDict = new Dictionary<string, AudioClip>();
    private AudioSource audioSource;

    private void Awake()
    {
        // 싱글톤 설정
        if (Instance == null)
        {
            Instance = this;
            Init();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Init()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        foreach (var sfx in sfxList)
        {
            if (!sfxDict.ContainsKey(sfx.name))
            {
                sfxDict.Add(sfx.name, sfx.clip);
            }
        }
    }

    /// <summary>
    /// 이름으로 효과음 재생
    /// </summary>
    public void Play(string name, float volume = 1f)
    {
        if (sfxDict.ContainsKey(name))
        {
            audioSource.PlayOneShot(sfxDict[name], volume);
        }
        else
        {
            Debug.LogWarning($"SFX '{name}' not found!");
        }
    }

    /// <summary>
    /// 3D 위치에서 재생 (임시 오브젝트 생성 후 자동 삭제)
    /// </summary>
    public void PlayAt(string name, Vector3 position, float volume = 1f)
    {
        if (sfxDict.ContainsKey(name))
        {
            AudioSource.PlayClipAtPoint(sfxDict[name], position, volume);
        }
        else
        {
            Debug.LogWarning($"SFX '{name}' not found!");
        }
    }
}
