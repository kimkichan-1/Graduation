# CLAUDE.md 개선 제안 사항

이 문서는 기존 CLAUDE.md에 추가되어야 할 중요한 아키텍처 패턴들을 제안합니다.
이러한 패턴들은 여러 파일을 함께 읽어야만 이해할 수 있는 "큰 그림" 아키텍처입니다.

---

## 추가 제안 섹션 1: Critical Architecture Patterns

### Singleton Lifecycle Management (3-Tier System)

이 프로젝트는 **3단계 싱글톤 지속성 계층**을 사용합니다:

**Tier 1: Permanent Singletons** (항상 유지)
- `GameManager.Instance` - 저장/로드 조정, 현재 슬롯 추적
- `SaveManager.Instance` - 파일 I/O 작업
- Main 메뉴 복귀 시에도 살아남음 (다음 게임 세션을 위해)

**Tier 2: Gameplay Singletons** (게임플레이 중만 유지)
- `PlayerController.Instance` - 플레이어 제어 및 상태
- `Inventory.instance` - 아이템 관리
- `PauseMenuUI.OnGoMainButton()`을 통해 Main 메뉴로 돌아갈 때 파괴됨

**Tier 3: Scene-Specific Singletons** (특정 씬에서만)
- `BossGameManager.Instance` - 보스 전투 상태
- `ShopManager.Instance` - 상점 시스템
- `BlacksmithMinigameManager.Instance` - 미니게임 관리
- 씬 전환 시 자동으로 파괴됨

**핵심 패턴: Conditional DontDestroyOnLoad**
```csharp
// DontDestroyOnLoadManager.cs
public static bool isReturningToMainMenu = false;

void Awake() {
    if (isReturningToMainMenu) {
        return; // DontDestroyOnLoad 스킵 - 객체가 파괴됨
    }
    // 일반 지속성 로직
    DontDestroyOnLoad(gameObject);
}
```

### Save System Architecture (2-Layer Design)

**Layer 1: SaveManager (Persistence Layer)**
- 순수 파일 I/O 작업만 처리
- JSON 직렬화 (JsonUtility 사용)
- 메서드: `SaveGame()`, `LoadGame()`, `DeleteSave()`, `GetAllSaveSlots()`

**Layer 2: GameManager (Business Logic Layer)**
- 데이터 수집/적용 조정
- `currentSaveSlot` 추적 관리
- 모든 게임 시스템과 조정

**왜 2단계인가?**
- 관심사의 명확한 분리
- SaveManager는 저장 방식만 알면 됨 (JSON, XML 등으로 쉽게 교체 가능)
- GameManager는 무엇을 저장할지만 알면 됨

### Multi-Stage Loading Sequence

**핵심 질문: 왜 저장된 씬을 바로 로드하지 않나요?**

**답변: DontDestroyOnLoad 객체 생성을 위해 Stage1을 먼저 로드해야 합니다**

```
Load Game Sequence:
1. GameManager.ApplySaveData() sets isLoadingGame = true
2. Load Stage1 FIRST (creates Player, Inventory, Camera via DontDestroyOnLoad)
3. VerifyEssentialObjects() checks if Player/Camera/Inventory exist
4. Load ACTUAL saved scene
5. Wait 0.3s for initialization
6. Apply weapon FIRST (affects other stats)
7. Apply stats, inventory, cards
8. Set isLoadingGame = false
```

**만약 저장된 씬을 직접 로드하면?**
- Player GameObject가 존재하지 않을 수 있음 (DontDestroyOnLoad로 생성되지 않음)
- Inventory.instance가 null일 수 있음
- Camera가 없어 화면이 검게 보일 수 있음

### Auto-Save Design Pattern

**핵심 결정: 플레이어의 현재 위치가 아닌 스폰 포인트에 저장**

```csharp
// PlayerController.OnSceneLoaded()
PlayerSpawnPoint spawnPoint = FindObjectOfType<PlayerSpawnPoint>();
if (spawnPoint != null) {
    savedSpawnPosition = spawnPoint.transform.position; // Auto-save용
}

// GameManager auto-save는 savedSpawnPosition 사용
```

**왜 스폰 포인트를 사용하나요?**
- 플레이어가 안전한 위치에서 항상 리스폰 보장
- 공중이나 적 안에서 리스폰 방지
- 레벨 디자이너가 스폰 위치 제어 가능

---

## 추가 제안 섹션 2: Data Flow & Critical Integration Points

### Stat Recalculation Cascade

**언제든 스탯이 변경되면, 이 흐름을 따라야 합니다:**

```
Stat Change Event
  ↓
PlayerController.RecalculateStats()
  ├─ Calculate moveSpeed = weaponMoveSpeed + bonusMoveSpeed
  ├─ Calculate dash stats from weapon
  └─ Calculate jump stats
  ↓
PlayerController.UpdateAllStatsUI()
  ↓
StatsUIManager.UpdateStatsUI()
  └─ UI 업데이트 완료
```

