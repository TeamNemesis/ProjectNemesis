# ProjectNemesis 아키텍처 상세 분석

## 📐 시스템 아키텍처 다이어그램

### 핵심 시스템 관계도

```
                            GameManager (Singleton)
                                    |
        +---------------------------+---------------------------+
        |                           |                           |
   ResourceManager            DataManager                PoolManager
        |                           |                           |
        |                           |                           |
   SkillManager               PlayerStatManager          UIManager
        |                           |                           |
        +---------------------------+---------------------------+
                                    |
                            InteractableManager
                                    |
                        CurrencyManager
```

---

## 🎮 게임플레이 시스템 흐름

### 1. 플레이어 시스템 아키텍처

```
Player (Main Controller)
    |
    +-- PlayerModel (Data)
    +-- PlayerView (Presentation)
    +-- CharacterController (Physics)
    |
    +-- Movement Subsystem
    |   +-- PlayerMover
    |   +-- PlayerDasher
    |
    +-- Combat Subsystem
    |   +-- PlayerWeaponController
    |   |   +-- PlayerWeaponSet (Current)
    |   |       +-- Rifle
    |   |       +-- Blade
    |   |       +-- HackingDevice
    |   |
    |   +-- PlayerNormalAttacker[]
    |   +-- PlayerSpecialAttacker[]
    |   +-- PlayerGrenadeAttacker
    |
    +-- Animation Subsystem
    |   +-- PlayerAnimator
    |   +-- PlayerAnimationEventForwarder
    |
    +-- Interaction Subsystem
    |   +-- InteractionController
    |   +-- InteractableDetector
    |   +-- InteractionGuideView
    |
    +-- State Machine
        +-- PlayerIdleState
        +-- PlayerMoveState
        +-- PlayerDashState
        +-- PlayerNormalAttackState
        +-- PlayerSpecialAttackState
        +-- PlayerGrenadeAttackState
```

**설계 패턴**:
- **State Pattern**: 플레이어 상태 관리
- **Strategy Pattern**: 무기별 공격 전략
- **Observer Pattern**: 애니메이션 이벤트
- **Component Pattern**: 기능별 분리

**강점**:
- 높은 응집도, 낮은 결합도
- 각 컴포넌트의 책임이 명확
- 새로운 무기/상태 추가가 용이

**개선 여지**:
```csharp
// 현재: 배열로 관리
[SerializeField] PlayerNormalAttacker[] _normalAttackers;

// 제안: Dictionary로 O(1) 접근
Dictionary<WeaponType, PlayerNormalAttacker> _normalAttackerMap;
// 이미 구현되어 있음! 👍
```

---

### 2. 몬스터 시스템 아키텍처

```
MonsterBase (Abstract Base)
    |
    +-- CharacterModelBase (Health, Damage)
    +-- IInitializePoolable (Pooling Interface)
    |
    +-- NavMeshAgent (AI Navigation)
    +-- State Machine (Idle, Move, Attack, Die)
    +-- DebuffHandler (Status Effects)
    |
    +-- Normal Monsters
    |   +-- NebulaVanguard
    |   +-- NebulaPhantom
    |   +-- BallSecurityRobot
    |   +-- AutoTurret
    |   +-- SecurityDogEModel
    |   +-- NebulaChemicalDisease
    |
    +-- Elite Monsters
    |   +-- Elite1
    |   +-- Elite2
    |   +-- Elite3
    |
    +-- Boss
        +-- Omega_X7
            +-- LaserTurret (Add-on)
            +-- MonsterGrenade (Add-on)
            +-- fanShapeDecal (Add-on)
            +-- ShotgunDecalEffect (Add-on)
```

**설계 패턴**:
- **Template Method Pattern**: MonsterBase의 가상 메서드
- **Object Pool Pattern**: 몬스터 재사용
- **Composite Pattern**: 보스 몬스터 + Add-ons

**메모리 최적화**:
```csharp
public class MonsterBase : IInitializePoolable
{
    // ✅ Object Pooling으로 GC 압력 감소
    public void OnSpawnedFromPool() { }
    public void OnReturnedToPool() { }
}
```

---

### 3. 스킬 시스템 아키텍처

```
SkillManager
    |
    +-- SkillBase (Abstract)
        |
        +-- Skill_One (Poison Theme)
        |   +-- GrenadePoison
        |   +-- PoisonDash
        |   +-- PoisonSpread
        |
        +-- Skill_Two (Explosion Theme)
        |   +-- ExplosionDeath
        |   +-- DashReinforcePrefab
        |
        +-- Skill_Three (Vortex Theme)
        |   +-- GrenadeVortex
        |   +-- KnockBackDash
        |   +-- RedShift
        |
        +-- Skill_Four (Tech Theme)
        |   +-- Drone
        |   +-- GrenadeEMP
        |
        +-- Skill_Five (미확인)
        |
        +-- Skill_Mutant (돌연변이)
        |
        +-- Skill_Collab (협력기)
            +-- GravityFlareRocket
            +-- GravityFlareRocketExplosion
```

