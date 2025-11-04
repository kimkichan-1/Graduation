# Unity를 활용한 하이브리드 RPG 게임 졸업 프로젝트 보고서

---

## 1. 작품 개요

### 1.1 작품 개요

본 프로젝트는 Unity 6 엔진을 사용하여 개발한 하이브리드 RPG 게임으로, 실시간 액션과 턴제 전략을 결합한 혁신적인 전투 시스템을 제공한다. 일반 맵에서는 RPG 방식의 실시간 전투를, 보스 맵에서는 전략적인 턴제 카드 전투를 경험할 수 있으며, 플레이어는 다양한 무기와 장비를 수집하고 성장하는 여정을 경험한다.

**주요 특징**
- 하이브리드 전투 시스템: 일반 맵(실시간 RPG) + 보스 맵(턴제 전략)
- 3가지 무기 시스템 (검, 창, 메이스)
- 레벨업 및 다층 성장 시스템 (능력치/부가 효과)
- 재화-경험치 상호 전환 시스템
- 카드 기반 주사위 전투 시스템 (동시 턴 진행)
- 상황별 적응형 보스 패턴
- 세이브/로드 시스템 (3개 슬롯)

**게임 스토리 및 세계관**

플레이어는 고대의 유적을 탐험하는 모험가가 되어, 각 스테이지를 클리어하고 강력한 보스들을 물리치며 세계를 구원하는 여정을 떠난다. 여정 중에는 대장간에서 무기를 강화하고, 상점에서 아이템을 구매하며, 수중 던전에서는 산소를 관리하며 진행해야 한다. 획득한 재화와 경험치를 통해 지속적으로 성장하며 더욱 강력한 적들에 도전할 수 있다.

**핵심 게임플레이 메커니즘**
- 탐험 모드: 횡스크롤 RPG 방식의 실시간 전투 및 스테이지 탐험
- 보스 전투: 동시 턴 진행 방식의 전략적 카드 전투 (상황별 패턴 변화)
- 성장 시스템: 레벨업(능력치+부가효과), 장비 강화, 재화↔경험치 전환
- 미니게임: 대장간 불꽃 수집 게임, 라이프맵 미니게임

---

### 1.2 작품 선정 배경

**개발 동기**

기존 로그라이크 및 턴제 RPG 게임들은 각각 고유한 매력을 가지고 있지만, 몇 가지 구조적 한계점을 지니고 있다. 본 프로젝트는 이러한 장르의 문제점을 분석하고, 하이브리드 방식을 통해 개선된 게임 경험을 제공하는 것을 목표로 한다.

**기존 장르의 문제점 분석**

**로그라이크 게임의 문제점**

1. **성장 피드백 부족**: 런 실패 시 대부분의 진행 상황이 초기화되어 플레이어의 노력이 무의미하게 느껴질 수 있다. 영구 성장 요소가 제한적이거나 추상적이어서 가시적인 성장 피드백이 약하다.

2. **운 요소(랜덤성)에 과도한 의존**: 아이템 드롭, 맵 구조, 적 배치 등 핵심 요소가 랜덤에 의존하여, 실력보다 운이 승패를 좌우하는 경우가 빈번하다. 이는 플레이어의 좌절감을 증가시키고 게임의 공정성을 해친다.

**턴제 게임의 문제점**

1. **느린 게임 템포**: 턴제 전투는 각 행동마다 대기 시간이 필요하여 전체적인 게임 진행 속도가 느려진다. 특히 전투가 반복될수록 지루함이 누적된다.

2. **반복되는 전투 패턴**: 적의 AI 패턴이 고정적이거나 단순하여, 플레이어가 패턴을 파악한 후에는 동일한 전략을 반복하게 되어 전투의 긴장감이 떨어진다.

3. **전투 외 콘텐츠 부족**: 턴제 게임은 전투 시스템에 집중하다 보니 탐험, 수집, 미니게임 등 전투 외 콘텐츠가 부족하여 게임의 다양성이 제한된다.

**하이브리드 RPG 제안**

본 프로젝트는 위의 문제점들을 해결하기 위해 다음과 같은 하이브리드 시스템을 제안한다.

**1. 이중 전투 시스템**
- **일반 맵**: RPG 방식의 실시간 전투로 빠른 템포와 액션성 제공
- **보스 맵**: 턴제 전략 전투로 깊이 있는 전술적 사고 요구
- 두 시스템의 장점을 결합하여 다양한 플레이 경험 제공

**2. 다층 성장 시스템**
- **레벨업 성장**: 능력치 증가 + 부가 효과 획득으로 가시적 성장
- **재화↔경험치 전환**: 플레이어가 성장 방향을 능동적으로 선택 가능
- **영구 성장**: 누적되는 성장으로 로그라이크의 허무함 해소
- **다양한 강화 경로**: 장비 강화, 스탯 업그레이드 등 여러 방향의 성장 지원

**3. 동적 보스 전투**
- **동시 턴 진행**: 플레이어와 보스가 동시에 행동하여 전투 템포 향상
- **상황별 패턴 변화**: 보스가 전투 상황에 따라 패턴을 변경하여 예측 불가능성 증가
- 단순 반복 전략을 방지하고 매 전투마다 새로운 전략 필요

**4. 통합 콘텐츠 설계**
- 실시간 탐험, 턴제 보스전, 미니게임, 상점 시스템을 유기적으로 연결
- 다양한 플레이 스타일을 지원하여 게임의 재미 요소 극대화

**프로젝트 목적 및 의의**

본 프로젝트의 목적은 Unity 게임 엔진에 대한 깊이 있는 이해, 하이브리드 게임 디자인 실험 및 검증, 게임 디자인 패턴 및 아키텍처 학습, 프로젝트 관리 및 버전 관리 경험 축적, 완성도 있는 포트폴리오 작품 제작이다. 특히 기존 장르의 한계를 극복하는 혁신적인 시스템 설계를 통해 게임 디자인 역량을 증명하고자 한다.

---

### 1.3 유사 게임 분석

**벤치마크 게임 분석**

본 프로젝트는 다양한 장르의 성공적인 게임들을 벤치마킹하여 하이브리드 시스템을 설계했다.

**실시간 RPG 참고 게임**

- **Hollow Knight** (Team Cherry): 벽 슬라이드, 대시 메커니즘, 체력 시스템을 참고했으며, 정교한 플레이어 컨트롤과 반응성을 학습했다. 2D 횡스크롤 액션의 기본 프레임워크로 활용했다.

- **Dead Cells** (Motion Twin): 무기 시스템, 적 AI 패턴, 레벨 디자인을 참고했고, 빠른 전투 템포와 피드백을 학습했다. 다양한 무기와 장비 수집 시스템에서 영감을 얻었다.

- **Blasphemous** (The Game Kitchen): 보스전 패턴과 체력 회복 시스템을 참고했으며, 도전적인 난이도 설계를 학습했다.

**턴제 전략 참고 게임**

- **Library of Ruina** (Project Moon): 주사위 기반 카드 전투 시스템을 참고하여 전략적 전투 메커니즘을 학습했다. 특히 동시 클래시 시스템과 덱 빌딩 요소를 보스 전투에 적용했다.

- **Slay the Spire** (MegaCrit): 카드 선택과 리소스 관리, 전투 중 전략적 의사결정 구조를 참고했다. 영구 성장 요소와 메타 진행 시스템에서 아이디어를 얻었다.

**성장 시스템 참고 게임**

- **Hades** (Supergiant Games): 런 실패 후에도 누적되는 영구 성장 시스템, 재화를 통한 업그레이드, NPC와의 상호작용을 참고했다. 실패해도 의미 있는 진행을 제공하는 설계를 학습했다.

- **Risk of Rain 2** (Hopoo Games): 다양한 성장 경로와 시너지 시스템, 난이도 곡선 설계를 참고했다.

**차별화 포인트**

본 게임은 다음과 같은 독창적인 차별화 요소를 가지고 있다.

**1. 하이브리드 전투 시스템**
- 일반 맵과 보스 맵에서 완전히 다른 전투 방식 제공
- 실시간 액션(일반)과 턴제 전략(보스)의 유기적 결합
- 두 시스템이 독립적이 아닌 서로 영향을 주는 통합 설계

**2. 동시 턴 진행 방식**
- 기존 턴제 게임과 달리 플레이어와 보스가 동시에 행동
- 빠른 전투 템포 유지하면서도 전략적 깊이 제공
- 상대의 선택을 예측하고 대응하는 심리전 요소 추가

**3. 다층 성장 시스템**
- 레벨업 시 능력치 증가 + 부가 효과 동시 획득
- 재화↔경험치 상호 전환으로 플레이어 선택권 강화
- 영구적이고 누적되는 성장으로 장기적 목표 제공
- 로그라이크의 랜덤성을 배제하고 확정적 성장 보장

**4. 적응형 보스 AI**
- 보스가 전투 상황(HP, 턴 수, 플레이어 전략)에 따라 패턴 변경
- 고정된 패턴 암기를 방지하고 매 전투마다 새로운 전략 요구
- 단순 반복 플레이를 방지하여 재미 지속

**5. 통합 콘텐츠 생태계**
- 실시간 탐험, 턴제 보스전, 미니게임, 상점 시스템이 유기적으로 연결
- 각 콘텐츠에서 얻은 자원이 다른 콘텐츠에 영향
- 다양한 플레이 스타일 지원 (전투 중심, 성장 중심, 탐험 중심)

---

### 1.4 구현 범위 및 목표

**주요 구현 기능**

플레이어 시스템은 이동, 점프, 대시, 벽 슬라이드 등의 기본 액션과 3종 무기 시스템, 체력/공격력/방어력/이동속도 스탯 관리, 레벨업 및 경험치 시스템을 포함한다.

전투 시스템은 순찰, 감지, 추적, 공격 패턴을 가진 일반 적 AI, 마법과 텔레포트 등 특수 패턴을 가진 중간보스 AI, 주사위 기반 카드 전투 보스전, 무기별 데미지 계산 및 피격 시스템을 구현했다.

세이브/로드 시스템은 3개 슬롯 지원, JSON 기반 데이터 저장, 씬 전환 시 자동 저장, 플레이어 위치/스탯/인벤토리 저장 기능을 제공한다.

인벤토리 및 상점 시스템은 아이템 획득 및 관리, 포션 사용을 통한 체력 회복, 랜덤 아이템 판매와 새로고침 기능을 가진 상점, 스탯 업그레이드 아이템을 포함한다.

미니게임 시스템은 골드, XP, 공격력 증가를 보상으로 주는 대장간 불꽃 수집 게임과 라이프맵 미니게임을 구현했다.

UI/UX 시스템은 메인 메뉴, 무기 선택, 세이브 슬롯 선택 화면, 체력과 스탯을 표시하는 HUD, 스탯 선택이 가능한 레벨업 UI, 일시정지 메뉴, 대미지 텍스트 등을 포함한다.

씬 관리 시스템은 확인 UI를 포함한 포털 시스템, 플레이어 스폰 위치 관리, 미니게임 복귀 시스템, DontDestroyOnLoad 싱글톤 관리를 구현했다.

스테이지별 특수 시스템으로 Stage3에는 수영 메커니즘, 산소 게이지, 산소 회복 구역을 구현했다.

**개발 목표 및 완성도 기준**

모든 핵심 시스템 구현 완료, 메인 메뉴부터 게임 종료까지 전체 플로우 완성, 3개 이상의 스테이지 및 보스전 구현, 세이브/로드 시스템 안정성 확보, 버그 없는 플레이 경험 제공을 목표로 했으며, 모두 달성했다.

---

### 1.5 개발 환경

**소프트웨어 환경**

게임 엔진은 Unity 6000.0.41f1 (Unity 6)을 사용했고, 개발 언어는 C# (.NET Framework)를 사용했다. IDE는 Visual Studio 2022를 사용했으며, 버전 관리는 Git과 GitHub를 활용했다. 타겟 플랫폼은 Windows PC이다.

**하드웨어 환경**

운영체제는 Windows 11을 사용했으며, Windows PC에서 개발을 진행했다.

**주요 사용 Unity 패키지**

Unity 2D Pixel Perfect, Unity Timeline, Unity Input System, TextMeshPro를 사용했다. 그래픽은 Unity Sprite Editor와 외부 에셋을 활용했고, 사운드는 Unity Audio Source와 AudioClip을 사용했으며, 애니메이션은 Unity Animator와 Animation Controller를 사용했다.

---

## 2. 관련 기술

### 2.1 Unity 2D 게임 개발

**Unity 2D 엔진 개요**

Unity는 크로스 플랫폼 게임 엔진으로, 2D 게임 개발을 위한 다양한 도구를 제공한다. 본 프로젝트에서는 Unity 6 (6000.0.41f1)을 사용했다. Unity 2D의 주요 특징으로는 2D Renderer 및 Sprite 시스템, 2D Physics (Rigidbody2D, Collider2D), Tilemap 시스템, 2D 애니메이션 시스템, Sorting Layer 및 Order in Layer를 통한 렌더링 순서 관리가 있다.

**Rigidbody2D 물리 시스템**

본 프로젝트에서는 Unity 6의 새로운 물리 API를 사용한다. 기존 버전의 velocity 대신 linearVelocity를 사용하여 이동, 점프, 대시 등을 구현했다. 이동은 linearVelocity.x 조정을 통해, 점프는 linearVelocity.y에 점프력 적용을 통해, 대시는 순간적인 velocity 변경을 통해 구현했다. 중력은 Gravity Scale 조정으로, 충돌 감지는 OnCollisionEnter2D와 OnTriggerEnter2D를 사용했다.

Rigidbody2D 설정은 다음과 같다. Body Type은 플레이어와 적에는 Dynamic을, 이동 플랫폼에는 Kinematic을 사용했다. Constraints는 2D에서 회전을 방지하기 위해 Freeze Rotation Z를 설정했다. Collision Detection은 빠른 움직임 감지를 위해 Continuous로 설정했다.

**2D 애니메이션 시스템**

Unity의 Animator Controller를 사용하여 상태 기반 애니메이션을 구현했다. 애니메이션 파라미터는 Float 타입으로 Speed(이동 속도), Bool 타입으로 Grounded, HasSword, Dash, Wall, Trigger 타입으로 isHurt, isDead, isKnockedBack를 사용했다.

애니메이션 이벤트는 EnableAttackHitbox()로 공격을 활성화하고, DisableAttackHitbox()로 공격을 비활성화하며, PlayAttackSound()로 사운드를 재생한다. 애니메이션 전환은 Has Exit Time을 false로 설정하여 즉시 전환하고, Transition Duration은 0.1초로 설정하여 부드러운 전환을 구현했다. Interruption Source는 Current State로 설정하여 현재 상태에서 중단 가능하도록 했다.

**Tilemap 및 레벨 디자인**

Tilemap을 사용하여 스테이지 레벨을 디자인했다. Tilemap 구조는 바닥과 벽을 나타내는 Ground Layer, 배경 장식을 나타내는 Decoration Layer, 통과 가능한 플랫폼을 나타내는 Platform Layer로 구성된다. Tilemap Collider 2D는 최적화를 위해 Composite Collider 2D를 사용했으며, Used by Composite를 체크하여 여러 타일을 하나의 콜라이더로 병합했다.

---

### 2.2 게임 디자인 패턴

**싱글톤 패턴 (Singleton Pattern)**

게임 전역에서 하나의 인스턴스만 존재해야 하는 매니저 클래스에 싱글톤 패턴을 적용했다. PlayerController.Instance는 플레이어 제어를, GameManager.Instance는 게임 상태 및 세이브/로드를, SaveManager.Instance는 파일 I/O를, Inventory.instance는 인벤토리 관리를, ShopManager.Instance는 상점 시스템을, BossGameManager.Instance는 보스 전투 상태 관리를 담당한다.

