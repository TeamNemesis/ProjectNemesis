# 방 생성 알고리즘 설명 (발표 자료용)

## 📋 개요

ProjectNemesis의 방 생성 시스템은 **절차적 생성(Procedural Generation)**과 **확률 기반 선택**을 결합한 던전 생성 알고리즘입니다. 플레이어가 문을 통해 이동할 때마다 다음 방과 선택지를 동적으로 생성하여 매 플레이마다 다른 경험을 제공합니다.

---

## 🏗️ 시스템 아키텍처

### 주요 컴포넌트

```
MapController (총괄 관리자)
    ├── RoomSpawner (방 생성)
    ├── DoorSpawner (문 생성)
    ├── DoorDecider (확률 계산 및 정책)
    └── MonsterController (몬스터 관리)
```

#### 1. **MapController** - 전체 흐름 제어
- 방 생성/소멸 관리
- 문 생성 및 상호작용 처리
- 게임 진행 상태 추적 (현재 방 번호, Lab 방 등장 여부 등)

#### 2. **RoomSpawner** - 방 인스턴스화
- RoomInfo(방 타입 + 옵션)를 받아 실제 방 프리팹을 생성
- ScriptableObject(SO) 기반 데이터 로딩
- 방 초기화 및 이벤트 연결

#### 3. **DoorDecider** - 정책 엔진 (핵심!)
- 다음 방 선택지 개수 결정 (1~3개)
- 다음 방 타입 결정 (확률 기반)
- 특수 규칙 적용 (보스 직전, 상점 등장 등)

#### 4. **DoorSpawner** - 문 인스턴스화
- 문 프리팹 생성 및 위치 설정
- 플레이어 상호작용 이벤트 연결

---

## 🎲 알고리즘 상세 설명

### Phase 1: 다음 문 개수 결정

```
입력: currentRoomIndex (0~14)
출력: nextDoorCount (0~3)
```

#### 특수 규칙
- **인덱스 1** (시작 직후): 무조건 **1개** (일반방)
- **인덱스 12** (보스 2단계 전): 확률 적용 + **상점 우선 포함**
- **인덱스 13** (보스 직전): 무조건 **1개** (보스방으로 가는 문)
- **인덱스 14** (보스방): **0개** (게임 종료)

#### 확률 기반 선택 (일반 구간)
```csharp
문 1개: 10% 확률 (_oneDoorChance)
문 2개: 60% 확률 (_twoDoorChance)
문 3개: 30% 확률 (_threeDoorChance)
```

**시각화:**
```
[시작방] → [1개] → [1~3개 랜덤] → ... → [1~3개 랜덤] → [상점 포함] → [1개 보스] → [종료]
  0번       1번         2~12번                    12번          13번        14번
```

---

### Phase 2: 방 타입 결정 (확률 기반 가중치 선택)

```
입력: 
  - count: 생성할 문 개수
  - currentRoomIndex: 현재 방 번호
  - hasLabRoomAppeared: Lab 방 등장 여부
  
출력: RoomType[] (Normal, Shop, Lab, Colosseum, Boss)
```

#### 방 타입 종류

| 타입 | 설명 | 특징 |
|------|------|------|
| **Normal** | 일반 방 | 보상/상호작용 오브젝트 포함 |
| **Shop** | 상점 방 | 아이템 구매 가능, 중복 불가 |
| **Lab** | 연구실 방 | 특수 이벤트, 게임당 1회만 |
| **Colosseum** | 콜로세움 | 엘리트 몬스터 전투, 중복 불가 |
| **Boss** | 보스 방 | 보스 전투, 마지막 방 |

#### 확률 가중치 시스템

각 방 타입은 **RoomDataSO**에 정의된 `BaseChance` 값을 가짐:

```csharp
Dictionary<RoomType, float> candidate
{
    { RoomType.Normal, 0.6 },      // 60%
    { RoomType.Shop, 0.15 },       // 15%
    { RoomType.Lab, 0.15 },        // 15%
    { RoomType.Colosseum, 0.1 }    // 10%
}
```

#### 중복 방지 로직