**데이터 관리**:
```
JSON Files (Resources)
    ↓
SkillBase.ReadJsonFile()
    ↓
List<skillJsonData>
    ↓
SkillData Objects
    ↓
Player Skill Selection
```

**설계 패턴**:
- **Strategy Pattern**: 스킬별 행동
- **Data-Driven Design**: JSON 기반 설정
- **Factory Pattern**: 스킬 생성

**개선 제안**:
```csharp
// 현재
[SerializeField] private string _skillDataPath;
TextAsset jsonFile = Resources.Load<TextAsset>(_skillDataPath);

// 제안: ScriptableObject 활용
[SerializeField] private SkillDataAsset _skillDataAsset;
var skillData = _skillDataAsset.Skills;
```

---

### 4. 맵 생성 시스템 아키텍처

```
MapController (Orchestrator)
    |
    +-- RoomSpawner
    |   +-- Room (Abstract)
    |       +-- StartRoom
    |       +-- NormalRoom
    |       +-- ShopRoom
    |       +-- LabRoom
    |       +-- ColosseumRoom
    |       +-- BossRoom
    |
    +-- DoorSpawner
    |   +-- Door
    |   +-- DoorView (Presentation)
    |   +-- DoorInteractor (Logic)
    |
    +-- DoorDecider
    |   +-- RoomInfo (Data)
    |   +-- RoomDataSO (ScriptableObject)
    |
    +-- MonsterController
        +-- MonsterSpawner
        +-- ObjectRandomSpawn
        +-- RandomSpawnPoints
```

**이벤트 흐름**:
```
1. DoorInteracted
    ↓
2. MapController.OnDoorInteracted()
    ↓
3. DestroyCurrentRoomObjects()
    ↓
4. RoomSpawner.SpawnRoom(RoomInfo)
    ↓
5. MapController.OnRoomSpawned(Room)
    ↓
6. MonsterController.SpawnMonsters()
    ↓
7. MapController.OnRoomStart Event
```

**설계 패턴**:
- **Observer Pattern**: 이벤트 기반 통신
- **Factory Pattern**: 룸 생성
- **Strategy Pattern**: 룸 타입별 로직
- **Facade Pattern**: MapController가 복잡도 숨김

**코드 품질**:
```csharp
// ✅ 방어적 프로그래밍
void OnRoomSpawned(Room room)
{
    if (room == null)
    {
        Debug.LogError("OnRoomSpawned called with null room");
        return;
    }
    // ...
}

// ✅ 명확한 책임 분리
// - RoomSpawner: 룸 인스턴스화만 담당
// - DoorSpawner: 문 생성만 담당
// - DoorDecider: 다음 룸 결정만 담당
// - MapController: 전체 오케스트레이션
```

---

### 5. 상호작용 시스템 아키텍처

```
IInteractable (Interface)
    |
    +-- InteractableObject (Base)
        |
        +-- RewardInteractableObject
        |   +-- ChromeInteractor
        |   +-- CreditInteractor
        |   +-- HealPackInteractor
        |   +-- MutantPackInteractor
        |   +-- TechSelectPackInteractor
        |   +-- TechUpgradePackInteractor
        |
        +-- DoorInteractor
        |
        +-- WeaponInteractor
        |
        +-- IShopItem (Interface)
            +-- HealPackInteractor_Shop
            +-- MutantPackInteractor_Shop
            +-- TechSelectPackInteractor_Shop
            +-- TechUpgradePackInteractor_Shop
```

**상호작용 흐름**:
```
Player
    |
    +-- InteractableDetector
        ↓ (Detects)
    IInteractable Object
        ↓ (Notifies)
    InteractionController
        ↓ (Shows)
    InteractionGuideView (UI)
        ↓ (Player Input)
    IInteractable.Interact()
        ↓
    Game Logic (Reward, Purchase, etc.)
```

**설계 패턴**:
- **Interface Segregation**: IInteractable, IShopItem
- **Decorator Pattern**: Shop 변형
- **Observer Pattern**: 감지 → 통지

---

## 🔄 데이터 흐름 분석

### 게임 시작 시퀀스

```
1. Unity Scene Load
    ↓
2. GameManager.Awake()
    |-- Singleton 초기화
    |-- DontDestroyOnLoad 설정
    ↓
3. GameManager.Initialize()
    |-- ResourceManager.Initialize()
    |-- DataManager.Initialize()
    |-- SkillManager.Initialize()
    |-- UIManager.Initialize()
    |-- CurrencyManager.Initialize()
    |-- PlayerStatManager.Initialize()
    |-- PoolManager.Initialize()
    ↓
4. MapController.Initialize(Player)
    |-- RoomSpawner.Initialize()
    |-- DoorDecider.Initialize()
    |-- MonsterController.Initialize()
    |-- 이벤트 구독
    ↓
5. StartRoom 생성
    ↓
6. Player 스폰
    ↓
7. 게임 시작
```

**의존성 그래프**:
```
GameManager
    ↓
ResourceManager ──→ PoolManager
    ↓                   ↓
DataManager ────→ SkillManager
    ↓                   ↓
PlayerStatManager   UIManager
```