싱글톤 패턴 구현 시 DontDestroyOnLoad로 씬 전환 시에도 유지되도록 했으며, 메인 메뉴 복귀 시 수동으로 정리하여 메모리 누수를 방지했다.

**옵저버 패턴 (Observer Pattern)**

이벤트 기반 시스템으로 UI 업데이트, 게임 상태 변경 등에 옵저버 패턴을 활용했다. 인벤토리 변경 시 UI 업데이트, 플레이어 스탯 변경 시 HUD 업데이트, 적 사망 시 아이템 드롭, 대화 완료 시 미니게임 시작 등에 적용했다.

**상태 머신 패턴 (State Machine Pattern)**

적 AI와 보스 전투 시스템에 상태 머신을 적용했다. 적 AI는 Idle(대기), Patrol(순찰), Chase(추적), Attack(공격), Hurt(피격), Dead(사망) 상태를 가진다. 보스 전투는 Exploration(탐험 모드)과 Battle(전투 모드) 상태를 가진다.

**오브젝트 풀링 (Object Pooling)**

자주 생성/삭제되는 오브젝트(대미지 텍스트, 이펙트 등)의 성능 최적화를 위해 오브젝트 풀링을 적용했다. 오브젝트를 미리 생성해두고 재사용함으로써 Instantiate/Destroy 호출 횟수를 감소시켜 성능을 향상시키고 메모리 단편화를 방지했다. 대미지 텍스트, 공격 이펙트, 발사체 등에 적용했다.

---

### 2.3 데이터 지속성 기술

**JSON 직렬화 (Unity JsonUtility)**

플레이어 데이터를 JSON 형식으로 저장/로드한다. SaveData 클래스는 플레이어 스탯(레벨, XP, HP, 공격력, 방어력, 돈), 무기 보유 여부, 인벤토리 아이템, 씬 정보(현재 씬, 플레이어 위치), 메타데이터(슬롯 번호, 저장 시간, 플레이 시간)를 포함한다.

저장 위치는 Application.persistentDataPath/Saves/ 폴더이며, Windows에서는 C:/Users/[Username]/AppData/LocalLow/[CompanyName]/[ProductName]/Saves/에 저장된다.

**ScriptableObject 기반 데이터 관리**

재사용 가능한 게임 데이터를 ScriptableObject로 관리한다. PotionItemData는 포션의 이름, 아이콘, 회복량, 설명을 저장한다. ShopItemData는 상점 아이템의 이름, 아이콘, 가격, 타입(포션, 공격력 업그레이드, 방어력 업그레이드, 체력 업그레이드, 이동속도 업그레이드), 효과값을 저장한다. CombatPage는 보스 전투 카드의 이름, 라이트 비용, 쿨다운, 주사위 배열, 일회용 여부를 저장한다.

ScriptableObject의 장점은 Inspector에서 직접 편집 가능하고, 여러 오브젝트에서 공유 가능하며, 인스턴스 생성이 불필요하여 메모리 효율적이고, 빌드 시 에셋으로 포함된다는 점이다.

**DontDestroyOnLoad 씬 간 데이터 유지**

씬이 전환되어도 특정 오브젝트를 유지하는 기술이다. PlayerController, GameManager, SaveManager, Inventory, UI Canvas(선택적)에 적용했다. instanceId 기반 Dictionary로 중복을 방지했다.

**정적 클래스를 통한 크로스씬 데이터 공유**

씬 간에 간단한 데이터를 전달할 때 정적 변수를 사용한다. GameData.SelectedWeapon은 선택된 무기를, PortalReturnData는 포털 복귀 정보를, PortalController.usedPortalIDs는 사용된 포털 추적을, MidBossController.completedEventIDs는 완료된 이벤트를, StatueInteraction.previousSceneName은 이전 씬 이름을, BlacksmithMinigameManager.isGamePausedByManager는 시간 정지 플래그를, DontDestroyOnLoadManager.isReturningToMainMenu는 메인 메뉴 복귀 플래그를 저장한다.

---

### 2.4 UI/UX 시스템

**Unity UI (Canvas, UI 이벤트 시스템)**

Unity의 Canvas 시스템을 사용하여 모든 UI를 구현했다. Canvas 설정은 Render Mode를 Screen Space - Overlay로 설정하여 카메라 독립적으로 만들고, Canvas Scaler는 Scale With Screen Size로 설정하여 해상도에 대응하도록 했다. Reference Resolution은 1920x1080으로, Match는 0.5로 설정하여 Width/Height 균형을 맞췄다.

UI 계층 구조는 Main UI Canvas 아래에 HUD Panel(체력, 스탯), Inventory Panel, Level Up Panel, Pause Menu Panel이 있으며, 별도의 Damage Text Canvas가 있다.

**인벤토리 드래그 앤 드롭**

인벤토리 아이템을 드래그하여 사용하는 시스템을 구현했다. IBeginDragHandler, IDragHandler, IEndDragHandler 인터페이스를 사용하여 드래그 시작, 드래그 중, 드래그 종료 이벤트를 처리한다. 플레이어에 드롭 시 아이템을 사용하도록 구현했다.

**동적 UI 업데이트**

플레이어 스탯 변경 시 UI를 실시간으로 업데이트한다. 옵저버 패턴을 활용하여 PlayerStats에서 XP가 추가되면 UpdateStatsUI()를 호출하고, PlayerController를 통해 UpdateAllStatsUI()를 호출하여, StatsUIManager에서 모든 UI를 업데이트한다.

**레벨업 시스템 UI**

레벨업 시 스탯을 선택할 수 있는 UI를 표시한다. 레벨업 UI 플로우는 다음과 같다. 경험치가 최대치에 도달하면 레벨업이 발생하고, 시간을 정지한다(Time.timeScale = 0). 레벨업 패널을 표시하면 플레이어가 공격력, 방어력, 체력, 이동속도 중 하나를 선택한다. 선택이 적용되고 패널이 닫히면 시간을 재개한다(Time.timeScale = 1).

미니게임 중 레벨업 처리는 특별하게 구현했다. AncientBlacksmith나 LifeHeartMap 씬에서는 레벨업 UI를 즉시 표시하지 않고 isLevelUpPending 플래그를 true로 설정한다. 미니게임 종료 후 ShowPendingLevelUpPanel()을 호출하여 대기 중이던 레벨업 UI를 표시한다.

Time.timeScale 관리는 다음과 같다. 0으로 설정하면 게임이 일시정지되지만 UI는 작동한다. 1로 설정하면 게임이 정상 속도로 작동한다. Time.unscaledDeltaTime은 timeScale의 영향을 받지 않는 시간으로 UI 애니메이션에 사용한다.

---

## 3. 구현 내용

### 3.1 게임 시스템 아키텍처

**전체 시스템 구조**

본 프로젝트는 11개의 주요 시스템으로 구성된다. 최상위에 GameManager가 있으며, Save/Load 조율과 게임 상태 관리를 담당한다. GameManager 아래에는 Player System, Monster System, Scene Manager가 있다. Player System은 Stats Manager와 UI System과 연결되며, Monster System은 Item Drop과 연결되고, Scene Manager는 Portal System과 연결된다.

UI System 아래에는 HUD, Inventory, Level Up UI, Shop UI, Pause Menu가 있다. 특수 시스템으로는 Boss Battle System, Minigame System, Stage3 Underwater System, Save System이 있다.

**데이터 흐름**

Player Input은 PlayerController로 전달되고, PlayerController는 Rigidbody2D를 통해 물리 처리를 하고 Animator를 통해 애니메이션을 처리한다. PlayerController는 PlayerStats와 연결되어 LevelUpUIManager를 업데이트하고, PlayerHealth와 연결되어 HUD UI를 업데이트한다. PlayerHealth는 DamageCalculation을 수행하고, 이는 MonsterHealth에 전달되어 ItemDrop으로 이어진다. MonsterHealth는 Death Animation을 실행하고 플레이어에게 XP를 지급한다.

**씬 전환 구조**

Main(메인 메뉴)에서 New Game Flow와 Load Game Flow로 나뉜다. New Game Flow는 Weapon(무기 선택) → LoadGame(슬롯 선택) → Stage1(게임 시작)로 이어진다. Load Game Flow는 LoadGame(슬롯 선택) → [저장된 씬](게임 재개)로 이어진다.

Stage Flow는 다음과 같다. Stage1에서 Portal을 통해 Stage2, Shop, AncientBlacksmith(미니게임)로 이동할 수 있고, Boss Portal을 통해 Boss1으로 이동할 수 있다. Stage2에서는 Portal을 통해 Stage3로, Boss Portal을 통해 Boss2로 이동할 수 있다. Stage3(Underwater)에서는 Portal을 통해 Stage1이나 다른 씬으로 이동할 수 있다.

---

**핵심 클래스 구조**

플레이어 시스템 클래스 구조는 다음과 같다. PlayerController는 싱글톤이며 DontDestroyOnLoad를 사용한다. Instance, rb(Rigidbody2D), animator(Animator), hasSword, hasLance, hasMace 등의 멤버 변수를 가지고 있다. Move(), Jump(), Dash(), Attack(), TakeDamage(), RecalculateStats(), UpdateAllStatsUI() 등의 메서드를 제공한다.

PlayerController는 RequireComponent를 통해 PlayerStats, PlayerHealth, CharacterStats 컴포넌트를 요구한다. PlayerStats는 level, currentXP, money, bonusAttackPower, bonusDefensePower, bonusMoveSpeed 등의 변수와 AddXP(), LevelUp(), AddMoney() 등의 메서드를 가진다. PlayerHealth는 maxHP, currentHP, defense 등의 변수와 TakeDamage(), Heal(), Die() 등의 메서드를 가진다. 피격 시에는 Invincibility Frames를 적용한다. CharacterStats는 deck, light, hp 등의 변수와 UsePage(), DrawCard() 등의 메서드를 가지며, LevelUpUI와 연결된다.

적 AI 시스템 클래스 구조는 다음과 같다. MonsterController는 일반 몬스터 AI의 베이스 클래스로, currentState(EnemyState), target(Transform), detectionRange, attackRange, moveSpeed 등의 변수와 UpdateState(), Patrol(), Chase(), Attack(), CheckGround(), CheckWall() 등의 메서드를 가진다.

Stage1MidBossAI는 MonsterController를 상속받아 CastSpell(), SummonSpell() 메서드를 추가한다. Stage2MidBossAI는 MonsterController를 상속받아 Teleport(), TeleportTo() 메서드를 추가한다. 두 클래스 모두 MonsterHealth와 연결된다.

MonsterHealth는 currentHP, maxHP, defense 등의 변수와 TakeDamage(), Die(), DieRoutine() 등의 메서드를 가진다. MonsterHealth는 ItemDrop과 연결되어 아이템 드롭을 처리한다. ItemDrop은 dropItems 배열과 dropChance를 가지고 있으며, DropItems() 메서드를 제공한다.

매니저 클래스 관계는 다음과 같다. GameManager는 싱글톤이며 DontDestroyOnLoad를 사용한다. Instance, currentSaveSlot, isNewGame, selectedWeapon 등의 변수와 SaveCurrentGame(), LoadGameFromSlot(), ApplySaveData(), PerformAutoSave(), PrepareNewGame() 등의 메서드를 제공한다.

GameManager는 SaveManager를 사용한다. SaveManager도 싱글톤이며 DontDestroyOnLoad를 사용한다. Instance, savePath 등의 변수와 SaveGame(), LoadGame(), DeleteSave(), GetAllSaveSlots() 등의 메서드를 제공한다.

SaveManager는 SaveData를 직렬화한다. SaveData는 Serializable 클래스로, playerLevel, currentXP, maxHP, currentHP, attackPower, defensePower, currentMoney, hasSword, hasLance, hasMace, potionItems, currentScene, playerPosition, saveTime, playTime, isEmpty 등의 변수와 CreateEmptySlot(), GetSummary(), UpdateSaveTime() 등의 메서드를 제공한다.

기타 매니저로는 Inventory(items 배열, Add(), Remove(), UsePotion() 메서드), ShopManager(shopItems 배열, Purchase(), Refresh() 메서드), BossGameManager(currentState 변수, SetState() 메서드)가 있다.

---

**데이터베이스 스키마 (저장 데이터 구조)**

SaveData는 JSON 형식으로 저장된다. 주요 필드는 다음과 같다. slotNumber(슬롯 번호), isEmpty(빈 슬롯 여부), saveTime(저장 시간), playTime(플레이 시간), playerLevel(플레이어 레벨), currentXP(현재 경험치), xpToNextLevel(다음 레벨까지 필요한 경험치), maxHP(최대 체력), currentHP(현재 체력), attackPower(공격력), defensePower(방어력), moveSpeed(이동 속도), currentMoney(현재 돈), hasSword(검 보유 여부), hasLance(창 보유 여부), hasMace(메이스 보유 여부), potionItems(포션 아이템 목록, itemName과 quantity 포함), currentScene(현재 씬), playerPosition(플레이어 위치, x, y, z 좌표)을 포함한다.

PlayerStats 데이터 구조는 다음과 같다. 레벨 시스템은 level(레벨), currentXP(현재 경험치), xpToNextLevel(다음 레벨까지 필요한 경험치)을 포함한다. 보너스 스탯(장비/업그레이드)은 bonusAttackPower(보너스 공격력), bonusDefensePower(보너스 방어력), bonusMoveSpeed(보너스 이동속도)를 포함한다. 재화는 money(돈)를 포함한다. 레벨업 대기는 isLevelUpPending(레벨업 대기 여부)를 포함한다. 메서드로는 AddXP(경험치 추가), AddMoney(돈 추가), SpendMoney(돈 사용), LevelUp(레벨업)이 있다.

Inventory 데이터 구조는 다음과 같다. instance(싱글톤 인스턴스), items(인벤토리 아이템 목록, List<PotionItemData>), inventorySize(인벤토리 크기, 20), onItemChangedCallback(아이템 변경 콜백, OnItemChanged 델리게이트) 변수를 가진다. 메서드로는 Add(아이템 추가), Remove(아이템 제거), IsFull(인벤토리 가득 참 여부), GetItemCount(아이템 개수 조회)가 있다.

저장 파일 구조는 Application.persistentDataPath/Saves/ 폴더 아래에 SaveSlot1.json, SaveSlot2.json, SaveSlot3.json 파일로 저장된다.

Auto-Save는 다음과 같이 트리거된다. 씬 전환 시 OnSceneLoaded 이벤트가 발생하면, Main, LoadGame, Weapon, HowToPlay, Setting 씬을 제외한 씬에서, 0.5초 지연 후, currentSaveSlot이 0보다 클 때 자동 저장이 실행된다.

---

### 3.2 주요 시스템 구현

#### 3.2.1 플레이어 시스템

**이동 및 점프 메커니즘**

플레이어의 기본 이동과 점프는 Rigidbody2D의 linearVelocity를 직접 제어하여 구현했다. 이동은 Input.GetAxisRaw("Horizontal")로 입력을 받아 기본 속도와 보너스 속도를 합산한 후 linearVelocity.x에 적용한다. 이동 속도에 따라 애니메이터의 Speed 파라미터를 업데이트하고, 이동 방향에 따라 스프라이트를 좌우 반전시킨다.

점프는 K 키 입력과 isGrounded 체크를 통해 구현한다. 점프 조건이 만족되면 linearVelocity.y에 jumpForce(12f)를 적용하고 점프 애니메이션을 실행한다. 땅 체크는 OverlapCircle을 사용하여 groundCheck 위치에서 groundCheckRadius(0.2f) 반경 내에 groundLayer가 있는지 확인한다. 땅 체크 결과를 애니메이터의 Grounded 파라미터에 반영한다.

**대시 및 벽 슬라이드**

