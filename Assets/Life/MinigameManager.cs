// 파일명: MinigameManager.cs (최종 업그레이드 버전)
using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class MinigameManager : MonoBehaviour
{
    [Header("대화 UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("미니게임 UI")]
    [SerializeField] private GameObject minigameUI;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI tapCountText;

    [Header("보상 UI")]
    [SerializeField] private GameObject rewardPanel;
    [SerializeField] private TextMeshProUGUI rewardText;

    [Header("대화 내용")]
    [SerializeField, TextArea(3, 10)] private string[] storyDialogue;
    [SerializeField, TextArea(3, 10)] private string[] instructionDialogue;

    [Header("미니게임 설정")]
    [SerializeField] private float gameDuration = 10f;

    private int finalTapCount = 0;

    void Start()
    {
        // 시작 시 모든 UI를 끈 상태로 대기
        dialoguePanel.SetActive(false);
        minigameUI.SetActive(false);
        rewardPanel.SetActive(false);
    }

    // DialogueTrigger가 이 함수를 호출하여 전체 시퀀스를 시작
    public void StartEventSequence()
    {
        StartCoroutine(FullSequenceCoroutine());
    }

    // 전체 흐름을 제어하는 메인 코루틴
    private IEnumerator FullSequenceCoroutine()
    {
        yield return StartCoroutine(ShowDialogue(storyDialogue));
        yield return StartCoroutine(ShowDialogue(instructionDialogue));
        yield return StartCoroutine(MinigameCoroutine());

        // GameManager의 ProcessMinigameResult를 직접 호출
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ProcessMinigameResult(finalTapCount);
        }

        ProcessAndShowReward();
    }

    // 대화창을 띄우고 진행하는 코루틴
    private IEnumerator ShowDialogue(string[] dialogueLines)
    {
        dialoguePanel.SetActive(true);
        foreach (var line in dialogueLines)
        {
            dialogueText.text = line;
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return));
            yield return null;
        }
        dialoguePanel.SetActive(false);
    }

    // 미니게임 진행 코루틴
    private IEnumerator MinigameCoroutine()
    {
        minigameUI.SetActive(true);
        float currentTime = gameDuration;
        int tapCount = 0;
        tapCountText.text = $"Hit Count: {tapCount}";

        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            timerText.text = $"Remaining Time: {currentTime:F0}";
            if (Input.GetKeyDown(KeyCode.Space))
            {
                tapCount++;
                tapCountText.text = $"Hit Count: {tapCount}";
            }
            yield return null;
        }
        finalTapCount = tapCount;
        minigameUI.SetActive(false);
    }

    // 보상 내용 처리 및 표시
    private void ProcessAndShowReward()
    {
        string rewardMessage = "No rewards have been obtained.";
        if (GameManager.Instance != null)
        {
            float buffValue = GameManager.Instance.regenBuffValue;
            if (buffValue >= 1.0f) rewardMessage = "I can feel the overflowing power of life!\n[Heal] Get buffs (Strong) (Boss play)";
            else if (buffValue >= 0.5f) rewardMessage = "I won the tears of an old tree!\n[Heal] Get buffs(Weak)(Boss play)";
            else if (buffValue > 0f) rewardMessage = "I barely felt the energy of life.";
        }

        rewardText.text = rewardMessage;
        rewardPanel.SetActive(true);
    }

    // 보상 창의 "돌아가기" 버튼에 연결할 함수
    public void OnClickReturnButton()
    {
        if (GameManager.Instance != null && !string.IsNullOrEmpty(GameManager.Instance.sceneNameBeforePortal))
        {
            // PlayerSpawner를 사용하지 않고, GameManager의 정보로 직접 이동
            SceneManager.LoadScene(GameManager.Instance.sceneNameBeforePortal);
            // 씬이 로드된 후 플레이어 위치를 설정하는 로직이 필요 (예: PlayerStartPoint 스크립트)
        }
    }
}
