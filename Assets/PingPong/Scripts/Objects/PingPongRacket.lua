---탁구채 스크립트
---플레이어가 잡고 공을 치는 오브젝트

--region Injection list
local _INJECTED_ORDER = 0
local function checkInject(OBJECT)
    _INJECTED_ORDER = _INJECTED_ORDER + 1
    assert(OBJECT, _INJECTED_ORDER .. "th object is missing")
    return OBJECT
end

---@type GameObject
---@details 게임 매니저 오브젝트
GameManagerObject = checkInject(GameManagerObject)
--endregion

--region Variables
XRHandAPI = CS.TwentyOz.VivenSDK.ExperimentExtension.Scripts.API.Experiment.XRHandAPI
Handedness = CS.TwentyOz.VivenSDK.Scripts.Core.Haptic.DataModels.SDKHandedness
FingerType = CS.TwentyOz.VivenSDK.Scripts.Core.Haptic.DataModels.SDKFingerType

---@type PingPongGameManager
local gameManager = nil

---@type VivenGrabbableModule
local grabbableModule = nil

---@type VivenRigidbodyControlModule
local rigidbodyModule = nil

---@type boolean
local isGrabbed = false

---@type number
---@details 공을 쳤을 때 추가되는 힘 배율
local hitForceMultiplier = 1.5
--endregion

--region Unity Lifecycle
function awake()
    gameManager = GameManagerObject:GetLuaComponent("PingPongGameManager")
    grabbableModule = self:GetComponent("VivenGrabbableModule")
    rigidbodyModule = self:GetComponent("VivenRigidbodyControlModule")
end

function start()
    -- Rigidbody 충돌 감지 모드 설정
    if rigidbodyModule ~= nil then
        local rigidBody = rigidbodyModule.Rigid
        rigidBody.collisionDetectionMode = CS.UnityEngine.CollisionDetectionMode.ContinuousDynamic
    end
end
--endregion

--region Interaction Events
function onGrab()
    isGrabbed = true

    -- 햅틱 피드백 (잡았을 때)
    PlayHaptic(0.2, 0.05)

    Debug.Log("탁구채 잡음")
end

function onRelease()
    isGrabbed = false
    Debug.Log("탁구채 놓음")
end
--endregion

--region Collision Events
---@details 공과 충돌했을 때 호출
function onCollisionEnter(collision)
    -- 공인지 확인 (태그 또는 이름으로)
    local otherName = collision.gameObject.name

    if string.find(otherName, "Ball") or string.find(otherName, "ball") then
        OnHitBall(collision)
    end
end

---@details 공을 쳤을 때 처리
function OnHitBall(collision)
    -- 햅틱 피드백 (강하게)
    PlayHaptic(0.6, 0.1)

    -- 점수 추가
    if gameManager ~= nil then
        gameManager.AddScore(10)
    end

    -- 공에 추가 힘 적용 (선택적)
    local ballRigidbody = collision.rigidbody
    if ballRigidbody ~= nil then
        local hitDirection = collision.contacts[0].normal * -1
        ballRigidbody:AddForce(hitDirection * hitForceMultiplier, CS.UnityEngine.ForceMode.Impulse)
    end

    Debug.Log("공을 쳤습니다!")
end
--endregion

--region Haptic
---@details 햅틱 피드백 재생
---@param intensity number 강도 (0.0 ~ 1.0)
---@param duration number 지속시간 (초)
function PlayHaptic(intensity, duration)
    if XRHandAPI.GetHandTrackingMode() == "None" then
        -- 컨트롤러 진동
        XR.StartControllerVibration(false, intensity, duration) -- 오른손
    else
        -- 비햅틱스 장갑
        local gloveIntensity = intensity * 0.1
        local gloveDuration = duration * 1000 -- 밀리초
        HandTracking.CommandVibrationHaptic(gloveIntensity, gloveDuration, Handedness.Right, FingerType.Index, false)
        HandTracking.CommandVibrationHaptic(gloveIntensity, gloveDuration, Handedness.Right, FingerType.Middle, false)
    end
end
--endregion

--region Public Functions
function GetIsGrabbed()
    return isGrabbed
end

function SetHitForceMultiplier(multiplier)
    hitForceMultiplier = multiplier
end
--endregion
