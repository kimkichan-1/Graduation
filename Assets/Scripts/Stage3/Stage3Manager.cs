using TMPro;
using UnityEngine;
using System.Collections; // 코루틴 사용 시 필요

public class Stage3Manager : MonoBehaviour
{
    public static Stage3Manager instance;

    [Header("SeaMonster 처치 미션")]
    [Tooltip("이 수만큼 SeaMonster를 처치해야 ClamMonster가 등장합니다.")]
    [SerializeField] private int seaMonsterKillRequirement = 5; // 예시: 5마리

    [Header("ClamMonster 스폰 설정")]
    [SerializeField] private GameObject clamMonsterPrefab; // 스폰할 조개 몬스터 프리팹
    [Tooltip("ClamMonster가 나타날 위치들 (빈 오브젝트들을 연결)")]
    [SerializeField] private Transform[] spawnPoints; // 여러 개의 스폰 위치

    [Header("대화 설정")]
    [SerializeField] private GameObject dialoguePanel; // 대화창 UI 패널
    [SerializeField] private TextMeshProUGUI dialogueText; // 대화 내용 텍스트
    [SerializeField, TextArea(3, 5)] private string[] entryDialogue; // 스테이지 입장 시 대화
    [SerializeField, TextArea(3, 5)] private string[] clamSpawnDialogue; // 조개 몬스터 스폰 시 대화

    private PlayerController playerController; // 플레이어 이동 제어용

    private int seaMonsterKills = 0; // 현재까지 처치한 SeaMonster 수
    private bool clamSpawnEventTriggered = false; // 스폰 이벤트가 한 번만 발생하도록
    // 씬을 떠날 때 참조하기 위해 변수로 저장
    private PlayerSwimming swimmingLogic;
    private PlayerOxygen oxygenLogic;

    void Awake()
    {
        // 싱글턴 인스턴스 설정
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // "Player" 태그를 가진 플레이어 오브젝트를 찾습니다.
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("씬에 'Player' 태그를 가진 오브젝트가 없습니다!");
            return;
        }
        playerController = player.GetComponent<PlayerController>(); // 플레이어 컨트롤러 참조 저장
        if (dialoguePanel != null && dialogueText != null && entryDialogue.Length > 0)
        {
            StartCoroutine(ShowStageEntryDialogue());
        }
        else
        {
            // 대화가 없으면 바로 시스템 활성화
            ActivateStage3Systems(player);
        }

        // --- PlayerSwimming 활성화 ---
        swimmingLogic = player.GetComponent<PlayerSwimming>();
        if (swimmingLogic != null)
        {
            swimmingLogic.enabled = true;
            Debug.Log("PlayerSwimming 활성화 완료.");
        }

