using UnityEngine;

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

    public void ReportSeaMonsterDeath()
    {
        // 이미 스폰 이벤트가 발생했다면 더 이상 카운트하지 않음
        if (clamSpawnEventTriggered) return;

        seaMonsterKills++;
        Debug.Log($"SeaMonster 처치! ({seaMonsterKills}/{seaMonsterKillRequirement})");

        // 목표 처치 수를 달성했다면
        if (seaMonsterKills >= seaMonsterKillRequirement)
        {
            clamSpawnEventTriggered = true; // 스폰 이벤트 발생 스위치 켜기
            Debug.Log("목표 달성! ClamMonster 웨이브를 시작합니다.");
            SpawnClamWave(); // 아래의 스폰 함수 호출
        }
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