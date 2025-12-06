# Viven SDK 프로젝트 개발 가이드

이 프로젝트는 TwentyOz의 Viven SDK 기반 Unity VR 메타버스 애플리케이션입니다.

## 온라인 문서

- **Wiki**: https://wiki.viven.app/developer
- **API Reference**: https://sdkdoc.viven.app/api/SDK/TwentyOz.VivenSDK
- **VObject 가이드**: https://wiki.viven.app/developer/contents/vobject
- **Grabbable 가이드**: https://wiki.viven.app/developer/contents/grabbable
- **Scripting 가이드**: https://wiki.viven.app/developer/dev-guide/viven-script

## 핵심 아키텍처

### 네임스페이스 구조
```
TwentyOz.VivenSDK                    # SDK 코어 API
TwentyOz.VivenSDK.Scripts.Core.Lua   # Lua 바인딩
Twoz.Viven.Interactions              # 상호작용 컴포넌트
Twoz.Viven.HandTracking              # 손 추적 시스템
```

### 핵심 컴포넌트 계층
```
VObject (기반, 네트워크 동기화)
    ↓
VivenGrabbableModule (잡기 가능)
    + VivenRigidbodyControlModule (물리 제어)
    + VivenGrabbableRigidView (네트워크 뷰)
    ↓
VivenLuaBehaviour (Lua 스크립트 실행)
```

### 필수 컴포넌트 조합

| 기능 | 필수 컴포넌트 |
|------|--------------|
| 네트워크 동기화 | VObject + VivenTransformView |
| 잡기 가능 | VObject + VivenGrabbableModule + VivenRigidbodyControlModule + VivenGrabbableRigidView |
| 앉기 가능 | VObject + VivenSittable + Collider |
| 탑승 가능 | VObject + VivenRidableModule + VivenCustomAnimationModule |

---

## Lua 스크립팅

### 의존성 주입 패턴 (필수)
```lua
--region Injection list
local _INJECTED_ORDER = 0
local function checkInject(OBJECT)
    _INJECTED_ORDER = _INJECTED_ORDER + 1
    assert(OBJECT, _INJECTED_ORDER .. "th object is missing")
    return OBJECT
end
local function NullableInject(OBJECT)
    _INJECTED_ORDER = _INJECTED_ORDER + 1
    if OBJECT == nil then
        Debug.Log(_INJECTED_ORDER .. "th object is missing")
    end
    return OBJECT
end

---@type GameObject
---@details 설명
TargetObject = checkInject(TargetObject)
--endregion
```

### 생명주기 함수
```lua
function awake()    -- 초기화 (GetComponent 호출)
function start()    -- 시작 처리
function onEnable() -- 이벤트 리스너 등록
function onDisable() -- 이벤트 리스너 해제
function update()    -- 프레임 업데이트
function fixedUpdate() -- 물리 업데이트
```

### 상호작용 이벤트
```lua
function onGrab()           -- 물체 잡음
function onRelease()        -- 물체 놓음
function onTriggerEnter(other) -- 트리거 진입
function onTriggerExit(other)  -- 트리거 탈출
```

### 컴포넌트 접근
```lua
-- C# 컴포넌트 가져오기
local grabbable = self:GetComponent("VivenGrabbableModule")
local rigidbody = self:GetComponent("VivenRigidbodyControlModule")

-- Lua 스크립트 가져오기
local manager = self:GetLuaComponent("GameManager")
local child = self:GetLuaComponentInChildren("ChildScript")
local parent = self:GetLuaComponentInParent("ParentScript")

-- 외부 오브젝트 컴포넌트
local comp = targetObject:GetComponent("ComponentName")
local luaComp = targetObject:GetLuaComponent("ScriptName")

-- 타입 지정 컴포넌트
local mesh = self:GetComponentInChildren(typeof(MeshRenderer))
local colliders = self:GetComponentsInChildren(typeof(CS.UnityEngine.Collider))
```

### 이벤트 리스너 등록/해제
```lua
function onEnable()
    grabbableModule.onGrabEvent:AddListener(onGrab)
    grabbableModule.onReleaseEvent:AddListener(onRelease)
    poseDetector.onPoseOrGesturePerformed:AddListener(onPoseDetected)
end

function onDisable()
    grabbableModule.onGrabEvent:RemoveListener(onGrab)
    grabbableModule.onReleaseEvent:RemoveListener(onRelease)
    poseDetector.onPoseOrGesturePerformed:RemoveListener(onPoseDetected)
end
```

### 모듈 가져오기
```lua
-- xlua 유틸리티 (코루틴 등)
local util = require 'xlua.util'

-- Lua 스크립트 모듈 가져오기
MyCallback = ImportLuaScript(EventCallbacks)
IStep = ImportLuaScript(IStep)
```

### 코루틴 사용
```lua
local util = require 'xlua.util'

local routine = nil

function startRoutine()
    routine = self:StartCoroutine(util.cs_generator(function()
        coroutine.yield(WaitForSeconds(1.0))
        -- 1초 후 실행
        Debug.Log("1초 경과")
    end))
end

function stopRoutine()
    if routine ~= nil then
        self:StopCoroutine(routine)
        routine = nil
    end
end
```

---

## XR 손 추적 API

