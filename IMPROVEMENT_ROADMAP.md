# ProjectNemesis 개선 로드맵

**프로젝트**: TeamNemesis/ProjectNemesis  
**작성일**: 2025-10-31  
**목표**: 코드 품질 향상 및 유지보수성 개선

---

## 🎯 개선 목표

### 단기 목표 (1-2주)
1. ✅ 코드 평가 완료
2. 🔧 파일 인코딩 문제 해결
3. 📝 코딩 스타일 가이드 확립
4. 🐛 즉시 수정 가능한 버그 해결

### 중기 목표 (1개월)
1. 📖 문서화 보완
2. ♻️ 리팩터링 (중복 코드 제거)
3. 🧪 테스트 인프라 구축
4. 🔒 보안 강화

### 장기 목표 (3개월)
1. 🏗️ 아키텍처 개선
2. ⚡ 성능 최적화
3. 🚀 CI/CD 파이프라인
4. 📊 코드 품질 지표 모니터링

---

## 📅 Week 1: 즉시 개선 항목

### Day 1-2: 파일 인코딩 수정

**문제**: 한글 주석이 깨져서 표시됨 (ISO-8859-1 인코딩)

**해결 방법**:

#### 방법 1: Visual Studio 사용
```
1. Visual Studio에서 파일 열기
2. File > Advanced Save Options
3. Encoding: UTF-8 with signature (UTF-8 BOM)
4. 저장
```

#### 방법 2: Python 스크립트 (일괄 처리)
```python
# fix_encoding.py 사용 (프로젝트 루트에 제공됨)
python fix_encoding.py
```

**체크리스트**:
- [ ] Assets/02_Scripts 폴더 전체 변환
- [ ] 변환 후 Git diff로 주석 확인
- [ ] 커밋 전 Unity에서 컴파일 에러 확인
- [ ] 변환 결과를 별도 브랜치로 커밋

**예상 시간**: 2-3시간  
**담당**: 팀 전체 (각자 담당 파일)

---

### Day 3: 코딩 스타일 가이드 작성

**목표**: 일관된 코딩 스타일 확립

**가이드 내용**:

```csharp
// 1. 네이밍 컨벤션
// ✅ Good
public class PlayerController { }        // PascalCase for classes
public int MaxHealth { get; set; }       // PascalCase for properties
private float _currentSpeed;             // _camelCase for private fields
const int MAX_PLAYER_COUNT = 4;          // UPPER_CASE for constants

// ❌ Bad
public class player_controller { }
public int maxHealth { get; set; }
private float CurrentSpeed;

// 2. 주석
/// <summary>
/// 플레이어의 대시를 수행합니다.
/// </summary>
/// <returns>대시 성공 여부</returns>
public bool PerformDash() { }

// 3. 파일 구조
// - 네임스페이스 사용 권장
// - 한 파일에 하나의 public 클래스

// 4. 코드 조직
// public → protected → private 순서
// Properties → Methods 순서
```

**파일 생성**:
- [ ] CODING_STYLE_GUIDE.md 작성
- [ ] .editorconfig 파일 생성 (자동 포맷팅)

**예상 시간**: 3-4시간  
**담당**: 팀 리더 + 리뷰어 전원

---

### Day 4-5: 하드코딩 문자열 제거

**Before**:
```csharp
Resources.Load<SkillManager>("Prefabs/Skill/SkillManager");
Resources.Load<UIManager>("Prefabs/Skill/UIManager");
```

**After**:
```csharp
// Constants.cs 확장
public static class ResourcePaths
{
    public const string SKILL_MANAGER = "Prefabs/Skill/SkillManager";
    public const string UI_MANAGER = "Prefabs/Skill/UIManager";
    public const string PREFAB_PLAYER = "Prefabs/Player/Player";
    // ... 모든 경로 추가
}

// 사용
Resources.Load<SkillManager>(ResourcePaths.SKILL_MANAGER);
```

**체크리스트**:
- [ ] Constants.cs에 ResourcePaths 클래스 추가
- [ ] 모든 Resources.Load() 호출 찾기
- [ ] 하드코딩된 문자열을 상수로 변경
- [ ] 컴파일 및 테스트

**예상 시간**: 4-6시간  
**담당**: endsun1234 (상호작용 시스템 담당)

---

## 📅 Week 2: 문서화 및 리팩터링

### Day 1-3: XML 문서화 주석 추가

**우선순위**:
1. Manager 클래스 (GameManager, SkillManager 등)
2. Public API (인터페이스, public 메서드)
3. 복잡한 로직

