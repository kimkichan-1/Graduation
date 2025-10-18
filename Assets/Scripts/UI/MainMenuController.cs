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
    private void CleanupDontDestroyOnLoadObjects()
    {
        // 모든 DontDestroyOnLoad 오브젝트 찾기
        DontDestroyOnLoadManager[] managers = FindObjectsOfType<DontDestroyOnLoadManager>(true);
        foreach (var manager in managers)
        {
            // GameManager, SaveManager는 유지
            if (manager.gameObject.name.Contains("GameManager") ||
                manager.gameObject.name.Contains("SaveManager"))
            {
                Debug.Log($"[MainMenu] {manager.gameObject.name} 유지");
                continue;
            }

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
