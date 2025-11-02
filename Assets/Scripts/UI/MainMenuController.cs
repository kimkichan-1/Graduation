using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Main 씬의 메뉴 버튼 관리
/// NewGame/LoadGame 버튼 처리
/// </summary>
public class MainMenuController : MonoBehaviour
{
    void Start()
    {
        // ⭐ GoMain으로 돌아온 경우 DontDestroyOnLoad 오브젝트들 정리
        if (DontDestroyOnLoadManager.isReturningToMainMenu)
        {
            Debug.Log("[MainMenu] GoMain으로 복귀 - DontDestroyOnLoad 오브젝트 정리 시작");
            CleanupDontDestroyOnLoadObjects();
        }
    }

    /// <summary>
    /// DontDestroyOnLoad 오브젝트들 정리
    /// </summary>
    /// <summary>
    /// DontDestroyOnLoad 오브젝트들 정리
    /// </summary>
    private void CleanupDontDestroyOnLoadObjects()
    {
        // 모든 DontDestroyOnLoad 오브젝트 찾기
        DontDestroyOnLoadManager[] managers = FindObjectsOfType<DontDestroyOnLoadManager>(true);

        Debug.Log($"[MainMenu] {managers.Length}개의 DontDestroyOnLoad 오브젝트를 찾았습니다. 파괴 대상을 검색합니다.");

        foreach (var manager in managers)
        {
            // --- ▼▼▼▼▼ 1. (★수정됨★) 이 if 문을 다시 추가/수정합니다 ▼▼▼▼▼ ---

            // DontDestroyOnLoadManager 컴포넌트의 instanceId를 직접 확인
            DontDestroyOnLoadManager ddolManager = manager.GetComponent<DontDestroyOnLoadManager>();

            // 'AudioManager'는 게임 세션이 아니라 앱 설정이므로 파괴하지 않고 "유지"합니다.
            if (ddolManager != null && ddolManager.instanceId == "AudioManager")
            {
                Debug.Log($"[MainMenu] {manager.gameObject.name} (AudioManager) 유지");
                continue; // 파괴하지 않고 다음으로 넘어감
            }

            // --- ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲ ---

            // 2. 그 외 (Player, GameManager, SaveManager 등) 모든 것은 파괴합니다.
            Debug.Log($"[MainMenu] DontDestroyOnLoad 오브젝트 파괴: {manager.gameObject.name}");
            Destroy(manager.gameObject);
        }

        Debug.Log("[MainMenu] DontDestroyOnLoad 오브젝트 정리 완료");
    }

    /// <summary>
    /// New Game 버튼 - Weapon 씬으로 이동
    /// </summary>
    public void OnNewGameButton()
    {
        // ⭐ DontDestroyOnLoadManager 플래그 리셋 (새 게임 시작)
        DontDestroyOnLoadManager.ResetMainMenuFlag();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.PrepareNewGame();
        }

        SceneManager.LoadScene("Weapon");
    }

    /// <summary>
    /// Load Game 버튼 - LoadGame 씬으로 이동
    /// </summary>
    public void OnLoadGameButton()
    {
        // ⭐ DontDestroyOnLoadManager 플래그 리셋 (게임 로드)
        DontDestroyOnLoadManager.ResetMainMenuFlag();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.isNewGame = false;
        }

        Debug.Log("Load Game - LoadGame 씬으로 이동");
        SceneManager.LoadScene("LoadGame");
    }

    /// <summary>
    /// How To Play 버튼
    /// </summary>
    public void OnHowToPlayButton()
    {
        SceneManager.LoadScene("HowToPlay");
    }

    /// <summary>
    /// Settings 버튼
    /// </summary>
    public void OnSettingsButton()
    {
        SceneManager.LoadScene("Setting");
    }

    /// <summary>
    /// Quit 버튼
    /// </summary>
    public void OnQuitButton()
    {
        Debug.Log("게임 종료");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
