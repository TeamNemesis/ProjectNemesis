# 플레이어 시스템 발표 자료

**발표 시간**: 3분  
**슬라이드 수**: 3장  
**주제**: ProjectNemesis 플레이어 시스템 아키텍처

---

# 🎮 Slide 1: 플레이어 시스템 개요 (1분)

## 시스템 구조

```
┌──────────────────────────────────────────┐
│         Player (메인 컨트롤러)            │
│    - 34개 C# 스크립트로 구성              │
│    - 컴포넌트 기반 모듈 설계               │
└────────────┬─────────────────────────────┘
             │
    ┌────────┼────────┐
    ↓        ↓        ↓
┌────────┐ ┌────┐ ┌─────────┐
│ Model  │ │View│ │ Control │
│(데이터)│ │(UI)│ │ (로직)  │
└────────┘ └────┘ └─────────┘
```

## 핵심 컴포넌트 (11개)

| 컴포넌트 | 역할 | 설명 |
|---------|------|------|
| **Player.cs** | 메인 컨트롤러 | 모든 컴포넌트 조율, 상태머신 관리 |
| **PlayerModel** | 데이터 | HP, 스탯, 상태 데이터 |
| **PlayerView** | 뷰 | UI 표현 (체력바, 상태 아이콘) |
| **PlayerMover** | 이동 | WASD 이동 처리 |
| **PlayerDasher** | 대시 | Space 대시 + 무적 시간 |
| **PlayerWeaponController** | 무기 관리 | 5종 무기 전환 |
| **PlayerNormalAttacker** | 일반 공격 | 좌클릭 공격 |
| **PlayerSpecialAttacker** | 특수 공격 | 우클릭 특수기 |
| **PlayerGrenadeAttacker** | 유탄 | E키 유탄 발사 |
| **PlayerAnimator** | 애니메이션 | 상태별 애니메이션 재생 |
| **StateMachine** | 상태 머신 | 6개 상태 전환 관리 |

### 설계 원칙

✅ **컴포넌트 기반 설계**: 각 기능을 독립적인 컴포넌트로 분리  
✅ **MVC 패턴 적용**: Model-View-Controller 분리  
✅ **이벤트 기반 통신**: 느슨한 결합으로 의존성 최소화  

---

# 🔄 Slide 2: 상태 머신 시스템 (1분)

## 6가지 플레이어 상태

```
          ┌─────────────┐
    ┌────▶│    Idle     │◀─────┐
    │     │   (대기)     │      │
    │     └──────┬──────┘      │
    │            │              │
    │     ┌──────┼──────┐      │
    │     ↓      ↓      ↓      │
    │  ┌────┐ ┌────┐ ┌────┐   │
    │  │Move│ │Dash│ │공격 │   │
    │  │이동│ │대시│ │상태 │   │
    │  └─┬──┘ └──┬─┘ └──┬─┘   │
    │    │       │      │      │
    └────┴───────┴──────┴──────┘
           모든 상태 → Idle
```

### 상태별 특징

#### 1. **PlayerIdleState** (대기)
- 입력 대기 상태
- 다른 모든 상태로 전환 가능
- 애니메이션: Idle

#### 2. **PlayerMoveState** (이동)
```csharp
조건: MoveInput != Vector3.zero
속도: 기본 5.0f (스탯에 따라 변동)
애니메이션: Walk/Run
```

#### 3. **PlayerDashState** (대시)
```csharp
조건: Space 입력 + 쿨타임 완료
거리: 5 유닛
무적 시간: 0.5초
쿨타임: 1.0초
```

#### 4. **PlayerNormalAttackState** (일반 공격)
```csharp
조건: 좌클릭 + 공격 가능
무기별 차이: 
  - Rifle: 연사
  - Shotgun: 산탄
  - Blade: 근접
애니메이션: Attack_Normal
```

#### 5. **PlayerSpecialAttackState** (특수 공격)
```csharp
조건: 우클릭 + 스킬 쿨타임 완료
무기별 특수기:
  - Rifle: 정밀 사격
  - Shotgun: 광역 사격
  - Blade: 회전 베기
쿨타임: 3~5초
```

#### 6. **PlayerGrenadeAttackState** (유탄)
```csharp
조건: E키 + 유탄 보유
투척: 포물선 궤적
폭발: 광역 피해
개수 제한: 3~5개
```

## 상태 전환 로직

```csharp
// StateMachine.cs
public void ChangeState(IState newState)
{
    _currentState?.Exit();    // 현재 상태 종료
    _currentState = newState; // 상태 변경
    _currentState?.Enter();   // 새 상태 시작
}

// 매 프레임 실행
public void Update()
{
    _currentState?.Execute(); // 상태 실행
}
```

### 상태 전환 우선순위

```
1. 공격 중 → 다른 행동 불가 (공격 종료 대기)
2. 대시 중 → 다른 행동 불가 (대시 종료 대기)
3. 이동 + 공격 → 공격 우선
4. 대시 입력 → 즉시 대시
```

---

