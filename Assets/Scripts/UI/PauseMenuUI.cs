using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic; // Queue를 위해 추가

/// <summary>
/// 일시정지 메뉴 UI 관리 (업그레이드됨: 3단 패널 전환)
/// </summary>
public class PauseMenuUI : MonoBehaviour
{
    [Header("UI 요소")]
    [SerializeField] private GameObject pauseMenuPanel;  // 최상위 갈색 창

    [Header("하위 패널 (서랍)")]
    [SerializeField] private GameObject mainPausePanel;     // Panel_Main
    [SerializeField] private GameObject settingsPanel;      // Settings_Panel
    [SerializeField] private GameObject audioSettingsPanel; // AudioSettings_Panel

    [Header("메인 패널 버튼 (자동 연결)")]
    [SerializeField] private Button closeButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button goMainButton;

    private bool isPaused = false;

    void Start()
    {
        // 1. 초기화: 메뉴 숨김
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);

        // 2. 서랍들도 확실히 초기화 (메인 서랍만 켜기)
        if (mainPausePanel != null) mainPausePanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (audioSettingsPanel != null) audioSettingsPanel.SetActive(false);

        // --- 3. 버튼 이벤트 '자동' 연결 ---

        // 3-1. 메인 서랍 버튼 (Setting, GoMain, Close)
        if (closeButton != null) closeButton.onClick.AddListener(ResumeGame);
        if (settingButton != null) settingButton.onClick.AddListener(OpenSettingsMenu);
        if (goMainButton != null) goMainButton.onClick.AddListener(OnGoMainButton);

        // 3-2. '설정' 서랍 버튼 (Audio, Back)
        if (settingsPanel != null)
        {
            // 'AudioButton'을 찾아서 연결
            Button audioBtn = settingsPanel.transform.FindDeepChild("AudioButton")?.GetComponent<Button>();
            if (audioBtn != null) audioBtn.onClick.AddListener(OpenAudioSettings);

            // 'BackButton'을 찾아서 연결
            Button backBtn_L1 = settingsPanel.transform.FindDeepChild("BackButton")?.GetComponent<Button>();
            if (backBtn_L1 != null) backBtn_L1.onClick.AddListener(BackToMainPause);
        }

        // 3-3. '오디오' 서랍 버튼 (Back)
        if (audioSettingsPanel != null)
        {
            // 'BackButton'을 찾아서 연결
            Button backBtn_L2 = audioSettingsPanel.GetComponentInChildren<Button>(true); // (비활성화된 자식 포함)
            if (backBtn_L2 != null && backBtn_L2.name.Contains("Back")) backBtn_L2.onClick.AddListener(BackToSettings);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Time.timeScale = 1f;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        isPaused = false;
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    // --- (일시정지 기본 기능) ---

    private void TogglePause()
    {
        if (isPaused) ResumeGame();
        else PauseGame();
    }

    private void PauseGame()
    {
        isPaused = true;
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);

        // ESC로 켤 땐 항상 '메인 서랍'부터 보이도록 리셋
        if (mainPausePanel != null) mainPausePanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (audioSettingsPanel != null) audioSettingsPanel.SetActive(false);

        Time.timeScale = 0f;
    }

    private void ResumeGame()
    {
        isPaused = false;
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    // --- (★ 버튼 5개가 호출할 5개의 함수 ★) ---

    // 1. (메인) Setting 버튼 -> 2차 메뉴 열기
    private void OpenSettingsMenu()
    {
        mainPausePanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    // 2. (2차 메뉴) Audio 버튼 -> 오디오 서랍 열기
    private void OpenAudioSettings()
    {
        settingsPanel.SetActive(false);
        audioSettingsPanel.SetActive(true);
    }

    // 3. (오디오 서랍) Back 버튼 -> 2차 메뉴로
    private void BackToSettings()
    {
        audioSettingsPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    // 4. (2차 메뉴) Back 버튼 -> 메인 서랍으로
    private void BackToMainPause()
    {
        settingsPanel.SetActive(false);
        mainPausePanel.SetActive(true);
    }

    // 5. (메인) GoMain 버튼
    private void OnGoMainButton()
    {
        StartCoroutine(GoMainCoroutine());
    }

    // (GoMainCoroutine 함수는 '새 게임' 버그 수정 포함)
    private System.Collections.IEnumerator GoMainCoroutine()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        PlayerStats.ResetStaticVariables();
        BossGameManager.ResetStaticVariables();

        DontDestroyOnLoadManager.isReturningToMainMenu = true;

        if (PlayerController.Instance != null) PlayerController.Instance = null;
        if (Inventory.instance != null) Inventory.instance = null;

        SceneManager.LoadScene("Main");
        yield return null;
    }
}

// `transform.Find`가 비활성화된 자식이나 손자들을 못 찾는 문제를 해결
public static class TransformExtensions
{
    public static Transform FindDeepChild(this Transform aParent, string aName)
    {
        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(aParent);
        while (queue.Count > 0)
        {
            var c = queue.Dequeue();
            if (c.name == aName)
                return c;
            foreach (Transform t in c)
                queue.Enqueue(t);
        }
        return null;
    }
}