### 네임스페이스
```lua
XRHandAPI = CS.TwentyOz.VivenSDK.ExperimentExtension.Scripts.API.Experiment.XRHandAPI
InteractionAPI = CS.TwentyOz.VivenSDK.ExperimentExtension.Scripts.API.Experiment.InteractionAPI
Handedness = CS.TwentyOz.VivenSDK.Scripts.Core.Haptic.DataModels.SDKHandedness
FingerType = CS.TwentyOz.VivenSDK.Scripts.Core.Haptic.DataModels.SDKFingerType
```

### 손 추적 모드 확인
```lua
local mode = XRHandAPI.GetHandTrackingMode()
-- "None": 컨트롤러 모드
-- "BHaptics": 비햅틱스 장갑 모드
```

### 강제 잡기
```lua
XRHandAPI.ForceGrabHandTracking(grabbableModule, isLeftHand)
```

### Interactor 상태 확인
```lua
local count = InteractionAPI.GetVerifiedColsCount(grabbableModule)
```

---

## 햅틱 피드백

### 컨트롤러 진동
```lua
-- XR.StartControllerVibration(isLeftHand, intensity, duration)
XR.StartControllerVibration(false, 0.6, 0.1) -- 오른손
XR.StartControllerVibration(true, 0.1, 0.1)  -- 왼손
```

### 비햅틱스 장갑 진동
```lua
-- HandTracking.CommandVibrationHaptic(intensity, duration, handedness, fingerType, isWaveform)
HandTracking.CommandVibrationHaptic(0.09, 50, Handedness.Right, FingerType.Index, false)
HandTracking.CommandVibrationHaptic(0.02, 50, Handedness.Left, FingerType.Thumb, false)
```

### 손가락 타입
- `FingerType.Thumb` - 엄지
- `FingerType.Index` - 검지
- `FingerType.Middle` - 중지
- `FingerType.Ring` - 약지
- `FingerType.Little` - 소지

---

## IStep 게임 플로우 패턴

타임라인 기반 단계별 진행 시스템:

```lua
IStep = ImportLuaScript(IStep)

-- IStep 인스턴스 생성
local step1 = IStep:new(
    { activeObj1, activeObj2 },     -- 활성화할 오브젝트
    { inactiveObj1, inactiveObj2 }, -- 비활성화할 오브젝트
    0.0,                            -- 타임라인 시간
    1,                              -- 스텝 번호
    "cutComplete",                  -- 클리어 조건
    { onStepStartFunc },            -- 시작 콜백
    { onStepResetFunc },            -- 리셋 콜백
    { onStepSkipFunc }              -- 스킵 콜백
)

-- 매개변수가 있는 콜백
local step2 = IStep:new(
    nil, nil, 5.0, 2, "grabComplete",
    { { myFunction, param1, param2 } }, -- {함수, 매개변수1, 매개변수2}
    nil, nil
)
```

### IStep 메서드
- `step:OnStepStart()` - 스텝 시작 (오브젝트 활성화/비활성화 + 콜백 실행)
- `step:OnStepClear()` - 스텝 완료
- `step:OnStepReset()` - 스텝 리셋
- `step:OnStepSkip()` - 스텝 스킵

---

## 개발 규칙

### Lua 스크립트 작성 규칙
1. 항상 `checkInject` 패턴으로 의존성 주입
2. `---@type` 타입 어노테이션 사용
3. `onEnable`에서 이벤트 등록, `onDisable`에서 해제
4. 지역 변수는 `local` 키워드 사용
5. 리전 주석 (`--region`, `--endregion`) 으로 코드 구조화

### 컴포넌트 접근 규칙
1. `awake()`에서 모든 GetComponent 호출
2. `start()`에서 초기 설정 수행
3. Rigidbody 직접 접근 금지 → `VivenRigidbodyControlModule` 사용

### 네트워크 규칙
1. VObject의 `objectId`는 자동 생성됨
2. `contentType`: Prepared (맵과 함께 로드)
3. `objectSyncType`: Continuous (지속적 동기화)

---

## 슬래시 커맨드

- `/viven:init` - 새 Viven 오브젝트 초기화
- `/viven:lua-script` - Lua 스크립트 생성
- `/viven:grabbable` - Grabbable 오브젝트 설정
- `/viven:component` - 컴포넌트 추가 가이드
- `/viven:network` - 네트워크 동기화 설정
- `/viven:step` - IStep 기반 게임 플로우 생성
- `/viven:docs [topic]` - 온라인 문서 조회
- `/viven:troubleshoot` - 문제 해결 가이드

---

## 프로젝트 구조

```
Assets/
├── TwentyOz/
│   ├── VivenSDK/           # SDK 코어
│   │   ├── Scripts/Core/   # C# 코어 스크립트
│   │   └── Client/         # 클라이언트 컴포넌트
│   └── Settings/           # 품질/렌더링 설정
├── [ProjectName]/
│   ├── Scripts/            # Lua 스크립트
│   │   ├── Manager/        # 게임 매니저
│   │   ├── Objects/        # 상호작용 오브젝트
│   │   ├── UI/             # UI 스크립트
│   │   └── Utils/          # 유틸리티
│   ├── Prefabs/            # 프리팹
│   ├── Scenes/             # 씬 파일
│   └── Models/             # 3D 모델
└── Plugins/                # 외부 플러그인
```

---

## 필수 패키지 의존성

```
com.unity.xr.management: 4.5.0+
com.unity.xr.openxr: 1.14.0+
com.unity.xr.hands: 1.5.0+
com.unity.xr.interaction.toolkit: 3.0.7+
com.unity.render-pipelines.universal: 17.0.4+
com.unity.inputsystem: 1.13.1+
com.unity.timeline: 1.8.7+
```
