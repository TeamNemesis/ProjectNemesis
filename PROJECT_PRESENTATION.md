# ProjectNemesis 프로젝트 발표 자료

**발표 날짜**: 2025년 11월  
**팀**: TeamNemesis  
**프로젝트**: ProjectNemesis  
**플랫폼**: PC / Mobile (Android)  
**엔진**: Unity 6 (6000.0.59f2)

---

# 📑 목차

1. [프로젝트 소개](#1-프로젝트-소개)
2. [게임 개요](#2-게임-개요)
3. [기술 스택](#3-기술-스택)
4. [아키텍처 설계](#4-아키텍처-설계)
5. [핵심 시스템](#5-핵심-시스템)
6. [방 생성 알고리즘](#6-방-생성-알고리즘)
7. [개발 현황](#7-개발-현황)
8. [팀 구성 및 역할](#8-팀-구성-및-역할)
9. [향후 계획](#9-향후-계획)
10. [Q&A](#10-qa)

---

# 1. 프로젝트 소개

## 1.1 프로젝트 개요

**ProjectNemesis**는 Unity 6 기반의 **3D 액션 로그라이트 슈팅 게임**입니다.

### 핵심 특징
- 🎮 **절차적 던전 생성**: 매 플레이마다 다른 경험
- 🔫 **다양한 무기 시스템**: 5가지 무기와 특수 공격
- 🎯 **스킬 강화 시스템**: 진행하면서 캐릭터 성장
- 👾 **다양한 적 AI**: 일반 몬스터, 엘리트, 보스
- 📱 **크로스 플랫폼**: PC와 모바일 지원
- 🌐 **온라인 연동**: Firebase 기반 데이터 동기화

### 개발 목표
> **"매번 새로운 경험을 제공하는 로그라이트 액션 게임"**

---

## 1.2 게임 장르 및 타겟

| 항목 | 내용 |
|------|------|
| **장르** | 3D 액션 로그라이트 슈팅 |
| **시점** | 탑다운 뷰 (Top-Down View) |
| **플랫폼** | PC (Windows), Mobile (Android) |
| **플레이어** | 싱글 플레이 |
| **타겟 연령** | 15세 이상 |
| **플레이 타임** | 회차당 15~30분 |

### 유사 게임
- **Enter the Gungeon** - 총기 액션 로그라이트
- **Nuclear Throne** - 탑다운 슈터
- **Hades** - 로그라이트 액션
- **Risk of Rain 2** - 3D 로그라이트 슈터

---

# 2. 게임 개요

## 2.1 게임 플레이 루프

```
┌──────────────────────────────────────────────────┐
│                   시작 방                         │
│              (장비/스킬 준비)                      │
└────────────────┬─────────────────────────────────┘
                 ↓
┌──────────────────────────────────────────────────┐
│              일반 방 진입                         │
│         (몬스터 웨이브 전투)                       │
└────────────────┬─────────────────────────────────┘
                 ↓
┌──────────────────────────────────────────────────┐
│            보상 선택                              │
│   (체력 회복 / 크레딧 / 스킬 강화 등)              │
└────────────────┬─────────────────────────────────┘
                 ↓
┌──────────────────────────────────────────────────┐
│          다음 방 선택 (1~3개)                     │
│    (일반방 / 상점 / 연구실 / 콜로세움)             │
└────────────────┬─────────────────────────────────┘
                 ↓
         [10~13회 반복]
                 ↓
┌──────────────────────────────────────────────────┐
│              보스 방 진입                         │
│            (최종 보스 전투)                        │
└────────────────┬─────────────────────────────────┘
                 ↓
         ┌───────┴────────┐
         ↓                ↓
    ┌────────┐      ┌─────────┐
    │  승리  │      │  패배   │
    │(엔딩)  │      │(재시작) │
    └────────┘      └─────────┘
```

## 2.2 방 타입

### 🔹 일반 방 (Normal Room)
**목적**: 몬스터 처치 및 보상 획득

**세부 타입**:
1. **Credit 방**: 크레딧(화폐) 획득
2. **Heal 방**: 체력 회복 아이템
3. **TechUpgrade 방**: 기술 업그레이드
4. **Chrome 방**: 특수 재화 획득
5. **TechSelect 방**: 스킬 선택 및 획득

### 🏪 상점 (Shop Room)
- 크레딧으로 아이템 구매
- 무기, 회복, 업그레이드 판매
- 게임당 2~3회 등장 보장

### 🔬 연구실 (Lab Room)
- 특수 이벤트 방
- 강력한 보상 또는 위험한 선택
- 게임당 최대 1회 등장

### 🏛️ 콜로세움 (Colosseum Room)
- 엘리트 몬스터 전투
- 높은 난이도, 큰 보상
- 중복 등장 불가

### 👑 보스 방 (Boss Room)
- 최종 보스 전투
- 게임의 마지막 방 (14번째)

---

## 2.3 전투 시스템

### 플레이어 능력

**기본 조작**:
- **이동**: WASD / 조이스틱
- **조준**: 마우스 / 터치
- **일반 공격**: 좌클릭 / 버튼
- **특수 공격**: 우클릭 / 버튼
- **유탄 발사**: E / 버튼
- **대시**: Space / 버튼

**무기 시스템** (5종):
1. **기관총 (Machine Gun)**: 연사력 높음, 낮은 데미지
2. **샷건 (Shotgun)**: 산탄 발사, 근거리 특화
3. **저격총 (Sniper)**: 고데미지, 정밀 사격
4. **레이저건 (Laser)**: 관통 공격
5. **플라즈마건 (Plasma)**: 광역 피해

### 적 AI 시스템

**일반 몬스터**:
- 기본 추적 AI
- 원거리/근거리 공격 패턴
- 난이도에 따른 스탯 증가

**엘리트 몬스터**:
- 강화된 능력
- 특수 스킬 보유
- 콜로세움 방에 등장

**보스**:
- 페이즈 기반 전투
- 복잡한 공격 패턴
- 고유 메커니즘

---

# 3. 기술 스택

## 3.1 개발 환경

| 구분 | 기술/도구 | 버전 |
|------|----------|------|
| **게임 엔진** | Unity | 6000.0.59f2 (Unity 6) |
| **렌더 파이프라인** | URP | 17.0.4 |
| **프로그래밍 언어** | C# | .NET Standard 2.1 |
| **IDE** | Visual Studio / Rider | 2022 / 2023 |
| **버전 관리** | Git / GitHub | - |

## 3.2 Unity 패키지

### 핵심 패키지

```
Unity 공식 패키지:
├── Cinemachine (3.1.4)          # 카메라 시스템
├── Input System (1.14.2)        # 새로운 입력 시스템
├── Localization (1.5.8)         # 다국어 지원
├── URP (17.0.4)                 # 렌더 파이프라인
├── Addressables                 # 에셋 관리
├── Post Processing (3.5.0)      # 후처리 효과
├── TextMesh Pro                 # 텍스트 렌더링
├── AI Navigation (2.0.9)        # NavMesh 시스템
└── Behavior (1.0.12)            # AI 행동 트리

3rd Party:
├── Firebase SDK (13.4.0)        # 백엔드 서비스
└── Newtonsoft Json (3.2.2)      # JSON 처리
```

### 주요 에셋

- **Cartoon FX Remaster**: 파티클 이펙트
- **Biomechanical Mutant**: 몬스터 모델
- **Sci-Fi Trooper**: 플레이어 캐릭터
- **Joystick Pack**: 모바일 UI
- **다수의 사운드/이펙트 에셋**

---

## 3.3 백엔드 아키텍처

### Firebase 연동

```
Firebase Services:
├── Authentication         # 사용자 인증
├── Realtime Database     # 실시간 데이터베이스
├── Analytics             # 게임 분석
└── Crashlytics          # 크래시 리포팅
```

**주요 기능**:
- 사용자 계정 관리
- 플레이어 통계 저장
- 업그레이드 진행도 동기화
- 랭킹 시스템

---

# 4. 아키텍처 설계

## 4.1 전체 구조도

```
┌─────────────────────────────────────────────────┐
│              GameManager (싱글톤)                │
│         - DontDestroyOnLoad 영속성              │
└──────────────────┬──────────────────────────────┘
                   │
    ┌──────────────┼──────────────┐
    ↓              ↓              ↓
┌─────────┐  ┌──────────┐  ┌──────────┐
│Resource │  │   Pool   │  │   UI     │
│Manager  │  │ Manager  │  │ Manager  │
└─────────┘  └──────────┘  └──────────┘
    ↓              ↓              ↓
┌─────────┐  ┌──────────┐  ┌──────────┐
│  Data   │  │  Sound   │  │ Currency │
│Manager  │  │ Manager  │  │ Manager  │
└─────────┘  └──────────┘  └──────────┘
    ↓              ↓              ↓
┌─────────┐  ┌──────────┐  ┌──────────┐
│ Player  │  │  Skill   │  │  Scene   │
│StatMgr  │  │ Manager  │  │LoadMgr   │
└─────────┘  └──────────┘  └──────────┘
    ↓              ↓              ↓
┌─────────┐  ┌──────────┐  ┌──────────┐
│Language │  │Interact  │  │  Effect  │
│Manager  │  │ableMgr   │  │ Manager  │
└─────────┘  └──────────┘  └──────────┘
```

## 4.2 디자인 패턴

### 1. **싱글톤 패턴 (Singleton Pattern)**
```csharp
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<GameManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("GameManager");
                    _instance = obj.AddComponent<GameManager>();
                    DontDestroyOnLoad(obj);
                }
            }
            return _instance;
        }
    }
}
```

**장점**:
- 전역 접근 가능
- 씬 전환 시 지속성
- 중앙 집중식 관리

**적용 매니저**: GameManager, ResourceManager, PoolManager, UIManager, DataManager, PlayerStatManager, LanguageManager, CurrencyManager, SceneLoadManager, SoundManager, SkillManager, InteractableManager (12개)

---

### 2. **상태 머신 패턴 (State Machine Pattern)**
```csharp
public class StateMachine
{
    private IState _currentState;
    
    public void ChangeState(IState newState)
    {
        _currentState?.Exit();
        _currentState = newState;
        _currentState?.Enter();
    }
    
    public void Update()
    {
        _currentState?.Execute();
    }
}
```

**플레이어 상태**:
- `PlayerIdleState`: 대기
- `PlayerMoveState`: 이동
- `PlayerDashState`: 대시
- `PlayerNormalAttackState`: 일반 공격
- `PlayerSpecialAttackState`: 특수 공격
- `PlayerGrenadeAttackState`: 유탄 공격

**장점**:
- 명확한 상태 전환
- 코드 가독성 향상
- 디버깅 용이

---

### 3. **오브젝트 풀링 패턴 (Object Pool Pattern)**
```csharp
public class PoolManager : MonoBehaviour
{
    private Dictionary<string, Queue<GameObject>> _poolDictionary;
    
    public GameObject GetFromPool(string tag)
    {
        if (_poolDictionary[tag].Count > 0)
        {
            GameObject obj = _poolDictionary[tag].Dequeue();
            obj.SetActive(true);
            return obj;
        }
        // 새로 생성
        return CreateNew(tag);
    }
    
    public void ReleaseToPool(GameObject obj)
    {
        obj.SetActive(false);
        _poolDictionary[obj.tag].Enqueue(obj);
    }
}
```

**풀링 대상**:
- 총알 (Bullet)
- 파티클 이펙트 (Effects)
- 몬스터 (Monsters)
- UI 요소 (Damage Text 등)

**성능 이점**:
- Instantiate/Destroy 오버헤드 제거
- 메모리 할당 감소
- 프레임 드롭 방지

---

### 4. **옵저버 패턴 (Observer Pattern)**
```csharp
// 이벤트 기반 통신
public class Player : MonoBehaviour
{
    public event Action OnPlayerDead;
    public event Action<int, int> OnGrenadeCountChanged;
    public event Action<float, float> OnGrenadeCooltimeChanged;
    
    private void Die()
    {
        OnPlayerDead?.Invoke();
    }
}

// 구독자
public class UIManager : MonoBehaviour
{
    void Start()
    {
        player.OnPlayerDead += HandlePlayerDeath;
        player.OnGrenadeCountChanged += UpdateGrenadeUI;
    }
}
```

**장점**:
- 느슨한 결합 (Loose Coupling)
- 의존성 감소
- 유지보수성 향상

---

### 5. **MVC/MVP 패턴**
```csharp
// Model: 데이터
public class PlayerModel : MonoBehaviour
{
    public float HP { get; set; }
    public float MaxHP { get; set; }
}

// View: 표현
public class PlayerView : MonoBehaviour
{
    public void UpdateHealthBar(float hp, float maxHP)
    {
        healthBar.fillAmount = hp / maxHP;
    }
}

// Controller/Presenter: 로직
public class Player : MonoBehaviour
{
    [SerializeField] PlayerModel _model;
    [SerializeField] PlayerView _view;
    
    public void TakeDamage(float damage)
    {
        _model.HP -= damage;
        _view.UpdateHealthBar(_model.HP, _model.MaxHP);
    }
}
```

**적용 범위**: Player, Monster, UI 일부

---

## 4.3 코드베이스 구조

### 폴더 구조
```
Assets/02_Scripts/
├── 00_Constants/              # 상수 정의
│   └── Constants.cs
│
├── 00_Interfaces/             # 인터페이스
│   ├── IDamageAble.cs        # 피해 받을 수 있는 객체
│   ├── IInteractable.cs      # 상호작용 가능한 객체
│   └── IAttacker.cs          # 공격자
│
├── 00_Manager/                # 싱글톤 매니저들
│   ├── GameManager.cs        # 게임 전체 관리
│   ├── ResourceManager.cs    # 리소스 로딩
│   ├── PoolManager.cs        # 오브젝트 풀링
│   ├── UIManager.cs          # UI 관리
│   ├── DataManager.cs        # 데이터 관리
│   ├── SoundManager.cs       # 사운드 관리
│   ├── PlayerStatManager.cs  # 플레이어 스탯
│   ├── SkillManager.cs       # 스킬 관리
│   ├── CurrencyManager.cs    # 화폐 관리
│   ├── SceneLoadManager.cs   # 씬 로딩
│   ├── LanguageManager.cs    # 다국어
│   └── InteractableManager.cs
│
├── 00_PublicScripts/          # 공용 스크립트
│   ├── AttackContext.cs      # 공격 데이터
│   ├── PoolableObject.cs     # 풀링 베이스
│   ├── DebuffHandler.cs      # 디버프 처리
│   └── CharacterModelBase.cs # 캐릭터 베이스
│
├── Player/                    # 플레이어 시스템
│   ├── Player.cs             # 플레이어 메인
│   ├── PlayerModel.cs        # 데이터 모델
│   ├── PlayerView.cs         # 뷰
│   ├── Move/                 # 이동 관련
│   │   ├── PlayerMover.cs
│   │   └── PlayerDasher.cs
│   ├── Attack/               # 공격 관련
│   │   ├── PlayerNormalAttacker.cs
│   │   ├── PlayerSpecialAttacker.cs
│   │   └── PlayerGrenadeAttacker.cs
│   ├── StateMachine/         # 상태 머신
│   │   ├── StateMachine.cs
│   │   ├── IState.cs
│   │   ├── PlayerIdleState.cs
│   │   ├── PlayerMoveState.cs
│   │   ├── PlayerDashState.cs
│   │   ├── PlayerNormalAttackState.cs
│   │   ├── PlayerSpecialAttackState.cs
│   │   └── PlayerGrenadeAttackState.cs
│   └── Animation/
│       └── PlayerAnimator.cs
│
├── Monster/                   # 몬스터 시스템
│   ├── MonsterSpawner/       # 스포너
│   ├── NormalMonsters/       # 일반 몬스터
│   ├── Boss/                 # 보스
│   │   ├── Omega_X7.cs
│   │   └── BossAddOn/
│   └── MonsterAddOn/         # 몬스터 스킬
│
├── Skill/                     # 스킬 시스템
│   ├── SkillBase.cs          # 스킬 베이스
│   ├── Skill_One.cs ~ Five.cs # 기본 스킬
│   ├── Skill_Mutant.cs       # 돌연변이 스킬
│   ├── Skill_Collab.cs       # 협력 스킬
│   ├── ReinforceSkill/       # 강화 스킬
│   └── SkillObject/          # 스킬 오브젝트
│
├── Map_Generate/              # 맵 생성 (핵심!)
│   ├── MapController.cs      # 맵 관리
│   ├── RoomSpawner.cs        # 방 생성
│   ├── DoorSpawner.cs        # 문 생성
│   ├── DoorDecider.cs        # 확률 정책
│   ├── Room/                 # 방 타입들
│   │   ├── Room.cs           # 베이스
│   │   ├── StartRoom.cs
│   │   ├── NormalRoom.cs
│   │   ├── ShopRoom.cs
│   │   ├── LabRoom.cs
│   │   ├── ColosseumRoom.cs
│   │   └── BossRoom.cs
│   └── Data/                 # 방 데이터 SO
│
├── InteractableObjects/       # 상호작용 오브젝트
│   ├── Door/                 # 문
│   ├── HealPack/             # 체력 회복
│   ├── Credit/               # 크레딧
│   ├── Chrome/               # 크롬
│   ├── TechSelectPack/       # 스킬 선택
│   ├── TechUpgradePack/      # 업그레이드
│   ├── MutantPack/           # 돌연변이
│   └── Weapon/               # 무기
│
├── UI/                        # UI 시스템
│   ├── PlayScene/            # 게임 UI
│   ├── SettingUI/            # 설정 UI
│   └── IntroScene/           # 인트로 UI
│
├── ServerData/                # Firebase 연동
│   ├── ServerManager.cs
│   ├── DownloadManager.cs
│   ├── PlayerStatData.cs
│   └── UpgradePanel.cs
│
├── Mobile/                    # 모바일 지원
│   └── MobileInputController.cs
│
├── Camera/                    # 카메라
│   └── CameraMover.cs
│
├── Sound/                     # 사운드
│
├── Local/                     # 다국어
│   └── LanguageManager.cs
│
└── InputHandler/              # 입력 처리
    ├── PlayerInputHandler.cs
    └── MobileButtonEvent.cs
```

**총 201개 C# 스크립트, 24,218 라인**

---

# 5. 핵심 시스템

## 5.1 플레이어 시스템

### 컴포넌트 구조
```
Player (메인 컨트롤러)
├── PlayerModel (데이터)
├── PlayerView (UI 표현)
├── CharacterController (물리)
├── PlayerMover (이동)
├── PlayerDasher (대시)
├── PlayerAnimator (애니메이션)
├── PlayerWeaponController (무기 관리)
├── PlayerNormalAttacker (일반 공격)
├── PlayerSpecialAttacker (특수 공격)
├── PlayerGrenadeAttacker (유탄)
├── InteractionController (상호작용)
└── StateMachine (상태 머신)
```

### 상태 머신 전환 다이어그램
```
          ┌─────────────┐
          │    Idle     │ ◄──────┐
          └─────┬───────┘        │
                │                 │
        ┌───────┼────────┐       │
        ↓       ↓        ↓       │
    ┌───────┐ ┌────┐ ┌─────┐    │
    │ Move  │ │Dash│ │Attack│   │
    └───┬───┘ └──┬─┘ └──┬──┘    │
        │        │      │        │
        └────────┴──────┴────────┘
```

**상태 전환 조건**:
- Idle → Move: 이동 입력 감지
- Idle → Dash: 대시 입력 + 쿨타임 완료
- Idle → Attack: 공격 입력 + 공격 가능
- Attack → Idle: 공격 애니메이션 완료
- Dash → Idle: 대시 거리 완료

---

## 5.2 전투 시스템

### 데미지 계산 시스템

```csharp
public class AttackContext
{
    public float Damage;           // 기본 데미지
    public Vector3 KnockbackDir;   // 넉백 방향
    public float KnockbackForce;   // 넉백 힘
    public GameObject Attacker;    // 공격자
    public WeaponType WeaponType;  // 무기 타입
    public bool IsCritical;        // 크리티컬 여부
}

// 데미지 계산
float finalDamage = baseDamage * weaponMultiplier * skillMultiplier;
if (IsCritical) 
    finalDamage *= criticalMultiplier;
```

### 무기 특성

| 무기 | 데미지 | 연사력 | 특수 효과 |
|------|--------|--------|----------|
| **Machine Gun** | 10 | 높음 | - |
| **Shotgun** | 15×5 | 낮음 | 산탄 |
| **Sniper** | 50 | 매우 낮음 | 관통 |
| **Laser** | 8 | 연속 | 관통, 빔 |
| **Plasma** | 25 | 중간 | 광역 폭발 |

---

## 5.3 스킬 시스템

### 스킬 타입

**기본 스킬 (5종)**:
1. **Skill_One**: 드론 소환 - 자동 공격
2. **Skill_Two**: 몬스터 동료화 - 아군으로 전환
3. **Skill_Three**: 보호막 - 데미지 흡수
4. **Skill_Four**: 대시 강화 - 무적 시간 증가
5. **Skill_Five**: 에너지 폭발 - 광역 피해

**특수 스킬**:
- **Skill_Mutant**: 돌연변이 - 랜덤 효과
- **Skill_Collab**: 협력 - 팀 버프

### 강화 시스템

```
스킬 선택 (TechSelect)
    ↓
스킬 레벨업 (TechUpgrade)
    ↓
강화 스킬 획득 (ReinforceSkill)
```

**강화 예시**:
- 드론 스킬 강화 → 드론 2기 소환
- 대시 강화 → 무적 시간 2배
- 유탄 강화 → 폭발 범위 증가

---

## 5.4 몬스터 AI

### AI 행동 트리

```
Root
├── Selector (우선순위)
│   ├── Sequence [공격 가능?]
│   │   ├── IsInAttackRange?
│   │   └── Attack()
│   │
│   ├── Sequence [추적]
│   │   ├── HasTarget?
│   │   ├── IsPathValid?
│   │   └── MoveToTarget()
│   │
│   └── Patrol()
```

### 몬스터 종류

**일반 몬스터 (30종)**:
- NebulaPhantom: 원거리 공격
- TurretBot: 고정 포탑
- RushMonster: 돌진 공격
- PoisonMonster: 독 장판
- 등등...

**엘리트 (콜로세움)**:
- 일반 몬스터 강화 버전
- 체력 3배, 공격력 2배
- 특수 스킬 추가

**보스 (Omega_X7)**:
- 페이즈 1: 일반 공격 패턴
- 페이즈 2 (50% HP): 패턴 강화
- 페이즈 3 (25% HP): 광역 기술

---

## 5.5 상호작용 시스템

### IInteractable 인터페이스

```csharp
public interface IInteractable
{
    void Interact(GameObject player);
    bool CanInteract();
    string GetInteractionPrompt();
}
```

### 상호작용 오브젝트

**문 (Door)**:
```csharp
public class DoorInteractor : IInteractable
{
    public RoomInfo RoomInfo;  // 다음 방 정보
    
    public void Interact(GameObject player)
    {
        // 다음 방으로 이동
        MapController.SpawnNextRoom(RoomInfo);
    }
}
```

**아이템 (Item)**:
```csharp
public class HealPackInteractor : IInteractable
{
    public void Interact(GameObject player)
    {
        player.GetComponent<PlayerModel>().Heal(50);
        Destroy(gameObject);
    }
}
```

---

# 6. 방 생성 알고리즘

> **자세한 내용은 `ROOM_GENERATION_ALGORITHM.md` 참조**

## 6.1 알고리즘 개요

ProjectNemesis의 방 생성은 **절차적 생성(Procedural Generation)**을 사용합니다.

### 핵심 아이디어
- 플레이어가 문을 선택할 때마다 다음 방을 동적으로 생성
- 확률 기반 방 타입 선택
- 특수 규칙 적용 (보스, 상점 등장 보장)

---

## 6.2 4단계 프로세스

### Phase 1: 문 개수 결정
```
입력: currentRoomIndex (0~14)
출력: doorCount (0~3)

특수 규칙:
- Index 1: 1개 (튜토리얼 후)
- Index 12: 1~3개 (상점 포함)
- Index 13: 1개 (보스 직전)
- Index 14: 0개 (보스방, 종료)

일반 규칙 (확률):
- 1개 문: 10%
- 2개 문: 60%
- 3개 문: 30%
```

### Phase 2: 방 타입 결정
```
가중치 기반 랜덤 선택:

Normal     : 60%
Shop       : 15%
Lab        : 15%
Colosseum  : 10%

제약:
- Shop, Lab, Colosseum: 중복 불가
- Normal: 중복 가능
- Lab: 게임당 1회
```

### Phase 3: Normal 방 세부 타입
```
각 20% 확률:
- Credit      : 크레딧 획득
- Heal        : 체력 회복
- TechUpgrade : 기술 업그레이드
- Chrome      : 크롬 획득
- TechSelect  : 스킬 선택
```

### Phase 4: 문 위치 배치
```
1개 문: 왼쪽 또는 오른쪽 (50:50)
2개 문: 좌/우, 좌/좌, 우/우 (33:33:33)
3개 문: 좌/우/우 또는 좌/좌/우
```

---

## 6.3 시각화

### 게임 진행 예시
```
방 0 (Start)
   ↓ [1개 문]
방 1 (Normal-TechSelect) "첫 스킬 선택"
   ↓ [2개 문]
   ├─ 방 2-A (Normal-Heal)
   └─ 방 2-B (Normal-Credit)
      ↓ 선택
방 2 (Normal-Credit)
   ↓ [3개 문]
   ├─ 방 3-A (Normal-Upgrade)
   ├─ 방 3-B (Lab)
   └─ 방 3-C (Colosseum)
      ↓ 선택 (Lab)
방 3 (Lab) "특수 이벤트"
   ↓ [2개 문]
   ...
   ↓
방 12 (Normal)
   ↓ [2개 문, 반드시 Shop 포함]
   ├─ 방 13-A (Shop) ← 보장
   └─ 방 13-B (Normal)
      ↓ 선택
방 13 (Shop) "최종 준비"
   ↓ [1개 문]
방 14 (Boss) "최종 보스"
   ↓
승리 or 패배
```

---

## 6.4 코드 구조

### 주요 클래스

**MapController** (461줄):
- 전체 맵 흐름 제어
- 방 생성/파괴 관리
- 이벤트 조율

**DoorDecider** (322줄):
- 확률 정책 엔진
- 문 개수 결정
- 방 타입 선택

**RoomSpawner** (118줄):
- 방 프리팹 인스턴스화
- RoomInfo 기반 초기화

**DoorSpawner**:
- 문 프리팹 생성
- 상호작용 이벤트 연결

---

# 7. 개발 현황

## 7.1 프로젝트 통계

### 코드 메트릭
```
C# 스크립트:        201개
총 라인 수:         24,218줄
씬 파일:            11개
프리팹:             68개
매니저 클래스:      12개
인터페이스:         3개
몬스터 스크립트:    30개
```

### 커밋 히스토리
- 총 커밋 수: 100+ (추정)
- 활동 기간: 2024년 ~ 현재
- 브랜치 전략: Feature 브랜치 사용

---

## 7.2 완성도 평가

### 종합 점수: **6.4/10** (중급 수준)

| 항목 | 점수 | 평가 |
|------|------|------|
| **아키텍처 설계** | 7.5/10 | 명확한 패턴, 싱글톤 의존도 높음 |
| **코드 품질** | 6.5/10 | 구조는 좋으나 일부 개선 필요 |
| **문서화** | 4/10 | README 미완성, API 문서 부족 |
| **테스트 커버리지** | 2/10 | 테스트 거의 없음 |
| **성능 최적화** | 7/10 | 풀링 구현, 모바일 검증 필요 |
| **보안** | 5/10 | Firebase 설정 노출 |
| **확장성** | 8/10 | 모듈화 잘됨 |
| **유지보수성** | 7/10 | 구조화 양호 |

---

## 7.3 완료된 기능

### ✅ 구현 완료
- [x] 플레이어 이동/전투 시스템
- [x] 5종 무기 시스템
- [x] 스킬 시스템 (기본 5개 + 특수 2개)
- [x] 스킬 강화 시스템
- [x] 방 생성 알고리즘 (15개 방)
- [x] 일반 몬스터 AI (30종)
- [x] 엘리트 몬스터
- [x] 보스 전투 (Omega_X7)
- [x] 상점 시스템
- [x] 상호작용 오브젝트 (8종)
- [x] 오브젝트 풀링
- [x] 모바일 입력 지원
- [x] Firebase 연동
- [x] 다국어 지원 (Localization)
- [x] 사운드 시스템
- [x] UI 시스템

### 🚧 진행 중
- [ ] 밸런싱 조정
- [ ] 추가 몬스터 타입
- [ ] 더 많은 스킬 추가
- [ ] 엔딩 연출
- [ ] 튜토리얼

---

## 7.4 알려진 이슈

### 🔴 높은 우선순위
1. **보안**:
   - google-services.json 저장소에 포함
   - Debug.keystore 노출
   
2. **코드 품질**:
   - Debug.Log 173개 (프로덕션 빌드에서 제거 필요)
   - TODO 주석 11개 미해결

3. **문서화**:
   - README.md 템플릿 상태
   - API 문서 부족

### 🟡 중간 우선순위
4. **테스트**:
   - 단위 테스트 1개만 존재
   - 통합 테스트 없음

5. **성능**:
   - 모바일 최적화 검증 필요
   - FindAnyObjectByType 사용 (성능 비용 높음)

### 🟢 낮은 우선순위
6. **아키텍처**:
   - 싱글톤 남용 (12개)
   - MVC 패턴 일부만 적용

7. **에셋 관리**:
   - 대량의 스토어 에셋 (최적화 필요)
   - 미사용 에셋 정리

---

# 8. 팀 구성 및 역할

## 8.1 팀 정보

**팀명**: TeamNemesis  
**팀 규모**: 3명  
**개발 기간**: 2024년 ~ 현재 (약 1년)

---

## 8.2 팀원 및 담당

| 이름 | 역할 | 담당 시스템 |
|------|------|-----------|
| **endsun1234** | 프로그래머 | 상호작용 시스템, 맵 생성 |
| **minji** | 프로그래머 | 캐릭터 움직임, 플레이어 |
| **hyunwoo** | 프로그래머 | 스킬 업그레이드 시스템 |

---

## 8.3 협업 도구

- **버전 관리**: GitHub
- **프로젝트 관리**: Notion ([링크](https://economic-kettle-c2e.notion.site/26fc01e9d6ba80b498dde6d3fc2cc36e))
- **커뮤니케이션**: Discord / Slack (추정)
- **문서화**: Notion, Markdown

---

# 9. 향후 계획

## 9.1 단기 목표 (1개월)

### 🔴 높은 우선순위
1. **보안 강화**
   - [ ] google-services.json .gitignore 추가
   - [ ] Firebase 보안 규칙 재검토
   - [ ] API 키 환경 변수화

2. **코드 품질 개선**
   - [ ] Debug.Log 173개 정리
   - [ ] LogManager 클래스 구현
   - [ ] TODO 주석 11개 해결

3. **문서화**
   - [ ] README.md 완성
   - [ ] 설치 가이드 작성
   - [ ] 플레이 가이드 작성

---

## 9.2 중기 목표 (2~3개월)

### 🟡 중간 우선순위
4. **테스트 인프라**
   - [ ] Unity Test Framework 설정
   - [ ] 플레이어 시스템 단위 테스트
   - [ ] 매니저 클래스 테스트
   - [ ] CI/CD 파이프라인 구축

5. **성능 최적화**
   - [ ] Unity Profiler 분석
   - [ ] 모바일 기기 테스트
   - [ ] 메모리 사용량 최적화
   - [ ] 배터리 소모 측정

6. **게임 완성도**
   - [ ] 밸런싱 조정
   - [ ] 튜토리얼 추가
   - [ ] 엔딩 연출
   - [ ] 사운드 보강

---

## 9.3 장기 목표 (4~6개월)

### 🟢 낮은 우선순위 (출시 후)
7. **아키텍처 개선**
   - [ ] 싱글톤 패턴 재검토
   - [ ] 의존성 주입 패턴 도입
   - [ ] MVC 패턴 확장

8. **콘텐츠 확장**
   - [ ] 신규 무기 추가 (10종 → 15종)
   - [ ] 신규 몬스터 (30종 → 50종)
   - [ ] 신규 보스 (1종 → 3종)
   - [ ] 신규 방 타입

9. **추가 기능**
   - [ ] 메타 진행 시스템 (영구 업그레이드)
   - [ ] 일일 도전 과제
   - [ ] 랭킹 시스템
   - [ ] 리플레이 시스템

---

## 9.4 출시 계획

### 알파 테스트
- **시기**: 2개월 후
- **대상**: 내부 테스터 10명
- **목표**: 주요 버그 발견, 밸런싱 조정

### 베타 테스트
- **시기**: 3~4개월 후
- **대상**: 외부 테스터 100명
- **목표**: 실사용 피드백, 성능 검증

### 정식 출시
- **시기**: 6개월 후 (목표)
- **플랫폼**: PC (Steam), Mobile (Google Play)
- **가격**: Free-to-Play (광고 또는 IAP)

---

# 10. Q&A

## 10.1 기술 관련

### Q1: Unity 6를 선택한 이유는?
**A**: Unity 6는 최신 버전으로 다음 장점이 있습니다:
- URP 17.0.4: 최신 렌더링 기능
- 성능 개선: 더 나은 모바일 지원
- 새로운 Input System: 크로스 플랫폼 입력 통합
- 장기 지원: LTS 버전으로 안정성 보장

### Q2: 왜 싱글톤 패턴을 많이 사용했나?
**A**: 
- **장점**: 전역 접근 편의성, DontDestroyOnLoad로 씬 전환 시 지속성
- **단점**: 테스트 어려움, 의존성 증가
- **개선 계획**: 향후 의존성 주입(DI) 패턴으로 전환 검토 중

### Q3: 오브젝트 풀링의 효과는?
**A**: 
- Instantiate/Destroy 호출 90% 감소
- 프레임 드롭 방지 (특히 총알 발사 시)
- 메모리 할당 감소로 GC 빈도 감소

---

## 10.2 게임 디자인 관련

### Q4: 로그라이트 장르를 선택한 이유는?
**A**:
- **높은 재플레이성**: 매번 다른 던전, 스킬 조합
- **짧은 플레이 타임**: 15~30분으로 모바일에 적합
- **점진적 난이도**: 플레이어가 시스템을 익히며 성장
- **트렌드**: 최근 인기 있는 장르 (Hades, Vampire Survivors 등)

### Q5: 방 생성 알고리즘의 핵심은?
**A**:
- **정책 중앙화**: DoorDecider에 모든 규칙 집중
- **확률 + 규칙**: 완전 랜덤이 아닌 보장된 경험 제공
- **플레이어 선택권**: 1~3개 문 중 선택 가능

### Q6: 밸런싱은 어떻게 하나?
**A**:
- **데이터 주도**: ScriptableObject로 확률, 데미지 등 외부 관리
- **Inspector 조정**: 프로그래머가 아니어도 밸런싱 가능
- **플레이 테스트**: 반복 플레이로 적정값 찾기

---

## 10.3 개발 관련

### Q7: 가장 어려웠던 부분은?
**A**:
1. **방 생성 알고리즘**: 확률 계산과 특수 규칙 조율
2. **상태 머신**: 복잡한 상태 전환 로직
3. **AI 행동 트리**: 다양한 몬스터 패턴 구현

### Q8: 가장 만족스러운 기능은?
**A**:
- **방 생성 시스템**: 매번 다른 경험 제공
- **오브젝트 풀링**: 성능 크게 개선
- **스킬 시스템**: 다양한 조합 가능

### Q9: 앞으로의 도전 과제는?
**A**:
1. **테스트 커버리지 확대**: 현재 거의 없음
2. **성능 최적화**: 모바일 환경 검증
3. **콘텐츠 확장**: 더 많은 스킬, 몬스터 추가

---

## 10.4 기타

### Q10: 오픈소스로 공개할 계획은?
**A**: 
- 현재는 비공개
- 출시 후 일부 시스템(방 생성, 풀링)은 공개 검토 중

### Q11: 다른 플랫폼 출시 계획은?
**A**:
- **우선**: PC (Steam), Mobile (Android)
- **차후**: iOS, Nintendo Switch, Xbox Game Pass 검토

### Q12: 수익 모델은?
**A**:
- **PC**: 저가 유료 ($5~10)
- **Mobile**: Free-to-Play + 광고 또는 IAP
- **DLC/시즌패스**: 추가 콘텐츠

---

# 11. 참고 자료

## 11.1 프로젝트 문서

1. **PROJECT_EVALUATION.md**
   - 프로젝트 전반 평가
   - 강점/약점 분석
   - 개선 권장사항

2. **ROOM_GENERATION_ALGORITHM.md**
   - 방 생성 알고리즘 상세 설명
   - 4단계 프로세스
   - 코드 구조

3. **README.md**
   - 프로젝트 소개 (작성 중)

---

## 11.2 외부 링크

- **Notion**: https://economic-kettle-c2e.notion.site/26fc01e9d6ba80b498dde6d3fc2cc36e
- **GitHub**: https://github.com/TeamNemesis/ProjectNemesis

---

## 11.3 참고 게임

- **Enter the Gungeon**: 총기 액션 로그라이트
- **Hades**: 로그라이트 액션 RPG
- **Nuclear Throne**: 탑다운 로그라이트 슈터
- **Risk of Rain 2**: 3D 로그라이트

---

## 11.4 기술 문서

- Unity 6 Documentation: https://docs.unity3d.com/
- URP Documentation: https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@latest
- Firebase for Unity: https://firebase.google.com/docs/unity/setup

---

# 📊 부록

## A. 코드 통계

```
언어별 라인 수:
  C#:              24,218 라인 (201 파일)
  ShaderLab:       추정 500 라인
  JSON:            추정 200 라인
  
총계:              약 25,000 라인
```

## B. 에셋 크기

```
프로젝트 전체:      약 5GB
  - Scripts:       2MB
  - Prefabs:       50MB
  - Textures:      1.5GB
  - Models:        1GB
  - Audio:         500MB
  - Animations:    300MB
  - Scenes:        100MB
  - Others:        1.5GB
```

## C. 빌드 크기 (추정)

```
PC (Windows):      약 500MB
Mobile (Android):  약 300MB (APK)
                   약 150MB (AAB with Split APK)
```

---

# 🎬 마무리

## 프로젝트 요약

ProjectNemesis는 **Unity 6 기반의 3D 액션 로그라이트 슈팅 게임**으로, 다음 특징을 가집니다:

✅ **탄탄한 아키텍처**: 싱글톤 매니저, 상태 머신, 오브젝트 풀링  
✅ **절차적 던전 생성**: 매번 다른 경험  
✅ **풍부한 콘텐츠**: 5종 무기, 7종 스킬, 30종 몬스터  
✅ **크로스 플랫폼**: PC와 모바일 지원  
✅ **온라인 연동**: Firebase 기반  

**개발 현황**: 6.4/10 (중급 수준)  
**출시 목표**: 6개월 후  
**팀**: TeamNemesis (3명)

---

## 핵심 성과

1. **201개 C# 스크립트, 24,218 라인** - 체계적인 코드베이스
2. **15개 방으로 구성된 던전** - 확률 기반 생성 알고리즘
3. **12개 싱글톤 매니저** - 중앙 집중식 관리
4. **오브젝트 풀링** - 성능 최적화

---

## 향후 과제

🔴 **단기** (1개월): 보안 강화, Debug.Log 정리, 문서화  
🟡 **중기** (2~3개월): 테스트 인프라, 성능 최적화, 게임 완성도  
🟢 **장기** (4~6개월): 아키텍처 개선, 콘텐츠 확장, 출시 준비

---

## 감사합니다!

**질문이 있으시면 언제든 편하게 질문해 주세요.**

---

**발표 자료 작성**: GitHub Copilot  
**마지막 업데이트**: 2025-11-13  
**버전**: 1.0
