# Viven SDK 문제 해결 가이드

일반적인 Viven SDK 문제와 해결 방법입니다.

## 일반적인 오류

### 1. "object is missing" 오류

**원인**: checkInject에서 Inspector 주입이 누락됨

**해결**:
1. Unity Inspector에서 해당 오브젝트 할당 확인
2. 주입 순서 확인 (오류 메시지의 숫자)
3. NullableInject 사용 고려 (선택적 주입)

```lua
-- 필수 주입
ObjectName = checkInject(ObjectName)

-- 선택적 주입 (없어도 됨)
OptionalObject = NullableInject(OptionalObject)
```

### 2. GetComponent 반환값 nil

**원인**: 컴포넌트가 오브젝트에 없음

**해결**:
1. Unity Inspector에서 컴포넌트 추가 확인
2. 컴포넌트 이름 철자 확인 (대소문자 구분)
3. awake()에서 GetComponent 호출 확인

```lua
function awake()
    local component = self:GetComponent("VivenGrabbableModule")
    if component == nil then
        Debug.LogError("VivenGrabbableModule not found!")
    end
end
```

### 3. 이벤트 리스너 호출 안됨

**원인**: onEnable에서 등록 안됨 또는 onDisable에서 해제됨

**해결**:
```lua
function onEnable()
    -- 반드시 여기서 등록
    component.event:AddListener(handlerFunction)
end

function onDisable()
    -- 반드시 여기서 해제
    component.event:RemoveListener(handlerFunction)
end
```

### 4. Grabbable 오브젝트 잡기 불가

**원인**: 필수 컴포넌트 누락

**해결**:
1. VObject 컴포넌트 확인
2. VivenGrabbableModule 확인
3. VivenRigidbodyControlModule 확인
4. VivenGrabbableRigidView 확인
5. Collider 확인 (isTrigger = false)
6. Rigidbody 확인

### 5. 네트워크 동기화 안됨

**원인**: View 컴포넌트 누락 또는 VObject 설정 문제

**해결**:
1. VObject 컴포넌트 확인
2. 적절한 View 컴포넌트 추가:
   - 일반: VivenTransformView
   - Grabbable: VivenGrabbableRigidView
3. objectSyncType 확인 (Continuous 권장)

### 6. Hand Tracking 포즈 감지 안됨

**원인**: VivenPoseOrGestureInteraction 설정 문제

**해결**:
1. detectHandType 확인 (Both/Left/Right)
2. VivenHandPose ScriptableObject 할당 확인
3. 이벤트 리스너 등록 확인

```lua
poseDetector.onPoseOrGesturePerformed:AddListener(onPoseDetected)
```

### 7. 햅틱 피드백 작동 안함

**원인**: 모드 확인 안함 또는 잘못된 API 사용

**해결**:
```lua
if XRHandAPI.GetHandTrackingMode() == "None" then
    -- 컨트롤러 모드
    XR.StartControllerVibration(false, 0.5, 0.1)
else
    -- 장갑 모드
    HandTracking.CommandVibrationHaptic(0.05, 50, Handedness.Right, FingerType.Index, false)
end
```

### 8. 코루틴 오류

**원인**: xlua.util 미사용

**해결**:
```lua
local util = require 'xlua.util'

local routine = self:StartCoroutine(util.cs_generator(function()
    coroutine.yield(WaitForSeconds(1.0))
    -- 1초 후 실행
end))
```

### 9. 타입 오류 (C# 타입 접근)

**원인**: typeof 또는 CS 네임스페이스 미사용

**해결**:
```lua
-- 올바른 방법
local mesh = self:GetComponentInChildren(typeof(MeshRenderer))
local colliders = self:GetComponentsInChildren(typeof(CS.UnityEngine.Collider))
local mode = CS.UnityEngine.CollisionDetectionMode.ContinuousDynamic
```

### 10. IStep 콜백 실행 안됨

**원인**: 콜백 테이블 형식 오류

**해결**:
```lua
-- 매개변수 없는 콜백
{ myFunction }

-- 매개변수 있는 콜백
{ { myFunction, param1, param2 } }
```

## 디버깅 팁

### Debug.Log 사용
```lua
Debug.Log("메시지")
Debug.LogWarning("경고")
Debug.LogError("오류")
```

### 변수 상태 확인
```lua
Debug.Log("변수값: " .. tostring(variable))
Debug.Log("테이블 길이: " .. #myTable)
```

### nil 체크
```lua
if component ~= nil then
    -- 안전하게 사용
end
```

문제 상황을 설명해주시면 해결 방법을 안내해드립니다.