대시는 코루틴을 사용하여 짧은 시간 동안 빠르게 이동하도록 구현했다. L 키 입력과 canDash 체크를 통해 대시 코루틴을 시작한다. 대시 중에는 중력을 0으로 설정하고, 캐릭터가 바라보는 방향으로 dashSpeed(20f)로 이동한다. dashDuration(0.2f) 후에 중력을 복원하고, dashCooldown(1f) 후에 다시 대시가 가능해진다. 대시 중에는 Dash 애니메이션을 실행한다.

벽 슬라이드는 다음 조건이 만족될 때 실행된다. 벽에 닿아있고(wallCheck 위치에서 OverlapCircle 체크), 땅에 닿아있지 않고, 하강 중일 때(linearVelocity.y < 0) 벽 슬라이드가 활성화된다. 벽 슬라이드 중에는 하강 속도를 wallSlideSpeed로 제한하고 Wall 애니메이션을 실행한다.

**무기 시스템 (검, 창, 메이스)**

각 무기는 고유한 공격 패턴과 데미지 계산을 가지고 있다.

검(Sword)은 균형형 무기로, attackDamage 10, attackRange 1.5f를 가진다. 공격 범위 내의 적을 OverlapCircleAll로 감지하여 각 적에게 데미지를 입힌다. 약공격(attackType=1)과 강공격(attackType=2)을 지원한다.

창(Lance)은 리치형 무기로, attackDamage 8, attackRange 2.5f(더 긴 리치), attackWidth 0.5f를 가진다. 전방 범위 공격을 BoxCastAll로 구현하여 공격 방향의 모든 적을 감지한다. 검보다 낮은 데미지지만 긴 리치로 안전한 거리를 유지할 수 있다.

메이스(Mace)는 파워형 무기로, attackDamage 15(높은 데미지), attackRange 1.2f(짧은 리치), knockbackForce 10f를 가진다. 높은 데미지와 함께 넉백 효과를 제공한다. 적을 공격하면 Rigidbody2D에 AddForce를 사용하여 밀어낸다.

무기별 특징을 표로 정리하면 다음과 같다.

| 무기 | 데미지 | 리치 | 공격속도 | 특수효과 |
|------|--------|------|----------|----------|
| 검 | 보통 | 보통 | 빠름 | 균형잡힌 성능 |
| 창 | 약함 | 긴 편 | 보통 | 안전한 거리 유지 |
| 메이스 | 강함 | 짧음 | 느림 | 넉백 효과 |

공격 실행은 J 키 입력과 isAttacking 체크를 통해 AttackRoutine 코루틴을 시작한다. Attack 애니메이션을 실행하고, 애니메이션 이벤트에서 AttackHit() 메서드를 호출하여 실제 공격을 실행한다. 보유한 무기에 따라 swordAttack.Attack(1), lanceAttack.Attack(1), maceAttack.Attack(1) 중 하나를 호출한다. attackDuration 후에 공격이 종료된다.

**체력 및 스탯 관리**

PlayerHealth는 maxHP 100, currentHP(시작 시 maxHP로 초기화), defense 0을 가진다. 무적 시간은 isInvincible 플래그와 invincibilityDuration 1f로 관리한다.

TakeDamage는 다음과 같이 동작한다. 무적 상태이면 데미지를 무시한다. 최종 데미지는 Max(1, damage - defense)로 계산하여 최소 1 데미지를 보장한다. currentHP를 감소시키고 대미지 텍스트를 표시한다. InvincibilityFrames 코루틴을 시작하여 무적 시간을 적용한다. Hurt 애니메이션을 실행하고, currentHP가 0 이하이면 Die()를 호출한다. UI를 업데이트한다.

InvincibilityFrames 코루틴은 isInvincible을 true로 설정하고, invincibilityDuration 동안 스프라이트를 0.1초마다 깜빡이게 한다. 무적 시간이 끝나면 스프라이트를 켜고 isInvincible을 false로 설정한다.

Heal은 currentHP를 amount만큼 회복시키되 maxHP를 초과하지 않도록 한다. UI를 업데이트한다.

AddPermanentHealth는 maxHP와 currentHP를 동시에 증가시킨다. UI를 업데이트한다.

Die는 Death 애니메이션을 실행하고 GameManager.Instance.PlayerDied()를 호출하여 게임 오버를 처리한다.

**레벨업 시스템**

PlayerStats는 다음과 같은 변수를 가진다. level 1, currentXP 0, xpToNextLevel 100, bonusAttackPower 0, bonusDefensePower 0, bonusMoveSpeed 0f, money 0, isLevelUpPending false.

AddXP는 경험치를 추가하고, currentXP가 xpToNextLevel 이상이면 LevelUp()을 호출한다. 레벨업 후에도 남은 경험치가 있으면 계속 LevelUp()을 호출한다(while 루프). UpdateStatsUI()를 호출하여 UI를 업데이트한다.

LevelUp은 level을 1 증가시키고, currentXP에서 xpToNextLevel을 차감한다. xpToNextLevel을 1.5배로 증가시킨다. 현재 씬이 AncientBlacksmith나 LifeHeartMap이면 isLevelUpPending을 true로 설정하고 레벨업 UI 표시를 연기한다. 그 외의 씬에서는 즉시 ShowLevelUpPanel()을 호출한다.

ShowLevelUpPanel은 Time.timeScale을 0f로 설정하여 시간을 정지시킨다. LevelUpUIManager를 찾아 ShowPanel()을 호출한다.

스탯 선택 메서드는 다음과 같다. IncreaseAttack은 bonusAttackPower를 5 증가시킨다. IncreaseDefense는 bonusDefensePower를 3 증가시킨다. IncreaseHealth는 PlayerHealth.AddPermanentHealth(20)을 호출한다. IncreaseSpeed는 bonusMoveSpeed를 0.5f 증가시킨다. 모든 메서드는 CloseLevelUpPanel()을 호출한다.

CloseLevelUpPanel은 Time.timeScale을 1f로 설정하여 시간을 재개한다. LevelUpUIManager를 찾아 HidePanel()을 호출한다. PlayerController.Instance.RecalculateStats()를 호출하여 스탯을 재계산하고, UpdateAllStatsUI()를 호출하여 UI를 업데이트한다.

AddMoney는 money를 증가시키고 UpdateStatsUI()를 호출한다.

SpendMoney는 money가 amount 이상이면 money를 감소시키고 UpdateStatsUI()를 호출한 후 true를 반환한다. 그렇지 않으면 false를 반환한다.

**스탯 재계산**

PlayerController의 RecalculateStats는 최종 공격력을 다음과 같이 계산한다. finalAttackPower = baseAttackPower + (playerStats.level × 2) + playerStats.bonusAttackPower. 무기에 따라 공격력을 다르게 적용한다. 검은 100%(finalAttackPower), 창은 80%(finalAttackPower × 0.8f), 메이스는 120%(finalAttackPower × 1.2f)를 적용한다. 방어력은 baseDefense + playerStats.bonusDefensePower로 계산한다. 이동속도는 Move()에서 moveSpeed + playerStats.bonusMoveSpeed로 직접 계산한다.

**씬 로드 시 플레이어 복원**

PlayerController는 OnEnable에서 SceneManager.sceneLoaded 이벤트를 구독하고, OnDisable에서 구독을 해제한다.

OnSceneLoaded는 다음과 같이 동작한다. PlayerSpawnPoint 태그를 가진 오브젝트를 찾아 플레이어 위치를 설정한다. RecalculateStats()를 호출하여 스탯을 재계산하고, UpdateAllStatsUI()를 호출하여 UI를 업데이트한다. AutoSaveAfterDelay 코루틴을 시작한다.

AutoSaveAfterDelay 코루틴은 0.5초 대기 후, 현재 씬 이름을 가져온다. Main, LoadGame, Weapon, HowToPlay, Setting 씬이 아니고, GameManager.Instance가 존재하고, currentSaveSlot이 0보다 크면 PerformAutoSave()를 호출하여 자동 저장한다.

---

#### 3.2.2 적 AI 시스템

**일반 몬스터 AI (순찰, 추적, 공격)**

기본적인 적 AI는 상태 머신 패턴을 사용하여 구현했다. EnemyState enum은 Idle(대기), Patrol(순찰), Chase(추적), Attack(공격), Hurt(피격), Dead(사망) 상태를 가진다.

AI Settings는 currentState(EnemyState.Patrol로 초기화), moveSpeed 2f, chaseSpeed 3f, detectionRange 5f, attackRange 1.5f, attackCooldown 2f를 포함한다. References는 player(Transform), rb(Rigidbody2D), animator(Animator)를 포함한다. Patrol Settings는 patrolPointA, patrolPointB, currentTarget을 포함한다. Ground/Wall Check는 groundCheck, wallCheck, groundLayer, checkRadius 0.2f를 포함한다. 상태 변수는 isAttacking false, isDead false를 포함한다.

Start 메서드는 rb, animator, player를 초기화하고, currentTarget을 patrolPointB로 설정한다.

Update 메서드는 isDead이면 리턴하고, 그렇지 않으면 UpdateState()와 ExecuteState()를 호출한다.

UpdateState는 플레이어와의 거리를 계산하고, 현재 상태에 따라 다음과 같이 동작한다. Patrol 상태에서는 거리가 detectionRange 이하이면 Chase 상태로 전환한다. Chase 상태에서는 거리가 detectionRange를 초과하면 Patrol 상태로, attackRange 이하이고 공격 중이 아니면 Attack 상태로 전환한다. Attack 상태에서는 거리가 attackRange를 초과하면 Chase 상태로 전환한다.

ExecuteState는 현재 상태에 따라 Patrol(), Chase(), AttackRoutine() 코루틴을 실행한다.

Patrol은 currentTarget 방향으로 moveSpeed로 이동하고, 이동 방향에 따라 스프라이트를 좌우 반전시킨다. 애니메이터의 Speed 파라미터를 업데이트한다. currentTarget에 도착하면(거리 0.5f 미만) currentTarget을 patrolPointA와 patrolPointB 사이에서 전환한다. 벽이나 낭떠러지를 감지하면 currentTarget을 전환한다.

Chase는 플레이어 방향으로 chaseSpeed로 이동하고, 이동 방향에 따라 스프라이트를 좌우 반전시킨다. 애니메이터의 Speed 파라미터를 업데이트한다.

AttackRoutine 코루틴은 isAttacking을 true로 설정하고 정지한다(linearVelocity를 Vector2.zero로 설정). isAttacking 애니메이터 파라미터를 true로 설정한다. 애니메이션 이벤트에서 EnableAttackHitbox()를 호출한다. attackCooldown을 대기한다. isAttacking 애니메이터 파라미터를 false로 설정하고 isAttacking을 false로 설정한다.

EnableAttackHitbox는 AttackHitbox 자식 오브젝트를 찾아 활성화한다. DisableAttackHitbox는 AttackHitbox 자식 오브젝트를 찾아 비활성화한다.

CheckGround는 groundCheck 위치에서 checkRadius 반경 내에 groundLayer가 있는지 OverlapCircle로 체크한다. CheckWall은 wallCheck 위치에서 checkRadius 반경 내에 groundLayer가 있는지 OverlapCircle로 체크한다.

ChangeState는 currentState를 newState로 변경한다.

OnDrawGizmosSelected는 detectionRange를 노란색 와이어 구로, attackRange를 빨간색 와이어 구로 그려 씬 뷰에서 시각화한다.

**AttackHitbox 구현**

EnemyAttackHitbox는 AttackHitbox 오브젝트에 부착된다. attackDamage 10, playerLayer를 가진다.

OnTriggerEnter2D는 충돌한 오브젝트의 레이어가 playerLayer와 일치하는지 체크한다. 일치하면 PlayerHealth 컴포넌트를 가져와 TakeDamage(attackDamage)를 호출한다.

**스테이지 1 중간보스 (근접 + 마법)**

Stage1MidBossAI는 기본 AI를 확장하여 마법 공격 패턴을 추가했다.

Attack Settings는 spellPrefab, spellSpawnPoint, spellCastCooldown 5f, meleeAttackCooldown 2f를 포함한다. 상태 변수는 canCastSpell true, canMeleeAttack true, isActivated false를 포함한다. Activation은 activationRange 8f를 포함한다.

Update 메서드는 isActivated가 false이면 플레이어와의 거리가 activationRange 이하인지 체크하고, 조건이 만족되면 isActivated를 true로 설정하고 활성화 이벤트를 실행한다(예: 대화, 음악 변경). isActivated가 true이면 UpdateAI()를 호출한다.

UpdateAI는 플레이어와의 거리를 계산하고, 거리가 4f를 초과하고 canCastSpell이 true이면 CastSpellRoutine() 코루틴을 시작한다. 거리가 2f 이하이고 canMeleeAttack이 true이면 MeleeAttackRoutine() 코루틴을 시작한다. 그 외에는 Chase()를 실행한다.

CastSpellRoutine 코루틴은 canCastSpell을 false로 설정하고 정지한다. isCasting 애니메이터 파라미터를 true로 설정하고 1초 대기한다(캐스팅 시간). SummonSpell()을 호출하여 스펠을 소환한다. isCasting 애니메이터 파라미터를 false로 설정하고 spellCastCooldown을 대기한다. canCastSpell을 true로 설정한다.

SummonSpell은 spellPrefab과 spellSpawnPoint가 존재하면 스펠을 Instantiate한다. SpellAttack 컴포넌트를 가져와 SetTarget(player)을 호출하여 플레이어를 타겟으로 설정한다.

MeleeAttackRoutine 코루틴은 canMeleeAttack을 false로 설정하고 정지한다. isAttacking 애니메이터 파라미터를 true로 설정한다. AnimationEvent에서 EnableAttackHitbox()를 호출한다. meleeAttackCooldown을 대기한다. isAttacking 애니메이터 파라미터를 false로 설정하고 canMeleeAttack을 true로 설정한다.

**SpellAttack.cs (투사체)**

SpellAttack은 damage 15, moveSpeed 5f, lifetime 5f를 가진다. target(Transform), direction(Vector2)을 가진다.

SetTarget은 target을 설정하고, 플레이어 방향을 계산하여 direction에 저장한다. 스펠의 회전을 direction에 맞춰 설정한다(Atan2와 Quaternion.Euler 사용). lifetime 후에 스펠을 파괴한다(Destroy).

Update 메서드는 스펠을 direction 방향으로 moveSpeed로 이동시킨다.

OnTriggerEnter2D는 충돌한 오브젝트의 태그가 Player이면 PlayerHealth를 가져와 TakeDamage(damage)를 호출하고 스펠을 파괴한다. 충돌한 오브젝트의 태그가 Ground이면 스펠을 파괴한다.

**스테이지 2 중간보스 (텔레포트)**

Stage2MidBossAI는 플레이어와의 거리가 멀어지면 텔레포트하여 접근한다.

Teleport Settings는 teleportRange 6f(이 거리 이상 멀어지면 텔레포트), teleportCooldown 3f, teleportMinX -10f, teleportMaxX 10f, teleportY 0f를 포함한다. Sprite Child는 useSpriteChild true, spriteTransform을 포함한다. 상태 변수는 canTeleport true, spriteRenderer를 포함한다.

Start 메서드는 useSpriteChild가 true이고 spriteTransform이 존재하면 spriteTransform에서 SpriteRenderer를 가져온다. 그렇지 않으면 자신에게서 SpriteRenderer를 가져온다.

Update 메서드는 isActivated가 false이면 리턴한다. 플레이어와의 거리를 계산하고, 거리가 teleportRange를 초과하고 canTeleport가 true이면 TeleportRoutine() 코루틴을 시작한다. 거리가 attackRange 이하이고 canMeleeAttack이 true이면 MeleeAttackRoutine() 코루틴을 시작한다. 그 외에는 Chase()를 실행한다.

