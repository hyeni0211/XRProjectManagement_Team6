# Viven SDK Claude Code 개발 도구

Viven SDK 기반 Unity VR 프로젝트를 위한 Claude Code 개발 가이드 및 도구 패키지입니다.

## 개요

이 패키지는 Viven SDK 프로젝트 개발 시 Claude Code의 도움을 극대화하기 위한 도구 모음입니다.

### 포함 내용

- **CLAUDE.md**: SDK 아키텍처, 패턴, 규칙을 담은 메인 가이드
- **슬래시 커맨드**: 8개의 Viven 전용 커맨드
- **코드 스니펫**: 재사용 가능한 Lua 템플릿
- **WebFetch 설정**: 온라인 문서 실시간 조회 권한

## 설치 방법

### 1. 파일 복사

```bash
# viven-claude-tools 폴더를 프로젝트에 다운로드한 후:

# CLAUDE.md를 프로젝트 루트로 복사
cp viven-claude-tools/CLAUDE.md ./CLAUDE.md

# .claude 폴더를 프로젝트 루트로 복사
cp -r viven-claude-tools/.claude ./.claude

# (선택) snippets 폴더를 참조용으로 보관
cp -r viven-claude-tools/snippets ./viven-snippets
```

### 2. 설치 확인

프로젝트 루트에 다음 구조가 있어야 합니다:

```
YourProject/
├── CLAUDE.md                    # 메인 가이드
├── .claude/
│   ├── settings.local.json      # WebFetch 권한
│   └── commands/
│       └── viven/
│           ├── init.md
│           ├── lua-script.md
│           ├── grabbable.md
│           ├── component.md
│           ├── network.md
│           ├── step.md
│           ├── docs.md
│           └── troubleshoot.md
└── Assets/
    └── TwentyOz/
        └── VivenSDK/            # SDK 설치됨
```

## 사용 방법

### 슬래시 커맨드

Claude Code에서 다음 커맨드를 사용할 수 있습니다:

| 커맨드 | 설명 |
|--------|------|
| `/viven:init` | 새 프로젝트/오브젝트 초기화 |
| `/viven:lua-script` | Lua 스크립트 생성 |
| `/viven:grabbable` | Grabbable 오브젝트 설정 |
| `/viven:component` | 컴포넌트 추가 가이드 |
| `/viven:network` | 네트워크 동기화 설정 |
| `/viven:step` | IStep 게임 플로우 생성 |
| `/viven:docs [topic]` | 온라인 문서 조회 |
| `/viven:troubleshoot` | 문제 해결 가이드 |

### 문서 조회 예시

```
/viven:docs grabbable
/viven:docs vobject
/viven:docs scripting
```

### 코드 스니펫 활용

`snippets/lua/` 폴더의 템플릿을 참조하여 스크립트를 작성합니다:

- `basic-script.lua` - 기본 스크립트 구조
- `grabbable-handler.lua` - Grabbable 오브젝트 핸들러
- `step-manager.lua` - IStep 기반 게임 매니저
- `event-callbacks.lua` - 이벤트 콜백 시스템
- `pose-detector.lua` - 손 포즈/제스처 감지

## 주요 기능

### 1. SDK 아키텍처 가이드
- 네임스페이스 구조
- 핵심 컴포넌트 계층
- 필수 컴포넌트 조합

### 2. Lua 스크립팅 패턴
- 의존성 주입 (checkInject)
- 생명주기 함수
- 이벤트 핸들링
- 컴포넌트 접근

### 3. XR 기능
- 손 추적 API
- 햅틱 피드백
- 포즈/제스처 감지

### 4. 게임 플로우
- IStep 기반 단계 관리
- 타임라인 연동
- 이벤트 콜백 시스템

## 온라인 문서

- Wiki: https://wiki.viven.app/developer
- API Reference: https://sdkdoc.viven.app/api/SDK/TwentyOz.VivenSDK

## 프로젝트별 커스터마이징

프로젝트별 규칙을 추가하려면 CLAUDE.md 끝에 섹션을 추가하세요:

```markdown
---

## 프로젝트 특화 설정

### 커스텀 컴포넌트
- MyGameManager
- MyCustomObject

### 프로젝트 규칙
- Lua 스크립트는 Assets/MyProject/Scripts에 위치
- 모든 오브젝트는 특정 네이밍 규칙 준수
```

## 요구사항

- Unity 6000.0 이상
- Viven SDK 설치됨
- Claude Code CLI

## 라이선스

이 도구는 Viven SDK 프로젝트 개발을 위해 자유롭게 사용할 수 있습니다.