**개선 제안**:
```csharp
// 현재: 하드코딩된 초기화 순서
void Initialize()
{
    _resourceManager.Initialize();
    _dataManager.Initialize(_resourceManager);  // 의존성 명시
    // ...
}

// 제안: Dependency Injection Container 활용
// - Zenject/VContainer 같은 DI 프레임워크 도입
// - 명시적 의존성 관리
// - 테스트 용이성 향상
```

---

## 🎨 UI 아키텍처 (추정)

```
UIManager
    |
    +-- Canvas
        +-- HUD
        |   +-- HealthBar
        |   +-- SkillCooldown UI
        |   +-- Currency Display
        |
        +-- Interaction UI
        |   +-- InteractionGuideView
        |
        +-- Skill Selection UI
        |   +-- SkillChoose
        |   +-- SkillBtn
        |
        +-- Shop UI
            +-- Shop Items List
            +-- Purchase Confirmation
```

**설계 패턴 권장**:
- **MVC/MVP Pattern**: UI 로직 분리
- **Command Pattern**: UI 액션 처리
- **Object Pool**: UI 요소 재사용

---

## 📊 성능 최적화 전략

### 현재 구현된 최적화

1. **Object Pooling**
   ```csharp
   IInitializePoolable
   - 몬스터 재사용
   - 투사체 재사용
   - 이펙트 재사용
   ```

2. **NavMesh 활용**
   ```csharp
   NavMeshAgent
   - 효율적인 AI 경로 찾기
   - CPU 부하 감소
   ```

3. **이벤트 기반 통신**
   ```csharp
   EventBus
   - Update() 폴링 대신 이벤트
   - 불필요한 체크 제거
   ```

### 추가 최적화 제안

1. **LOD (Level of Detail)**
   ```csharp
   - 거리에 따른 몬스터 디테일 조절
   - 파티클 효과 LOD
   ```

2. **Culling 최적화**
   ```csharp
   - Occlusion Culling
   - Frustum Culling
   ```

3. **Physics 최적화**
   ```csharp
   - Fixed Timestep 조정
   - Collision Matrix 최적화
   ```

---

## 🧩 확장성 분석

### 새 기능 추가 시나리오

#### 1. 새로운 무기 추가
```csharp
✅ Easy:
1. WeaponType enum에 새 타입 추가
2. PlayerXXXAttacker 클래스 생성 (상속)
3. PlayerWeaponSet에 등록
4. 프리팹 생성

예상 시간: 2-4시간
```

#### 2. 새로운 몬스터 추가
```csharp
✅ Easy:
1. MonsterBase 상속 클래스 생성
2. 공격 패턴 구현 (가상 메서드 오버라이드)
3. 프리팹 생성
4. Pool에 등록

예상 시간: 3-6시간
```

#### 3. 새로운 스킬 추가
```csharp
⚠️ Medium:
1. SkillBase 상속 클래스 생성
2. JSON 데이터 작성
3. 스킬 오브젝트 구현
4. UI에 추가

예상 시간: 4-8시간
```

#### 4. 새로운 룸 타입 추가
```csharp
✅ Easy:
1. RoomType enum에 추가
2. Room 상속 클래스 생성
3. RoomDataSO 생성
4. DoorDecider 로직 업데이트

예상 시간: 2-4시간
```

---

## 🔐 보안 및 치팅 방지

### 현재 상태
```
⚠️ 클라이언트 측 데이터 관리
- 스탯, 화폐 등이 클라이언트에서 관리
- 멀티플레이어 시 취약점 가능성
```

### 권장사항 (멀티플레이어 계획 시)
```csharp
1. 서버 권한 검증
   - 중요 데이터는 서버에서 관리
   - 클라이언트는 예측만

2. 암호화
   - PlayerPrefs 대신 암호화된 저장소
   - 세이브 데이터 검증

3. 안티치트
   - 메모리 해킹 탐지
   - 비정상적인 값 체크
```

---

## 📖 문서화 현황

### 현재
- 일부 클래스에 한글 주석
- XML 문서화 주석 부족
- README 기본 정보만

### 개선 계획
```markdown
1. 각 시스템별 README
   - Assets/02_Scripts/Player/README.md
   - Assets/02_Scripts/Monster/README.md
   - Assets/02_Scripts/Skill/README.md

2. API 문서 자동 생성
   - DocFX 또는 Doxygen 활용

3. 아키텍처 다이어그램
   - PlantUML 또는 Mermaid
   - Git에 소스 커밋
```

---

## 🎓 학습 리소스

팀원들을 위한 추천 학습 자료:

### Unity 디자인 패턴
- [Unity Design Patterns](https://github.com/Habrador/Unity-Programming-Patterns)
- [Game Programming Patterns](https://gameprogrammingpatterns.com/)

### Clean Code
- [C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [Unity Best Practices](https://unity.com/how-to/best-practices-organizing-projects)

### 성능 최적화
- [Unity Optimization Tips](https://docs.unity3d.com/Manual/BestPracticeUnderstandingPerformanceInUnity.html)

---

**작성일**: 2025-10-31  
**작성자**: Copilot Architecture Analysis Agent