# ⚔️ Slide 3: 전투 시스템 (1분)

## 무기 시스템 (5종)

### 무기 특성표

| 무기 | 데미지 | 연사력 | 사거리 | 특수 효과 |
|------|--------|--------|--------|----------|
| **🔫 Rifle** | 15 | 높음 | 중거리 | 정밀 사격 |
| **💥 Shotgun** | 10×5 | 낮음 | 근거리 | 산탄 (5발) |
| **🗡️ Blade** | 30 | 중간 | 근접 | 회전 베기 |
| **⚡ Laser** | 8 | 연속 | 장거리 | 관통 |
| **🔮 Plasma** | 25 | 중간 | 중거리 | 광역 폭발 |

### 무기 전환 시스템

```csharp
// PlayerWeaponController.cs
public void ChangeWeapon(WeaponType newWeapon)
{
    // 1. 현재 무기 비활성화
    currentWeapon.SetActive(false);
    
    // 2. 새 무기 활성화
    newWeapon.SetActive(true);
    
    // 3. 공격 컴포넌트 교체
    _normalAttacker = _normalAttackerMap[newWeapon];
    _specialAttacker = _specialAttackerMap[newWeapon];
}
```

## 공격 시스템

### 데미지 계산

```csharp
public class AttackContext
{
    float baseDamage = weaponDamage;
    float finalDamage = baseDamage 
                      * statMultiplier    // 스탯 보정
                      * skillMultiplier   // 스킬 보정
                      * criticalMultiplier; // 크리티컬 (1.5x)
}
```

### 공격 흐름

```
입력 감지
    ↓
상태 전환 (AttackState)
    ↓
애니메이션 재생
    ↓
[애니메이션 이벤트]
    ↓
투사체 생성 (Pool에서)
    ↓
적 충돌 감지
    ↓
데미지 계산 & 적용
    ↓
이펙트 재생
    ↓
상태 복귀 (Idle)
```

### 유탄 시스템

**투척 메커니즘**:
```csharp
// PlayerGrenadeAttacker.cs
public void ThrowGrenade()
{
    // 1. 발사 방향 계산
    Vector3 throwDirection = CalculateParabolicPath();
    
    // 2. 유탄 생성 (Pool)
    GameObject grenade = PoolManager.Get("Grenade");
    
    // 3. 물리 적용
    grenade.GetComponent<Rigidbody>().velocity = throwDirection;
    
    // 4. 개수 감소
    grenadeCount--;
}
```

**폭발 처리**:
- 타이머: 2초 후 자동 폭발
- 충돌: 즉시 폭발
- 범위: 반경 5 유닛
- 피해: 광역 50 데미지

## 입력 처리

### 크로스 플랫폼 입력

**PC (Unity Input System)**:
```
이동:     WASD / 화살표
조준:     마우스
일반 공격: 좌클릭
특수 공격: 우클릭
유탄:     E
대시:     Space
상호작용: F
```

**Mobile (조이스틱 + 버튼)**:
```
이동:     가상 조이스틱
조준:     자동 조준 (가까운 적)
공격:     버튼 (자동 연사)
특수:     버튼
유탄:     버튼
대시:     스와이프 또는 버튼
```

### 입력 처리 코드

```csharp
// PlayerInputHandler.cs
void Update()
{
    if (!CanGetInput) return;
    
    // 이동 입력
    Vector2 input = _inputActions.Player.Move.ReadValue<Vector2>();
    _moveInput = new Vector3(input.x, 0, input.y);
    
    // 공격 입력 (단발)
    if (_inputActions.Player.NormalAttack.triggered)
        _normalAttackPressed = true;
        
    if (_inputActions.Player.SpecialAttack.triggered)
        _specialAttackPressed = true;
        
    // 대시 입력
    if (_inputActions.Player.Dash.triggered)
        _dashPressed = true;
}
```

---

# 📊 핵심 수치 정리

## 시스템 규모

```
플레이어 스크립트:    34개
상태 종류:           6개
무기 종류:           5개
컴포넌트:           11개
지원 플랫폼:         PC + Mobile
```

## 성능 최적화

✅ **오브젝트 풀링**: 투사체, 이펙트 재사용  
✅ **이벤트 기반**: Update 호출 최소화  
✅ **상태 캐싱**: 불필요한 GetComponent 제거  
✅ **컴포넌트 분리**: 기능별 독립적 처리  

## 특징

🎯 **모듈화**: 각 기능이 독립적으로 동작  
🎯 **확장성**: 새 무기/상태 추가 용이  
🎯 **유지보수성**: MVC 패턴으로 명확한 구조  
🎯 **크로스 플랫폼**: PC/Mobile 동시 지원  

---

# 💡 기술적 하이라이트

## 1. 상태 머신 패턴의 이점

```
✓ 명확한 행동 분리
✓ 디버깅 용이 (현재 상태 확인 가능)
✓ 상태별 로직 캡슐화
✓ 전환 규칙 중앙 관리
```

## 2. 컴포넌트 기반 설계

