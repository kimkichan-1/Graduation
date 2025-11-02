using UnityEngine;

/// <summary>
/// 이 씬에서 재생할 BGM을 AudioManager에게 알려줍니다.
/// </summary>
public class SceneMusic : MonoBehaviour
{
    [Header("이 씬의 BGM")]
    [SerializeField] private AudioClip sceneMusicClip;

    [Header("옵션")]
    [SerializeField] private bool playOnStart = true; // 씬 시작 시 바로 재생

    void Start()
    {
        if (playOnStart && sceneMusicClip != null)
        {
            if (AudioManager.Instance != null)
            {
                // '두뇌'에게 "이 노래 틀어줘!"라고 신청서 제출
                AudioManager.Instance.PlayBGM(sceneMusicClip);
            }
            else
            {
                Debug.LogError("AudioManager가 씬에 없습니다! Main 씬부터 시작해야 합니다.");
            }
        }
    }
}
