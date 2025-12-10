# Viven 네트워크 동기화 설정

Viven SDK의 네트워크 동기화 설정 가이드입니다.

## VObject 기본 설정

```
displayName: "오브젝트 표시 이름"
objectId: (GUID 자동 생성, 수정 금지)
contentType: Prepared / VObject / MapContent
objectSyncType: Continuous / OnChanged / Manual
```

### Content Type
- **Prepared**: 맵과 함께 로드되는 오브젝트
- **VObject**: 별도로 업로드하여 모든 맵에서 사용
- **MapContent**: VMap 빌드에 포함

### Sync Type
- **Continuous**: 매 틱마다 동기화 (실시간 상호작용)
- **OnChanged**: 값 변경 시에만 동기화 (네트워크 효율적)
- **Manual**: 개발자가 수동으로 동기화 호출

## View 컴포넌트

### VivenTransformView
- Transform (Position, Rotation, Scale) 동기화
- 일반 오브젝트용

### VivenRigidbodyView
- Rigidbody Transform 동기화
- 물리 오브젝트용

### VivenGrabbableRigidView
- Grabbable 오브젝트의 Rigidbody 동기화
- 잡기 가능 오브젝트 전용

## 동기화 흐름

```
Local VObject 상태 변경
    ↓
View 컴포넌트가 변경 감지
    ↓
SyncType에 따라 동기화 결정
    ↓
서버로 전송
    ↓
다른 클라이언트가 수신 및 적용
```

## Lua에서 네트워크 상태 확인

```lua
-- VObject 접근
local vObject = self:GetComponent("VObject")

-- 동기화 타입 확인
local syncType = vObject.objectSyncType

-- Interacting Interactor 확인 (잡은 사람)
if grabbableModule.InteractingInteractor ~= nil then
    local interactorTransform = grabbableModule.InteractingInteractor.InteractingTransform
end
```

## 네트워크 이벤트 처리

```lua
-- 로컬에서만 실행할 코드
if isLocalPlayer then
    -- 로컬 플레이어 전용 로직
end

-- 모든 클라이언트에서 실행
function onGrab()
    -- 네트워크 동기화됨
end
```

## 주의사항

1. **objectId 수정 금지**: 에디터에서 자동 생성됨
2. **View 컴포넌트 필수**: 동기화가 필요한 오브젝트에 반드시 추가
3. **Continuous vs OnChanged**: 실시간 상호작용은 Continuous, 그 외는 OnChanged 권장
4. **네트워크 부하 고려**: 너무 많은 Continuous 오브젝트는 피하기

사용자의 요청에 따라 적절한 네트워크 설정을 안내해주세요.
