# 2D Unity Action RPG Game

Unity 6로 제작된 2D 액션 RPG 게임 프로젝트입니다.

## 프로젝트 정보

- **엔진**: Unity 6000.0.41f1 (Unity 6)
- **장르**: 2D 액션 RPG / 플랫포머
- **플랫폼**: Windows
- **개발 언어**: C#

## 주요 기능

### 플레이어 시스템
- 3가지 무기 타입 (검, 창, 메이스) - 각각 고유한 능력치와 공격 패턴
- 고급 이동 메커니즘 (벽 슬라이드, 대시, 점프)
- 레벨업 시스템과 능력치 관리
- 체력, 방어력, 공격력 시스템

### 전투 시스템
- 일반 몬스터 전투 (실시간 액션)
- 중간 보스 전투 (스테이지별 고유 패턴)
- 최종 보스 전투 (카드 기반 턴제 시스템)
- 주사위 메커니즘을 활용한 전략적 전투

### 게임 컨텐츠
- 다중 스테이지 (Stage1, Stage2, Stage3 + 보스 스테이지)
- 상점 시스템 (아이템 구매 및 능력치 강화)
- 미니게임 (대장간, 라이프맵)
- 인벤토리 및 아이템 시스템
- 세이브/로드 시스템 (3개 슬롯)

## 조작법

- **화살표 키 / WASD**: 이동
- **K**: 점프
- **L**: 대시
- **J**: 공격
- **Space**: 턴 확인 (보스 전투)
- **ESC**: 일시정지 메뉴

## 프로젝트 구조

```
Assets/
├── Scripts/
│   ├── Player/          # 플레이어 관련 스크립트
│   ├── Monster/         # 적 AI 및 보스 시스템
│   ├── UI/              # UI 및 인벤토리 시스템
│   ├── BossBattle/      # 보스 전투 주사위 시스템
│   ├── SaveSystem/      # 세이브/로드 시스템
│   ├── Shop/            # 상점 시스템
│   ├── Stage3/          # 수중 스테이지 시스템
│   └── CardSystem/      # 카드 메커니즘
├── Ancient/             # 대장간 미니게임
├── Life/                # 라이프맵 미니게임
└── [Scene Files]        # Unity 씬 파일들
```

## 개발 환경 설정

### 요구사항
- Unity Hub
- Unity 6000.0.41f1 이상

### 프로젝트 열기
1. Unity Hub를 실행합니다
2. 프로젝트 추가 → 이 저장소 폴더 선택
3. Unity 6000.0.41f1로 프로젝트를 엽니다
4. Play 모드로 테스트합니다

## 게임 플로우

```
Main (메인 메뉴)
  ↓
Weapon (무기 선택)
  ↓
LoadGame (슬롯 선택)
  ↓
Stage1 → Stage2 → Stage3 → Boss Stages
  ↓
Shop / Minigames (선택적)
```

## 주요 싱글톤 시스템

- `PlayerController.Instance` - 플레이어 제어
- `GameManager.Instance` - 게임 상태 및 세이브/로드
- `SaveManager.Instance` - 파일 I/O
- `Inventory.instance` - 인벤토리 관리
- `ShopManager.Instance` - 상점 시스템

## Git 브랜치 전략

- `main` - 메인 브랜치 (안정 버전)
- `develop` - 통합 개발 브랜치
- `kim` - 개발 브랜치
- `feature/*` - 기능별 개발 브랜치

## 라이선스

이 프로젝트는 교육 목적으로 제작되었습니다.

---

## Project Information (English)

A 2D Action RPG game built with Unity 6.

### Key Features
- Three weapon types with unique stats and attack patterns
- Advanced movement mechanics (wall sliding, dashing)
- Level-up system with stat management
- Turn-based boss battles with dice mechanics
- Multi-stage progression with mini-games
- Shop and inventory systems
- Save/Load system with 3 slots

### Controls
- **Arrow Keys / WASD**: Movement
- **K**: Jump
- **L**: Dash
- **J**: Attack
- **Space**: Confirm turn (boss battle)
- **ESC**: Pause menu

### Development
- Engine: Unity 6000.0.41f1
- Platform: Windows
- Language: C#
