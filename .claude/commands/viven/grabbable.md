# Viven Grabbable 오브젝트 설정

VR에서 잡을 수 있는 오브젝트를 설정합니다.

## 필수 컴포넌트

1. **VObject** - 네트워크 동기화 기반
2. **VivenGrabbableModule** - 잡기 기능
3. **VivenRigidbodyControlModule** - 물리 제어
4. **VivenGrabbableRigidView** - 네트워크 뷰
5. **Collider** (BoxCollider, SphereCollider 등)
6. **Rigidbody**

## VivenGrabbableModule 설정

### Grab Type
- `Kinematic` - 물리 무시, 손에 고정
- `Velocity` - 물리 적용, 자연스러운 움직임

### 주요 속성
```
grabType: Velocity (권장)
parentToHandOnGrab: true (잡을 때 손의 자식으로)
throwForce: 5.0 (던지기 힘)
holdTimeThreshold: 1.0 (Hold 인식 시간)
```

## Lua 스크립트 템플릿

```lua
---[Grabbable 오브젝트 설명]

--region Injection list
local _INJECTED_ORDER = 0
local function checkInject(OBJECT)
    _INJECTED_ORDER = _INJECTED_ORDER + 1
    assert(OBJECT, _INJECTED_ORDER .. "th object is missing")
    return OBJECT
end

---@type string
---@details 게임 매니저 스크립트 이름
gameManagerName = checkInject(gameManagerName)
--endregion

--region Variables
XRHandAPI = CS.TwentyOz.VivenSDK.ExperimentExtension.Scripts.API.Experiment.XRHandAPI
Handedness = CS.TwentyOz.VivenSDK.Scripts.Core.Haptic.DataModels.SDKHandedness
FingerType = CS.TwentyOz.VivenSDK.Scripts.Core.Haptic.DataModels.SDKFingerType

---@type GameManager
local gameManager = nil

---@type VivenGrabbableModule
local grabbableModule = nil

---@type VivenRigidbodyControlModule
local rigidbodyModule = nil

---@type Transform
local handInteractorTransform = nil
--endregion

--region Unity Lifecycle
function awake()
    gameManager = self:GetLuaComponentInParent(gameManagerName)
    grabbableModule = self:GetComponent("VivenGrabbableModule")
    rigidbodyModule = self:GetComponent("VivenRigidbodyControlModule")
end

function start()
    local rigidBody = rigidbodyModule.Rigid
    rigidBody.collisionDetectionMode = CS.UnityEngine.CollisionDetectionMode.ContinuousDynamic
end

function onEnable()
    -- 필요시 이벤트 리스너 등록
end

function onDisable()
    -- 필요시 이벤트 리스너 해제
end
--endregion

--region Interaction Events
function onGrab()
    -- 잡은 손 정보 저장
    handInteractorTransform = grabbableModule.InteractingInteractor.InteractingTransform

    -- 햅틱 피드백
    if XRHandAPI.GetHandTrackingMode() == "None" then
        -- 컨트롤러 진동
        XR.StartControllerVibration(false, 0.3, 0.1) -- 오른손
    else
        -- 비햅틱스 장갑 진동
        HandTracking.CommandVibrationHaptic(0.05, 50, Handedness.Right, FingerType.Index, false)
    end

    -- 게임 매니저에 알림
    if gameManager ~= nil then
        gameManager.OnGrabObject()
    end
end

function onRelease()
    handInteractorTransform = nil

    -- 게임 매니저에 알림
    if gameManager ~= nil then
        gameManager.OnReleaseObject()
    end
end

function onShortClick()
    -- 짧게 클릭 (잡은 상태에서)
end

function onLongClick()
    -- 길게 클릭 (1초 이상)
end
--endregion

--region Public Functions
function SetActive(isActive)
    grabbableModule:FlushInteractableCollider()
    self.gameObject:SetActive(isActive)
end
--endregion
```

## 햅틱 피드백 패턴

### 컨트롤러 진동
```lua
XR.StartControllerVibration(isLeftHand, intensity, duration)
-- intensity: 0.0 ~ 1.0
-- duration: 초 단위
```

### 비햅틱스 장갑 진동
```lua
HandTracking.CommandVibrationHaptic(intensity, duration, handedness, fingerType, isWaveform)
-- intensity: 0.0 ~ 1.0
-- duration: 밀리초 단위
-- handedness: Handedness.Left / Handedness.Right
-- fingerType: FingerType.Thumb/Index/Middle/Ring/Little
```

## 강제 잡기 (Hand Tracking)

```lua
-- 포즈가 올바를 때 강제로 잡기
XRHandAPI.ForceGrabHandTracking(grabbableModule, isLeftHand)
```

사용자의 요청에 따라 적절한 Grabbable 설정과 스크립트를 생성해주세요.