**트리거 지점:**
- 무기 장착 시 (`EquipWeapon()`)
- 레벨업 스탯 선택 시 (`PlayerStats.FinishUpgrade()`)
- 상점 구매 시 (`ShopItemData.ApplyEffect()`)
- 미니게임 보상 시 (`BlacksmithMinigameManager.CalculateAndGrantRewards()`)
- 저장 데이터 로드 시 (`GameManager.ApplyDataToPlayerCoroutine()`)

**주의: 이 흐름을 건너뛰면 UI와 실제 스탯이 동기화되지 않습니다!**

### Load Sequence Critical Ordering

**핵심 규칙: 무기를 먼저 적용한 후 다른 스탯 적용**

```csharp
// GameManager.ApplyDataToPlayerCoroutine()

// 1. 무기 먼저 적용 (다른 스탯에 영향을 줌)
if (tempLoadData.hasSword) {
    player.EquipSword(swordStats);
}

// 2. 그 다음 스탯 적용
playerStats.bonusAttackPower = tempLoadData.bonusAttackPower;
playerStats.bonusMoveSpeed = tempLoadData.bonusMoveSpeed;

// 3. 파생 스탯 재계산
player.RecalculateStats();
```

**왜 순서가 중요한가?**
- `WeaponStats`는 기본 moveSpeed를 제공합니다
- `bonusMoveSpeed`는 무기 속도에 **추가**됩니다
- 무기를 나중에 적용하면 보너스가 덮어씌워질 수 있습니다

### Time.timeScale Coordination

**문제: 여러 시스템이 Time.timeScale을 제어함**
- `LevelUpUIManager` - 레벨업 UI 중 일시정지
- `BlacksmithMinigameManager` - 미니게임 제어
- `PauseMenuUI` - ESC 메뉴
- `DialogueController` - 대화 중

**해결책: isGamePausedByManager 플래그**
```csharp
// BlacksmithMinigameManager.EndGame()
Time.timeScale = 0f;
isGamePausedByManager = true; // PlayerStats가 자동 재개하지 못하도록

// PlayerStats.FinishUpgrade()
if (!BlacksmithMinigameManager.isGamePausedByManager) {
    Time.timeScale = 1f; // 미니게임이 시간을 제어하지 않을 때만 재개
}
```

**주의: 미니게임 종료 시 이 플래그를 설정하지 않으면 시간 제어 충돌 발생!**

### Portal Self-Destruction Pattern

**핵심 패턴: 정적 목록을 사용한 일회용 포털**

```csharp
// PortalController.cs
public static List<string> usedPortalIDs = new List<string>();

void Start() {
    if (usedPortalIDs.Contains(portalID)) {
        Destroy(gameObject); // 이미 사용됨
        return;
    }
}

void OnConfirm() {
    usedPortalIDs.Add(portalID); // 사용됨으로 표시
    SceneManager.LoadScene(sceneToLoad);
    Destroy(gameObject); // 즉시 파괴
}
```

**왜 이렇게 작동하나요?**
- `usedPortalIDs`는 정적이므로 씬 로드 간에 유지됩니다
- 씬이 다시 로드될 때 포털은 Start()에서 자신을 파괴합니다
- 이를 통해 포털 재사용을 방지합니다

**주의: 새 게임을 시작할 때 이 목록을 지워야 합니다!**
- `GameManager.SaveCurrentGame()`에서 SaveData에 저장됨
- `GameManager.ApplyDataToPlayerCoroutine()`에서 복원됨

---

## 추가 제안 섹션 3: Static Data Architecture

### Complete Static Data Inventory

이 프로젝트는 씬 간 데이터 지속성을 위해 여러 정적 변수/컬렉션을 사용합니다:

**Game State:**
- `GameData.SelectedWeapon` (string) - 무기 선택 씬에서의 무기 선택
- `GameManager.isLoadingGame` (bool) - 로드 프로세스 중 초기화 건너뛰기
- `GameManager.isNewGame` (bool) - New Game vs Load Game 플로우 구분

**Cross-Scene Progress:**
- `PortalController.usedPortalIDs` (List<string>) - 사용된 포털 추적
- `MidBossController.completedEventIDs` (List<string>) - 완료된 이벤트 추적

**Scene Return Info:**
- `StatueInteraction.previousSceneName` (string) - 미니게임 복귀용
- `PortalReturnData.hasReturnInfo` (bool) - 복귀 정보 있음 플래그
- `PortalReturnData.returnPosition` (Vector3) - 복귀 위치
- `PortalReturnData.previousSceneName` (string) - 복귀할 씬

