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

    [Header("���� ����Ʈ")]
    public List<SfxData> sfxList = new List<SfxData>();

    private Dictionary<string, AudioClip> sfxDict = new Dictionary<string, AudioClip>();
    private AudioSource audioSource;

    private void Awake()
    {
        // �̱��� ����
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
    /// �̸����� ȿ���� ���
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
    /// 3D ��ġ���� ��� (�ӽ� ������Ʈ ���� �� �ڵ� ����)
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
