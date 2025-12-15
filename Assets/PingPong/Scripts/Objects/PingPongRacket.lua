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
---@details 공을 쳤을 때 추가되는 힘 배율 (강화됨)
local hitForceMultiplier = 8.0

---@type Vector3
---@details 이전 프레임 라켓 위치 (속도 계산용)
local previousPosition = nil

---@type Vector3
---@details 라켓 속도 (매 프레임 계산)
local racketVelocity = Vector3.zero

---@type GameObject
---@details 타겟 박스 (공이 날아갈 목표)
local targetBox = nil
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

    -- 초기 위치 저장
    previousPosition = self.transform.position

    -- 타겟 박스 찾기 (BallLauncher의 부모나 씬에서)
    targetBox = GameObject.Find("TargetBox")
    if targetBox == nil then
        targetBox = GameObject.Find("Target")
    end
    if targetBox == nil then
        -- BallLauncher 오브젝트를 타겟으로 사용
        targetBox = GameObject.Find("BallLauncher")
    end
end

function update()
    -- 라켓 속도 계산 (이전 프레임과 현재 프레임의 위치 차이)
    if previousPosition ~= nil then
        racketVelocity = (self.transform.position - previousPosition) / Time.deltaTime
    end
    previousPosition = self.transform.position
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

---@details 공을 쳤을 때 처리 (실제 탁구 물리 반사)
function OnHitBall(collision)
    -- 햅틱 피드백 (강하게)
    PlayHaptic(0.8, 0.15)

    -- 점수 추가
    if gameManager ~= nil then
        gameManager.AddScore(10)
    end

    local ballRigidbody = collision.rigidbody
    if ballRigidbody == nil then
        Debug.Log("공을 쳤습니다! (Rigidbody 없음)")
        return
    end

    -- 1. 공의 입사 속도 (충돌 직전 속도)
    local incomingVelocity = collision.relativeVelocity
    local incomingSpeed = incomingVelocity.magnitude

    -- 2. 충돌 노멀 (라켓 면의 방향)
    local contactNormal = collision.contacts[0].normal

    -- 3. 반사 벡터 계산 (입사각 = 반사각)
    -- Reflect: V - 2 * dot(V, N) * N
    local reflectedDirection = Vector3.Reflect(incomingVelocity.normalized, contactNormal)

    -- 4. 라켓 휘두르는 속도 계산
    local racketSpeed = racketVelocity.magnitude
    local racketInfluence = racketVelocity.normalized

    -- 5. 최종 방향 계산
    -- 반사 벡터 60% + 라켓 휘두른 방향 40% (라켓 속도가 클수록 영향 증가)
    local racketWeight = math.min(racketSpeed / 10.0, 0.6)  -- 최대 60%
    local reflectWeight = 1.0 - racketWeight

    local finalDirection
    if racketSpeed > 0.5 then
        -- 라켓을 휘둘렀으면 휘두른 방향 반영
        finalDirection = (reflectedDirection * reflectWeight + racketInfluence * racketWeight).normalized
    else
        -- 가만히 있으면 순수 반사
        finalDirection = reflectedDirection
    end

    -- 6. 최종 속도 계산
    -- 기본 반사 속도 + 라켓 속도 보너스 + 힘 배율
    local baseSpeed = incomingSpeed * 1.2  -- 반발 계수 (탁구공은 잘 튕김)
    local racketBonus = racketSpeed * 1.5  -- 라켓 휘두른 속도 보너스
    local finalSpeed = (baseSpeed + racketBonus) * (hitForceMultiplier / 5.0)

    -- 최소/최대 속도 제한
    finalSpeed = math.max(finalSpeed, 5.0)   -- 최소 5
    finalSpeed = math.min(finalSpeed, 30.0)  -- 최대 30

    -- 7. 공 속도 설정 (AddForce 대신 직접 velocity 설정)
    local finalVelocity = finalDirection * finalSpeed
    ballRigidbody.linearVelocity = finalVelocity

    -- 8. 스핀 추가 (라켓 움직임에 따른 스핀)
    local spinForce = Vector3.Cross(racketVelocity, contactNormal) * 0.5
    ballRigidbody.angularVelocity = ballRigidbody.angularVelocity + spinForce

    Debug.Log(string.format("공을 쳤습니다! 입사속도: %.1f, 반사속도: %.1f, 라켓속도: %.1f",
        incomingSpeed, finalSpeed, racketSpeed))
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
