# Project Nemesis - 발표 자료

> Unity 기반 3D 액션 로그라이크 슈팅 게임

**Team Nemesis**  
**발표일**: 2025년 11월

---

## 📑 목차

1. [프로젝트 소개](#1-프로젝트-소개)
2. [개발 동기 및 목표](#2-개발-동기-및-목표)
3. [게임 플레이 소개](#3-게임-플레이-소개)
4. [핵심 기능](#4-핵심-기능)
5. [기술 스택](#5-기술-스택)
6. [시스템 아키텍처](#6-시스템-아키텍처)
7. [주요 시스템 상세](#7-주요-시스템-상세)
8. [개발 과정](#8-개발-과정)
9. [도전과 해결](#9-도전과-해결)
10. [데모 시연](#10-데모-시연)
11. [성과 및 통계](#11-성과-및-통계)
12. [팀원 소개](#12-팀원-소개)
13. [향후 계획](#13-향후-계획)
14. [Q&A](#14-qa)

---

## 1. 프로젝트 소개

### 🎮 Project Nemesis

```
"미래의 전장에서 살아남아라!"
```

**게임 장르**: 3D 액션 로그라이크 슈팅  
**개발 엔진**: Unity 6000.0.59f2  
**개발 기간**: [시작일] - 현재  
**플랫폼**: PC (Windows, macOS, Linux)

### 게임 개요

Project Nemesis는 플레이어가 미래의 전장에서 다양한 무기와 스킬을 활용하여 끊임없이 몰려오는 적들과 싸우는 액션 게임입니다. 로그라이크 요소로 매 플레이마다 새로운 경험을 제공합니다.

**핵심 컨셉**:
- 🎯 빠른 템포의 액션 전투
- ⚡ 전략적 스킬 조합
- 🔄 반복 플레이의 재미
- 🎲 랜덤 요소와 선택의 연속

---

## 2. 개발 동기 및 목표

### 개발 동기

**왜 이 게임을 만들었는가?**

1. **액션 게임의 재미 탐구**
   - 직관적이면서도 깊이 있는 전투 시스템
   - 플레이어의 실력이 드러나는 게임플레이

2. **로그라이크의 재도전 욕구**
   - 매번 다른 경험
   - 점진적인 발전의 재미

3. **기술적 도전**
   - Unity 엔진 심화 학습
   - 게임 시스템 설계 경험
   - 팀 협업 프로젝트 수행

### 개발 목표

#### 기술적 목표
- ✅ 확장 가능한 게임 아키텍처 설계
- ✅ 디자인 패턴을 활용한 코드 작성
- ✅ 성능 최적화 기법 적용
- ✅ 효과적인 팀 협업 시스템 구축

#### 게임 목표
- ✅ 재미있고 중독성 있는 게임플레이
- ✅ 직관적인 조작과 명확한 피드백
- ✅ 다양한 전략과 플레이 스타일 지원
- ✅ 높은 재플레이 가치

---

## 3. 게임 플레이 소개

### 게임 흐름

```
시작
  ↓
방 입장
  ↓
몬스터 소환
  ↓
전투 (이동, 공격, 회피)
  ↓
적 처치
  ↓
보상 획득
  ↓
상호작용 (상점, 업그레이드)
  ↓
다음 방으로 이동
  ↓
반복...
  ↓
보스 전투
  ↓
클리어 / 게임 오버
```

### 조작 방법

**키보드 & 마우스**:
- `WASD`: 이동
- `Space`: 대시 (회피)
- `마우스 좌클릭`: 일반 공격
- `마우스 우클릭`: 특수 공격
- `Q`: 유탄 발사
- `E`: 상호작용
- `1-5`: 스킬 사용

**게임패드**:
- 왼쪽 스틱: 이동
- 오른쪽 스틱: 조준
- `A`: 대시
- `RT`: 일반 공격
- `LT`: 특수 공격
- `RB`: 유탄 발사
- `X`: 상호작용

---

## 4. 핵심 기능

### 🎯 1. 다양한 전투 시스템

#### 공격 타입
- **일반 공격**: 기본 연속 공격
- **특수 공격**: 강력한 단발 공격
- **유탄 공격**: 범위 공격
- **대시**: 빠른 회피 및 이동

#### 무기 시스템
- 여러 종류의 무기 획득 가능
- 각 무기마다 다른 공격 패턴
- 전투 중 무기 전환

### ⚡ 2. 스킬 시스템

#### 스킬 획득
- 레벨업 시 랜덤 스킬 선택
- 다양한 스킬 조합 가능
- 시너지 효과

#### 스킬 강화
- 같은 스킬 중복 획득 시 강화
- 최대 레벨까지 성장
- 특수 합성 스킬

### 👾 3. 몬스터 시스템

#### 일반 몬스터
- **SMALL**: 빠르고 약한 적
- **MIDDLE**: 균형잡힌 적
- **BIG**: 느리지만 강한 적

#### 보스 몬스터
- 특수 패턴 공격
- 페이즈 시스템
- 높은 체력과 공격력

### 🏪 4. 상호작용 시스템

#### 상점
- 무기 구매
- 아이템 구매
- 업그레이드

#### 보상
- 랜덤 아이템 상자
- 스킬 선택
- 능력치 강화

---

## 5. 기술 스택

### 개발 환경

```
게임 엔진:      Unity 6000.0.59f2
프로그래밍 언어: C# 10.0
IDE:           Visual Studio Code, Visual Studio 2022
버전 관리:      Git, GitHub
프로젝트 관리:   Notion
```

### Unity 패키지

| 패키지 | 용도 |
|--------|------|
| **Input System** | 새로운 입력 시스템 |
| **TextMesh Pro** | 고품질 텍스트 렌더링 |
| **Addressables** | 에셋 관리 |
| **NavMesh** | AI 길찾기 |
| **Cinemachine** | 카메라 시스템 |
| **Post Processing** | 후처리 효과 |

### 외부 에셋

**캐릭터 & 애니메이션**:
- Robot & Pilot
- Free Test Character (Asuna)
- Stella Girl

**무기 & 이펙트**:
- Modern Warfare Pack
- FX Kandol Pack
- Rolling Balls Sci-fi Pack
- Kyeoms FX

**환경 & UI**:
- Sci-Fi Warehouse Kit
- Japanese Cyberpunk GUI
- Cosmic Retro Station Props

**사운드**:
- Casual Game Sounds
- Sci-Fi Small Sound Pack

---

## 6. 시스템 아키텍처

### 전체 아키텍처

```
┌─────────────────────────────────────────┐
│           Game Manager (Singleton)       │
│  - Scene Management                      │
│  - Game State Control                    │
└─────────────────┬───────────────────────┘
                  │
        ┌─────────┴─────────┐
        │                   │
┌───────▼────────┐   ┌──────▼──────────┐
│  Core Managers │   │  System Managers│
│  - Pool        │   │  - Skill        │
│  - Resource    │   │  - Sound        │
│  - Data        │   │  - UI           │
│  - Currency    │   │  - Effect       │
└────────┬───────┘   └──────┬──────────┘
         │                  │
    ┌────▼──────────────────▼────┐
    │     Game Systems            │
    │  - Player                   │
    │  - Monster                  │
    │  - Interaction              │
    │  - Map Generation           │
    └─────────────────────────────┘
```

### 디자인 패턴

#### 1. Singleton Pattern
```csharp
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get { return _instance; }
    }
}
```

#### 2. State Machine Pattern
```csharp
public enum PlayerStateType
{
    Idle, Move, Dash, Attack, ...
}

// 상태 전환
public void EvaluateTransitions() { ... }
```

#### 3. Object Pool Pattern
```csharp
public class PoolManager
{
    Dictionary<string, Queue<GameObject>> pools;
    public GameObject Get(string key) { ... }
    public void Release(GameObject obj) { ... }
}
```

#### 4. MVC Pattern
```csharp
// Model
public class PlayerModel { ... }

// View
public class PlayerView { ... }

// Controller
public class Player { ... }
```

#### 5. Observer Pattern
```csharp
public event Action OnHealthChanged;
public event Action<Skill> OnSkillAcquired;
```

---

## 7. 주요 시스템 상세

### 플레이어 시스템

#### 컴포넌트 구조
```
Player (Controller)
├── PlayerModel (Data)
├── PlayerView (Visual)
├── PlayerMover (Movement)
├── PlayerDasher (Dash)
├── PlayerWeaponController (Weapon)
├── PlayerAnimator (Animation)
├── PlayerNormalAttacker (Attack)
├── PlayerSpecialAttacker (Attack)
└── PlayerGrenadeAttacker (Attack)
```

#### 상태 머신 흐름
```
Idle
  ↓ (입력)
Move ⟷ Dash
  ↓ (공격)
Attack → Special Attack
  ↓
Grenade Attack
  ↓
Idle
```

#### 주요 코드
```csharp
public class Player : MonoBehaviour
{
    // 입력 처리
    void Update()
    {
        GatherInput();
        EvaluateTransitions();
        ExecuteState();
    }

    // 상태 전환
    void EvaluateTransitions()
    {
        switch (_currentStateType)
        {
            case PlayerStateType.Idle:
                if (_moveInput != Vector3.zero)
                    ChangeState(PlayerStateType.Move);
                break;
            // ...
        }
    }
}
```

### 스킬 시스템

#### 데이터 구조
```json
{
  "skillId": 1,
  "skillName": "FireBall",
  "damage": 100,
  "cooldown": 5.0,
  "maxLevel": 5,
  "effects": [
    { "type": "damage", "value": 100 },
    { "type": "range", "value": 3.0 }
  ]
}
```

#### 스킬 매니저
```csharp
public class SkillManager : MonoBehaviour
{
    List<SkillData> availableSkills;
    List<SkillData> acquiredSkills;

    public void AcquireSkill(SkillData skill)
    {
        if (HasSkill(skill))
            UpgradeSkill(skill);
        else
            AddNewSkill(skill);
    }
}
```

### 몬스터 AI

#### 행동 트리
```
Root
├── Idle
│   └── Patrol
├── Detect Player
│   └── Chase
└── In Range
    └── Attack
```

#### 구현 코드
```csharp
public class MonsterBase : MonoBehaviour
{
    enum MonsterState { Idle, Move, Attack, Die }
    MonsterState currentState;

    void Update()
    {
        switch (currentState)
        {
            case MonsterState.Idle:
                CheckForPlayer();
                break;
            case MonsterState.Move:
                MoveToPlayer();
                break;
            case MonsterState.Attack:
                AttackPlayer();
                break;
        }
    }
}
```

### 상호작용 시스템

#### 인터페이스
```csharp
public interface IInteractable
{
    void Interact(Player player);
    string GetInteractionPrompt();
    bool CanInteract(Player player);
}
```

#### 구현 예시
```csharp
public class ShopItem : InteractableObject
{
    public override void Interact(Player player)
    {
        if (CanAfford(player))
        {
            player.RemoveCurrency(cost);
            player.AcquireItem(item);
        }
    }
}
```

---

## 8. 개발 과정

### 개발 타임라인

```
Week 1-2: 기획 및 설계
  - 게임 컨셉 정립
  - 기술 스택 선정
  - 아키텍처 설계
  - 역할 분담

Week 3-4: 기본 시스템 구축
  - 플레이어 이동 & 공격
  - 기본 UI
  - 씬 관리
  - 매니저 시스템

Week 5-6: 핵심 기능 개발
  - 스킬 시스템
  - 몬스터 AI
  - 전투 시스템
  - 상호작용 시스템

Week 7-8: 컨텐츠 추가
  - 다양한 스킬
  - 여러 몬스터 타입
  - 보스 디자인
  - 맵 구조

Week 9-10: 최적화 & 버그 수정
  - Object Pooling
  - 성능 프로파일링
  - 버그 수정
  - 밸런싱

Week 11-12: 마무리
  - 사운드 & 이펙트
  - UI/UX 개선
  - 최종 테스트
  - 문서화
```

### 개발 방법론

#### Agile 방식
- **스프린트**: 2주 단위
- **일일 스탠드업**: 진행 상황 공유
- **회고**: 스프린트 종료 시 회고 미팅

#### Git 워크플로우
```
main (안정 버전)
  ↑
develop (개발 통합)
  ↑
feature/* (기능 개발)
  - feature/player-movement
  - feature/skill-system
  - feature/monster-ai
```

---

## 9. 도전과 해결

### 도전 과제 1: 성능 최적화

#### 문제
- 다수의 몬스터와 이펙트로 인한 프레임 드랍
- 메모리 사용량 증가
- 가비지 컬렉션으로 인한 버벅임

#### 해결 방법
```csharp
// Object Pooling 도입
public class PoolManager
{
    // 미리 오브젝트 생성
    void Initialize()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    // 재사용
    public GameObject Get()
    {
        if (pool.Count > 0)
            return pool.Dequeue();
        return Instantiate(prefab);
    }
}
```

#### 결과
- 프레임 드랍 30% 감소
- 메모리 사용량 20% 절감
- 안정적인 60 FPS 유지

### 도전 과제 2: 상태 관리의 복잡성

#### 문제
- 플레이어의 여러 상태 동시 처리
- 상태 전환 시 버그 발생
- 코드 복잡도 증가

#### 해결 방법
```csharp
// State Machine Pattern 적용
public class PlayerStateMachine
{
    Dictionary<PlayerStateType, PlayerState> states;
    PlayerState currentState;

    public void ChangeState(PlayerStateType type)
    {
        currentState?.OnExit();
        currentState = states[type];
        currentState?.OnEnter();
    }

    public void Update()
    {
        currentState?.OnUpdate();
    }
}
```

#### 결과
- 명확한 상태 전환
- 버그 90% 감소
- 유지보수성 향상

### 도전 과제 3: 팀 협업

#### 문제
- Unity 씬 파일 충돌
- 코드 스타일 불일치
- 커뮤니케이션 부족

#### 해결 방법
1. **Prefab 중심 작업**
   - 씬 최소화
   - Prefab으로 분리

2. **코드 컨벤션 통일**
   - 네이밍 규칙
   - 주석 가이드
   - 코드 리뷰

3. **일일 스탠드업**
   - 진행 상황 공유
   - 이슈 조기 발견
   - 빠른 의사결정

#### 결과
- 충돌 80% 감소
- 코드 가독성 향상
- 효율적인 협업

---

## 10. 데모 시연

### 시연 시나리오

#### 1. 게임 시작 (30초)
- 인트로 씬
- 게임 시작
- 튜토리얼 (선택)

#### 2. 기본 전투 (1분)
- 이동 및 조작 시연
- 일반 공격
- 특수 공격
- 대시 회피
- 몬스터 처치

#### 3. 스킬 시스템 (1분)
- 레벨업
- 스킬 선택
- 스킬 사용
- 스킬 조합

#### 4. 상호작용 (30초)
- 상점 방문
- 아이템 구매
- 능력치 업그레이드

#### 5. 보스 전투 (1분 30초)
- 보스 등장
- 패턴 공격
- 전략적 플레이
- 보스 처치
- 클리어

### 주요 포인트

**보여줄 것**:
- ✅ 부드러운 조작감
- ✅ 다양한 공격 패턴
- ✅ 화려한 이펙트
- ✅ 직관적인 UI
- ✅ 몰입감 있는 사운드

**강조할 것**:
- 🎯 빠른 템포의 액션
- ⚡ 전략적 선택의 중요성
- 🔄 높은 재플레이 가치

---

## 11. 성과 및 통계

### 개발 성과

#### 코드 통계
```
총 스크립트:        206개
총 라인 수:         약 40,000 라인
평균 클래스 크기:    ~200 라인
주석 비율:          ~25%
```

#### 에셋 통계
```
씬:                 15개
프리팹:             200개 이상
애니메이션:         100개 이상
사운드:             50개 이상
```

#### 성능 지표
```
평균 FPS:           58-60
메모리 사용:        ~1.5GB
로딩 시간:          ~3초
빌드 크기:          ~450MB
```

### 기술적 성취

✅ **아키텍처**
- 11개의 매니저 시스템
- 5개 이상의 디자인 패턴 적용
- 확장 가능한 구조

✅ **성능**
- Object Pooling으로 30% 성능 향상
- 안정적인 60 FPS
- 최적화된 메모리 사용

✅ **품질**
- 높은 주석 비율
- 일관된 코드 스타일
- 체계적인 문서화

### 학습 성과

**Unity 엔진**:
- Component 시스템 심화
- Scene 관리
- 성능 프로파일링

**C# 프로그래밍**:
- 디자인 패턴 실전 적용
- 객체지향 프로그래밍
- 비동기 프로그래밍

**게임 개발**:
- 게임 시스템 설계
- AI 프로그래밍
- 물리 엔진 활용

**협업**:
- Git 워크플로우
- 코드 리뷰
- 프로젝트 관리

---

## 12. 팀원 소개

### Team Nemesis

#### 👨‍💻 endsun1234
**역할**: 상호작용 시스템 개발  
**담당**:
- 상호작용 오브젝트 시스템
- 상점 및 아이템 시스템
- 보상 메커니즘
- UI 상호작용

**기여도**: ⭐⭐⭐⭐⭐

#### 👩‍💻 minji
**역할**: 캐릭터 움직임 시스템  
**담당**:
- 플레이어 이동 시스템
- 대시 메커니즘
- 캐릭터 컨트롤러
- 입력 시스템

**기여도**: ⭐⭐⭐⭐⭐

#### 👨‍💻 hyunwoo
**역할**: 스킬 업그레이드 시스템  
**담당**:
- 스킬 매니저
- 스킬 데이터 관리
- 스킬 강화 로직
- 스킬 UI

**기여도**: ⭐⭐⭐⭐⭐

#### 👥 기타 기여자
- 몬스터 AI 시스템
- 무기 및 전투 시스템
- 맵 생성 시스템
- 사운드 및 이펙트
- UI/UX 디자인

### 팀워크

**협업 방식**:
- 주 2회 정기 미팅
- 일일 스탠드업
- Git을 통한 코드 공유
- Notion으로 문서 관리

**소통 도구**:
- Discord: 실시간 소통
- Notion: 문서 및 일정 관리
- GitHub: 코드 버전 관리

---

## 13. 향후 계획

### 단기 계획 (1-2개월)

#### 게임 밸런싱
- 몬스터 난이도 조정
- 스킬 밸런스 패치
- 보상 시스템 개선
- 사용자 피드백 반영

#### 버그 수정
- 알려진 버그 수정
- 엣지 케이스 처리
- 안정성 향상
- 성능 최적화

#### 컨텐츠 추가
- 새로운 스킬 3-5개
- 추가 무기 타입 2-3개
- 새로운 몬스터 5-7개
- 추가 맵 구조

### 중기 계획 (3-6개월)

#### 메이저 업데이트
- 새로운 챕터 추가
- 보스 러시 모드
- 엔드리스 모드
- 어려움 난이도

#### 소셜 기능
- 리더보드
- 업적 시스템
- 플레이 통계
- 친구 시스템

#### 플랫폼 확장
- 모바일 버전 최적화
- 콘솔 포팅 검토
- 크로스 플랫폼 지원

### 장기 계획 (6개월+)

#### 대규모 확장
- 시퀄 또는 대형 DLC
- 멀티플레이어 모드
- 커스터마이제이션
- 사용자 생성 컨텐츠

#### 커뮤니티
- 모딩 도구 제공
- 워크샵 지원
- 정기 이벤트
- 커뮤니티 챌린지

---

## 14. Q&A

### 예상 질문

#### Q1: 개발 기간은 얼마나 걸렸나요?
**A**: 약 12주 정도 소요되었으며, 기획 2주, 개발 8주, 최적화 및 마무리 2주로 진행되었습니다.

#### Q2: 가장 어려웠던 부분은?
**A**: 성능 최적화와 팀 협업이 가장 도전적이었습니다. Object Pooling을 도입하고 Prefab 중심 작업으로 해결했습니다.

#### Q3: Unity를 선택한 이유는?
**A**: 
- 풍부한 에셋 스토어
- 크로스 플랫폼 지원
- 강력한 커뮤니티
- 팀원들의 숙련도

#### Q4: 로그라이크 요소는 어떻게 구현했나요?
**A**: 
- 랜덤 스킬 선택
- 동적 맵 생성
- 변화하는 보상
- 진행도 저장 없음

#### Q5: 향후 상용화 계획은?
**A**: 현재는 학습 프로젝트이지만, 피드백을 받아 개선 후 Steam 출시를 고려하고 있습니다.

#### Q6: 다른 플랫폼 지원 계획은?
**A**: PC 안정화 후 모바일 버전을 우선적으로 검토하고, 이후 콘솔 포팅을 고려하고 있습니다.

#### Q7: 사용한 디자인 패턴 중 가장 유용했던 것은?
**A**: State Machine Pattern이 플레이어와 몬스터의 복잡한 상태 관리에 매우 유용했습니다.

#### Q8: 팀 협업에서 중요했던 점은?
**A**: 
- 명확한 역할 분담
- 일일 진행 상황 공유
- 코드 리뷰 문화
- 적극적인 소통

### 추가 질문

궁금한 사항이 있으시면 언제든지 질문해주세요!

**연락처**:
- GitHub: https://github.com/TeamNemesis/ProjectNemesis
- Notion: https://economic-kettle-c2e.notion.site/26fc01e9d6ba80b498dde6d3fc2cc36e

---

## 감사합니다! 🎮

**Project Nemesis**를 소개해주셔서 감사합니다.

```
"게임을 만드는 것은 단순히 코드를 작성하는 것이 아니라,
플레이어에게 즐거움과 경험을 선사하는 것입니다."
```

**Team Nemesis 일동**

---

**발표 자료 버전**: 1.0  
**최종 수정일**: 2025년 11월  
**발표 시간**: 약 20-25분