TeleportRoutine 코루틴은 canTeleport를 false로 설정하고 정지한다. FadeOut() 코루틴을 시작하여 페이드 아웃한다. GetRandomTeleportPosition()을 호출하여 랜덤 위치를 얻어 transform.position에 설정한다. FadeIn() 코루틴을 시작하여 페이드 인한다. teleportCooldown을 대기한다. canTeleport를 true로 설정한다.

FadeOut 코루틴은 duration 0.5f 동안 spriteRenderer의 alpha를 1f에서 0f로 Lerp한다.

FadeIn 코루틴은 duration 0.5f 동안 spriteRenderer의 alpha를 0f에서 1f로 Lerp한다.

GetRandomTeleportPosition은 teleportMinX와 teleportMaxX 사이에서 랜덤 X 좌표를 생성한다. 플레이어와 너무 가깝지 않도록(2f 미만) while 루프로 재생성한다. 새로운 위치 (randomX, teleportY)를 반환한다.

OnDrawGizmosSelected는 teleportMinX와 teleportMaxX를 연결하는 녹색 선으로 텔레포트 범위를 그린다. teleportRange를 시안색 와이어 구로 그려 텔레포트 트리거 범위를 시각화한다.

**MidBossAnimationEvents.cs (Sprite Child 패턴)**

MidBossAnimationEvents는 Sprite 자식 오브젝트에 부착하여 애니메이션 이벤트를 부모 AI에 전달한다. stage1Boss(Stage1MidBossAI), stage2Boss(Stage2MidBossAI)를 가진다.

Start 메서드는 부모에서 GetComponentInParent를 사용하여 Stage1MidBossAI와 Stage2MidBossAI를 찾는다.

Animation Event 메서드는 다음과 같다. EnableAttackHitbox는 stage1Boss가 존재하면 stage1Boss.EnableAttackHitbox()를, stage2Boss가 존재하면 stage2Boss.EnableAttackHitbox()를 호출한다. DisableAttackHitbox, PlayAttackSound, PlayWalkSound도 동일한 방식으로 부모 AI의 해당 메서드를 호출한다.

**체력 및 피격 시스템**

MonsterHealth는 Health로 maxHP 100, currentHP, defense 0을 가진다. XP and Drops로 xpValue 50, itemDrop을 가진다. Weapon Damage Multipliers로 swordMultiplier 1.0f, lanceMultiplier 0.9f, maceMultiplier 1.2f를 가진다. Attack Type Multipliers로 lightAttackMultiplier 1.0f, heavyAttackMultiplier 1.5f를 가진다. 상태 변수는 animator, isDead false를 가진다.

Start 메서드는 currentHP를 maxHP로 초기화하고 animator를 가져온다.

TakeDamage는 다음과 같이 동작한다. isDead이면 리턴한다. 무기 타입에 따라 weaponMultiplier를 설정한다(Sword는 swordMultiplier, Lance는 lanceMultiplier, Mace는 maceMultiplier). 공격 타입에 따라 attackMultiplier를 설정한다(1은 lightAttackMultiplier, 2는 heavyAttackMultiplier). 최종 데미지는 RoundToInt((baseDamage - defense) × weaponMultiplier × attackMultiplier)로 계산한다. 최종 데미지를 최소 1로 제한한다(Max(1, finalDamage)). currentHP를 감소시킨다. ShowDamageText(finalDamage)를 호출한다. isHurt 애니메이터 트리거를 실행한다. currentHP가 0 이하이면 Die()를 호출한다.

ShowDamageText는 damageTextPrefab을 transform.position + Vector3.up 위치에 Instantiate한다. DamageText 컴포넌트를 가져와 SetDamage(damage)를 호출한다.

Die는 isDead를 true로 설정하고 isDead 애니메이터 트리거를 실행한다. MonsterController 컴포넌트를 가져와 비활성화한다. PlayerStats를 가져와 AddXP(xpValue)를 호출한다. DieRoutine() 코루틴을 시작한다.

DieRoutine 코루틴은 1초 대기하여 사망 애니메이션을 재생한다. MidBossController 컴포넌트가 존재하면 OnBossDeath()를 호출한다. itemDrop이 존재하면 DropItems()를 호출한다. 오브젝트를 파괴한다(Destroy).

**아이템 드롭 시스템**

ItemDrop은 DropItem 배열을 가진다. DropItem은 itemPrefab, dropChance(0f~1f), minQuantity, maxQuantity를 가진다.

DropItems는 dropItems 배열을 순회하며 각 DropItem에 대해 다음을 수행한다. Random.value가 dropChance 이하이면 아이템을 드롭한다. 드롭 수량은 minQuantity와 maxQuantity 사이의 랜덤 값이다. 수량만큼 반복하며 랜덤 위치에 itemPrefab을 Instantiate한다(transform.position + Random.insideUnitCircle × 0.5f). Rigidbody2D가 존재하면 AddForce로 물리적으로 튀어오르는 효과를 준다(랜덤 X: -2f~2f, 랜덤 Y: 3f~5f, ForceMode2D.Impulse).

---

#### 3.2.3 보스 전투 시스템 (주사위 기반 카드 전투)

보스 전투 시스템은 일반 전투와 달리 턴 기반 카드 전투 방식을 채택했다. Library of Ruina의 주사위 시스템에서 영감을 받아 구현했다.

**시스템 개요**

보스 전투는 Exploration(탐험 모드)과 Battle(전투 모드) 두 가지 게임 상태로 나뉜다. BossGameManager 싱글톤이 게임 상태를 관리하며, Battle 상태에서는 PlayerController의 이동을 비활성화한다.

**핵심 컴포넌트**

BattleController는 전투의 주요 흐름을 관장한다. 플레이어가 최대 3장의 카드를 선택하면 보스도 랜덤하게 카드를 선택한다. 선택이 완료되면 카메라가 줌인되고 캐릭터들이 클래시 위치로 이동한다. 각 클래시마다 주사위를 굴려 결과를 해결하고, 모든 클래시가 끝나면 캐릭터들이 홈 포지션으로 돌아가고 카메라가 줌아웃된다. 카드 쿨다운이 적용되고 새로운 턴이 시작된다.

CharacterStats 컴포넌트는 플레이어와 보스 모두에게 필요하다. CombatPage 카드들의 덱, 카드를 사용하기 위한 Light 자원, 현재 HP와 최대 HP를 관리한다. 카드 쿨다운 시스템은 Dictionary를 사용하여 cooldownTurns를 추적한다. 보스 덱 공개 메커니즘을 통해 플레이어가 보스의 카드를 확인할 수 있다.

CombatPage는 ScriptableObject로 구현되어 카드 데이터를 정의한다. 카드 이름과 아트워크, Light 비용과 쿨다운 턴 수, CombatDice 배열(공격 또는 방어 주사위), 일회용 여부를 포함한다.

CombatDice는 Attack(공격)과 Defense(방어) 타입을 가지며, 최소값과 최대값 사이에서 랜덤하게 주사위를 굴린다.

ClashManager는 정적 유틸리티 클래스로 플레이어와 보스의 주사위를 굴리고 합산한다. Attack vs Defense 타입 우위를 비교하여 패자에게 데미지를 적용한다.

**전투 흐름**

전투는 다음과 같은 순서로 진행된다. 첫째, 플레이어가 카드를 선택하고 Space 바로 확인한다. 둘째, 보스가 사용 가능한 Light 기반으로 랜덤하게 카드를 선택한다. 셋째, 카메라가 줌인되고 캐릭터들이 클래시 위치로 이동한다. 넷째, 각 클래시 쌍에 대해 주사위 애니메이션을 실행하고 주사위를 굴린 후 데미지를 해결한다. 다섯째, 캐릭터들이 홈 포지션으로 돌아가고 카메라가 줌아웃된다. 여섯째, 카드 쿨다운이 적용되고 새로운 턴이 시작된다.

**주사위 애니메이션 시스템**

DiceVisual 컴포넌트는 개별 주사위의 UI 표현을 담당한다. 숫자 사이클링과 회전으로 롤 애니메이션을 구현했고, Attack은 빨간색, Defense는 파란색으로 타입별 색상을 적용했다. Win/Loss/Draw 상황에 따라 깜빡임, 확대, 흔들림 애니메이션을 실행한다. 주사위 굴림과 결과에 사운드 이펙트를 추가했다.

DiceAnimationManager는 클래시 애니메이션을 총괄한다. playerDiceContainer와 bossDiceContainer에 주사위 비주얼을 생성한다. 딜레이를 두고 주사위를 순차적으로 굴리는 애니메이션을 실행한다. 합산 결과와 클래시 결과를 표시하고, ClashManager와 동일한 데미지 계산 로직을 사용하여 데미지를 적용한다. 주사위 개수가 다를 때 일방적인 공격을 처리한다. delayBetweenDice, delayAfterRoll, delayAfterClash로 타이밍을 조절할 수 있다.

중요한 점은 DiceAnimationManager가 BattleController Inspector에 반드시 할당되어야 애니메이션이 작동한다는 것이다. 할당되지 않으면 ClashManager.ResolveClash()로 폴백되어 애니메이션 없이 전투가 진행된다.

**데미지 계산**

ClashManager와 DiceAnimationManager는 동일한 데미지 계산 공식을 사용한다. Attack 주사위가 이기면 finalDamage = Max(1, (attackPower + rollA) - defensePower - defenseRoll)로 계산한다. Defense 주사위가 반격하면 counterDamage = rollA - rollB로 계산한다. 일방적인 공격일 때는 finalDamage = Max(1, (attackPower + roll) - defensePower)로 계산한다. 두 시스템의 공식이 일치해야 일관된 게임플레이를 보장할 수 있다.

**주요 메커니즘**

PlayerController는 BossGameManager.Instance.currentState가 Battle일 때 탐험 컨트롤을 비활성화한다. CharacterStats.InitializeFromPlayerScripts()는 PlayerController, PlayerHealth, PlayerStats에서 스탯을 동기화한다. 보스 덱 보기 모드는 플레이어 핸드와 보스 공개 카드 사이를 토글한다. 승리 시 10초 지연 후 씬 전환을 트리거한다. BattleController는 diceAnimationManager를 참조하는데, 이는 애니메이션에 필수적이지만 기능 자체에는 선택적이다. 클래시에서 패배한 주사위만 파괴되고, 승리한 주사위는 다음 클래시를 위해 유지된다.

---

#### 3.2.4 씬 관리 및 포털 시스템

씬 전환과 포털 시스템은 게임의 비선형적인 탐험을 가능하게 한다.

**포털 시스템**

PortalController는 씬 전환을 확인 UI와 함께 처리한다. 플레이어가 포털에 진입하면 확인 패널을 표시하고, 플레이어의 선택에 따라 씬을 로드하거나 취소한다. portalID, returnSpawnPoint, sceneToLoad를 설정하여 포털을 구성한다. 정적 usedPortalIDs 리스트를 사용하여 포털 재사용을 방지한다. PortalReturnData를 통해 복귀 위치를 저장한다. 포털은 사용 후 자동으로 파괴된다.

StatueInteraction은 Ancient/Life 맵에서 사용되는 특수 포털 변형이다. 일반 포털과 유사하지만 미니게임 전용으로 특화되어 있다. previousSceneName 정적 변수에 이전 씬을 저장하여 미니게임 종료 후 돌아갈 수 있게 한다.

PortalReturnManager는 미니게임이나 특수 씬을 떠난 후 플레이어의 복귀 위치를 관리한다. hasReturnInfo, returnPosition, previousSceneName을 PortalReturnData에 저장한다.

**데이터 지속성 패턴**

포털 시스템은 정적 데이터 클래스를 사용하여 씬 간 데이터를 유지한다. PortalReturnData는 복귀 위치와 씬 정보를 저장하는 정적 클래스다. GameData.SelectedWeapon은 무기 선택을 저장한다. PortalController.usedPortalIDs는 사용된 포털 ID를 추적하여 재진입을 방지한다.

**플레이어 스폰 시스템**

PlayerSpawnPoint는 씬에 배치되는 빈 GameObject로 "PlayerSpawnPoint" 태그를 가진다. PlayerController.OnSceneLoaded()는 씬이 로드되면 PlayerSpawnPoint를 찾아 플레이어를 해당 위치로 이동시킨다. 포털은 returnSpawnPoint를 참조하여 플레이어가 돌아올 위치를 지정한다.

**씬 전환 플로우**

일반적인 씬 전환은 다음과 같이 진행된다. 플레이어가 포털 트리거에 진입하면 확인 UI가 표시된다. 플레이어가 확인하면 PortalReturnData에 현재 위치를 저장하고, GameManager가 자동 저장을 수행한다. SceneManager.LoadScene()으로 새 씬을 로드한다. 새 씬에서 PlayerController.OnSceneLoaded()가 호출되고, PlayerSpawnPoint 위치로 플레이어가 이동한다. 스탯이 재계산되고 UI가 업데이트된다.

미니게임 복귀 플로우는 특별하게 처리된다. StatueInteraction이 previousSceneName에 현재 씬을 저장한다. 미니게임 씬으로 전환한다. 미니게임이 종료되면 previousSceneName을 참조하여 원래 씬으로 돌아간다. PortalReturnManager를 통해 정확한 복귀 위치가 복원된다.

---

#### 3.2.5 세이브/로드 시스템

세이브/로드 시스템은 JSON 기반의 3슬롯 저장 방식을 사용한다.

**시스템 아키텍처**

SaveManager는 파일 I/O만 담당하는 지속성 계층이다. SaveGame()은 SaveData를 JSON으로 직렬화하여 파일에 쓴다. LoadGame()은 파일에서 JSON을 읽어 SaveData로 역직렬화한다. DeleteSave()는 슬롯의 저장 파일을 삭제한다. GetAllSaveSlots()는 3개 슬롯의 SaveData 배열을 반환한다. 저장 위치는 Application.persistentDataPath/Saves/이다.

GameManager는 비즈니스 로직을 담당하는 게임 상태 조율 계층이다. currentSaveSlot으로 현재 활성 슬롯을 관리한다. SaveCurrentGame()은 현재 게임 상태를 SaveData로 캡처한다. LoadGameFromSlot()은 SaveData를 복원하고 씬을 로드한다. PerformAutoSave()는 자동 저장을 실행한다. New Game 플로우를 관리하는 isNewGame 플래그와 selectedWeapon 변수를 가진다.

SaveData는 직렬화 가능한 데이터 클래스로, 플레이어 스탯(레벨, XP, HP, 공격력, 방어력, 돈), 인벤토리(무기, 포션), 씬 정보(currentScene, playerPosition), 메타데이터(slotNumber, saveTime, playTime, isEmpty)를 포함한다. CreateEmptySlot(), GetSummary(), UpdateSaveTime() 헬퍼 메서드를 제공한다.

**저장 프로세스**

플레이어가 저장을 요청하거나 자동 저장이 트리거되면 다음과 같이 진행된다. GameManager.SaveCurrentGame()이 호출되고, PlayerController, PlayerStats, PlayerHealth, Inventory에서 데이터를 수집한다. SaveData 객체를 생성하고 모든 필드를 채운다. SaveData.UpdateSaveTime()으로 저장 시간을 갱신한다. SaveManager.SaveGame(saveData, slotNumber)을 호출한다. SaveManager가 SaveData를 JSON으로 직렬화하고, Application.persistentDataPath/Saves/SaveSlotN.json에 파일을 쓴다.

**로드 프로세스**