**System Coordination:**
- `BlacksmithMinigameManager.isGamePausedByManager` (bool) - 시간 제어 조정
- `DontDestroyOnLoadManager.isReturningToMainMenu` (bool) - UI 지속성 제어
- `DontDestroyOnLoadManager.instances` (Dictionary<string, GameObject>) - 모든 지속 객체 추적
- `PlayerStats.isLevelUpPending` (bool) - 지연된 레벨업 UI 추적

**Static Data Lifecycle:**
```
New Game:
  → 모든 정적 데이터 초기화됨
  → GameData.SelectedWeapon만 Weapon 씬에서 설정됨

During Gameplay:
  → 정적 목록이 누적됨 (usedPortalIDs, completedEventIDs)
  → 조정 플래그가 설정/해제됨

Save Game:
  → GameManager가 SaveData로 정적 컬렉션 복사
  → JSON 파일에 직렬화됨

Load Game:
  → SaveData에서 정적 컬렉션 복원
  → 플래그가 적절히 설정됨

Return to Main Menu:
  → DontDestroyOnLoadManager.instances 지워짐
  → 다른 정적 데이터는 유지됨 (다음 게임까지)
```

---

## 추가 제안 섹션 4: Common Pitfalls & Solutions

### Pitfall 1: FindGameObjectWithTag 성능

**문제:**
```csharp
// 미니게임에서 흔히 사용됨
PlayerStats stats = GameObject.FindGameObjectWithTag("Player")
    .GetComponent<PlayerStats>();
```

**왜 문제인가?**
- `FindGameObjectWithTag()`는 모든 GameObject를 검색합니다 (느림)
- 매 프레임 호출되면 성능 문제 발생
- 태그가 올바르게 설정되지 않으면 null 참조

**해결책:**
```csharp
// 캐시된 참조 사용
private PlayerStats playerStats;

void Start() {
    // PlayerController가 싱글톤이므로
    if (PlayerController.Instance != null) {
        playerStats = PlayerController.Instance.GetComponent<PlayerStats>();
    }
}
```

### Pitfall 2: 중복 Damage 계산

**문제:**
- `DiceAnimationManager`와 `ClashManager` 모두 damage 계산을 구현
- 한 곳을 업데이트하고 다른 곳을 잊어버리기 쉬움
- 게임플레이 버그로 이어질 수 있음

**현재 상태:**
```csharp
// DiceAnimationManager.ApplyClashDamage()
int finalDamage = Mathf.Max(1,
    (attackPower + attackRoll) - defensePower - defenseRoll);

// ClashManager.ResolveClash()
int finalDamage = Mathf.Max(1,
    (attackPower + attackRoll) - defensePower - defenseRoll);
```

**더 나은 설계:**
```csharp
public static class CombatCalculator {
    public static int CalculateClashDamage(
        int attackPower, int attackRoll,
        int defensePower, int defenseRoll
    ) {
        return Mathf.Max(1,
            (attackPower + attackRoll) - defensePower - defenseRoll);
    }
}

// 두 클래스 모두 이것을 사용
int damage = CombatCalculator.CalculateClashDamage(...);
```

### Pitfall 3: 싱글톤 초기화 순서

**문제:**
```csharp
void Start() {
    // PlayerController.Instance가 아직 준비되지 않았을 수 있음!
    PlayerController.Instance.RecalculateStats();
}
```

**왜 문제인가?**
- Unity는 Start() 호출 순서를 보장하지 않습니다
- 싱글톤이 Awake()에서 초기화되었더라도, 컴포넌트가 준비되지 않았을 수 있습니다

**해결책:**
```csharp
void Start() {
    if (PlayerController.Instance != null) {
        // 또는 코루틴으로 프레임 대기
        StartCoroutine(InitializeAfterFrame());
    }
}

IEnumerator InitializeAfterFrame() {
    yield return null; // 1프레임 대기
    PlayerController.Instance.RecalculateStats();
}
```

### Pitfall 4: Scene 특정 컴포넌트 누수

**문제: Stage3-specific 컴포넌트가 다른 씬에서 활성화 상태로 남아있음**

```csharp
// 나쁜 예 - Stage3Manager가 없으면
PlayerSwimming swimming = player.GetComponent<PlayerSwimming>();
// swimming.enabled가 여전히 true일 수 있음!
```

**해결책: Stage3Manager 패턴**
```csharp
// Stage3Manager.cs
void Start() {
    playerSwimming.enabled = true;
    playerOxygen.enabled = true;
}

void OnDestroy() {
    if (playerSwimming != null) playerSwimming.enabled = false;
    if (playerOxygen != null) playerOxygen.enabled = false;
}
```

**교훈: 씬별 컴포넌트는 기본적으로 비활성화하고, 씬 관리자가 활성화하도록 하세요**

### Pitfall 5: Time.timeScale 누수

