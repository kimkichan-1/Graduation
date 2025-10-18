using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// 일시정지 메뉴 UI 관리
/// ESC 키로 열고 닫기 가능
/// </summary>
public class PauseMenuUI : MonoBehaviour
{
    [Header("UI 요소")]
    [SerializeField] private GameObject pauseMenuPanel;  // 일시정지 메뉴 패널

    [Header("버튼")]
    [SerializeField] private Button closeButton;         // 닫기 버튼
    [SerializeField] private Button settingButton;       // 설정 버튼 (추후 구현)
    [SerializeField] private Button goMainButton;        // 메인으로 버튼

    private bool isPaused = false;

    void Start()
    {
        // 초기화: 메뉴 숨김
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        // 버튼 이벤트 연결
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnCloseButton);
        }

        if (settingButton != null)
        {
            settingButton.onClick.AddListener(OnSettingButton);
        }

        if (goMainButton != null)
        {
            goMainButton.onClick.AddListener(OnGoMainButton);
        }

        // 씬 로드 이벤트 구독
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        // 씬 로드 이벤트 구독 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // 오브젝트가 파괴될 때 시간 복구 (안전장치)
        Time.timeScale = 1f;
    }

    /// <summary>
    /// 씬 로드 완료 시 호출
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 게임 씬으로 들어갈 때마다 일시정지 메뉴 강제로 닫기
        Debug.Log($"[PauseMenu] 씬 '{scene.name}' 로드됨 - 일시정지 메뉴 초기화");

        // 패널 강제로 끄기
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        // 상태 초기화
        isPaused = false;
        Time.timeScale = 1f;
    }

    void Update()
    {
        // ESC 키 입력 감지
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    /// <summary>
    /// 일시정지 토글
    /// </summary>
    private void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    /// <summary>
    /// 게임 일시정지
    /// </summary>
    private void PauseGame()
    {
        isPaused = true;

        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
        }

        // 시간 정지
        Time.timeScale = 0f;

        Debug.Log("[PauseMenu] 게임 일시정지");
    }

    /// <summary>
    /// 게임 재개
    /// </summary>
    private void ResumeGame()
    {
        isPaused = false;

        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        // 시간 재개
        Time.timeScale = 1f;

        Debug.Log("[PauseMenu] 게임 재개");
    }

    /// <summary>
    /// Close 버튼 클릭
    /// </summary>
    private void OnCloseButton()
    {
        Debug.Log("[PauseMenu] Close 버튼 클릭");
        ResumeGame();
    }

    /// <summary>
    /// Setting 버튼 클릭 (추후 구현)
    /// </summary>
    private void OnSettingButton()
    {
        Debug.Log("[PauseMenu] Setting 버튼 클릭 (추후 구현 예정)");

        // TODO: Setting 씬으로 이동하거나 설정 패널 열기
        // SceneManager.LoadScene("Setting");
    }

    /// <summary>
    /// GoMain 버튼 클릭
    /// </summary>
    private void OnGoMainButton()
    {
        Debug.Log("[PauseMenu] GoMain 버튼 클릭 - 메인 화면으로 이동");

        // 코루틴 시작 (확실한 파괴 후 씬 로드)
        StartCoroutine(GoMainCoroutine());
    }

    /// <summary>
    /// Main 씬으로 이동하는 코루틴
    /// 플래그 설정 후 씬 로드만 수행 (정리는 씬 로드 시 자동 처리)
    /// </summary>
    private System.Collections.IEnumerator GoMainCoroutine()
    {
        // 1. 일시정지 메뉴 완전히 닫기
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
        Time.timeScale = 1f;
        isPaused = false;

        // 2. ⭐ DontDestroyOnLoadManager가 작동하지 않도록 플래그 설정
        DontDestroyOnLoadManager.isReturningToMainMenu = true;
        Debug.Log("[PauseMenu] 메인 화면 복귀 플래그 설정");

        // 3. Player, Inventory 싱글톤 Instance를 null로 초기화
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance = null;
            Debug.Log("[PauseMenu] PlayerController.Instance = null");
        }

        if (Inventory.instance != null)
        {
            Inventory.instance = null;
            Debug.Log("[PauseMenu] Inventory.instance = null");
        }

        // 4. 메인 씬으로 이동 (DontDestroyOnLoad 아닌 오브젝트들은 자동 파괴됨)
        Debug.Log("[PauseMenu] Main 씬 로드 시작");
        SceneManager.LoadScene("Main");

        yield return null;
    }

    /// <summary>
    /// 외부에서 일시정지 메뉴 열기
    /// </summary>
    public void OpenPauseMenu()
    {
        if (!isPaused)
        {
            PauseGame();
        }
    }

    /// <summary>
    /// 외부에서 일시정지 메뉴 닫기
    /// </summary>
    public void ClosePauseMenu()
    {
        if (isPaused)
        {
            ResumeGame();
        }
    }
}