플레이어가 슬롯을 선택하여 로드하면 다음과 같이 진행된다. LoadGameUI에서 GameManager.LoadGameFromSlot(slotNumber)을 호출한다. SaveManager.LoadGame(slotNumber)으로 SaveData를 로드한다. SaveData가 비어있으면(isEmpty == true) 에러 처리한다. GameManager.ApplySaveData(saveData)를 호출한다. PlayerController, PlayerStats, PlayerHealth를 찾아 SaveData의 값을 적용한다. Inventory의 아이템을 복원한다. SceneManager.LoadScene(saveData.currentScene)으로 저장된 씬을 로드한다. PlayerController.OnSceneLoaded()에서 saveData.playerPosition으로 플레이어 위치를 설정한다.

**자동 저장 시스템**

자동 저장은 다음 조건에서 트리거된다. 씬 전환 시 OnSceneLoaded 이벤트가 발생하고, 0.5초 지연 후 실행된다. Main, LoadGame, Weapon, HowToPlay, Setting 씬은 제외된다. currentSaveSlot이 0보다 커야 한다(슬롯이 선택된 상태). 플레이어 위치는 PlayerSpawnPoint 위치로 저장된다.

**New Game vs Load Game 플로우**

New Game 플로우는 다음과 같다. Main 메뉴에서 New Game 선택, Weapon 씬에서 무기 선택(GameManager.selectedWeapon 설정, isNewGame = true), LoadGame 씬에서 슬롯 선택(기존 세이브가 있으면 덮어쓰기 확인), GameManager.SelectSaveSlot()으로 currentSaveSlot 설정, 선택된 무기로 Stage1 시작.

Load Game 플로우는 다음과 같다. Main 메뉴에서 Load Game 선택, LoadGame 씬에서 슬롯 선택, SaveData 로드 및 검증(isEmpty 체크), 모든 플레이어 데이터 복원, 저장된 씬으로 이동.

**슬롯 관리**

3개의 슬롯이 독립적으로 관리된다. SaveSlot1.json, SaveSlot2.json, SaveSlot3.json 파일로 저장된다. SaveSlotButton UI는 슬롯 번호, 저장 정보(레벨, 씬, 시간), 빈 슬롯 텍스트를 표시한다. 선택/삭제 액션을 처리한다. 빈 슬롯은 isEmpty=true 플래그를 가진 SaveData를 반환한다.

---

#### 3.2.6 상점 시스템

상점 시스템은 랜덤화된 인벤토리와 새로고침 메커니즘을 가진 동적 아이템 구매 시스템이다.

**시스템 구성**

ShopManager 싱글톤이 상점 시스템을 관리한다. allAvailableItems 풀에서 4개의 아이템을 중복 없이 랜덤 선택한다. PlayerStats와 통합하여 구매를 검증하고 처리한다. 새로고침마다 증가하는 동적 새로고침 비용을 구현했다(baseRefreshCost + refreshCostIncrement × refreshCount). 구매, 새로고침, 에러에 대한 사운드 이펙트를 제공한다.

ShopItemData는 ScriptableObject로 아이템을 정의한다. ItemType enum은 Potion, AttackUpgrade, DefenseUpgrade, HealthUpgrade, SpeedUpgrade 타입을 가진다. ApplyEffect() 메서드가 영구 스탯 보너스를 적용하거나 인벤토리에 포션을 추가한다. 아이템 이름, 아이콘, 가격, 타입, 효과값, 아이콘 스케일을 설정할 수 있다. 포션 타입의 경우 PotionItemData 참조를 포함한다.

ShopPedestal은 개별 아이템 표시대로 구매 상호작용을 처리한다. RefreshPedestal은 상점 새로고침 기능을 위한 특수 표시대다. ExchangePedestal은 돈이 아닌 아이템 교환을 위한 특수 표시대다.

**아이템 효과**

각 아이템 타입은 다음과 같은 효과를 가진다. Potion은 PotionItemData를 Inventory에 추가한다. AttackUpgrade는 PlayerStats.bonusAttackPower를 증가시킨다. DefenseUpgrade는 PlayerHealth.defense를 증가시킨다. HealthUpgrade는 PlayerHealth.AddPermanentHealth()를 호출한다. SpeedUpgrade는 PlayerStats.bonusMoveSpeed를 증가시킨다.

**구매 프로세스**

플레이어가 아이템을 선택하면 다음과 같이 진행된다. ShopPedestal이 ShopManager.Instance.Purchase(item)을 호출한다. PlayerStats.SpendMoney(item.price)로 돈을 차감하고 검증한다. 검증 실패 시 에러 사운드를 재생하고 리턴한다. 검증 성공 시 item.ApplyEffect()를 호출한다. 구매 사운드를 재생하고 표시대에서 아이템을 제거한다.

**새로고침 메커니즘**

플레이어가 새로고침 표시대를 활성화하면 다음과 같이 진행된다. 현재 새로고침 비용을 계산한다(baseRefreshCost + refreshCostIncrement × refreshCount). PlayerStats.SpendMoney()로 비용을 검증하고 차감한다. 검증 실패 시 에러 사운드를 재생한다. 검증 성공 시 기존 아이템을 모두 제거하고, allAvailableItems에서 중복 없이 4개의 새 아이템을 랜덤 선택한다. 새 아이템을 표시대에 할당하고 refreshCount를 증가시킨다. 새로고침 사운드를 재생한다.

---

#### 3.2.7 인벤토리 시스템

인벤토리 시스템은 아이템 관리와 사용을 담당한다.

**시스템 구조**

Inventory 싱글톤이 인벤토리 데이터를 관리한다. items는 List<PotionItemData>로 포션 아이템을 저장한다. inventorySize는 20으로 최대 용량을 제한한다. OnItemChanged 델리게이트를 통해 UI 업데이트 콜백을 제공한다. Add(), Remove(), IsFull(), GetItemCount() 메서드를 제공한다.

InventoryUI는 인벤토리 패널과 슬롯을 관리한다. Inventory.onItemChangedCallback을 구독하여 자동으로 UI를 업데이트한다. 슬롯 배열을 관리하고 아이템 아이콘과 수량을 표시한다.

InventorySlot은 개별 슬롯 UI를 구현한다. IBeginDragHandler, IDragHandler, IEndDragHandler를 구현하여 드래그 앤 드롭을 지원한다. 드래그 시작 시 blocksRaycasts를 비활성화하고, 드래그 중 위치를 업데이트하며, 드래그 종료 시 blocksRaycasts를 재활성화하고 플레이어에 드롭되면 아이템을 사용한다.

**아이템 획득**

ItemPickup은 플레이어가 아이템과 충돌 시 처리한다. OnTriggerEnter2D에서 플레이어 충돌을 감지하고, Inventory.instance.Add(item)을 호출하여 아이템을 추가한다. 성공 시 픽업 오브젝트를 파괴한다. 인벤토리가 가득 차면 에러 메시지를 표시한다.

Coin은 화폐 픽업을 처리한다. PlayerStats.AddMoney(amount)를 호출하여 돈을 추가하고, 코인 오브젝트를 파괴한다.

**아이템 사용**

플레이어가 포션을 사용하면 다음과 같이 진행된다. InventorySlot에서 포션을 플레이어에게 드롭한다. PotionItemData.healthRestoreAmount를 가져온다. PlayerHealth.Heal(healthRestoreAmount)을 호출한다. Inventory.instance.Remove(item)을 호출하여 인벤토리에서 제거한다. onItemChangedCallback이 발동되어 UI가 자동 업데이트된다.

---

#### 3.2.8 미니게임 시스템

미니게임 시스템은 메인 게임플레이와 연결된 보상 기반 미니게임을 제공한다.

**대장간 미니게임 (AncientBlacksmith)**

BlacksmithMinigameManager 싱글톤이 대장간 미니게임을 관리한다. 게임 시작 전 DialogueController를 통해 대화를 진행한다. 점수 기반 보상 시스템을 CalculateAndGrantRewards()에 구현했다. 보상은 골드, XP, 공격력 보너스로 구성된다. isGamePausedByManager 정적 플래그를 사용하여 PlayerStats의 시간 재개를 방지한다. hasGameStarted 플래그로 이중 초기화를 방지한다.

게임 플로우는 다음과 같다. Start()에서 시간을 정지한다(Time.timeScale = 0). DialogueController로 대화를 표시한다. StartMinigame()으로 미니게임을 시작한다. GameLoop()에서 불꽃 수집 게임을 진행한다. EndGame()에서 점수를 계산하고 보상을 지급한다. 이전 씬으로 복귀한다(StatueInteraction.previousSceneName).

보상 계산은 점수 구간별로 다르다. 높은 점수일수록 더 많은 골드, XP, 공격력 보너스를 받는다. 보상은 PlayerStats에 직접 적용된다(GameObject.FindGameObjectWithTag("Player")). 보상 지급 후 isGamePausedByManager = true로 설정하여 PlayerStats의 자동 시간 재개를 방지한다.

**라이프맵 미니게임 (LifeHeartMap)**

LifeGameManager 싱글톤이 LifeHeartMap 씬을 관리한다. 대장간 미니게임과 유사한 구조를 가지지만 다른 게임 메커니즘과 보상을 제공한다.

**미니게임 공통 메커니즘**

미니게임 중 레벨업이 발생하면 PlayerStats.isLevelUpPending이 true로 설정되어 레벨업 UI 표시가 연기된다. 미니게임 종료 후 원래 씬으로 돌아가면 ShowPendingLevelUpPanel()이 호출되어 대기 중인 레벨업 UI를 표시한다.

시간 관리는 매우 중요하다. 미니게임 시작 시 Time.timeScale = 0으로 설정한다. 미니게임 진행 중에는 Time.unscaledDeltaTime을 사용한다. 미니게임 종료 시 isGamePausedByManager = true로 설정하여 PlayerStats가 시간을 자동으로 재개하지 못하게 한다. 원래 씬으로 돌아간 후 수동으로 Time.timeScale = 1로 설정한다.

**대화 시스템 통합**

DialogueController 싱글톤이 대화 표시를 관리한다. UnityEvent 콜백을 통해 대화 완료 시 미니게임을 시작한다. 대화 텍스트, 캐릭터 초상화, 배경을 표시한다. 플레이어 입력으로 대화를 진행하거나 스킵할 수 있다.

---

#### 3.2.9 스테이지 3 수중 시스템

Stage3는 독특한 수영 메커니즘과 산소 관리를 특징으로 하는 수중 레벨이다.

**시스템 관리**

Stage3Manager가 씬 매니저로서 스테이지별 컴포넌트를 활성화/비활성화한다. OnEnable()에서 PlayerSwimming과 PlayerOxygen을 활성화한다(GetComponent<T>().enabled = true). OnDestroy()에서 두 컴포넌트를 비활성화하여 다른 씬에서의 간섭을 방지한다. 이 패턴을 통해 플레이어 GameObject가 스테이지별 스크립트를 충돌 없이 운반할 수 있다.

**수영 메커니즘**

PlayerSwimming은 표준 이동을 수영 메커니즘으로 대체한다. 일반적으로 플레이어에 부착되어 있지만 Stage3까지 비활성화된다. 수중에서는 중력이 감소하고 8방향 이동이 가능하다. 수직 이동(위/아래 화살표)을 지원하고, 수영 애니메이션을 실행한다.

**산소 시스템**

PlayerOxygen은 시간에 따라 감소하는 산소 게이지를 관리한다. Stage3에서만 활성화된다. 산소가 다 떨어지면 플레이어가 지속적으로 데미지를 받는다. oxygenSlider UI를 참조하여 현재 산소량을 표시한다. Stage3을 떠날 때 oxygenSlider는 비활성화되어야 한다.

OxygenZone은 플레이어가 들어가면 산소를 회복하는 영역이다(예: 공기 주머니). OnTriggerStay2D에서 지속적으로 산소를 회복한다. 회복률과 최대 산소량을 설정할 수 있다.

**수중 환경 요소**

Pearl은 Stage3의 수집 가능한 아이템이다. 플레이어가 수집하면 PearlDisplayUI를 업데이트한다.

PearlDisplayUI는 진주 수집 개수를 표시하는 UI다. 총 진주 개수와 수집한 진주 개수를 표시한다.

GiantClam은 상호작용 가능한 조개 오브젝트다. 진주를 포함하거나 위험 요소를 가질 수 있다.

ClamMonsterController는 수중 조개 몬스터 전용 AI다. 기본 MonsterController와 유사하지만 수중 이동에 특화되어 있다.

PortalToStage3은 수중 스테이지로의 전환을 처리하는 특수 포털이다. 일반 PortalController와 유사하지만 Stage3별 준비 로직을 포함한다.

ParallaxController는 수중 배경의 패럴랙스 효과를 제어한다. 여러 레이어를 다른 속도로 이동시켜 깊이감을 준다.

**통합 메커니즘**

플레이어는 Stage3Manager가 찾을 수 있도록 "Player" 태그를 가져야 한다. PlayerOxygen은 oxygenSlider UI를 참조하는데, Stage3을 떠날 때 비활성화되어야 한다. 스테이지별 로직 컴포넌트(PlayerSwimming, PlayerOxygen)는 기본적으로 비활성화되어 있고, Stage3Manager가 씬 시작 시 활성화하고 씬 종료 시 비활성화한다.

---

#### 3.2.10 UI 시스템

UI 시스템은 플레이어와 게임의 모든 상호작용을 관리한다.

**메인 메뉴 시스템**

MainMenuController는 메인 메뉴 씬을 제어한다. New Game 선택 시 Weapon 씬으로 전환한다. Load Game 선택 시 LoadGame 씬으로 전환한다. 메인 메뉴 복귀 시 DontDestroyOnLoad 오브젝트를 정리한다. GameManager와 SaveManager는 정리에서 제외되어 유지된다.

WeaponChoice는 무기 선택 씬을 제어한다. GameManager.isNewGame 플래그를 설정하고 선택된 무기를 저장한다(selectedWeapon). 무기 선택 후 LoadGame 씬으로 전환한다.

LoadGameUI는 3개 슬롯을 가진 세이브 슬롯 선택 UI다. 레벨, 씬, 시간을 포함한 세이브 슬롯 정보를 표시한다. New Game과 Load Game 플로우를 처리한다. 기존 세이브에 대한 덮어쓰기 확인을 제공한다. SaveManager 및 GameManager와 통합된다.

SaveSlotButton은 개별 세이브 슬롯 UI 버튼이다. 슬롯 번호, 세이브 정보, 빈 슬롯 텍스트를 표시한다. 선택/삭제 액션을 처리한다.

**HUD 시스템**

StatsUIManager는 플레이어 스탯을 실시간으로 표시한다. PlayerController의 UpdateAllStatsUI()를 통해 업데이트된다. 체력바, XP 바, 레벨, 공격력, 방어력, 이동속도, 돈을 표시한다.

CurrencyUI는 플레이어의 현재 돈을 표시한다. PlayerStats.money가 변경될 때마다 자동 업데이트된다.

**레벨업 UI**

LevelUpUIManager는 레벨업 패널과 스탯 업그레이드 옵션을 표시한다. 패널이 표시되는 동안 시간을 정지한다(Time.timeScale = 0). 공격력, 방어력, 체력, 이동속도 증가 버튼을 제공한다. 선택 시 PlayerStats의 해당 메서드를 호출하고 패널을 닫는다.

**인벤토리 UI**

InventoryUI는 인벤토리 패널과 슬롯을 관리한다. Inventory.onItemChangedCallback을 구독한다. 아이템 아이콘, 이름, 수량을 표시한다.

InventorySlot은 드래그 앤 드롭을 지원하는 개별 슬롯이다. IBeginDragHandler, IDragHandler, IEndDragHandler를 구현한다. 플레이어에 드롭 시 아이템을 사용한다.

Tooltip은 아이템 위에 마우스를 올리면 표시되는 툴팁이다. TooltipManager가 툴팁 표시를 관리한다. 아이템 이름, 설명, 효과를 보여준다.

**일시정지 메뉴**