```
✓ 재사용 가능한 컴포넌트
✓ 테스트 독립성
✓ 낮은 결합도
✓ 높은 응집도
```

## 3. 이벤트 시스템

```csharp
// 발행자
public event Action<int, int> OnGrenadeCountChanged;

// 구독자 (UI)
player.OnGrenadeCountChanged += UpdateGrenadeUI;

// 호출
OnGrenadeCountChanged?.Invoke(current, max);
```

**장점**:
- UI와 로직 분리
- 다중 구독자 지원
- 의존성 역전

---

# 🎬 발표 스크립트 (3분)

## 1분차: 시스템 개요
> "ProjectNemesis의 플레이어 시스템은 **34개의 C# 스크립트**로 구성되어 있으며, **컴포넌트 기반 모듈 설계**를 따릅니다."
>
> "핵심은 **11개의 주요 컴포넌트**입니다. Player.cs가 메인 컨트롤러로서 모든 것을 조율하고, PlayerModel은 데이터, PlayerView는 UI를 담당합니다."
>
> "이동, 대시, 공격 등 각 기능이 **독립된 컴포넌트**로 구현되어 있어 유지보수가 쉽고 확장이 용이합니다."

## 2분차: 상태 머신
> "플레이어는 **6가지 상태**로 관리됩니다. Idle, Move, Dash, 그리고 3가지 공격 상태죠."
>
> "**상태 머신 패턴**을 사용해 명확한 상태 전환을 구현했습니다. 예를 들어, 대시 중에는 다른 행동을 할 수 없고, 공격 중에도 마찬가지입니다."
>
> "각 상태는 Enter, Execute, Exit 메서드를 가지며, 상태 전환 시 안전하게 정리됩니다."

## 3분차: 전투 시스템
> "전투는 **5종의 무기**로 진행됩니다. Rifle, Shotgun, Blade, Laser, Plasma - 각각 다른 특성을 가지고 있습니다."
>
> "무기마다 **일반 공격과 특수 공격**이 있고, 추가로 **유탄 시스템**도 구현되어 있습니다. 유탄은 포물선 궤적으로 날아가 2초 후 폭발합니다."
>
> "입력은 **크로스 플랫폼**을 지원합니다. PC는 마우스와 키보드, 모바일은 가상 조이스틱과 버튼으로 동일한 경험을 제공합니다."

---

# 📌 Q&A 예상 질문

### Q1: 왜 상태 머신 패턴을 사용했나요?
**A**: 플레이어의 복잡한 행동을 명확하게 분리하고, 디버깅을 쉽게 하기 위해서입니다. Inspector에서 현재 상태를 실시간으로 확인할 수 있어 개발 중 큰 도움이 됩니다.

### Q2: 컴포넌트가 너무 많지 않나요?
**A**: 초기에는 복잡해 보일 수 있지만, 각 컴포넌트가 단일 책임만 가지므로 **유지보수가 훨씬 쉽습니다**. 예를 들어, 대시 기능을 수정할 때 PlayerDasher.cs만 보면 됩니다.

### Q3: 모바일 최적화는 어떻게 했나요?
**A**: 오브젝트 풀링으로 Instantiate/Destroy를 줄이고, 이벤트 기반 통신으로 Update 호출을 최소화했습니다. 또한 자동 조준 시스템을 추가해 터치 조작을 편하게 만들었습니다.

### Q4: 새로운 무기를 추가하기 어렵나요?
**A**: 매우 쉽습니다. PlayerNormalAttacker를 상속받아 새 클래스를 만들고, WeaponController에 등록만 하면 됩니다. 기존 코드는 전혀 수정하지 않아도 됩니다.

### Q5: 애니메이션과 상태는 어떻게 연동되나요?
**A**: PlayerAnimator 컴포넌트가 상태 변경 이벤트를 구독하고 있습니다. 상태가 바뀌면 자동으로 해당 애니메이션을 재생합니다.

---

# 🎯 결론

## 플레이어 시스템의 강점

✅ **명확한 아키텍처**: 상태 머신 + 컴포넌트 기반  
✅ **높은 확장성**: 새 기능 추가 용이  
✅ **낮은 결합도**: 이벤트 기반 통신  
✅ **크로스 플랫폼**: PC/Mobile 동시 지원  
✅ **성능 최적화**: 풀링, 캐싱 적용  

## 개선 계획

🔄 **AI 보조 시스템**: 모바일 자동 조준 개선  
🔄 **더 많은 무기**: 10종으로 확장  
🔄 **스킬 시너지**: 무기별 스킬 조합  
🔄 **애니메이션 강화**: IK, 리타게팅  

---

**발표 완료!**

이 자료는 3분 발표에 최적화되어 있으며, 각 슬라이드를 1분씩 설명하면 됩니다.

- **Slide 1**: 시스템 구조와 컴포넌트 소개
- **Slide 2**: 상태 머신 작동 원리
- **Slide 3**: 전투 시스템과 무기 특성

**추가 자료가 필요하면 PROJECT_PRESENTATION.md의 섹션 5를 참조하세요.**
