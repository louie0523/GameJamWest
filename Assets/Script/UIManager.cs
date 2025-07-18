using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject Black;

    private Image blackImage;

    public bool EndFade = false;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        blackImage = Black.GetComponent<Image>();

        if (blackImage == null)
            Debug.LogError("Black 오브젝트에 Image 컴포넌트가 없습니다.");
        
        if(EndFade)
        {
            FadeOut(5f);
        }
    }


    public void FadeIn(float duration)
    {
        Black.SetActive(true);
        blackImage.DOFade(1f, duration);
    }


    public void FadeOut(float duration)
    {
        blackImage.DOFade(0f, duration).OnComplete(() => {
            Black.SetActive(false);
        });
    }
}