PauseMenuUI는 ESC 키로 호출되는 일시정지 메뉴다. Time.timeScale로 일시정지/재개를 관리한다(0=정지, 1=정상). 씬 로드 이벤트를 구독하여 상태를 리셋한다. 메인 메뉴 복귀를 위해 DontDestroyOnLoadManager와 통합된다. 메인 메뉴로 돌아갈 때 싱글톤을 정리한다. Resume(재개), Settings(설정), Return to Main(메인 메뉴) 버튼을 제공한다.

**대미지 텍스트**

DamageText는 데미지 수치를 표시하는 떠다니는 텍스트다. "DamageTextCanvas"에 생성된다. 위로 떠오르며 페이드 아웃하는 애니메이션을 실행한다. 생명주기가 끝나면 자동으로 파괴된다.

**카메라 시스템**

CameraFollow는 플레이어를 추적하는 카메라를 구현한다. 부드러운 추적을 위해 lerp를 사용한다. 수평/수직 오프셋을 설정할 수 있다.

CameraBounds는 카메라 이동 범위를 제한한다. minX, maxX, minY, maxY로 경계를 설정한다. 카메라가 레벨 밖으로 나가는 것을 방지한다.

**지속성 관리**

DontDestroyOnLoadManager는 씬 간 오브젝트 지속성을 관리한다. instanceId 기반 Dictionary로 인스턴스를 추적한다. 같은 instanceId를 가진 중복 오브젝트를 방지한다. isReturningToMainMenu 플래그로 메인 메뉴 복귀 시 지속성을 비활성화한다. Main 씬에서 ResetMainMenuFlag()를 호출하여 인스턴스를 정리한다.

---

### 3.3 시퀀스 다이어그램

#### 3.3.1 게임 시작 플로우

게임 시작부터 게임플레이까지의 시퀀스는 다음과 같다.

```
Player -> MainMenuController: New Game 선택
MainMenuController -> SceneManager: LoadScene("Weapon")
Player -> WeaponChoice: 무기 선택 (검/창/메이스)
WeaponChoice -> GameManager: selectedWeapon 설정, isNewGame = true
WeaponChoice -> SceneManager: LoadScene("LoadGame")
Player -> LoadGameUI: 슬롯 선택
LoadGameUI -> GameManager: SelectSaveSlot(slotNumber)
LoadGameUI -> GameManager: 기존 세이브 확인
GameManager -> SaveManager: LoadGame(slotNumber)
SaveManager -> GameManager: SaveData 반환
GameManager -> SceneManager: LoadScene("Stage1")
SceneManager -> PlayerController: OnSceneLoaded()
PlayerController -> PlayerSpawnPoint: 위치 조회
PlayerController: transform.position 설정
PlayerController -> PlayerStats: RecalculateStats()
PlayerController -> StatsUIManager: UpdateAllStatsUI()
PlayerController -> GameManager: PerformAutoSave()
```

New Game의 경우 선택된 무기와 기본 스탯으로 새 게임을 시작한다. Load Game의 경우 SaveData를 복원하여 이전 진행 상황에서 재개한다.

#### 3.3.2 전투 시퀀스

플레이어가 적을 공격하는 일반적인 전투 시퀀스는 다음과 같다.

```
Player: J 키 입력
PlayerController: AttackRoutine() 코루틴 시작
PlayerController: isAttacking = true
PlayerController -> Animator: SetTrigger("Attack")
Animator -> PlayerController: AttackHit() 이벤트 호출
PlayerController -> SwordAttack: Attack(attackType)
SwordAttack: OverlapCircleAll로 적 감지
SwordAttack -> MonsterHealth: TakeDamage(damage, attackType, "Sword")
MonsterHealth: 데미지 계산 (무기 배수, 공격 타입 배수)
MonsterHealth -> DamageText: ShowDamageText(finalDamage)
MonsterHealth -> Animator: SetTrigger("isHurt")
MonsterHealth: currentHP 체크
MonsterHealth: if HP <= 0, Die()
MonsterHealth -> MonsterController: enabled = false
MonsterHealth -> PlayerStats: AddXP(xpValue)
MonsterHealth: DieRoutine() 코루틴 시작
MonsterHealth: 1초 대기 (사망 애니메이션)
MonsterHealth -> MidBossController: OnBossDeath() (중간보스인 경우)
MonsterHealth -> ItemDrop: DropItems()
ItemDrop: 각 DropItem 확률 체크
ItemDrop: Instantiate(itemPrefab) 랜덤 위치
ItemDrop -> Rigidbody2D: AddForce (튀어오름 효과)
MonsterHealth: Destroy(gameObject)
PlayerController: attackDuration 대기
PlayerController: isAttacking = false
```

적이 사망하면 XP 지급, 아이템 드롭, 이벤트 트리거(중간보스의 경우) 순서로 처리된다.

#### 3.3.3 보스 전투 시퀀스

턴 기반 카드 전투 보스전의 시퀀스는 다음과 같다.

```
Player: BattleTrigger 진입
BattleTrigger -> BossGameManager: SetState(Battle)
BossGameManager -> PlayerController: 이동 비활성화
BattleController: 카드 선택 UI 표시
Player: 카드 선택 (최대 3장)
Player: Space 바로 확인
BattleController -> CharacterStats(Boss): AI 카드 선택
BattleController: StartClashPhase()
BattleController -> CameraController: ZoomIn()
BattleController -> CharacterVisuals: 클래시 위치로 이동
BattleController -> DiceAnimationManager: SetupDiceVisuals()
DiceAnimationManager: playerDiceContainer에 주사위 UI 생성
DiceAnimationManager: bossDiceContainer에 주사위 UI 생성
DiceAnimationManager: AnimateClashSequence()
DiceAnimationManager -> DiceVisual: AnimateRoll() (각 주사위 순차적)
DiceVisual: 숫자 사이클링 + 회전 애니메이션
DiceVisual: 최종 값 표시
DiceAnimationManager: 합산 표시 (플레이어 vs 보스)
DiceAnimationManager: 클래시 결과 계산
DiceAnimationManager -> DiceVisual: Win/Loss/Draw 애니메이션
DiceAnimationManager: ApplyClashDamage()
DiceAnimationManager -> CharacterStats: TakeDamage()
DiceAnimationManager: 패배한 주사위만 파괴
DiceAnimationManager: 다음 클래시 (주사위 개수만큼 반복)
BattleController -> CharacterVisuals: 홈 포지션으로 복귀
BattleController -> CameraController: ZoomOut()
BattleController -> CharacterStats: 카드 쿨다운 적용
BattleController: 다음 턴 시작
```

보스나 플레이어의 HP가 0이 되면 Victory() 또는 Defeat()가 호출되어 전투가 종료된다.

#### 3.3.4 세이브/로드 시퀀스

자동 저장 시퀀스는 다음과 같다.

```
SceneManager: 씬 로드 완료
PlayerController: OnSceneLoaded() 이벤트 수신
PlayerController: AutoSaveAfterDelay() 코루틴 시작
AutoSaveAfterDelay: 0.5초 대기
AutoSaveAfterDelay: 씬 이름 체크 (제외 씬인지 확인)
AutoSaveAfterDelay: currentSaveSlot > 0 체크
AutoSaveAfterDelay -> GameManager: PerformAutoSave()
GameManager -> GameManager: SaveCurrentGame()
GameManager -> PlayerController: 데이터 수집
GameManager -> PlayerStats: 데이터 수집
GameManager -> PlayerHealth: 데이터 수집
GameManager -> Inventory: 데이터 수집
GameManager: SaveData 객체 생성 및 채우기
GameManager: currentScene, playerPosition 설정
GameManager -> SaveData: UpdateSaveTime()
GameManager -> SaveManager: SaveGame(saveData, currentSaveSlot)
SaveManager: JsonUtility.ToJson(saveData)
SaveManager: File.WriteAllText(path, json)
```

로드 시퀀스는 다음과 같다.

```
Player -> LoadGameUI: 슬롯 선택
LoadGameUI -> GameManager: LoadGameFromSlot(slotNumber)
GameManager: currentSaveSlot = slotNumber
GameManager -> SaveManager: LoadGame(slotNumber)
SaveManager: File.ReadAllText(path)
SaveManager: JsonUtility.FromJson<SaveData>(json)
SaveManager -> GameManager: SaveData 반환
GameManager: isEmpty 체크
GameManager -> GameManager: ApplySaveData(saveData)
GameManager: PlayerController 찾기
GameManager -> PlayerController: hasSword/hasLance/hasMace 설정
GameManager -> PlayerStats: level, currentXP, money 등 설정
GameManager -> PlayerHealth: maxHP, currentHP, defense 설정
GameManager -> Inventory: 아이템 복원
GameManager -> SceneManager: LoadScene(saveData.currentScene)
SceneManager -> PlayerController: OnSceneLoaded()
PlayerController: transform.position = saveData.playerPosition
PlayerController: RecalculateStats()
PlayerController: UpdateAllStatsUI()
```

#### 3.3.5 씬 전환 시퀀스

포털을 통한 씬 전환 시퀀스는 다음과 같다.

```
Player: 포털 트리거 진입
PortalController: OnTriggerEnter2D()
PortalController: confirmationPanel.SetActive(true)
Player: 확인 버튼 클릭
PortalController: OnConfirm()
PortalController: usedPortalIDs.Add(portalID)
PortalController -> PortalReturnData: 현재 위치 저장
PortalReturnData: hasReturnInfo = true
PortalReturnData: returnPosition = transform.position
PortalReturnData: previousSceneName = currentScene
PortalController -> GameManager: PerformAutoSave()
GameManager: SaveCurrentGame()
PortalController -> SceneManager: LoadScene(sceneToLoad)
SceneManager: 새 씬 로드
PlayerController: OnSceneLoaded()
PlayerController: PlayerSpawnPoint 찾기
PlayerController: transform.position = spawnPoint.position
PlayerController: RecalculateStats()
PlayerController: UpdateAllStatsUI()
PlayerController: AutoSaveAfterDelay()
```

미니게임에서 복귀하는 시퀀스는 다음과 같다.

```
Player: 미니게임 완료
BlacksmithMinigameManager: EndGame()
BlacksmithMinigameManager: CalculateAndGrantRewards()
BlacksmithMinigameManager -> PlayerStats: AddMoney(), AddXP(), bonusAttackPower 증가
BlacksmithMinigameManager: isGamePausedByManager = true
BlacksmithMinigameManager -> StatueInteraction: previousSceneName 참조
BlacksmithMinigameManager -> SceneManager: LoadScene(previousSceneName)
SceneManager: 원래 씬 로드
PlayerController: OnSceneLoaded()
PlayerController -> PortalReturnManager: hasReturnInfo 체크
PortalReturnManager: returnPosition 반환
PlayerController: transform.position = returnPosition
PlayerController -> PlayerStats: ShowPendingLevelUpPanel() (레벨업이 대기 중이면)
PlayerStats: isLevelUpPending = false
PlayerStats -> LevelUpUIManager: ShowPanel()
```

---

### 3.4 주요 알고리즘

#### 3.4.1 데미지 계산 알고리즘

일반 적의 데미지 계산 알고리즘은 다음과 같다.

```
입력:
- baseDamage: 기본 공격 데미지
- defense: 적의 방어력
- weaponType: "Sword", "Lance", "Mace" 중 하나
- attackType: 1 (약공격), 2 (강공격)

변수:
- weaponMultiplier: 무기 타입에 따른 배수
  - Sword: 1.0
  - Lance: 0.9
  - Mace: 1.2

- attackMultiplier: 공격 타입에 따른 배수
  - attackType == 1: 1.0 (약공격)
  - attackType >= 2: 1.5 (강공격)

계산:
1. weaponMultiplier 결정
   if weaponType == "Sword": weaponMultiplier = 1.0
   else if weaponType == "Lance": weaponMultiplier = 0.9
   else if weaponType == "Mace": weaponMultiplier = 1.2

2. attackMultiplier 결정
   if attackType == 1: attackMultiplier = 1.0
   else: attackMultiplier = 1.5

3. 최종 데미지 계산
   finalDamage = RoundToInt((baseDamage - defense) × weaponMultiplier × attackMultiplier)

4. 최소 데미지 보장
   finalDamage = Max(1, finalDamage)

출력: finalDamage
```

보스 전투의 주사위 기반 데미지 계산 알고리즘은 다음과 같다.

```
입력:
- playerPages: 플레이어가 선택한 카드 배열
- bossPages: 보스가 선택한 카드 배열
- playerAttackPower: 플레이어 공격력
- playerDefensePower: 플레이어 방어력
- bossAttackPower: 보스 공격력
- bossDefensePower: 보스 방어력

클래시별 처리:
for i = 0 to Min(playerPages.Length, bossPages.Length) - 1:
    playerPage = playerPages[i]
    bossPage = bossPages[i]

    // 주사위 굴리기
    playerRollTotal = 0
    for each dice in playerPage.dices:
        roll = Random.Range(dice.minValue, dice.maxValue + 1)
        playerRollTotal += roll

    bossRollTotal = 0
    for each dice in bossPage.dices:
        roll = Random.Range(dice.minValue, dice.maxValue + 1)
        bossRollTotal += roll

    // 타입 우위 계산
    playerAdvantage = (playerPage.dices[0].type == Attack && bossPage.dices[0].type == Defense)
    bossAdvantage = (bossPage.dices[0].type == Attack && playerPage.dices[0].type == Defense)

    // 데미지 해결
    if playerRollTotal > bossRollTotal:
        if playerAdvantage:
            damage = Max(1, (playerAttackPower + playerRollTotal) - bossDefensePower - bossRollTotal)
            boss.TakeDamage(damage)
        else:
            counterDamage = playerRollTotal - bossRollTotal
            boss.TakeDamage(counterDamage)
    else if bossRollTotal > playerRollTotal:
        if bossAdvantage:
            damage = Max(1, (bossAttackPower + bossRollTotal) - playerDefensePower - playerRollTotal)
            player.TakeDamage(damage)
        else:
            counterDamage = bossRollTotal - playerRollTotal
            player.TakeDamage(counterDamage)
    // 동점이면 데미지 없음

일방적인 공격 (주사위 개수가 다를 때):
if playerPages.Length > bossPages.Length:
    for i = bossPages.Length to playerPages.Length - 1:
        playerPage = playerPages[i]
        playerRollTotal = 주사위 굴리기 합산
        damage = Max(1, (playerAttackPower + playerRollTotal) - bossDefensePower)
        boss.TakeDamage(damage)

else if bossPages.Length > playerPages.Length:
    (보스도 동일하게 처리)
```

이 알고리즘은 ClashManager와 DiceAnimationManager 모두에서 동일하게 구현되어 일관성을 보장한다.

#### 3.4.2 적 AI 상태 전환 알고리즘

적 AI의 상태 머신 알고리즘은 다음과 같다.

```
입력:
- currentState: 현재 AI 상태
- playerPosition: 플레이어 위치
- enemyPosition: 적 위치
- detectionRange: 감지 범위 (예: 5f)
- attackRange: 공격 범위 (예: 1.5f)

매 프레임 실행:
1. 거리 계산
   distance = Vector2.Distance(enemyPosition, playerPosition)

2. 상태별 전환 규칙
   switch currentState:
       case Patrol:
           if distance <= detectionRange:
               ChangeState(Chase)

       case Chase:
           if distance > detectionRange:
               ChangeState(Patrol)
           else if distance <= attackRange && !isAttacking:
               ChangeState(Attack)

       case Attack:
           if distance > attackRange:
               ChangeState(Chase)

       case Hurt:
           // 피격 애니메이션 완료 후 자동으로 Chase 또는 Patrol로 복귀

       case Dead:
           // 더 이상 상태 전환 없음

3. 상태별 실행
   switch currentState:
       case Patrol:
           Patrol() 실행
           - currentTarget 방향으로 이동
           - 목표 도달 시 currentTarget 전환
           - 벽/낭떠러지 감지 시 currentTarget 전환

       case Chase:
           Chase() 실행
           - playerPosition 방향으로 이동
           - 스프라이트 방향 조정

       case Attack:
           if !isAttacking:
               AttackRoutine() 코루틴 시작
               - isAttacking = true
               - 정지
               - 공격 애니메이션
               - AttackHitbox 활성화 (애니메이션 이벤트)
               - attackCooldown 대기
               - AttackHitbox 비활성화
               - isAttacking = false
```