        // --- ▼▼▼▼▼ 수정된 부분 (PlayerOxygen 활성화 추가) ▼▼▼▼▼ ---
        // 플레이어에게서 PlayerOxygen 컴포넌트를 찾아 변수에 저장
        oxygenLogic = player.GetComponent<PlayerOxygen>();
        if (oxygenLogic != null)
        {
            // PlayerOxygen 스크립트를 활성화시킵니다.
            oxygenLogic.enabled = true;
            Debug.Log("PlayerOxygen 활성화 완료.");
        }
        // --- ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲ ---
    }
    private IEnumerator ShowStageEntryDialogue()
    {
        // 대화 시작 전에 게임 일시정지 및 플레이어 이동 불가
        Time.timeScale = 0f;
        if (playerController != null) playerController.canMove = false;

        dialoguePanel.SetActive(true);
        foreach (var line in entryDialogue)
        {
            dialogueText.text = line;
            // W 키 또는 마우스 클릭 대기 (Time.timeScale이 0일 때도 Input은 작동)
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.W));
            yield return null; // 한 프레임 더 대기하여 입력 중복 방지
        }
        dialoguePanel.SetActive(false);

        // 대화 종료 후 게임 재개 및 플레이어 이동 가능
        if (playerController != null) playerController.canMove = true;
        Time.timeScale = 1f;

        // 대화가 끝나면 Stage3 시스템 활성화
        ActivateStage3Systems(playerController.gameObject); // playerController에서 player 오브젝트 다시 얻기
    }
    private void ActivateStage3Systems(GameObject player)
    {
        // 이 함수는 Start에서 직접 호출되거나, 입장 대화 코루틴 마지막에 호출됩니다.
        swimmingLogic = player.GetComponent<PlayerSwimming>();
        if (swimmingLogic != null) swimmingLogic.enabled = true;

        oxygenLogic = player.GetComponent<PlayerOxygen>();
        if (oxygenLogic != null) oxygenLogic.enabled = true;

        Debug.Log("Stage3 시스템 (Swimming, Oxygen) 활성화 완료.");
    }

    public void ReportSeaMonsterDeath()
    {
        if (clamSpawnEventTriggered) return;

        seaMonsterKills++;
        Debug.Log($"SeaMonster 처치! ({seaMonsterKills}/{seaMonsterKillRequirement})");

        if (seaMonsterKills >= seaMonsterKillRequirement)
        {
            clamSpawnEventTriggered = true;
            Debug.Log("목표 달성! ClamMonster 스폰 이벤트를 시작합니다.");

            // --- ▼▼▼▼▼ 수정된 부분 (스폰 전 대화 코루틴 시작) ▼▼▼▼▼ ---
            // 스폰 함수를 직접 호출하는 대신, 대화 코루틴을 시작합니다.
            if (dialoguePanel != null && dialogueText != null && clamSpawnDialogue.Length > 0)
            {
                StartCoroutine(ShowClamSpawnDialogue());
            }
            else
            {
                // 대화 내용이 없으면 바로 스폰
                SpawnClamWave();
            }
            // --- ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲ ---
        }
    }
    private IEnumerator ShowClamSpawnDialogue()
    {
        // 대화 시작 전에 게임 일시정지 및 플레이어 이동 불가
        Time.timeScale = 0f;
        // playerController 참조가 없을 수 있으므로 다시 찾아줍니다. (안전 장치)
        if (playerController == null) playerController = FindObjectOfType<PlayerController>();
        if (playerController != null) playerController.canMove = false;

        dialoguePanel.SetActive(true);
        foreach (var line in clamSpawnDialogue)
        {
            dialogueText.text = line;
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.W));
            yield return null;
        }
        dialoguePanel.SetActive(false);

        // 대화 종료 후 게임 재개 및 플레이어 이동 가능
        if (playerController != null) playerController.canMove = true;
        Time.timeScale = 1f;

        // 대화가 끝나면 조개 몬스터 스폰
        SpawnClamWave();
    }

    // [Private 함수] 지정된 모든 위치에 ClamMonster를 스폰
    private void SpawnClamWave()
    {
        // 프리팹이 연결되었는지 확인
        if (clamMonsterPrefab == null)
        {
            Debug.LogError("ClamMonster 프리팹이 Stage3Manager에 연결되지 않았습니다.");
            return;
        }

        // spawnPoints 배열에 있는 모든 위치에 몬스터 생성
        foreach (Transform point in spawnPoints)
        {
            Instantiate(clamMonsterPrefab, point.position, point.rotation);
        }
    }

    // 이 Stage3Manager 오브젝트가 파괴될 때 (즉, Stage3 씬을 떠날 때) 호출됩니다.
    private void OnDestroy()
    {
        // 씬을 떠날 때 Stage3 전용 컴포넌트들을 비활성화하여
        // 다른 씬에 영향을 주지 않도록 합니다.
        if (swimmingLogic != null)
        {
            swimmingLogic.enabled = false;
        }

        // --- ▼▼▼▼▼ 수정된 부분 (PlayerOxygen 비활성화 추가) ▼▼▼▼▼ ---
        if (oxygenLogic != null)
        {
            oxygenLogic.enabled = false;
            // 산소 UI도 함께 비활성화합니다.
            if (oxygenLogic.oxygenSlider != null)
            {
                oxygenLogic.oxygenSlider.gameObject.SetActive(false);
            }
        }
        // --- ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲ ---
    }
}