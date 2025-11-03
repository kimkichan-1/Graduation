using UnityEngine;
using UnityEngine.SceneManagement; // ★ 1. 씬 관리자 추가

/// <summary>
/// 이 씬에서 재생할 BGM을 AudioManager에게 알려줍니다.
/// (업데이트됨: 씬 로드 시마다 BGM을 신청하도록 변경)
/// </summary>
public class SceneMusic : MonoBehaviour
{
    [Header("이 씬의 BGM")]
    [SerializeField] private AudioClip sceneMusicClip;

    [Header("옵션")]
    [SerializeField] private bool playOnStart = true; // 씬 시작 시 바로 재생

    // --- ▼▼▼ 2. 씬 로드 이벤트를 구독/해제하도록 변경 ▼▼▼ ---
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // --- (기존 Start() 함수는 삭제) ---

    // --- ▼▼▼ 3. 씬이 로드될 때마다 이 함수가 실행됨 ▼▼▼ ---
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 4. 이 스크립트가 활성화된 씬(scene)과
        //    방금 로드된 씬(scene.name)이 '같은 씬'인지 확인
        // (이게 없으면 Stage1의 SceneMusic이 Main씬에서 또 실행됨)
        if (this.gameObject.scene.name != scene.name)
        {
            return;
        }

        // 5. 씬이 같고, playOnStart가 켜져있다면 BGM 신청
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