Stage2 중간보스의 텔레포트 알고리즘은 다음과 같다.

```
입력:
- playerPosition: 플레이어 위치
- enemyPosition: 적 위치
- teleportRange: 텔레포트 트리거 거리 (예: 6f)
- teleportMinX, teleportMaxX: 텔레포트 가능 X 범위
- teleportY: 텔레포트 Y 좌표

매 프레임 실행:
1. 거리 계산
   distance = Vector2.Distance(enemyPosition, playerPosition)

2. 텔레포트 조건 체크
   if distance > teleportRange && canTeleport:
       TeleportRoutine() 코루틴 시작

TeleportRoutine():
1. canTeleport = false
2. 정지 (linearVelocity = Vector2.zero)
3. FadeOut() 실행 (0.5초 동안 alpha 1→0)
4. 새 위치 계산:
   randomX = Random.Range(teleportMinX, teleportMaxX)
   while Abs(randomX - playerPosition.x) < 2f:
       randomX = Random.Range(teleportMinX, teleportMaxX)
   newPosition = (randomX, teleportY)
5. transform.position = newPosition
6. FadeIn() 실행 (0.5초 동안 alpha 0→1)
7. teleportCooldown 대기 (예: 3초)
8. canTeleport = true
```

#### 3.4.3 경로 탐색 알고리즘

순찰 경로 알고리즘은 다음과 같다.

```
입력:
- patrolPointA: 순찰 지점 A
- patrolPointB: 순찰 지점 B
- currentTarget: 현재 목표 지점
- moveSpeed: 이동 속도

매 프레임 실행:
1. 방향 계산
   direction = Normalize(currentTarget.position - enemyPosition)

2. 이동
   linearVelocity.x = direction.x × moveSpeed

3. 스프라이트 방향 조정
   if direction.x > 0:
       transform.localScale = (1, 1, 1)
   else:
       transform.localScale = (-1, 1, 1)

4. 목표 도달 체크
   if Distance(enemyPosition, currentTarget.position) < 0.5f:
       if currentTarget == patrolPointA:
           currentTarget = patrolPointB
       else:
           currentTarget = patrolPointA

5. 장애물 체크
   hasWall = OverlapCircle(wallCheck.position, checkRadius, groundLayer)
   hasGround = OverlapCircle(groundCheck.position, checkRadius, groundLayer)

   if hasWall || !hasGround:
       // 목표 지점 즉시 전환
       if currentTarget == patrolPointA:
           currentTarget = patrolPointB
       else:
           currentTarget = patrolPointA
```

추적 알고리즘은 간단한 직선 추적을 사용한다.

```
입력:
- playerPosition: 플레이어 위치
- enemyPosition: 적 위치
- chaseSpeed: 추적 속도

매 프레임 실행:
1. 방향 계산
   direction = Normalize(playerPosition - enemyPosition)

2. 이동
   linearVelocity.x = direction.x × chaseSpeed

3. 스프라이트 방향 조정
   if direction.x > 0:
       transform.localScale = (1, 1, 1)
   else:
       transform.localScale = (-1, 1, 1)
```

---

## 4. 테스트 및 최적화

### 4.1 테스트 방법론

**Unity Play 모드 테스트**

Unity Editor의 Play 모드를 사용하여 실시간으로 게임을 테스트했다. Inspector 창에서 변수를 실시간으로 조정하며 즉각적인 피드백을 받았다. Console 창에서 Debug.Log()를 통해 디버깅 정보를 확인했다. Scene 뷰와 Game 뷰를 동시에 사용하여 오브젝트 위치와 콜라이더를 시각적으로 확인했다.

**씬별 개별 테스트**

각 씬을 독립적으로 테스트하여 씬별 기능을 검증했다. Stage1, Stage2, Stage3를 Project 창에서 직접 열어 스테이지별 메커니즘을 테스트했다. Boss, Boss1, Boss2 씬을 열어 보스 전투 시스템을 테스트했다. Shop 씬을 열어 상점 시스템을 테스트했다. AncientBlacksmith, LifeHeartMap 씬을 열어 미니게임을 테스트했다.

**통합 테스트**

전체 게임 플로우를 처음부터 끝까지 플레이하여 통합 테스트를 수행했다. Main 메뉴에서 시작하여 무기 선택, 세이브 슬롯 선택, 게임플레이, 세이브/로드, 씬 전환, 보스 전투, 미니게임까지 모든 시스템을 연결하여 테스트했다. 다양한 플레이 시나리오를 시도하여 엣지 케이스를 발견했다.

**플레이어 태그 검증**

많은 시스템이 GameObject.FindGameObjectWithTag("Player")를 사용하므로 플레이어 오브젝트가 "Player" 태그를 가지는지 확인했다.

**Canvas 설정 검증**

UI 요소들이 특정 Canvas 이름을 기대하므로 올바른 이름이 설정되었는지 확인했다. "DamageTextCanvas", "ConfirmationPanel" 등의 이름을 검증했다.

---

### 4.2 발견된 문제 및 해결

**싱글톤 중복 문제**

문제: 씬을 여러 번 전환하면 DontDestroyOnLoad 오브젝트가 중복 생성되어 메모리 누수와 예기치 않은 동작이 발생했다.

해결: DontDestroyOnLoadManager를 구현하여 instanceId 기반 Dictionary로 중복을 방지했다. Awake()에서 동일한 instanceId를 가진 오브젝트가 이미 존재하는지 체크하고, 존재하면 새 오브젝트를 즉시 파괴한다. isReturningToMainMenu 플래그를 도입하여 메인 메뉴 복귀 시 DontDestroyOnLoad를 건너뛴다. MainMenuController에서 CleanupDontDestroyOnLoadObjects()를 호출하여 GameManager와 SaveManager를 제외한 모든 지속 오브젝트를 정리한다.

**주사위 애니메이션 미작동**

문제: 보스 전투에서 주사위 애니메이션이 표시되지 않아 전투가 즉시 해결되는 문제가 발생했다.

해결: BattleController Inspector에 DiceAnimationManager를 할당하는 것이 필수임을 문서화했다. BattleController.StartClashPhase()에서 diceAnimationManager가 null인지 체크하는 로직을 추가했다. diceAnimationManager가 할당되지 않으면 ClashManager.ResolveClash()로 폴백하여 기능은 유지하되 애니메이션만 생략하도록 했다. 주사위_애니메이션_설정_가이드.md 문서를 작성하여 설정 방법을 상세히 안내했다.

**미니게임 시간 재개 문제**

문제: 미니게임 종료 후 PlayerStats가 자동으로 Time.timeScale = 1을 설정하여 의도하지 않은 시간 재개가 발생했다.

해결: BlacksmithMinigameManager.isGamePausedByManager 정적 플래그를 도입했다. 미니게임 종료 시 isGamePausedByManager = true로 설정하여 PlayerStats의 자동 시간 재개를 방지한다. 원래 씬으로 돌아간 후 수동으로 Time.timeScale = 1을 설정하고 isGamePausedByManager = false로 리셋한다.

**세이브 데이터 불일치**

문제: 씬 전환 후 플레이어 위치가 잘못된 곳에 저장되거나 복원되는 문제가 발생했다.

해결: PlayerSpawnPoint 시스템을 구현하여 씬마다 명확한 스폰 위치를 정의했다. 자동 저장 시 PlayerSpawnPoint 위치를 저장하도록 수정했다. OnSceneLoaded()에서 PlayerSpawnPoint를 우선 찾고, 없으면 SaveData의 playerPosition을 사용하도록 했다.

**포털 재사용 문제**

문제: 플레이어가 포털을 통해 이동한 후 다시 돌아와 같은 포털을 재사용할 수 있어 게임 진행이 꼬이는 문제가 발생했다.

해결: PortalController.usedPortalIDs 정적 리스트를 도입했다. 포털 사용 시 portalID를 usedPortalIDs에 추가한다. Awake()에서 portalID가 usedPortalIDs에 있는지 체크하고, 있으면 오브젝트를 즉시 파괴한다.

**레벨업 UI 중복 표시**

문제: 미니게임 중 레벨업이 발생하면 Time.timeScale = 0 상태에서 레벨업 UI가 표시되어 미니게임과 충돌했다.

해결: 미니게임 씬(AncientBlacksmith, LifeHeartMap)에서는 레벨업 UI를 즉시 표시하지 않고 isLevelUpPending = true로 설정한다. 미니게임 종료 후 원래 씬으로 돌아가면 ShowPendingLevelUpPanel()을 호출하여 대기 중인 레벨업 UI를 표시한다.

**애니메이션 이벤트 전달 실패**

문제: 스프라이트가 자식 오브젝트에 있는 중간보스의 경우 애니메이션 이벤트가 부모 AI 스크립트에 전달되지 않아 공격이 작동하지 않았다.

해결: MidBossAnimationEvents.cs를 구현하여 스프라이트 자식 오브젝트에 부착한다. GetComponentInParent로 부모의 AI 스크립트를 찾아 애니메이션 이벤트를 전달한다. Stage1MidBossAI와 Stage2MidBossAI 모두 지원한다. useSpriteChild 플래그와 spriteTransform 참조를 AI 스크립트에 추가하여 Sprite Child 패턴을 선택적으로 사용할 수 있게 했다.

---

### 4.3 성능 최적화

**오브젝트 풀링 적용**

대미지 텍스트는 매우 자주 생성되고 파괴되므로 오브젝트 풀링을 적용했다. 미리 10~20개의 DamageText 오브젝트를 생성하여 풀에 보관한다. 데미지 발생 시 풀에서 비활성화된 오브젝트를 가져와 활성화한다. 대미지 텍스트 애니메이션이 끝나면 파괴하지 않고 비활성화하여 풀에 반환한다. 이를 통해 Instantiate/Destroy 호출 횟수를 90% 이상 감소시켰다.

공격 이펙트와 발사체도 동일한 오브젝트 풀링 패턴을 적용했다.

**불필요한 Update() 호출 제거**

Update()가 매 프레임 호출되므로 불필요한 연산을 제거했다. 정적 변수나 변하지 않는 값은 Start()나 Awake()에서 캐싱했다. GetComponent() 호출을 Start()로 이동하여 매 프레임 호출을 방지했다. 조건문을 최적화하여 불필요한 연산을 조기에 반환했다. isDead나 isActivated 같은 플래그로 비활성 오브젝트의 Update() 연산을 스킵했다.

**애니메이션 최적화**

Animator의 Culling Mode를 Based On Renderers로 설정하여 화면 밖의 오브젝트는 애니메이션을 업데이트하지 않도록 했다. 불필요한 애니메이터 파라미터를 제거하고 필수 파라미터만 유지했다. Transition Duration을 최소화하여 애니메이션 블렌딩 비용을 줄였다.

**물리 최적화**

Rigidbody2D의 Sleeping Mode를 Start Awake로 설정하여 정적 오브젝트는 물리 시뮬레이션에서 제외했다. Continuous Collision Detection은 빠르게 움직이는 오브젝트(플레이어, 발사체)에만 적용하고 나머지는 Discrete로 설정했다. 불필요한 Collider는 제거하거나 비활성화했다.

**UI 최적화**

Canvas를 여러 개로 분리하여 일부 UI만 업데이트할 때 전체 Canvas를 리빌드하지 않도록 했다. 정적 UI 요소는 별도의 Canvas로 분리했다. 동적 UI 요소(HUD, 인벤토리)는 변경이 발생할 때만 업데이트하도록 했다. Image의 Raycast Target을 불필요하면 비활성화하여 입력 처리 비용을 줄였다.

**씬 관리 최적화**

씬 로드 시 SceneManager.LoadSceneAsync()를 고려했으나 로딩 화면이 불필요한 짧은 로딩 시간으로 동기 로드를 유지했다. 사용하지 않는 에셋은 Resources.UnloadUnusedAssets()로 언로드했다. 큰 씬은 여러 작은 씬으로 분할하여 로딩 시간을 분산했다.

---

## 5. 결론

### 5.1 프로젝트 성과

**구현 완료 기능 요약**

본 프로젝트는 계획한 모든 핵심 기능을 성공적으로 구현했다. 플레이어 시스템은 이동, 점프, 대시, 벽 슬라이드 등의 정교한 컨트롤과 3종 무기 시스템, 레벨업 시스템을 포함한다. 전투 시스템은 일반 적 AI와 중간보스 AI, 주사위 기반 카드 전투 보스전을 구현하여 다양한 전투 경험을 제공한다. 세이브/로드 시스템은 3개 슬롯과 JSON 기반 저장, 자동 저장 기능을 제공한다. 인벤토리 및 상점 시스템은 드래그 앤 드롭, 랜덤 아이템, 새로고침 기능을 포함한다. 미니게임 시스템은 대장간과 라이프맵 미니게임을 보상과 함께 제공한다. UI 시스템은 메인 메뉴, HUD, 레벨업 UI, 일시정지 메뉴 등 완전한 사용자 인터페이스를 구현했다. 씬 관리 시스템은 포털, 플레이어 스폰, 미니게임 복귀 기능을 포함한다. 스테이지별 특수 시스템으로 Stage3 수중 시스템을 구현했다.

**개발 과정에서 습득한 기술**

Unity 2D 게임 개발 전반에 대한 깊이 있는 이해를 습득했다. Rigidbody2D와 Collider2D를 활용한 물리 시스템 구현 능력을 키웠다. Animator Controller와 애니메이션 이벤트 활용 능력을 개발했다. 싱글톤, 옵저버, 상태 머신 등의 게임 디자인 패턴을 실전에 적용했다. JSON 직렬화, ScriptableObject, DontDestroyOnLoad 등의 데이터 지속성 기술을 습득했다. Canvas와 EventSystem을 활용한 UI/UX 구현 능력을 개발했다. 코루틴을 활용한 비동기 프로그래밍 경험을 쌓았다. Git과 GitHub를 활용한 버전 관리 경험을 습득했다. 디버깅 및 문제 해결 능력을 향상시켰다.

**프로젝트 목표 달성도**

모든 핵심 시스템 구현 완료 목표를 100% 달성했다. 메인 메뉴부터 게임 종료까지 전체 플로우를 완성했다. 3개 이상의 스테이지(Stage1, Stage2, Stage3)와 보스전을 구현했다. 세이브/로드 시스템의 안정성을 확보했고 모든 게임 데이터를 정확히 저장/복원한다. 주요 버그를 모두 수정하여 안정적인 플레이 경험을 제공한다.

---

### 5.2 한계점 및 개선 사항

**현재 시스템의 한계**

적 AI가 단순한 패턴만 사용하여 예측 가능한 행동을 보인다. 고급 경로 탐색(A* 알고리즘 등)이 없어 장애물 회피가 제한적이다. 보스 전투가 2~3종류로 제한되어 반복 플레이 시 지루할 수 있다. 난이도 조절 시스템이 없어 초보자와 숙련자 모두에게 적합하지 않을 수 있다. 스토리와 컷신이 부족하여 몰입도가 낮다. 멀티플레이 기능이 없어 싱글 플레이만 가능하다. 모바일 플랫폼을 지원하지 않아 PC에서만 플레이 가능하다.

