using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 게임 전체 상태 관리자
/// 현재 세이브 슬롯 추적, 데이터 수집/적용
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance;

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning($"GameManager 인스턴스가 이미 존재합니다! 새로운 오브젝트 '{gameObject.name}' 파괴");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
    #endregion

    [Header("게임 상태")]
    public int currentSaveSlot = -1;           // 현재 플레이 중인 슬롯 번호 (1-3, -1 = 미설정)
    public bool isNewGame = false;             // 신규 게임 여부
    public bool isLoadingGame = false;         // 세이브 로드 중 여부 (PlayerStats 초기화 방지용)
    public string selectedWeapon = "None";     // Weapon 씬에서 선택한 무기

    [Header("플레이 시간 추적")]
    private float sessionStartTime;            // 세션 시작 시간
    private float totalPlayTime = 0f;          // 총 플레이 시간

    [Header("무기 스탯 (Inspector에서 할당)")]
    public WeaponStats swordStats;             // Sword WeaponStats
    public WeaponStats maceStats;              // Mace WeaponStats
    public WeaponStats lanceStats;             // Lance WeaponStats

    void Start()
    {
        sessionStartTime = Time.time;
    }

    void Update()
    {
        // 플레이 시간 업데이트
        if (currentSaveSlot > 0)
        {
            totalPlayTime += Time.deltaTime;
        }
    }

    /// <summary>
    /// 신규 게임 시작 준비
    /// </summary>
    public void PrepareNewGame()
    {
        isNewGame = true;
        selectedWeapon = "None";
        currentSaveSlot = -1;
        totalPlayTime = 0f;
        sessionStartTime = Time.time;
        Debug.Log("신규 게임 모드로 설정");
    }

    /// <summary>
    /// 기존 게임 로드 준비
    /// </summary>
    public void PrepareLoadGame(int slotNumber)
    {
        isNewGame = false;
        currentSaveSlot = slotNumber;
        Debug.Log($"슬롯 {slotNumber} 로드 준비");
    }

    /// <summary>
    /// 슬롯 선택 (신규 게임 또는 덮어쓰기)
    /// </summary>
    public void SelectSaveSlot(int slotNumber)
    {
        currentSaveSlot = slotNumber;
        Debug.Log($"슬롯 {slotNumber} 선택됨");
    }

    /// <summary>
    /// 현재 게임 상태 수집하여 SaveData로 변환
    /// </summary>
    /// <param name="spawnPosition">PlayerSpawnPoint 위치 (null이면 현재 플레이어 위치 사용)</param>
    public SaveData CollectSaveData(Vector3? spawnPosition = null)
    {
        SaveData data = new SaveData(currentSaveSlot);

        // 플레이어 찾기
        PlayerController player = PlayerController.Instance;
        if (player == null)
        {
            Debug.LogWarning("플레이어를 찾을 수 없습니다. 기본 데이터로 저장합니다.");
            return data;
        }

        PlayerStats stats = player.GetComponent<PlayerStats>();
        PlayerHealth health = player.GetComponent<PlayerHealth>();

        // 메타 정보
        data.playTime = totalPlayTime;
        data.saveName = $"Save {currentSaveSlot}";

        // 플레이어 기본 스탯
        if (stats != null)
        {
            data.playerLevel = stats.level;
            data.currentXp = stats.currentXp;
            data.xpToNextLevel = stats.xpToNextLevel;
            data.currentMoney = stats.currentMoney;
            data.bonusAttackPower = stats.bonusAttackPower;
            data.bonusMoveSpeed = stats.bonusMoveSpeed;
        }

        if (health != null)
        {
            data.currentHealth = (int)health.currentHealth;
            data.maxHealth = (int)health.maxHealth;
            data.defense = health.defense;
            data.baseMaxHealth = health.baseMaxHealth;
        }

        // 장착 무기 - 플레이어가 아직 무기를 줍지 않았더라도 선택한 무기 정보 유지
        data.hasSword = player.hasSword;
        data.hasMace = player.hasMace;
        data.hasLance = player.hasLance;

        if (player.hasSword)
        {
            data.equippedWeapon = "Sword";
        }
        else if (player.hasMace)
        {
            data.equippedWeapon = "Mace";
        }
        else if (player.hasLance)
        {
            data.equippedWeapon = "Lance";
        }
        else if (!string.IsNullOrEmpty(selectedWeapon) && selectedWeapon != "None")
        {
            // 플레이어가 무기를 아직 줍지 않았지만 선택한 무기가 있음 (NewGame 직후)
            data.equippedWeapon = selectedWeapon;
        }
        else
        {
            data.equippedWeapon = "None";
        }

        // 위치 정보 - spawnPosition이 제공되면 그것을 사용, 아니면 현재 플레이어 위치
        Vector3 positionToSave = spawnPosition ?? player.transform.position;
        data.currentScene = SceneManager.GetActiveScene().name;
        data.positionX = positionToSave.x;
        data.positionY = positionToSave.y;
        data.positionZ = positionToSave.z;

        // 인벤토리 (간단 버전 - 추후 확장 가능)
        if (Inventory.instance != null)
        {
            data.inventoryItems.Clear();
            foreach (var item in Inventory.instance.items)
            {
                data.inventoryItems.Add(item.itemName);
            }
        }

        // 게임 진행 상태
        data.completedEvents.Clear();
        data.completedEvents.AddRange(MidBossController.completedEventIDs);

        data.usedPortals.Clear();
        data.usedPortals.AddRange(PortalController.usedPortalIDs);

        return data;
    }

    /// <summary>
    /// SaveData를 현재 게임에 적용
    /// </summary>
    public void ApplySaveData(SaveData data)
    {
        if (data == null || data.isEmpty)
        {
            Debug.LogError("빈 세이브 데이터는 적용할 수 없습니다!");
            return;
        }

        // 로딩 플래그 설정 (PlayerStats Start() 초기화 방지)
        isLoadingGame = true;

        currentSaveSlot = data.slotNumber;
        totalPlayTime = data.playTime;
        selectedWeapon = data.equippedWeapon;

        // 정적 데이터 복원 (WeaponSpawner를 위해 GameData에도 설정)
        GameData.SelectedWeapon = data.equippedWeapon;

        MidBossController.completedEventIDs.Clear();
        MidBossController.completedEventIDs.AddRange(data.completedEvents);

        PortalController.usedPortalIDs.Clear();
        PortalController.usedPortalIDs.AddRange(data.usedPortals);

        // 씬 로드 (플레이어 스탯은 씬 로드 후 적용)
        SceneManager.sceneLoaded += OnSceneLoadedForRestore;
        SceneManager.LoadScene(data.currentScene);

        // 임시 저장 (씬 로드 후 사용)
        tempLoadData = data;
    }

    private SaveData tempLoadData; // 씬 로드 중 임시 저장

    /// <summary>
    /// 씬 로드 후 플레이어에게 데이터 적용
    /// </summary>
    private void OnSceneLoadedForRestore(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoadedForRestore;

        if (tempLoadData == null) return;

        // 플레이어 찾기 (약간의 지연 필요할 수 있음)
        Invoke(nameof(ApplyDataToPlayer), 0.5f);
    }

    private void ApplyDataToPlayer()
    {
        if (tempLoadData == null) return;

        PlayerController player = PlayerController.Instance;
        if (player == null)
        {
            Debug.LogError("플레이어를 찾을 수 없습니다!");
            isLoadingGame = false; // 실패 시에도 플래그 해제
            return;
        }

        PlayerStats stats = player.GetComponent<PlayerStats>();
        PlayerHealth health = player.GetComponent<PlayerHealth>();

        // 플레이어 스탯 적용
        if (stats != null)
        {
            stats.level = tempLoadData.playerLevel;
            stats.currentXp = tempLoadData.currentXp;
            stats.xpToNextLevel = tempLoadData.xpToNextLevel;
            stats.currentMoney = tempLoadData.currentMoney;
            stats.bonusAttackPower = tempLoadData.bonusAttackPower;
            stats.bonusMoveSpeed = tempLoadData.bonusMoveSpeed;

            // UI 업데이트
            if (stats.uiManager != null)
            {
                stats.uiManager.UpdateLevelText(stats.level);
                stats.uiManager.UpdateXpBar(stats.currentXp, stats.xpToNextLevel);
            }

            // Money UI 업데이트
            stats.NotifyMoneyChanged();
        }

        if (health != null)
        {
            health.maxHealth = tempLoadData.maxHealth;
            health.currentHealth = tempLoadData.currentHealth;
            health.defense = tempLoadData.defense;
            health.baseMaxHealth = tempLoadData.baseMaxHealth;
            health.UpdateHealthUI();
        }

        // 위치 적용
        player.transform.position = new Vector3(
            tempLoadData.positionX,
            tempLoadData.positionY,
            tempLoadData.positionZ
        );

        // 무기는 WeaponSpawner가 스폰하고 플레이어가 주워서 장착함
        // LoadGame 시에는 무기를 자동으로 장착하지 않음

        player.RecalculateStats();
        player.UpdateAllStatsUI();

        // 로딩 완료 - 플래그 해제
        isLoadingGame = false;

        tempLoadData = null;
    }

    /// <summary>
    /// 자동 저장
    /// </summary>
    /// <param name="spawnPosition">PlayerSpawnPoint 위치 (null이면 현재 플레이어 위치 사용)</param>
    public void AutoSave(Vector3? spawnPosition = null)
    {
        if (currentSaveSlot <= 0)
        {
            Debug.LogWarning("저장할 슬롯이 설정되지 않았습니다.");
            return;
        }

        SaveData data = CollectSaveData(spawnPosition);
        SaveManager.Instance.SaveGame(currentSaveSlot, data);
    }

    /// <summary>
    /// 수동 저장
    /// </summary>
    public void ManualSave()
    {
        AutoSave(); // 동일한 로직
    }
}
