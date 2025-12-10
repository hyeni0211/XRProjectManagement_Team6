# Viven SDK 문서 조회

Viven SDK 온라인 문서를 실시간으로 조회합니다.

## 사용법

```
/viven:docs [topic]
```

## 조회 가능한 주제

| 주제 | URL | 설명 |
|------|-----|------|
| developer | https://wiki.viven.app/developer | 개발자 가이드 전체 |
| vobject | https://wiki.viven.app/developer/contents/vobject | VObject 제작법 |
| grabbable | https://wiki.viven.app/developer/contents/grabbable | Grabbable 제작법 |
| scripting | https://wiki.viven.app/developer/dev-guide/viven-script | Lua 스크립팅 가이드 |
| interaction | https://wiki.viven.app/developer/dev-guide/interaction | 상호작용 가이드 |
| network | https://wiki.viven.app/developer/dev-guide/network | 네트워크 가이드 |
| timeline | https://wiki.viven.app/developer/dev-guide/timeline | 타임라인 가이드 |
| api | https://sdkdoc.viven.app/api/SDK/TwentyOz.VivenSDK | API 레퍼런스 |

## 조회 방법

이 커맨드가 실행되면 WebFetch 도구를 사용하여 해당 문서를 조회합니다.

### 예시 요청

사용자: `/viven:docs grabbable`

응답:
1. https://wiki.viven.app/developer/contents/grabbable 페이지 조회
2. 핵심 내용 요약 제공
3. 관련 코드 예제 추출
4. 추가 참조 링크 안내

## 문서 조회 시 제공 정보

1. **개요**: 해당 기능의 목적과 용도
2. **필수 컴포넌트**: 필요한 Unity 컴포넌트 목록
3. **설정 방법**: Inspector 설정 가이드
4. **코드 예제**: Lua/C# 예제 코드
5. **주의사항**: 일반적인 실수와 해결 방법

## WebFetch 권한

이 기능을 사용하려면 `.claude/settings.local.json`에 다음 권한이 필요합니다:

```json
{
  "permissions": {
    "allow": [
      "WebFetch(domain:wiki.viven.app)",
      "WebFetch(domain:sdkdoc.viven.app)"
    ]
  }
}
```

사용자가 요청한 주제의 문서를 WebFetch로 조회하고 핵심 내용을 정리해주세요.
