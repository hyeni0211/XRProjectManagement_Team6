# Viven SDK 프로젝트/오브젝트 초기화

Viven SDK 프로젝트 또는 새 오브젝트를 초기화합니다.

## 수행 작업

1. **프로젝트 구조 검증**
   - Assets/TwentyOz/VivenSDK 폴더 존재 확인
   - 필수 패키지 의존성 확인 (XR Management, OpenXR, XR Hands, URP)

2. **새 오브젝트 초기화 시**
   - VObject 컴포넌트 추가 안내
   - 필요한 컴포넌트 조합 안내
   - 기본 Lua 스크립트 템플릿 생성

## 초기화 체크리스트

### 프로젝트 수준
- [ ] Viven SDK 패키지 설치됨
- [ ] XR Plugin Management 설정됨
- [ ] OpenXR/Oculus 플러그인 활성화됨
- [ ] URP 렌더 파이프라인 설정됨
- [ ] Input System 설정됨

### 오브젝트 수준 (Grabbable)
- [ ] VObject 컴포넌트 추가
- [ ] VivenGrabbableModule 추가
- [ ] VivenRigidbodyControlModule 추가
- [ ] VivenGrabbableRigidView 추가
- [ ] Collider 설정
- [ ] Lua 스크립트 연결

## 기본 Lua 스크립트 템플릿

```lua
---[오브젝트 설명]

--region Injection list
local _INJECTED_ORDER = 0
local function checkInject(OBJECT)
    _INJECTED_ORDER = _INJECTED_ORDER + 1
    assert(OBJECT, _INJECTED_ORDER .. "th object is missing")
    return OBJECT
end

---@type GameObject
---@details 설명
TargetObject = checkInject(TargetObject)
--endregion

--region Variables
---@type VivenGrabbableModule
local grabbableModule = nil
--endregion

--region Unity Lifecycle
function awake()
    grabbableModule = self:GetComponent("VivenGrabbableModule")
end

function start()
end

function onEnable()
end

function onDisable()
end
--endregion

--region Interaction Events
function onGrab()
end

function onRelease()
end
--endregion
```

사용자의 요청에 따라 위 체크리스트를 확인하고, 필요한 설정이나 코드를 생성해주세요.