**문제:**
```csharp
void ShowLevelUpUI() {
    Time.timeScale = 0f; // 일시정지
}

// 그런데 Time.timeScale을 재개하는 것을 잊어버림!
```

**결과: 게임이 영원히 일시정지됨**

**해결책: 항상 쌍으로 작업**
```csharp
void ShowLevelUpUI() {
    Time.timeScale = 0f;
}

void OnUpgradeSelected() {
    ApplyUpgrade();
    Time.timeScale = 1f; // 항상 재개
}
```

**더 나은 해결책: Stack 기반 시스템**
```csharp
public class TimeManager {
    private static Stack<string> pauseReasons = new Stack<string>();

    public static void RequestPause(string reason) {
        pauseReasons.Push(reason);
        Time.timeScale = 0f;
    }

    public static void RequestResume(string reason) {
        if (pauseReasons.Count > 0 && pauseReasons.Peek() == reason) {
            pauseReasons.Pop();
            Time.timeScale = pauseReasons.Count == 0 ? 1f : 0f;
        }
    }
}
```

---

## 추가 제안 섹션 5: Quick Reference for Common Tasks

### Finding Objects Across Scenes

**플레이어 찾기:**
```csharp
// 최선 (가능하다면)
PlayerController player = PlayerController.Instance;

// 미니게임에서
GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
PlayerStats stats = playerObj.GetComponent<PlayerStats>();

// 로드 시퀀스에서
PlayerController player = FindObjectOfType<PlayerController>();
```

**주의: 미니게임은 태그 조회를 사용합니다 (Player는 DontDestroyOnLoad이므로)**

### Triggering Auto-Save

```csharp
// 씬 전환 시 자동으로 트리거됨
// PlayerController.OnSceneLoaded()에서

// 수동 트리거:
if (GameManager.Instance != null && GameManager.Instance.currentSaveSlot > 0) {
    GameManager.Instance.AutoSave(savedSpawnPosition);
}
```

### Adding Permanent Stat Bonuses

```csharp
// 올바른 방법 (자동으로 UI 업데이트)
PlayerStats stats = PlayerController.Instance.GetComponent<PlayerStats>();
stats.AddAttackPower(bonusAmount);
stats.AddMoveSpeed(speedBonus);

// PlayerController.RecalculateStats()가 자동으로 호출됨
// UI가 자동으로 업데이트됨
```

### Portal 시스템 디버깅

```csharp
// 사용된 포털 확인
Debug.Log("Used portals: " + string.Join(", ", PortalController.usedPortalIDs));

// 특정 포털이 사용되었는지 확인
if (PortalController.usedPortalIDs.Contains("AncientPortal_1")) {
    Debug.Log("Ancient Portal 1 already used");
}

// 새 게임을 위해 리셋 (GameManager가 자동으로 처리)
PortalController.usedPortalIDs.Clear();
```

---

## 권장사항: CLAUDE.md 구조 개선

기존 CLAUDE.md는 포괄적이지만, 다음과 같은 구조 개선을 권장합니다:

1. **최상단에 Quick Reference 섹션 유지** ✓ (이미 있음)

2. **"Code Architecture" 섹션을 두 부분으로 분리:**
   - Part A: Individual System Documentation (현재 내용)
   - Part B: Cross-System Architecture Patterns (새로 추가할 내용)

3. **새 섹션 추가:**
   - "Critical Architecture Patterns" (싱글톤, 저장 시스템, 로딩 시퀀스)
   - "Data Flow & Integration Points" (스탯 전파, 시간 관리)
   - "Static Data Architecture" (모든 정적 데이터 인벤토리)
   - "Common Pitfalls & Solutions" (실제 버그 방지 패턴)

4. **"Common Development Patterns" 섹션 확장:**
   - 기존: 작업별 체크리스트 (좋음!)
   - 추가: 왜 그 단계들이 필요한지 설명

5. **Quick Reference 카드 추가 (맨 마지막):**
   - "어디에서 데이터를 찾을 것인가"
   - "주요 메서드 체이닝"
   - "일반적인 gotchas"
   - "디버깅 팁"

---

## 결론

이러한 개선사항들은 다음과 같이 도움이 됩니다:

1. **더 빠른 온보딩**: 새로운 Claude Code 인스턴스가 여러 파일을 읽지 않고도 아키텍처 이해 가능
2. **버그 방지**: 일반적인 함정들이 명시적으로 문서화됨
3. **더 나은 의사결정**: 왜 특정 패턴이 사용되는지 이해하여 일관성 있는 코드 작성 가능
4. **더 쉬운 디버깅**: 데이터가 어디에 있고 어떻게 흐르는지 빠르게 찾을 수 있음

이러한 섹션들을 기존 CLAUDE.md에 추가하면, 코드베이스를 처음 접하는 개발자들이 훨씬 더 생산적으로 작업할 수 있을 것입니다.
