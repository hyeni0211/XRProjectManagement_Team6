# Viven Lua 스크립트 생성

Viven SDK용 Lua 스크립트를 생성합니다.

## 생성 시 필수 패턴

### 1. 의존성 주입 (checkInject)
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
ObjectName = checkInject(ObjectName)
--endregion
```

### 2. 타입 어노테이션
```lua
---@type VivenGrabbableModule
local grabbableModule = nil

---@type boolean
local isActive = false

---@type number
local count = 0

---@type string
local name = ""

---@type table
local items = {}

---@type Vector3
local position = Vector3.zero

---@type Transform
local targetTransform = nil
```

### 3. 생명주기 함수
```lua
function awake()    -- GetComponent 호출
function start()    -- 초기 설정
function onEnable() -- 이벤트 리스너 등록
function onDisable() -- 이벤트 리스너 해제
function update()    -- 프레임 업데이트
function fixedUpdate() -- 물리 업데이트
```

### 4. 이벤트 함수
```lua
function onGrab()
function onRelease()
function onTriggerEnter(other)
function onTriggerExit(other)
function onCollisionEnter(collision)
function onCollisionExit(collision)
```

### 5. 이벤트 리스너 등록/해제
```lua
function onEnable()
    component.event:AddListener(handlerFunction)
end

function onDisable()
    component.event:RemoveListener(handlerFunction)
end
```

## 스크립트 유형별 템플릿

### Manager 스크립트
- 게임 전체 흐름 관리
- 여러 하위 스크립트 참조
- 공개 함수로 상태 관리

### Object 스크립트
- 개별 오브젝트 동작
- Grabbable 이벤트 처리
- 물리/충돌 처리

### UI 스크립트
- UI 상태 관리
- 사용자 입력 처리

## 컴포넌트 접근 방법

```lua
-- C# 컴포넌트
self:GetComponent("ComponentName")
object:GetComponent("ComponentName")

-- Lua 스크립트
self:GetLuaComponent("ScriptName")
self:GetLuaComponentInChildren("ScriptName")
self:GetLuaComponentInParent("ScriptName")
object:GetLuaComponent("ScriptName")

-- 타입 지정
self:GetComponentInChildren(typeof(MeshRenderer))
self:GetComponentsInChildren(typeof(CS.UnityEngine.Collider))
```

사용자가 원하는 스크립트 유형과 기능을 파악하여 적절한 템플릿을 생성해주세요.