**예시**:
```csharp
/// <summary>
/// 게임의 전역 상태를 관리하는 싱글톤 매니저입니다.
/// </summary>
/// <remarks>
/// DontDestroyOnLoad로 씬 전환 시에도 유지되며,
/// 모든 하위 매니저들의 생명주기를 관리합니다.
/// </remarks>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// GameManager의 싱글톤 인스턴스를 가져옵니다.
    /// </summary>
    /// <value>
    /// 게임에 하나만 존재하는 GameManager 인스턴스
    /// </value>
    public static GameManager Instance { get; }
    
    /// <summary>
    /// 모든 매니저를 초기화합니다.
    /// </summary>
    /// <remarks>
    /// 초기화 순서:
    /// 1. ResourceManager
    /// 2. DataManager
    /// 3. SkillManager
    /// 4. UIManager
    /// 5. 나머지 매니저들
    /// </remarks>
    void Initialize() { }
}
```

**체크리스트**:
- [ ] Manager 클래스 (9개)
- [ ] Player 시스템 주요 클래스
- [ ] Monster 시스템 주요 클래스
- [ ] Skill 시스템 주요 클래스
- [ ] Map 시스템 주요 클래스

**예상 시간**: 8-12시간 (팀원 분담)  
**담당**: 전체 팀 (각자 담당 시스템)

---

### Day 4-5: 중복 코드 리팩터링

**발견된 중복 패턴**:

#### 1. Shop 아이템 중복
```
HealPackInteractor ←→ HealPackInteractor_Shop
MutantPackInteractor ←→ MutantPackInteractor_Shop
TechSelectPackInteractor ←→ TechSelectPackInteractor_Shop
```

**개선안**:
```csharp
// Before: 2개의 클래스
public class HealPackInteractor : InteractableObject { }
public class HealPackInteractor_Shop : InteractableObject, IShopItem { }

// After: 1개의 클래스 + 설정
public class HealPackInteractor : InteractableObject, IShopItem 
{
    [SerializeField] private bool _isShopItem;
    [SerializeField] private int _price;
    
    public override void Interact()
    {
        if (_isShopItem)
        {
            if (!TryPurchase(_price)) return;
        }
        GiveReward();
    }
}
```

**체크리스트**:
- [ ] Shop 아이템 통합
- [ ] 테스트 및 검증

**예상 시간**: 4-6시간  
**담당**: endsun1234

---

## 📅 Week 3: 테스트 인프라 구축

### Day 1-2: Unity Test Framework 설정

**설치 및 설정**:
```
1. Window > Package Manager
2. Unity Registry > Test Framework 설치
3. Assets/Tests 폴더 생성
   - EditMode/  (비즈니스 로직 테스트)
   - PlayMode/  (게임플레이 통합 테스트)
```

**첫 번째 테스트 작성**:
```csharp
// Assets/Tests/EditMode/PlayerStatTests.cs
using NUnit.Framework;
using UnityEngine;

public class PlayerStatTests
{
    [Test]
    public void PlayerStat_TakeDamage_ReducesHealth()
    {
        // Arrange
        var player = new GameObject().AddComponent<PlayerModel>();
        player.SetMaxHealth(100);
        
        // Act
        player.TakeDamage(30, null);
        
        // Assert
        Assert.AreEqual(70, player.CurrentHealth);
    }
    
    [Test]
    public void PlayerStat_TakeDamage_CannotGoBelowZero()
    {
        // Arrange
        var player = new GameObject().AddComponent<PlayerModel>();
        player.SetMaxHealth(100);
        
        // Act
        player.TakeDamage(150, null);
        
        // Assert
        Assert.AreEqual(0, player.CurrentHealth);
    }
}
```

**체크리스트**:
- [ ] Test Framework 설치
- [ ] 테스트 폴더 구조 생성
- [ ] 예제 테스트 5개 작성
- [ ] 팀원 교육 (테스트 작성법)

**예상 시간**: 6-8시간  
**담당**: 팀 리더

---

### Day 3-5: 핵심 로직 단위 테스트

**우선순위 테스트 대상**:

1. **PlayerStatManager**
   - [ ] 스탯 증가/감소
   - [ ] 최대/최소값 제한
   - [ ] 스탯 초기화

2. **CurrencyManager**
   - [ ] 화폐 획득
   - [ ] 화폐 소비
   - [ ] 잔액 부족 시 처리

3. **SkillManager**
   - [ ] 스킬 획득
   - [ ] 스킬 레벨업
   - [ ] 스킬 활성화

4. **PoolManager**
   - [ ] 오브젝트 생성
   - [ ] 오브젝트 반환
   - [ ] 풀 확장

**목표**: 코드 커버리지 30% 이상

**예상 시간**: 12-16시간 (팀원 분담)  
**담당**: 전체 팀

---

## 📅 Week 4: 보안 및 최적화

### Day 1-2: PlayerPrefs 암호화

**현재 문제**:
```csharp
// 평문으로 저장 - 쉽게 수정 가능
PlayerPrefs.SetInt("PlayerLevel", 10);
PlayerPrefs.SetFloat("Currency", 1000f);
```

