---공 스포너 스크립트
---일정 간격으로 공을 생성 (Viven SDK 버전)

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
        Debug.Log(_INJECTED_ORDER .. "th object is missing (nullable)")
    end
    return OBJECT
end

---@type GameObject[]
---@details 공 프리팹 배열
BallPrefabs = checkInject(BallPrefabs)

---@type GameObject
---@details 공이 생성될 위치 (Transform)
SpawnPoint = checkInject(SpawnPoint)

---@type GameObject
---@details 게임 매니저 오브젝트
GameManagerObject = NullableInject(GameManagerObject)
--endregion

--region Variables
local util = require 'xlua.util'

---@type ConveyorGameManager
local gameManager = nil

---@type number
---@details 공 생성 간격 (초)
local spawnInterval = 0.8

---@type boolean
---@details 스폰 중 여부
local isSpawning = false

---@type Coroutine
local spawnRoutine = nil
--endregion

--region Unity Lifecycle
function awake()
    if GameManagerObject ~= nil then
        gameManager = GameManagerObject:GetLuaComponent("ConveyorGameManager")
    end
end

function start()
    -- 게임 매니저가 없으면 자동 시작
    if gameManager == nil then
        StartSpawning()
    end
end

function onDisable()
    StopSpawning()
end
--endregion

--region Spawning
---@details 공 스폰 시작
function StartSpawning()
    if isSpawning then return end

    isSpawning = true

    spawnRoutine = self:StartCoroutine(util.cs_generator(function()
        while isSpawning do
            SpawnRandomBall()
            coroutine.yield(WaitForSeconds(spawnInterval))
        end
    end))
end

---@details 공 스폰 중지
function StopSpawning()
    isSpawning = false

    if spawnRoutine ~= nil then
        self:StopCoroutine(spawnRoutine)
        spawnRoutine = nil
    end
end

---@details 랜덤 공 생성
function SpawnRandomBall()
    if BallPrefabs == nil or #BallPrefabs == 0 then
        Debug.Log("[ERROR] BallPrefabs 배열이 비어있습니다. 프리팹을 할당하세요.")
        return
    end

    -- 랜덤 인덱스 선택 (Lua 배열은 1부터 시작)
    local randomIndex = math.random(1, #BallPrefabs)
    local ballPrefab = BallPrefabs[randomIndex]

    if ballPrefab == nil then
        Debug.Log("[ERROR] 선택된 공 프리팹이 nil입니다.")
        return
    end

    -- 공 생성
    local spawnPos = SpawnPoint.transform.position
    local ball = GameObject.Instantiate(ballPrefab)
    ball.transform.position = spawnPos
    ball.transform.rotation = CS.UnityEngine.Quaternion.identity
end
--endregion

--region Settings
---@details 스폰 간격 설정
---@param interval number 간격 (초)
function SetSpawnInterval(interval)
    spawnInterval = interval
end

---@details 스폰 간격 반환
function GetSpawnInterval()
    return spawnInterval
end

---@details 스폰 중 여부 반환
function GetIsSpawning()
    return isSpawning
end
--endregion
