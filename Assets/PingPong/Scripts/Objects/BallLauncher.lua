---공 발사 기계 스크립트
---일정 간격으로 공을 생성하고 발사, 좌우로 이동

--region Injection list
local _INJECTED_ORDER = 0
local function checkInject(OBJECT)
    _INJECTED_ORDER = _INJECTED_ORDER + 1
    assert(OBJECT, _INJECTED_ORDER .. "th object is missing")
    return OBJECT
end

---@type GameObject
---@details 공 프리팹
BallPrefab = checkInject(BallPrefab)

---@type GameObject
---@details 공 발사 위치 (기계의 총구)
LaunchPoint = checkInject(LaunchPoint)

---@type GameObject
---@details 플레이어 위치 (공이 날아갈 목표)
PlayerTarget = checkInject(PlayerTarget)

---@type string
---@details 게임 매니저 스크립트 이름
gameManagerName = checkInject(gameManagerName)
--endregion

--region Variables
local util = require 'xlua.util'

---@type PingPongGameManager
local gameManager = nil

---@type number
---@details 공 발사 간격 (초)
local launchInterval = 2.0

---@type number
---@details 공 발사 속도
local ballSpeed = 5.0

---@type number
---@details 기계 이동 속도
local machineSpeed = 2.0

---@type number
---@details 기계 이동 범위 (좌우)
local moveRange = 3.0

---@type boolean
---@details 발사 중 여부
local isLaunching = false

---@type boolean
---@details 이동 중 여부
local isMoving = false

---@type Vector3
---@details 시작 위치
local startPosition = nil

---@type number
---@details 현재 이동 방향 (1: 오른쪽, -1: 왼쪽)
local moveDirection = 1

---@type Coroutine
local launchRoutine = nil

---@type Coroutine
local moveRoutine = nil
--endregion

--region Unity Lifecycle
function awake()
    gameManager = self:GetLuaComponentInParent(gameManagerName)
    startPosition = self.transform.position
end

function start()
end

function onDisable()
    StopLaunching()
    StopMoving()
end
--endregion

--region Launching
---@details 공 발사 시작
function StartLaunching()
    if isLaunching then return end

    isLaunching = true
    StartMoving()

    launchRoutine = self:StartCoroutine(util.cs_generator(function()
        while isLaunching do
            LaunchBall()
            coroutine.yield(WaitForSeconds(launchInterval))
        end
    end))

    Debug.Log("공 발사 시작!")
end

---@details 공 발사 중지
function StopLaunching()
    isLaunching = false
    StopMoving()

    if launchRoutine ~= nil then
        self:StopCoroutine(launchRoutine)
        launchRoutine = nil
    end

    Debug.Log("공 발사 중지")
end

---@details 공 한 개 발사
function LaunchBall()
    if BallPrefab == nil then
        Debug.LogError("BallPrefab이 설정되지 않았습니다!")
        return
    end

    if LaunchPoint == nil then
        Debug.LogError("LaunchPoint가 설정되지 않았습니다!")
        return
    end

    -- 공 생성 (프로젝트 표준 패턴: GameObject.Instantiate 사용)
    local ball = GameObject.Instantiate(BallPrefab)
    ball.transform.position = LaunchPoint.transform.position
    ball.transform.rotation = LaunchPoint.transform.rotation

    -- Rigidbody로 발사
    local rb = ball:GetComponent(typeof(CS.UnityEngine.Rigidbody))
    if rb ~= nil then
        local direction = LaunchPoint.transform.forward
        if PlayerTarget ~= nil then
            direction = (PlayerTarget.transform.position - LaunchPoint.transform.position).normalized
        end
        rb:AddForce(direction * ballSpeed, CS.UnityEngine.ForceMode.Impulse)
    end

    Debug.Log("공 발사!")
end
--endregion

--region Machine Movement
---@details 좌우 이동 시작
function StartMoving()
    if isMoving then return end

    isMoving = true

    moveRoutine = self:StartCoroutine(util.cs_generator(function()
        while isMoving do
            MoveMachine()
            coroutine.yield(nil) -- 매 프레임
        end
    end))
end

---@details 좌우 이동 중지
function StopMoving()
    isMoving = false

    if moveRoutine ~= nil then
        self:StopCoroutine(moveRoutine)
        moveRoutine = nil
    end
end

---@details 기계 이동 (매 프레임 호출)
function MoveMachine()
    -- 현재 위치
    local currentPos = self.transform.position

    -- 이동
    local movement = Vector3.right * moveDirection * machineSpeed * Time.deltaTime
    self.transform.position = currentPos + movement

    -- 범위 체크 및 방향 전환
    local distanceFromStart = self.transform.position.x - startPosition.x

    if distanceFromStart > moveRange then
        moveDirection = -1
    elseif distanceFromStart < -moveRange then
        moveDirection = 1
    end
end
--endregion

--region Settings
---@details 공 발사 속도 설정
function SetBallSpeed(speed)
    ballSpeed = speed
end

---@details 공 발사 간격 설정
function SetLaunchInterval(interval)
    launchInterval = interval
end

---@details 기계 이동 속도 설정
function SetMachineSpeed(speed)
    machineSpeed = speed
end

---@details 기계 이동 범위 설정
function SetMoveRange(range)
    moveRange = range
end
--endregion

--region Getters
function GetBallSpeed()
    return ballSpeed
end

function GetLaunchInterval()
    return launchInterval
end

function GetMachineSpeed()
    return machineSpeed
end

function GetIsLaunching()
    return isLaunching
end
--endregion
