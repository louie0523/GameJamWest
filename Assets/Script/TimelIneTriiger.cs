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
        // PlayableDirector가 할당되지 않았다면 현재 오브젝트에서 찾기
        if (playableDirector == null)
        {
            playableDirector = FindObjectOfType<PlayableDirector>();
        }

        // TimelineAsset이 할당되지 않았다면 PlayableDirector에서 가져오기
        if (timelineAsset == null && playableDirector != null)
        {
            timelineAsset = playableDirector.playableAsset as TimelineAsset;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && playOnEnter)
        {
            // 한 번만 실행하는 옵션이 켜져있고 이미 트리거되었다면 리턴
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

    // 타임라인 재생 상태 확인
    public bool IsTimelinePlaying()
    {
        return playableDirector != null && playableDirector.state == PlayState.Playing;
    }

    // 타임라인 재설정 (다시 트리거 가능하게 만들기)
    public void ResetTrigger()
    {
        hasTriggered = false;
    }

    // 특정 시간으로 타임라인 이동
    public void SetTimelineTime(float time)
    {
        if (playableDirector != null)
        {
            playableDirector.time = time;
        }
    }

    // 타임라인 완료 이벤트 리스너
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
        // 타임라인 완료 후 추가 작업이 필요하다면 여기에 작성
    }
}