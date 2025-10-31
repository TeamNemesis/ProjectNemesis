# ProjectNemesis 팀원 빠른 시작 가이드

**대상**: 신규 팀원 또는 코드베이스에 익숙하지 않은 팀원  
**목적**: 30분 안에 프로젝트 이해 및 기여 시작

---

## 🚀 5분 만에 시작하기

### 1. 환경 설정 (한 번만)

```bash
# 1. 저장소 클론
git clone https://github.com/TeamNemesis/ProjectNemesis.git
cd ProjectNemesis

# 2. Unity Hub로 프로젝트 열기
# Unity 버전: 2022.3.5f1
# 프로젝트 경로 추가 → 열기
```

### 2. 첫 실행

```
1. Unity 에디터에서 Assets/01_Scenes/Play.unity 열기
2. Play 버튼 클릭
3. 게임 플레이 확인:
   - WASD: 이동
   - Shift: 대시
   - 마우스 좌클릭: 일반 공격
   - 마우스 우클릭: 특수 공격
   - G: 수류탄
```

### 3. 첫 번째 작업 시작

```bash
# 1. 새 브랜치 생성
git checkout -b feature/your-feature-name

# 2. 작업 진행
# ... 코드 수정 ...

# 3. 커밋 및 푸시
git add .
git commit -m "Add: your feature description"
git push origin feature/your-feature-name

# 4. Pull Request 생성
# GitHub에서 PR 생성 → 리뷰 요청
```

---

## 📂 프로젝트 구조 이해하기

### 핵심 폴더

```
Assets/
├── 01_Scenes/              ← 게임 씬
│   └── Play.unity          ← 메인 게임 씬
│
├── 02_Scripts/             ← 모든 C# 스크립트
│   ├── 00_Manager/         ← 게임 매니저들 (여기서 시작!)
│   ├── Player/             ← 플레이어 관련
│   ├── Monster/            ← 몬스터 관련
│   ├── Skill/              ← 스킬 시스템
│   ├── Map_Generate/       ← 맵 생성
│   └── InteractableObjects/ ← 상호작용 오브젝트
│
└── 03_Prefabs/             ← 프리팹
```

### 시작점 찾기

| 작업 내용 | 시작 파일 |
|----------|----------|
| 게임 전체 흐름 | `Assets/02_Scripts/00_Manager/GameManager.cs` |
| 플레이어 조작 | `Assets/02_Scripts/Player/Player.cs` |
| 몬스터 AI | `Assets/02_Scripts/Monster/MonsterBase.cs` |
| 스킬 추가 | `Assets/02_Scripts/Skill/SkillBase.cs` |
| 맵 생성 | `Assets/02_Scripts/Map_Generate/MapController.cs` |

---

## 🎯 담당별 작업 가이드

### endsun1234 (상호작용 시스템)

**주요 파일**:
```
Assets/02_Scripts/InteractableObjects/
Assets/02_Scripts/Interaction/
Assets/02_Scripts/00_Interfaces/IInteractable.cs
```

**시작 팁**:
1. `IInteractable` 인터페이스 이해하기
2. `InteractableObject` 베이스 클래스 확인
3. 기존 상호작용 오브젝트 예제 보기 (HealPack, WeaponInteractor 등)

**새 상호작용 오브젝트 추가하기**:
```csharp
// 1. IInteractable 구현
public class MyInteractableItem : InteractableObject
{
    public override void Interact()
    {
        // 상호작용 로직
        Debug.Log("플레이어가 아이템과 상호작용했습니다!");
    }
}

// 2. 프리팹 생성
// 3. InteractionController에 등록
```

---

### minji (캐릭터 움직임)

**주요 파일**:
```
Assets/02_Scripts/Player/Move/
Assets/02_Scripts/Player/StateMachine/
Assets/02_Scripts/Camera/
```

**시작 팁**:
1. `PlayerMover.cs` - 기본 이동 로직
2. `PlayerDasher.cs` - 대시 로직
3. State Machine 패턴 이해하기

**움직임 수정하기**:
```csharp
// PlayerMover.cs
[SerializeField] private float _moveSpeed = 5f;  // 이동 속도 조절
[SerializeField] private float _rotationSpeed = 10f;  // 회전 속도 조절

// PlayerDasher.cs
[SerializeField] private float _dashDistance = 5f;  // 대시 거리 조절
[SerializeField] private float _dashCooldown = 1f;  // 대시 쿨다운 조절
```

---

### hyunwoo (스킬 업그레이드 시스템)

**주요 파일**:
```
Assets/02_Scripts/Skill/
Assets/02_Scripts/00_Manager/SkillManager.cs
```

**시작 팁**:
1. `SkillBase.cs` - 스킬 베이스 클래스
2. `SkillManager.cs` - 스킬 관리
3. JSON 데이터 구조 이해하기

**새 스킬 추가하기**:
```csharp
// 1. SkillBase 상속
public class Skill_Six : SkillBase
{
    protected override void InitSkillDictionary()
    {
        // 스킬 데이터 초기화
    }
    
    public override void ApplySkill(SkillData skilldata)
    {
        // 스킬 효과 적용
    }
}

// 2. JSON 데이터 작성 (Resources/SkillData/)
// 3. SkillManager에 등록
```

---

## 🔧 자주 하는 작업들

### Unity에서 디버그 로그 보기

