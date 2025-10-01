// 파일명: DialogueController.cs (UI 자동 재연결 기능 추가)
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // ★★★ 씬 관리를 위해 필수! ★★★
using UnityEngine.UI;             // ★★★ Button을 사용하기 위해 필수! ★★★

public class DialogueController : MonoBehaviour
{
    public static DialogueController Instance { get; private set; }

    // --- ★★★ 수정된 부분: Inspector에서 이름으로 찾도록 변경 ★★★ ---
    [Header("찾아올 UI 정보")]
    [Tooltip("Hierarchy 창에 있는 대화창 Panel의 정확한 이름")]
    [SerializeField] private string dialoguePanelName = "DialoguePanel";
    [Tooltip("대화창 Panel 안에 있는 Text의 정확한 이름")]
    [SerializeField] private string dialogueTextName = "Dialoguetext";
    [Tooltip("대화창 Panel 안에 있는 Button의 정확한 이름")]
    [SerializeField] private string nextButtonName = "Next";

    // 실제 UI 오브젝트들을 담을 변수들 (이제 private입니다)
    private GameObject dialoguePanel;
    private TextMeshProUGUI dialogueText;
    private Button nextButton;
    // --- 여기까지 ---

    private Queue<string> sentences;
    private UnityEvent onDialogueEnd;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // 이 컨트롤러는 씬 전환 시 파괴되지 않음

        sentences = new Queue<string>();
    }

    // --- ★★★ 씬이 로드될 때마다 이 함수들이 호출됩니다 ★★★ ---
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 씬 로드가 완료되면 UI 참조를 다시 설정합니다.
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetupReferences();
    }
    // --- 여기까지 ---

    // ★★★ UI 참조를 설정하는 새로운 함수 ★★★
    private void SetupReferences()
    {
        // 씬에서 이름으로 패널을 찾습니다.
        // 비활성화 상태일 수 있으므로, 최상위 오브젝트부터 모두 검색합니다.
        Transform[] allTransforms = FindObjectsOfType<Transform>(true);
        foreach (Transform t in allTransforms)
        {
            if (t.name == dialoguePanelName)
            {
                dialoguePanel = t.gameObject;
                break;
            }
        }

        if (dialoguePanel != null)
        {
            // 찾은 패널의 자식 중에서 텍스트와 버튼을 찾습니다.
            dialogueText = dialoguePanel.transform.Find(dialogueTextName)?.GetComponent<TextMeshProUGUI>();
            nextButton = dialoguePanel.transform.Find(nextButtonName)?.GetComponent<Button>();

            if (dialogueText == null) Debug.LogError($"DialogueController: '{dialoguePanelName}' 아래에서 '{dialogueTextName}' 이름의 Text를 찾을 수 없습니다.");
            if (nextButton == null) Debug.LogError($"DialogueController: '{dialoguePanelName}' 아래에서 '{nextButtonName}' 이름의 Button을 찾을 수 없습니다.");
            else
            {
                // 버튼 이벤트를 코드로 직접 연결
                nextButton.onClick.RemoveAllListeners();
                nextButton.onClick.AddListener(DisplayNextSentence);
            }

            // 찾은 후에는 확실하게 비활성화
            dialoguePanel.SetActive(false);
        }
    }

    // (StartDialogue, DisplayNextSentence 등 나머지 함수는 보내주신 코드와 동일하게 유지)
    public void StartDialogue(string[] dialogueLines, UnityEvent onEndAction = null)
    {
        if (dialoguePanel == null)
        {
            Debug.LogError("DialogueController: 대화창 Panel이 설정되지 않아 대화를 시작할 수 없습니다. 씬에 해당 이름의 Panel이 있는지 확인하세요.");
            return;
        }

        dialoguePanel.SetActive(true);
        sentences.Clear();

        foreach (string sentence in dialogueLines)
        {
            sentences.Enqueue(sentence);
        }

        onDialogueEnd = onEndAction;
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0) { EndDialogue(); return; }
        string sentence = sentences.Dequeue();
        if (dialogueText != null) dialogueText.text = sentence;
    }

    private void EndDialogue()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (onDialogueEnd != null) onDialogueEnd.Invoke();
    }

    public bool IsDialogueActive()
    {
        if (dialoguePanel == null) return false;
        return dialoguePanel.activeInHierarchy;
    }
}