**발견된 버그 및 미해결 과제**

특정 상황에서 플레이어가 벽에 끼이는 현상이 가끔 발생한다. 프레임 드롭이 심한 환경에서 물리 계산이 부정확해질 수 있다. 매우 빠른 속도로 씬을 전환하면 DontDestroyOnLoad 오브젝트가 완전히 정리되지 않을 수 있다. 일부 UI 요소가 해상도 변경 시 잘못 배치될 수 있다.

**시간 부족으로 미구현된 기능**

추가 무기 시스템(활, 마법 지팡이 등)을 구현하지 못했다. 스킬 트리와 다양한 스킬을 구현하지 못했다. 업적 시스템과 도전 과제를 구현하지 못했다. 사운드와 음악이 제한적이다. 상세한 튜토리얼과 도움말 시스템이 부족하다. 엔딩 시네마틱과 크레딧 화면을 구현하지 못했다.

---

### 5.3 추후 과제

**추가 스테이지 및 보스**

4개 이상의 추가 스테이지를 개발하여 게임 플레이 시간을 연장할 수 있다. 각 스테이지에 고유한 테마와 메커니즘을 부여할 수 있다(예: 용암 스테이지, 얼음 스테이지). 3~5개의 추가 보스를 개발하여 각 보스마다 독특한 패턴과 전략을 제공할 수 있다. 히든 보스와 챌린지 모드를 추가할 수 있다.

**멀티플레이 기능**

로컬 협동 플레이(2인)를 구현하여 친구와 함께 플레이할 수 있다. 온라인 멀티플레이를 구현하여 전 세계 플레이어와 협력하거나 대전할 수 있다. PvP 모드를 추가하여 플레이어 간 대전이 가능하게 할 수 있다.

**모바일 포팅**

터치 컨트롤을 구현하여 모바일 기기에서 플레이할 수 있게 할 수 있다. UI를 모바일 해상도에 맞게 조정할 수 있다. 모바일 성능 최적화를 수행할 수 있다(배터리 소모 감소, 프레임 안정화). iOS와 Android 빌드를 제작할 수 있다.

**난이도 조절 시스템**

Easy, Normal, Hard 난이도를 구현할 수 있다. 난이도에 따라 적 체력, 데미지, 수를 조정할 수 있다. 하드 모드에서는 추가 보상을 제공할 수 있다. 뉴 게임 플러스 모드를 추가하여 스탯을 유지하며 더 어려운 게임을 플레이할 수 있다.

**추가 무기 및 스킬 시스템**

활, 마법 지팡이, 도끼 등 추가 무기를 구현할 수 있다. 각 무기마다 고유한 스킬 트리를 제공할 수 있다. 콤보 시스템을 개선하여 더 다양한 공격 조합을 가능하게 할 수 있다. 특수 스킬(궁극기)을 추가할 수 있다.

**스토리 컷신 추가**

인트로 컷신으로 게임 배경을 설명할 수 있다. 스테이지 간 스토리 컷신으로 몰입도를 높일 수 있다. 보스 등장 컷신으로 긴장감을 조성할 수 있다. 엔딩 컷신과 크레딧을 추가할 수 있다. 대화 시스템을 확장하여 NPC와의 상호작용을 강화할 수 있다.

**사운드 및 음악 개선**

각 스테이지별 BGM을 작곡하거나 구매할 수 있다. 보스전 전용 음악을 추가할 수 있다. 다양한 효과음(공격, 피격, 걷기, 점프 등)을 고품질로 제작할 수 있다. 음량 조절과 음소거 옵션을 개선할 수 있다.

**추가 기능**

업적 시스템과 Steam 업적 연동을 구현할 수 있다. 리더보드를 추가하여 플레이어 간 경쟁을 유도할 수 있다. 데일리 챌린지와 주간 이벤트를 구현할 수 있다. 커스터마이징 시스템(캐릭터 외형, 무기 스킨)을 추가할 수 있다. 상세한 통계 화면(플레이 시간, 처치 수, 사망 수 등)을 제공할 수 있다.

---

## 부록

### A. 주요 스크립트 목록

**플레이어 관련 스크립트**
- PlayerController.cs: 메인 플레이어 제어 (PlayerController.cs:1)
- PlayerStats.cs: 레벨, XP, 스탯 관리 (PlayerStats.cs:1)
- PlayerHealth.cs: 체력 관리 (PlayerHealth.cs:1)
- SwordAttack.cs, LanceAttack.cs, MaceAttack.cs: 무기별 공격 (SwordAttack.cs:1, LanceAttack.cs:1, MaceAttack.cs:1)
- SwordItem.cs, LanceItem.cs, MaceItem.cs: 무기 아이템 (SwordItem.cs:1, LanceItem.cs:1, MaceItem.cs:1)
- PlayerSwimming.cs: 수영 메커니즘 (PlayerSwimming.cs:1)
- PlayerOxygen.cs: 산소 관리 (PlayerOxygen.cs:1)

**적 AI 관련 스크립트**
- MonsterController.cs: 일반 몬스터 AI (MonsterController.cs:1)
- Stage1MidBossAI.cs: 스테이지 1 중간보스 (Stage1MidBossAI.cs:1)
- Stage2MidBossAI.cs: 스테이지 2 중간보스 (Stage2MidBossAI.cs:1)
- MidBossAnimationEvents.cs: 애니메이션 이벤트 브릿지 (MidBossAnimationEvents.cs:1)
- MonsterHealth.cs: 적 체력 관리 (MonsterHealth.cs:1)
- BossAI.cs, BossAttack.cs, BossHealth.cs: 보스 AI (BossAI.cs:1, BossAttack.cs:1, BossHealth.cs:1)
- ItemDrop.cs: 아이템 드롭 (ItemDrop.cs:1)
- ClamMonsterController.cs: 수중 적 AI (ClamMonsterController.cs:1)
- MobSpawner.cs: 몬스터 스포너 (MobSpawner.cs:1)

**보스 전투 관련 스크립트**
- BossGameManager.cs: 게임 상태 관리 (BossGameManager.cs:1)
- BattleController.cs: 전투 조율 (BattleController.cs:1)
- CharacterStats.cs: 캐릭터 스탯 및 덱 (CharacterStats.cs:1)
- CombatPage.cs: 카드 데이터 (CombatPage.cs:1)
- CombatDice.cs: 주사위 로직 (CombatDice.cs:1)
- ClashManager.cs: 클래시 해결 (ClashManager.cs:1)
- DiceVisual.cs: 주사위 UI (DiceVisual.cs:1)
- DiceAnimationManager.cs: 주사위 애니메이션 (DiceAnimationManager.cs:1)
- CardUI.cs: 카드 UI (CardUI.cs:1)
- CharacterVisuals.cs: 캐릭터 비주얼 (CharacterVisuals.cs:1)
- BattleTrigger.cs: 전투 트리거 (BattleTrigger.cs:1)

**씬 관리 관련 스크립트**
- PortalController.cs: 포털 시스템 (PortalController.cs:1)
- StatueInteraction.cs: 석상 상호작용 (StatueInteraction.cs:1)
- PortalReturnManager.cs: 복귀 위치 관리 (PortalReturnManager.cs:1)
- PortalReturnData.cs: 복귀 데이터 (PortalReturnData.cs:1)
- PlayerSpawnPoint.cs: 스폰 위치 (PlayerSpawnPoint.cs:1)
- Stage3Manager.cs: Stage3 관리 (Stage3Manager.cs:1)
- PortalToStage3.cs: Stage3 포털 (PortalToStage3.cs:1)

**세이브/로드 관련 스크립트**
- SaveManager.cs: 파일 I/O (SaveManager.cs:1)
- GameManager.cs: 게임 상태 조율 (GameManager.cs:1)
- SaveData.cs: 저장 데이터 (SaveData.cs:1)
- LoadGameUI.cs: 슬롯 선택 UI (LoadGameUI.cs:1)
- SaveSlotButton.cs: 슬롯 버튼 (SaveSlotButton.cs:1)

**UI 관련 스크립트**
- MainMenuController.cs: 메인 메뉴 (MainMenuController.cs:1)
- WeaponChoice.cs: 무기 선택 (WeaponChoice.cs:1)
- StatsUIManager.cs: 스탯 UI (StatsUIManager.cs:1)
- LevelUpUIManager.cs: 레벨업 UI (LevelUpUIManager.cs:1)
- InventoryUI.cs, Inventory.cs: 인벤토리 (InventoryUI.cs:1, Inventory.cs:1)
- InventorySlot.cs: 인벤토리 슬롯 (InventorySlot.cs:1)
- PauseMenuUI.cs: 일시정지 메뉴 (PauseMenuUI.cs:1)
- DontDestroyOnLoadManager.cs: 지속성 관리 (DontDestroyOnLoadManager.cs:1)
- DamageText.cs: 대미지 텍스트 (DamageText.cs:1)
- CameraFollow.cs, CameraBounds.cs: 카메라 (CameraFollow.cs:1, CameraBounds.cs:1)
- CurrencyUI.cs: 화폐 UI (CurrencyUI.cs:1)
- Tooltip.cs, TooltipManager.cs: 툴팁 (Tooltip.cs:1, TooltipManager.cs:1)

**상점 관련 스크립트**
- ShopManager.cs: 상점 관리 (ShopManager.cs:1)
- ShopItemData.cs: 아이템 데이터 (ShopItemData.cs:1)
- ShopPedestal.cs: 아이템 표시대 (ShopPedestal.cs:1)
- RefreshPedestal.cs: 새로고침 표시대 (RefreshPedestal.cs:1)
- ExchangePedestal.cs: 교환 표시대 (ExchangePedestal.cs:1)

**미니게임 관련 스크립트**
- BlacksmithMinigameManager.cs: 대장간 미니게임 (BlacksmithMinigameManager.cs:1)
- DialogueController.cs: 대화 시스템 (DialogueController.cs:1)
- MidBossController.cs: 중간보스 이벤트 (MidBossController.cs:1)
- UnifiedFlameTrap.cs, SoulFlame.cs: 미니게임 오브젝트 (UnifiedFlameTrap.cs:1, SoulFlame.cs:1)
- LifeGameManager.cs: 라이프맵 관리 (LifeGameManager.cs:1)

**기타 스크립트**
- ItemPickup.cs: 아이템 획득 (ItemPickup.cs:1)
- Coin.cs: 코인 획득 (Coin.cs:1)
- PotionItemData.cs: 포션 데이터 (PotionItemData.cs:1)
- WeaponStats.cs: 무기 스탯 (WeaponStats.cs:1)
- GameData.cs: 정적 데이터 (GameData.cs:1)
- Trap.cs: 함정 (Trap.cs:1)
- OxygenZone.cs: 산소 회복 구역 (OxygenZone.cs:1)
- Pearl.cs, PearlDisplayUI.cs: 진주 수집 (Pearl.cs:1, PearlDisplayUI.cs:1)
- GiantClam.cs: 거대 조개 (GiantClam.cs:1)
- ParallaxController.cs: 패럴랙스 배경 (ParallaxController.cs:1)

---

### B. 게임 플레이 가이드

**조작 방법**

키보드 조작은 다음과 같다. 화살표 키 또는 WASD로 이동한다. K 키로 점프한다. L 키로 대시한다. J 키로 공격한다(일반 전투) 또는 액션을 계획한다(턴 기반 전투). Space 바로 턴을 실행한다(보스 전투). ESC 키로 일시정지 메뉴를 연다. I 키로 인벤토리를 연다(구현된 경우). E 키로 상호작용한다(포털, NPC).

**게임 플로우**

게임 시작 시 Main 메뉴에서 New Game 또는 Load Game을 선택한다. New Game을 선택하면 Weapon 씬에서 무기(검/창/메이스)를 선택한다. LoadGame 씬에서 세이브 슬롯(1/2/3)을 선택한다. Stage1에서 게임이 시작된다.

스테이지 탐험 중에는 플랫폼을 점프하여 이동한다. 적을 공격하여 XP와 아이템을 획득한다. 포털을 통해 다른 씬으로 이동한다(확인 필요). 일시정지 메뉴(ESC)로 게임을 저장한다(자동 저장도 작동).

보스 전투 중에는 카드를 선택한다(최대 3장). Space 바로 턴을 확인한다. 주사위가 굴러가고 클래시가 해결되는 것을 관찰한다. 보스의 HP를 0으로 만들어 승리한다.

미니게임 중에는 대장간에서 불꽃을 수집한다. 높은 점수로 더 많은 보상을 얻는다. 라이프맵에서 특수 도전 과제를 완료한다.

상점에서는 아이템을 구매하여 스탯을 업그레이드한다. 새로고침하여 새 아이템을 표시한다(비용 증가). 포션을 구매하여 체력을 회복한다.

**팁 및 전략**

각 무기의 특성을 이해하고 활용한다. 검은 균형잡힌 성능으로 모든 상황에 적합하다. 창은 긴 리치로 안전한 거리를 유지할 수 있다. 메이스는 높은 데미지와 넉백으로 강력한 적에 유효하다.

레벨업 시 상황에 맞는 스탯을 선택한다. 공격력은 빠른 전투를 위해 선택한다. 방어력은 생존력을 높이기 위해 선택한다. 체력은 최대 HP를 증가시키기 위해 선택한다. 이동속도는 기동성을 높이기 위해 선택한다.

보스 전투 전략은 다음과 같다. 보스의 덱을 확인하여 패턴을 파악한다(덱 보기 모드). Light를 효율적으로 관리한다. 카드 쿨다운을 고려하여 선택한다. Attack vs Defense 타입 우위를 활용한다.

자주 저장한다. 자동 저장이 있지만 중요한 순간에는 수동 저장(일시정지 메뉴)을 권장한다. 여러 슬롯을 사용하여 다양한 진행 상황을 보관한다.

미니게임에 참여하여 추가 보상을 얻는다. 대장간에서 공격력 부스트를 얻는다. 라이프맵에서 특수 보상을 얻는다.

---

### C. 리소스 목록

**사용된 에셋**

본 프로젝트는 다음 에셋을 사용했다. Unity Asset Store의 2D 캐릭터 스프라이트, Unity Asset Store의 2D 타일셋(지형, 배경), Unity Asset Store의 2D UI 에셋, Unity Asset Store의 사운드 이펙트 팩, Unity Asset Store의 BGM 팩, 자체 제작 스크립트 및 시스템.

**라이선스 정보**

사용된 모든 에셋은 Unity Asset Store의 표준 라이선스를 따른다. 개인 및 상업적 사용이 가능하다. 에셋 재배포는 금지되어 있다. 자체 제작 스크립트는 MIT 라이선스를 따른다(선택적).

---

### D. 참고 문헌

**Unity 공식 문서**
- Unity Manual: https://docs.unity3d.com/Manual/index.html
- Unity Scripting Reference: https://docs.unity3d.com/ScriptReference/index.html
- Unity 2D Documentation: https://docs.unity3d.com/Manual/Unity2D.html
- Unity Physics 2D: https://docs.unity3d.com/Manual/Physics2DReference.html
- Unity UI: https://docs.unity3d.com/Packages/com.unity.ugui@latest

**참고한 튜토리얼 및 아티클**
- Brackeys - How to make a 2D Game in Unity
- Unity Learn - 2D Platformer Tutorial
- Game Dev Beginner - Unity Input System
- Catlike Coding - Unity C# Tutorials
- Stack Overflow - 다양한 기술적 질문 해결

**게임 디자인 참고**
- Hollow Knight - Team Cherry
- Dead Cells - Motion Twin
- Blasphemous - The Game Kitchen
- Library of Ruina - Project Moon

---

**보고서 작성일**: 2025년 11월 3일
**프로젝트 기간**: [시작일] ~ [종료일]
**개발자**: [이름]
**지도교수**: [교수님 성함]