**특수 방(Shop, Lab, Colosseum)은 중복 선택 불가**
```csharp
1. 방 타입 선택
2. 특수 방이면 → 후보군에서 제거
3. Normal 방이면 → 후보군 유지 (중복 가능)
```

**예시:**
```
문 3개 생성 시:
  - 1번째: Colosseum 선택 → 후보에서 제거
  - 2번째: Shop 선택 → 후보에서 제거
  - 3번째: Normal 또는 Lab만 가능
```

---

### Phase 3: Normal 방 세부 타입 결정

Normal 방은 다시 5가지 세부 타입으로 나뉨:

| 타입 | 설명 | 확률 |
|------|------|------|
| **Credit** | 크레딧 획득 | 20% |
| **Heal** | 체력 회복 | 20% |
| **TechUpgrade** | 기술 업그레이드 | 20% |
| **Chrome** | 크롬 획득 | 20% |
| **TechSelect** | 스킬 선택 | 20% |

```csharp
float rand = Random.Range(0f, 1.0f);

if (rand < 0.2) → Credit
else if (rand < 0.4) → Heal
else if (rand < 0.6) → TechUpgrade
else if (rand < 0.8) → Chrome
else → TechSelect
```

---

### Phase 4: 문 위치 결정

각 방 프리팹은 **왼쪽/오른쪽 문 스폰 포인트**를 가짐:

```csharp
_doorSpawnPointsLeft[]   // 왼쪽 문 위치들
_doorSpawnPointsRight[]  // 오른쪽 문 위치들
```

#### 문 개수별 배치 전략

**1개 문:**
```
왼쪽 또는 오른쪽 중 하나를 50:50 확률로 선택
```

**2개 문:**
```
옵션 1 (33%): 왼쪽[0] + 오른쪽[0]
옵션 2 (33%): 왼쪽[0] + 왼쪽[1]
옵션 3 (33%): 오른쪽[0] + 오른쪽[1]
```

**3개 문:**
```
왼쪽[0] + 오른쪽[0] + 오른쪽[1]
또는
왼쪽[0] + 왼쪽[1] + 오른쪽[0]
```

---

## 🔄 전체 프로세스 플로우차트

```
┌─────────────────────────┐
│   플레이어 문 상호작용    │
└───────────┬─────────────┘
            ↓
┌─────────────────────────┐
│  OnDoorInteracted 이벤트 │
└───────────┬─────────────┘
            ↓
┌─────────────────────────┐
│   플레이어 이동 애니메이션 │
│     (DoorInteraction)    │
└───────────┬─────────────┘
            ↓
┌─────────────────────────┐
│    현재 방/문 파괴      │
│  (DestroyCurrentRoom)   │
└───────────┬─────────────┘
            ↓
┌─────────────────────────┐
│  DoorDecider 호출       │
│  - 문 개수 결정          │
│  - 방 타입 결정          │
└───────────┬─────────────┘
            ↓
┌─────────────────────────┐
│  RoomSpawner 호출       │
│  - 방 프리팹 생성        │
│  - 플레이어 위치 이동     │
└───────────┬─────────────┘
            ↓
┌─────────────────────────┐
│  OnRoomSpawned 이벤트   │
│  - BGM 재생             │
│  - 몬스터 스폰           │
└───────────┬─────────────┘
            ↓
┌─────────────────────────┐
│  DoorSpawner 호출       │
│  - 다음 문들 생성        │
└───────────┬─────────────┘
            ↓
┌─────────────────────────┐
│   전투 시작 (OnRoomStart)│
└─────────────────────────┘
```

---

## 🎯 알고리즘의 핵심 특징

### 1. **정책 중앙화 (Policy Centralization)**

모든 확률과 규칙이 **DoorDecider**에 집중:
- 확률 변경 시 한 곳만 수정하면 됨
- 테스트와 밸런싱이 용이
- MapController는 정책을 신뢰하고 실행만 담당

```csharp
// 잘못된 설계 (분산된 정책)
if (currentRoom == 13) 
    doorCount = 1; // MapController에서 판단

// 올바른 설계 (중앙화된 정책)
doorCount = doorDecider.GetNextDoorCount(currentRoomIndex);
```