**개선**:
```csharp
// SecurePlayerPrefs 래퍼 클래스
public static class SecurePlayerPrefs
{
    private const string ENCRYPTION_KEY = "YOUR_SECRET_KEY";
    
    public static void SetInt(string key, int value)
    {
        string encrypted = Encrypt(value.ToString());
        PlayerPrefs.SetString(key, encrypted);
    }
    
    public static int GetInt(string key, int defaultValue = 0)
    {
        string encrypted = PlayerPrefs.GetString(key, null);
        if (string.IsNullOrEmpty(encrypted)) return defaultValue;
        
        string decrypted = Decrypt(encrypted);
        return int.Parse(decrypted);
    }
    
    private static string Encrypt(string data) { /* AES 암호화 */ }
    private static string Decrypt(string data) { /* AES 복호화 */ }
}
```

**체크리스트**:
- [ ] SecurePlayerPrefs 클래스 구현
- [ ] 기존 PlayerPrefs 호출 변경
- [ ] 암호화 키 안전하게 관리

**예상 시간**: 4-6시간  
**담당**: 팀 리더

---

### Day 3-5: 메모리 프로파일링 및 최적화

**도구**: Unity Profiler

**체크 항목**:
1. **GC Allocations**
   - 매 프레임 발생하는 GC 확인
   - string 연결 최소화
   - LINQ 사용 주의

2. **Physics**
   - Collision Matrix 최적화
   - Rigidbody 슬립 설정

3. **Rendering**
   - Draw Call 최적화
   - Batching 확인

**측정 및 개선**:
```
Before: FPS 측정 → 최적화 → After: FPS 측정
목표: 모바일 60 FPS, PC 144 FPS
```

**예상 시간**: 8-12시간  
**담당**: minji (움직임 담당), 팀 리더

---

## 🎯 성공 지표 (KPI)

### 코드 품질
- [ ] 인코딩 문제 해결: 100% (0/168 파일)
- [ ] 코딩 스타일 준수: 90% 이상
- [ ] XML 문서화: 주요 클래스 100%
- [ ] 테스트 커버리지: 30% 이상

### 성능
- [ ] PC 평균 FPS: 144+
- [ ] 모바일 평균 FPS: 60+
- [ ] 씬 로딩 시간: 3초 이하
- [ ] GC 발생: 초당 1회 이하

### 유지보수성
- [ ] 하드코딩 문자열: 0개
- [ ] 중복 코드: 50% 감소
- [ ] TODO 주석: 이슈 트래커로 이동
- [ ] 코드 리뷰: 모든 PR에 대해 실시

---

## 🔄 지속적 개선 프로세스

### 주간 루틴
```
월요일: 스프린트 계획 (1시간)
화~목요일: 개발 및 리뷰
금요일: 
  - 코드 리뷰 (2시간)
  - 회고 (1시간)
  - 다음 주 계획 (1시간)
```

### 코드 리뷰 체크리스트
```markdown
- [ ] 코딩 스타일 가이드 준수
- [ ] XML 문서화 주석 작성
- [ ] 단위 테스트 작성
- [ ] 하드코딩된 값 없음
- [ ] 성능 영향 확인
- [ ] 보안 이슈 없음
```

### 월간 검토
```
- 코드 품질 지표 리뷰
- 성능 프로파일링
- 기술 부채 평가
- 로드맵 업데이트
```

---

## 📚 참고 자료

### 내부 문서
- [CODE_EVALUATION.md](./CODE_EVALUATION.md) - 코드 평가 보고서
- [ARCHITECTURE_ANALYSIS.md](./ARCHITECTURE_ANALYSIS.md) - 아키텍처 분석
- [CODING_STYLE_GUIDE.md](./CODING_STYLE_GUIDE.md) - 코딩 스타일 가이드 (생성 예정)

### 외부 자료
- [Unity Best Practices](https://unity.com/how-to/best-practices-organizing-projects)
- [C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [Clean Code by Robert C. Martin](https://www.amazon.com/Clean-Code-Handbook-Software-Craftsmanship/dp/0132350882)

---

## ✅ 체크포인트

### 1주차 완료 기준
- [x] 코드 평가 문서 작성
- [ ] 파일 인코딩 변환 (0/168)
- [ ] 코딩 스타일 가이드 작성
- [ ] 하드코딩 문자열 정리

### 2주차 완료 기준
- [ ] XML 문서화 (주요 클래스)
- [ ] 중복 코드 제거
- [ ] 팀 리뷰 1회

### 3주차 완료 기준
- [ ] Test Framework 설정
- [ ] 단위 테스트 20개 이상
- [ ] 테스트 커버리지 30%

### 4주차 완료 기준
- [ ] 보안 개선 적용
- [ ] 성능 프로파일링
- [ ] 최적화 완료

---

**작성**: 2025-10-31  
**다음 업데이트**: 매주 금요일  
**책임자**: 팀 리더
