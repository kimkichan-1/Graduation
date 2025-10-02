// 파일명: GameManager.cs (수정 완료)
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // ★★★ 수정된 부분: 코루틴을 사용하기 위해 필수! ★★★

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int finalTapCount = 0;
    public float regenBuffValue = 0f;
    public Vector3 playerPositionBeforePortal;
    public string sceneNameBeforePortal;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // ★★★ 수정된 부분: OnSceneLoaded 함수가 이제 코루틴을 '시작'시키는 역할만 합니다. ★★★
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 만약 돌아온 씬이 저장된 씬 이름과 같다면
        if (!string.IsNullOrEmpty(sceneNameBeforePortal) && scene.name == sceneNameBeforePortal)
        {
            // 직접 복구 작업을 하지 않고, 코루틴을 시작시킵니다.
            StartCoroutine(RestoreSceneState());
        }
    }

    // ★★★ 새로 추가된 코루틴 함수 ★★★
    // 실제 복구 작업은 이 함수에서 한 프레임 뒤에 안전하게 이루어집니다.
    IEnumerator RestoreSceneState()
    {
        // 단 한 프레임만 기다립니다.
        // 이 시간 동안 ParallaxBackground 같은 다른 스크립트들이 초기화될 시간을 줍니다.
        yield return null;

        // --- 기존 OnSceneLoaded에 있던 복구 코드가 이 안으로 들어왔습니다 ---
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = playerPositionBeforePortal;
        }

        if (Camera.main != null)
        {
            Camera.main.cullingMask = -1; // 카메라 설정 복구
        }

        // 사용한 복귀 정보는 초기화합니다.
        sceneNameBeforePortal = null;
    }

    public void ProcessMinigameResult(int tapCount)
    {
        finalTapCount = tapCount;
        if (tapCount < 30) regenBuffValue = 0f;
        else if (tapCount < 50) regenBuffValue = 0.5f;
        else if (tapCount < 100) regenBuffValue = 1.0f;
        else regenBuffValue = 2.0f;
        Debug.Log($"미니게임 종료! 최종 연타: {finalTapCount}, 재생 버프: {regenBuffValue}hp/sec");
    }
}