### 2. **가중치 기반 랜덤 선택**

단순 랜덤이 아닌 **가중치를 고려한 선택**:

```csharp
총 가중치 = 0.6 + 0.15 + 0.15 + 0.1 = 1.0
랜덤값 = 0.0 ~ 1.0 사이

0.0 ~ 0.6  → Normal (60%)
0.6 ~ 0.75 → Shop (15%)
0.75 ~ 0.9 → Lab (15%)
0.9 ~ 1.0  → Colosseum (10%)
```

### 3. **방어적 프로그래밍 (Defensive Programming)**

예외 상황 철저히 처리:

```csharp
// 확률 합이 0인 경우 → 균등 분포로 폴백
if (totalChance <= 0f) {
    Debug.LogWarning("weights sum to 0, using uniform fallback");
    return RandomUniform();
}

// 후보군이 비어있는 경우 → Normal로 채움
if (candidate.Count == 0) {
    types.Add(RoomType.Normal);
}
```

### 4. **데이터 주도 설계 (Data-Driven Design)**

확률값을 코드가 아닌 **ScriptableObject**에 저장:
- 프로그래머가 아니어도 밸런싱 가능
- Unity Inspector에서 실시간 조정
- 빌드 없이 테스트 가능

---

## 📊 실제 게임 진행 예시

### 예시 1: 표준 진행

```
방 0 (Start) → 1개 문 → 방 1 (Normal-TechSelect)
              ↓
방 1 → 2개 문 → 방 2-A (Normal-Heal)
                방 2-B (Shop)
              ↓ (Shop 선택)
방 2 (Shop) → 3개 문 → 방 3-A (Normal-Credit)
                       방 3-B (Lab)
                       방 3-C (Colosseum)
              ↓
...
              ↓
방 12 → 2개 문 (반드시 Shop 포함) → 방 13-A (Shop)
                                     방 13-B (Normal-Upgrade)
              ↓ (Shop 선택)
방 13 (Shop) → 1개 문 → 방 14 (Boss)
              ↓
방 14 (Boss) → 게임 종료
```

### 예시 2: 콜로세움 진입

```
방 5 → 2개 문 → 방 6-A (Colosseum)
                방 6-B (Normal)
              ↓ (Colosseum 선택)
방 6 (Colosseum) → 엘리트 몬스터 스폰
                → 몬스터 처치 후 문 생성
                → 3개 문 (Colosseum 중복 불가)
```

---

## 🛠️ 확률 밸런싱 가이드

### Inspector에서 조정 가능한 값들

```csharp
[DoorDecider 컴포넌트]
- _oneDoorChance: 0.1 (10%)
- _twoDoorChance: 0.6 (60%)
- _threeDoorChance: 0.3 (30%)

- door_CreditChance: 0.2 (20%)
- door_HealChance: 0.2 (20%)
- door_UpgradeChance: 0.2 (20%)
- door_ChromeChance: 0.2 (20%)
- door_SkillPackChance: 0.2 (20%)
```

### 밸런싱 팁

**난이도 조정:**
```
쉽게: Heal 확률 증가 (0.2 → 0.3)
어렵게: Heal 확률 감소 (0.2 → 0.1)
```

**선택의 폭 조정:**
```
많은 선택지: _threeDoorChance 증가
적은 선택지: _oneDoorChance 증가
```

**특수 방 출현율:**
```
RoomDataSO에서 BaseChance 수정
Lab.BaseChance: 0.15 → 0.25 (더 자주 등장)
Colosseum.BaseChance: 0.1 → 0.05 (덜 등장)
```

---

## 🔍 기술적 세부사항

### 코루틴 기반 비동기 처리

```csharp
IEnumerator GoNextRoomRoutine(DoorInteractor doorInteractor)
{
    // 1. 플레이어 이동 애니메이션
    yield return StartCoroutine(_player.DoorInteractionRoutine());
    
    // 2. 로딩 패널 표시 이벤트
    OnDoorInteractionFinished?.Invoke(doorInteractor);
    
    // 3. 1초 대기 (전환 효과)
    yield return new WaitForSeconds(1.0f);
    
    // 4. 현재 방 파괴
    DestroyCurrentRoomObjects();
}
```