```csharp
// 일반 로그
Debug.Log("일반 메시지");

// 경고
Debug.LogWarning("경고 메시지");

// 에러
Debug.LogError("에러 메시지");

// 조건부 로그 (성능 고려)
#if UNITY_EDITOR
    Debug.Log("에디터에서만 출력");
#endif
```

### 새 프리팹 만들기

```
1. Hierarchy에서 GameObject 설정
2. 필요한 컴포넌트 추가
3. Project 창의 Prefabs 폴더로 드래그
4. 원본 GameObject 삭제
```

### 스크립트 컴파일 에러 해결

```
1. Console 창 확인 (Ctrl + Shift + C)
2. 에러 더블클릭 → 해당 코드로 이동
3. 문제 수정
4. 저장 (Ctrl + S)
5. Unity가 자동으로 재컴파일
```

---

## 💡 코딩 팁

### 네이밍 규칙 (중요!)

```csharp
// ✅ Good
public class PlayerController { }        // 클래스: PascalCase
public int MaxHealth { get; set; }       // 프로퍼티: PascalCase
private float _currentSpeed;             // private 필드: _camelCase
const int MAX_PLAYERS = 4;               // 상수: UPPER_CASE

// ❌ Bad - 사용하지 마세요
public class player_controller { }
public int maxhealth { get; set; }
private float CurrentSpeed;
```

### 주석 작성법

```csharp
/// <summary>
/// 이 메서드가 하는 일을 한 줄로 설명
/// </summary>
/// <param name="damage">입힐 데미지</param>
/// <returns>공격 성공 여부</returns>
public bool Attack(float damage)
{
    // 단순 설명은 // 사용
    if (damage <= 0) return false;
    
    // 복잡한 로직은 설명 추가
    // TODO: 크리티컬 히트 확률 추가
    ApplyDamage(damage);
    return true;
}
```

### SerializeField vs Public

```csharp
// ✅ Good - Inspector에 노출하되 외부 접근 차단
[SerializeField] private float _health;
public float Health => _health;  // 읽기 전용 프로퍼티

// ⚠️ Bad - 모든 곳에서 수정 가능
public float health;
```

---

## 🐛 문제 해결

### "Cannot find type or namespace"

```
원인: using 문 누락 또는 어셈블리 참조 문제
해결: 
1. 필요한 using 추가
   using UnityEngine;
   using System.Collections.Generic;
2. 스크립트가 올바른 폴더에 있는지 확인
```

### "NullReferenceException"

```
원인: 참조가 설정되지 않음
해결:
1. Inspector에서 참조 할당 확인
2. 코드에서 null 체크 추가
   if (myObject != null) { }
```

### "게임이 느려요"

```
해결:
1. Profiler 열기 (Ctrl + 7)
2. Play 버튼으로 게임 실행
3. CPU/GPU 사용량 확인
4. 문제 발견 시 팀에 공유
```

---

## 📞 도움 요청하기

### 질문하기 전 체크리스트

- [ ] 에러 메시지를 정확히 읽어봤나요?
- [ ] Google/Unity 문서를 검색해봤나요?
- [ ] 비슷한 코드를 프로젝트에서 찾아봤나요?
- [ ] 최소 15분 이상 시도해봤나요?

### 좋은 질문 예시

```
❌ Bad: "에러가 나요"
✅ Good: 
"PlayerMover.cs 45번째 줄에서 NullReferenceException이 발생합니다.
_characterController를 Inspector에서 할당했는데도 null로 나옵니다.
무엇이 문제일까요?"
```

### 도움 받을 곳

1. **팀 채팅** - 일반적인 질문
2. **GitHub Issues** - 버그 리포트
3. **Pull Request** - 코드 리뷰 요청
4. **팀 미팅** - 복잡한 기술 논의

---

## 📚 학습 리소스

### Unity 기초
- [Unity Learn](https://learn.unity.com/) - 공식 튜토리얼
- [Brackeys YouTube](https://www.youtube.com/user/Brackeys) - Unity 튜토리얼

### C# 기초
- [Microsoft C# 가이드](https://docs.microsoft.com/ko-kr/dotnet/csharp/)
- [C# 기초 강의](https://www.youtube.com/watch?v=GhQdlIFylQ8) (한글)

### 프로젝트 관련
- [CODE_EVALUATION.md](./CODE_EVALUATION.md) - 프로젝트 코드 평가
- [ARCHITECTURE_ANALYSIS.md](./ARCHITECTURE_ANALYSIS.md) - 아키텍처 상세 분석
- [IMPROVEMENT_ROADMAP.md](./IMPROVEMENT_ROADMAP.md) - 개선 계획

---

## ✅ 첫 주 목표

### Day 1
- [ ] 개발 환경 설정 완료
- [ ] 게임 실행 및 플레이 테스트
- [ ] 프로젝트 구조 파악

### Day 2-3
- [ ] 담당 시스템 코드 읽기
- [ ] 간단한 파라미터 수정 (속도, 데미지 등)
- [ ] Git 브랜치 생성 및 첫 커밋

### Day 4-5
- [ ] 첫 번째 기능 구현
- [ ] Pull Request 생성
- [ ] 코드 리뷰 참여

---

## 🎉 축하합니다!

이제 ProjectNemesis 프로젝트에 기여할 준비가 되었습니다!

질문이 있으면 언제든 팀원들에게 물어보세요. 
우리는 함께 성장하는 팀입니다! 💪

---

**작성**: 2025-10-31  
**유지보수**: 프로젝트 리더  
**피드백**: GitHub Issues로 제안해주세요
