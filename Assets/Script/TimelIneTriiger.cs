using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

public class TimelineTriiger : MonoBehaviour
{
    [Header("Timeline Settings")]
    public PlayableDirector playableDirector;
    public TimelineAsset timelineAsset;

    [Header("Trigger Settings")]
    public bool playOnce = true;
    public bool playOnEnter = true;
    public bool stopOnExit = false;

    private bool hasTriggered = false;

    private void Start()
    {
        // PlayableDirector�� �Ҵ���� �ʾҴٸ� ���� ������Ʈ���� ã��
        if (playableDirector == null)
        {
            playableDirector = FindObjectOfType<PlayableDirector>();
        }

        // TimelineAsset�� �Ҵ���� �ʾҴٸ� PlayableDirector���� ��������
        if (timelineAsset == null && playableDirector != null)
        {
            timelineAsset = playableDirector.playableAsset as TimelineAsset;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && playOnEnter)
        {
            // �� ���� �����ϴ� �ɼ��� �����ְ� �̹� Ʈ���ŵǾ��ٸ� ����
            if (playOnce && hasTriggered) return;

            PlayTimeline();
            hasTriggered = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && stopOnExit)
        {
            StopTimeline();
        }
    }

    public void PlayTimeline()
    {
        if (playableDirector != null)
        {
            if (timelineAsset != null)
            {
                playableDirector.playableAsset = timelineAsset;
            }

            playableDirector.Play();
            Debug.Log($"Timeline Started: {timelineAsset?.name}");
        }
        else
        {
            Debug.LogError("PlayableDirector is not assigned!");
        }
    }

    public void StopTimeline()
    {
        if (playableDirector != null)
        {
            playableDirector.Stop();
            Debug.Log("Timeline Stopped");
        }
    }

    public void PauseTimeline()
    {
        if (playableDirector != null)
        {
            playableDirector.Pause();
            Debug.Log("Timeline Paused");
        }
    }

    public void ResumeTimeline()
    {
        if (playableDirector != null)
        {
            playableDirector.Resume();
            Debug.Log("Timeline Resumed");
        }
    }

    // Ÿ�Ӷ��� ��� ���� Ȯ��
    public bool IsTimelinePlaying()
    {
        return playableDirector != null && playableDirector.state == PlayState.Playing;
    }

    // Ÿ�Ӷ��� �缳�� (�ٽ� Ʈ���� �����ϰ� �����)
    public void ResetTrigger()
    {
        hasTriggered = false;
    }

    // Ư�� �ð����� Ÿ�Ӷ��� �̵�
    public void SetTimelineTime(float time)
    {
        if (playableDirector != null)
        {
            playableDirector.time = time;
        }
    }

    // Ÿ�Ӷ��� �Ϸ� �̺�Ʈ ������
    private void OnEnable()
    {
        if (playableDirector != null)
        {
            playableDirector.stopped += OnTimelineFinished;
        }
    }

    private void OnDisable()
    {
        if (playableDirector != null)
        {
            playableDirector.stopped -= OnTimelineFinished;
        }
    }

    private void OnTimelineFinished(PlayableDirector director)
    {
        Debug.Log("Timeline Finished");
        // Ÿ�Ӷ��� �Ϸ� �� �߰� �۾��� �ʿ��ϴٸ� ���⿡ �ۼ�
    }
}