### 메모리 관리 - 오브젝트 풀링

```csharp
// 방 파괴 시 풀링 오브젝트 반환
var poolables = _currentRoom.PoolableObjectsInRoom;
foreach (var obj in poolables) {
    GameManager.Instance.PoolManager.ReleaseToPool(obj);
}
```

### 이벤트 기반 아키텍처

```csharp
// 느슨한 결합 (Loose Coupling)
RoomSpawner.OnRoomSpawned += MapController.OnRoomSpawned;
DoorSpawner.DoorInteracted += MapController.OnDoorInteracted;
MonsterController.OnAllMonsterDefeated += MapController.StartReward;
```

---

## 📈 성능 최적화

### 1. **지연 생성 (Lazy Generation)**
- 플레이어가 문을 통과할 때만 다음 방 생성
- 미리 생성하지 않아 메모리 절약

### 2. **즉시 파괴 (Immediate Cleanup)**
- 이전 방을 즉시 파괴하여 메모리 확보
- 풀링 오브젝트는 재활용

### 3. **캐싱 (Caching)**
```csharp
// 매번 계산하지 않고 캐싱
private RoomType[] _candidateTypes;
private Dictionary<RoomType, float> _candidateWeights;
```

---

## 🎨 발표 시각화 자료 제안

### 슬라이드 1: 시스템 개요
- 4개 주요 컴포넌트 다이어그램
- 절차적 생성 vs 수동 설계 비교

### 슬라이드 2: 알고리즘 플로우
- 플로우차트 (위 참조)
- 단계별 애니메이션

### 슬라이드 3: 확률 시스템
- 원 그래프로 방 타입 확률 표시
- 가중치 선택 과정 시각화

### 슬라이드 4: 실제 게임 예시
- 스크린샷으로 15개 방 진행 과정
- 선택지 화면 캡처

### 슬라이드 5: 특수 규칙
- 인덱스별 특수 처리 표
- 중복 방지 로직 다이어그램

### 슬라이드 6: 장점 및 확장성
- 데이터 주도 설계의 이점
- 향후 추가 가능한 기능 (절차적 레벨 생성 등)

---

## 🔮 향후 개선 방향

### 1. **적응형 난이도 (Adaptive Difficulty)**
```csharp
// 플레이어 체력에 따라 Heal 방 확률 조정
if (player.HP < 0.3f) {
    door_HealChance *= 1.5f; // Heal 확률 50% 증가
}
```

### 2. **메타 진행 (Meta Progression)**
```csharp
// 플레이 횟수에 따라 특수 방 해금
if (playCount > 10) {
    UnlockRoomType(RoomType.SecretLab);
}
```

### 3. **시드 기반 생성 (Seeded Generation)**
```csharp
// 같은 시드면 같은 던전 생성 (리플레이/공유 가능)
Random.InitState(seed);
```

### 4. **분기 시각화 (Branch Visualization)**
```csharp
// 미니맵에 다음 방 미리보기 표시
ShowNextRoomPreview(doorTypes);
```

---

## 📚 참고 자료

### 관련 코드 파일
- `MapController.cs` (461줄) - 전체 흐름 제어
- `DoorDecider.cs` (322줄) - 확률 정책 엔진
- `RoomSpawner.cs` (118줄) - 방 생성
- `DoorSpawner.cs` - 문 생성
- `Room.cs` - 방 베이스 클래스

### 디자인 패턴
- **Singleton Pattern**: GameManager, DataManager
- **Strategy Pattern**: DoorDecider (정책 분리)
- **Observer Pattern**: 이벤트 기반 통신
- **Object Pool Pattern**: 몬스터/이펙트 재활용

### 게임 디자인 원칙
- **Player Agency**: 플레이어에게 선택권 제공
- **Randomness with Control**: 완전 랜덤이 아닌 제어된 랜덤
- **Risk vs Reward**: 위험한 방일수록 좋은 보상

---

**작성일**: 2025-11-13  
**버전**: 1.0  
**작성자**: GitHub Copilot (요청: @endsun1234)
