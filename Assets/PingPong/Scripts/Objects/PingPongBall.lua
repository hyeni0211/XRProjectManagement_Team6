---탁구공 스크립트
---발사되고 물리적으로 움직이는 공

--region Injection list
local _INJECTED_ORDER = 0
local function checkInject(OBJECT)
    _INJECTED_ORDER = _INJECTED_ORDER + 1
    assert(OBJECT, _INJECTED_ORDER .. "th object is missing")
    return OBJECT
end

---@type string
---@details 게임 매니저 스크립트 이름 (선택적)
gameManagerName = nil
--endregion

--region Variables
local util = require 'xlua.util'

---@type Rigidbody
local rigidBody = nil

---@type number
---@details 공의 수명 (초) - 이 시간 후 자동 삭제
local lifeTime = 10.0

---@type number
---@details 발사 속도
local launchSpeed = 5.0

---@type boolean
---@details 발사되었는지 여부
local isLaunched = false

---@type Coroutine
local destroyRoutine = nil
--endregion

--region Unity Lifecycle
function awake()
    rigidBody = self:GetComponent(typeof(CS.UnityEngine.Rigidbody))
end

function start()
    -- 생성 시 자동 삭제 타이머 시작
    StartDestroyTimer()
end

function onDisable()
    -- 코루틴 정리
    if destroyRoutine ~= nil then
        self:StopCoroutine(destroyRoutine)
        destroyRoutine = nil
    end
end
--endregion

--region Launch
---@details 특정 방향으로 공 발사
---@param direction Vector3 발사 방향
---@param speed number 발사 속도
function Launch(direction, speed)
    if isLaunched then return end

    isLaunched = true
    launchSpeed = speed

    if rigidBody ~= nil then
        -- 속도 초기화
        rigidBody.linearVelocity = Vector3.zero
        rigidBody.angularVelocity = Vector3.zero

        -- 발사
        rigidBody:AddForce(direction * speed, CS.UnityEngine.ForceMode.Impulse)
    end

    Debug.Log("공 발사! 속도: " .. speed)
end

---@details 플레이어 방향으로 발사
---@param targetPosition Vector3 목표 위치
---@param speed number 발사 속도
function LaunchToward(targetPosition, speed)
    local direction = (targetPosition - self.transform.position).normalized
    Launch(direction, speed)
end
--endregion

--region Destroy Timer
---@details 자동 삭제 타이머 시작
function StartDestroyTimer()
    destroyRoutine = self:StartCoroutine(util.cs_generator(function()
        coroutine.yield(WaitForSeconds(lifeTime))
        DestroySelf()
    end))
end

---@details 공 삭제
function DestroySelf()
    CS.UnityEngine.Object.Destroy(self.gameObject)
end
--endregion

--region Collision Events
function onCollisionEnter(collision)
    local otherName = collision.gameObject.name

    -- 바닥에 떨어지면 삭제 (선택적)
    if string.find(otherName, "Floor") or string.find(otherName, "Ground") then
        -- 잠시 후 삭제
        destroyRoutine = self:StartCoroutine(util.cs_generator(function()
            coroutine.yield(WaitForSeconds(2.0))
            DestroySelf()
        end))
    end
end
--endregion

--region Setters
function SetLifeTime(time)
    lifeTime = time
end

function SetLaunchSpeed(speed)
    launchSpeed = speed
end
--endregion
