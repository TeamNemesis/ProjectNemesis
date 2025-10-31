# ProjectNemesis

TeamNemesis의 ProjectNemesis 작업용 Repository입니다.

프로젝트 노션 링크 : https://economic-kettle-c2e.notion.site/26fc01e9d6ba80b498dde6d3fc2cc36e?source=copy_link

---

# Project Nemesis

> Unity 기반의 로그라이크 액션 게임 프로젝트

## 📌 프로젝트 소개

본 프로젝트는 Unity를 활용한 로그라이크 장르의 액션 게임으로, 다양한 스킬 조합과 무작위 맵 생성을 통해 높은 재플레이 가치를 제공합니다.

### 주요 특징
- 🎮 **다양한 전투 시스템**: 3가지 무기 타입 (Rifle, Blade, HackingDevice)
- ⚡ **풍부한 스킬 시스템**: 5개 이상의 스킬 트리
- 🗺️ **절차적 맵 생성**: 매번 다른 던전 경험
- 👾 **다양한 적 타입**: 일반 몬스터, 엘리트, 보스
- 🎯 **상호작용 시스템**: 무기, 아이템, 상점 등

## 📊 프로젝트 현황

- **코드 규모**: ~19,000 줄, 168개 C# 파일
- **전체 평가**: ⭐⭐⭐⭐ (4.2/5)
- **개발 상태**: 활발한 개발 중

자세한 평가는 [코드 평가 보고서](./CODE_EVALUATION.md)를 참조하세요.

## 📚 문서

### 신규 팀원
- 📖 [빠른 시작 가이드](./QUICK_START_GUIDE.md) - **여기서 시작하세요!**

### 개발자
- 🏗️ [아키텍처 분석](./ARCHITECTURE_ANALYSIS.md) - 시스템 구조 상세 설명
- 📋 [코드 평가 보고서](./CODE_EVALUATION.md) - 강점과 개선점 분석
- 🛣️ [개선 로드맵](./IMPROVEMENT_ROADMAP.md) - 향후 개선 계획

### 도구
- 🔧 [인코딩 수정 스크립트](./fix_encoding.py) - UTF-8 BOM 변환 도구

## 🗂️ 폴더 구조

```
Assets/
├── 01_Scenes/              # 게임 씬
│   └── Play.unity          # 메인 게임 씬
├── 02_Scripts/             # C# 스크립트
│   ├── 00_Constants/       # 상수 및 이벤트버스
│   ├── 00_Interfaces/      # 인터페이스
│   ├── 00_Manager/         # 게임 매니저들
│   ├── Player/             # 플레이어 시스템
│   ├── Monster/            # 몬스터 시스템
│   ├── Skill/              # 스킬 시스템
│   ├── Map_Generate/       # 맵 생성
│   └── InteractableObjects/ # 상호작용 오브젝트
├── 03_Prefabs/             # 프리팹
└── 99_StoreAssets/         # 외부 에셋
```

## 🚀 시작하기

### 요구사항
- **Unity 버전**: `2022.3.5f1` (필수)
- **OS**: Windows 10/11, macOS 10.15+
- **RAM**: 8GB 이상 권장

### 설치 방법

1. **저장소 클론**
   ```bash
   git clone https://github.com/TeamNemesis/ProjectNemesis.git
   cd ProjectNemesis
   ```

2. **Unity Hub로 프로젝트 열기**
   - Unity Hub 실행
   - "Add" → 클론한 폴더 선택
   - Unity 2022.3.5f1로 열기

3. **게임 실행**
   - `Assets/01_Scenes/Play.unity` 씬 열기
   - Play 버튼 클릭

### 게임 조작법
- **WASD**: 이동
- **Shift**: 대시
- **마우스 좌클릭**: 일반 공격
- **마우스 우클릭**: 특수 공격
- **G**: 수류탄
- **E**: 상호작용

## 💡 기술 스택

### 핵심 기술
- **엔진**: Unity 2022.3.5f1 (LTS)
- **언어**: C# 9.0
- **AI**: Unity NavMesh
- **물리**: Unity Physics

### 외부 패키지
- TextMesh Pro - UI 텍스트
- Unity Input System - 입력 처리
- Newtonsoft.Json - 데이터 직렬화
- Particle Pack - 이펙트

### 디자인 패턴
- Singleton (GameManager)
- State Machine (Player, Monster)
- Object Pool (Monster, Projectile)
- Observer (Event Bus)
- Component (모듈화)

## 👥 팀원

| 이름 | 담당 | 주요 작업 |
|------|------|----------|
| **endsun1234** | 상호작용 시스템 | InteractableObject, Shop, Reward |
| **minji** | 캐릭터 움직임 | Player Movement, Dash, Camera |
| **hyunwoo** | 스킬 시스템 | Skill Tree, Upgrades, Effects |

## 🤝 기여 방법

### 브랜치 전략
```
main              - 배포 가능한 안정 버전
develop           - 개발 통합 브랜치
feature/xxx       - 새 기능 개발
bugfix/xxx        - 버그 수정
hotfix/xxx        - 긴급 패치
```

### 워크플로우
1. **이슈 생성** - GitHub Issues에 작업 내용 등록
2. **브랜치 생성** - `feature/기능명` 형식
3. **개발 진행** - 작은 단위로 자주 커밋
4. **Pull Request** - 코드 리뷰 요청
5. **리뷰 & 머지** - 승인 후 develop에 병합

### 커밋 메시지 규칙
```
Add: 새 기능 추가
Fix: 버그 수정
Update: 기능 개선
Remove: 코드/파일 삭제
Docs: 문서 수정
Style: 코드 포맷팅
Refactor: 리팩터링
Test: 테스트 추가/수정
```

### 코드 리뷰 체크리스트
- [ ] 코딩 스타일 가이드 준수
- [ ] XML 문서화 주석 작성
- [ ] 하드코딩된 값 없음
- [ ] 컴파일 에러 없음
- [ ] 게임 플레이 테스트 완료

## 🐛 버그 리포트

버그를 발견하셨나요? [GitHub Issues](https://github.com/TeamNemesis/ProjectNemesis/issues)에 다음 정보와 함께 등록해주세요:

- **제목**: [Bug] 간단한 설명
- **재현 방법**: 단계별 설명
- **예상 동작**: 어떻게 작동해야 하는지
- **실제 동작**: 현재 어떻게 작동하는지
- **환경**: Unity 버전, OS 등

## 📝 라이선스

이 프로젝트는 교육 및 포트폴리오 목적으로 제작되었습니다.

---

## 🔗 링크

- 📘 [프로젝트 노션](https://economic-kettle-c2e.notion.site/26fc01e9d6ba80b498dde6d3fc2cc36e?source=copy_link)
- 💬 [팀 디스코드](#) (추가 예정)
- 🎮 [플레이 영상](#) (추가 예정)

---

**Last Updated**: 2025-10-31  
**Version**: 0.1.0 (Alpha)
