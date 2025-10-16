// 파일명: MinigameManager.cs
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
        dialoguePanel.SetActive(false);
        minigameUI.SetActive(false);
        rewardPanel.SetActive(false);
    }

    public void StartEventSequence()
    {
        StartCoroutine(FullSequenceCoroutine());
    }

    private IEnumerator FullSequenceCoroutine()
    {
        yield return StartCoroutine(ShowDialogue(storyDialogue));
        yield return StartCoroutine(ShowDialogue(instructionDialogue));
        yield return StartCoroutine(MinigameCoroutine());

        if (LifeGameManager.Instance != null)
        {
            // 1. 미니게임 결과를 GameManager로 전송
            LifeGameManager.Instance.ProcessMinigameResult(finalTapCount);

            // --- ▼▼▼▼▼ 수정된 부분 (이 줄을 추가하세요) ▼▼▼▼▼ ---
            // 2. GameManager에게 즉시 체력 회복을 명령
            LifeGameManager.Instance.ApplyImmediateHeal();
            // --- ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲ ---
        }

        ProcessAndShowReward();
    }

    private IEnumerator ShowDialogue(string[] dialogueLines)
    {
        dialoguePanel.SetActive(true);
        foreach (var line in dialogueLines)
        {
            dialogueText.text = line;
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.W));
            yield return null;
        }
        dialoguePanel.SetActive(false);
    }

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

    private void ProcessAndShowReward()
    {
        string rewardMessage = "아무런 보상도 얻지 못했다.";
        if (LifeGameManager.Instance != null)
        {
            float buffValue = LifeGameManager.Instance.regenBuffValue;
            if (buffValue >= 1.0f) rewardMessage = "생명의 넘치는 힘이 느껴진다!\n[치유] 버프 획득(강)(보스전에서 발동)";
            else if (buffValue >= 0.5f) rewardMessage = "고목의 눈물을 얻었다!\n[치유] 버프 획득(약)(보스전에서 발동)";
            else if (buffValue > 0f) rewardMessage = "생명의 기운을 희미하게나마 느꼈다.";
        }

        rewardText.text = rewardMessage;
        rewardPanel.SetActive(true);
    }

    public void OnClickReturnButton()
    {
        if (LifeGameManager.Instance != null && !string.IsNullOrEmpty(LifeGameManager.Instance.sceneNameBeforePortal))
        {
            SceneManager.LoadScene(LifeGameManager.Instance.sceneNameBeforePortal);
        }
    }
}
