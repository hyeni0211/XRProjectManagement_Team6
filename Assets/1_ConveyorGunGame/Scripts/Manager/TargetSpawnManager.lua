--- TargetSpawnManager: 타겟 스폰 관리 (Object Pooling 방식)
--- 씬에 미리 배치된 오브젝트를 풀에서 가져와 재사용
--- VIVEN SDK에서 동적 VObject Instantiate가 불가능하므로 풀링 방식 사용

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

---@type GameObject
---@details 타겟 스폰 위치 (PropSpawnPoint)
SpawnPoint = checkInject(SpawnPoint)

---@type GameObject
---@details 타겟 풀 부모 오브젝트 (자식들이 풀 아이템)
TargetPool = checkInject(TargetPool)

---@type GameObject
---@details 게임 매니저 오브젝트
GameManagerObject = NullableInject(GameManagerObject)

--endregion

--region Variables

local util = require 'xlua.util'

---@type number
---@details 스폰 간격 (초)
local spawnInterval = 2.0

---@type number
---@details 최대 동시 타겟 수
local maxTargetCount = 10

---@type boolean
---@details 스폰 활성화 여부
local isSpawning = false

---@type boolean
---@details 스폰 일시정지 여부
local isPaused = false

---@type table
---@details 활성화된 타겟 목록 {object, poolIndex}
local activeTargets = {}

---@type any
---@details 스폰 코루틴 참조
local spawnCoroutine = nil

---@type ConveyorGameManager
---@details 게임 매니저 참조
local gameManager = nil

---@type boolean
---@details 초기화 완료 여부
local isInitialized = false

---@type boolean
---@details 풀 초기화 중 여부
local isPoolInitializing = false

--endregion

--region Pool Variables

---@type table
---@details 풀 테이블 {available = {인덱스...}, inUse = {인덱스...}}
local pool = { available = {}, inUse = {} }

---@type table
---@details 풀 오브젝트 배열 (GameObject)
local poolObjects = {}

---@type table
---@details 풀 스크립트 배열 (Target 스크립트)
local poolScripts = {}

---@type table
---@details 초기 위치/회전 저장 테이블
local poolInitialPose = {}

---@type table
---@details MeshRenderer 배열의 배열
local poolMeshRenderers = {}

---@type table
---@details Collider 배열의 배열
local poolColliders = {}

---@type table
---@details VivenGrabbableModule 배열 (선택적)
local poolGrabbables = {}

---@type table
---@details 모든 Grabbable 모듈 목록
local allGrabbableModules = {}

---@type Vector3
---@details 숨김 위치 (풀에서 비활성화 시 이동할 위치)
local HIDE_POSITION = nil

--endregion

--region Unity Lifecycle

function awake()
    -- Pool 체크
    if not TargetPool then
        Debug.Log("[ERROR] TargetSpawnManager: TargetPool이 할당되지 않았습니다!")
        isInitialized = false
        return
    end

    -- 숨김 위치 초기화
    HIDE_POSITION = Vector3(0, -9999, 0)

    -- 게임 매니저 참조
    if GameManagerObject then
        gameManager = GameManagerObject:GetLuaComponent("ConveyorGameManager")
    end

    -- 풀 초기화
    isPoolInitializing = true
    InitializePool()
    isPoolInitializing = false

    isInitialized = true
    -- Debug.Log("TargetSpawnManager 초기화 완료 - 풀 크기: " .. #poolObjects)
end

function start()
    if not isInitialized then return end
end

function onEnable()
    if not isInitialized then return end
end

function onDisable()
    if not isInitialized then return end
    StopSpawning()
end

--endregion

--region Pool Management

---@details 풀 초기화
function InitializePool()
    if not TargetPool then return end

    -- 테이블 초기화
    poolObjects = {}
    poolScripts = {}
    poolInitialPose = {}
    poolMeshRenderers = {}
    poolColliders = {}
    poolGrabbables = {}
    pool.available = {}
    pool.inUse = {}

    -- 자식 오브젝트 수집
    for i = 0, TargetPool.transform.childCount - 1 do
        local child = TargetPool.transform:GetChild(i).gameObject
        local index = i + 1  -- Lua는 1부터 시작

        poolObjects[index] = child

        -- Target 스크립트 가져오기
        local targetScript = child:GetLuaComponent("Target")
        poolScripts[index] = targetScript

        -- 초기 위치 저장
        poolInitialPose[index] = {
            Pos = child.transform.position,
            Rot = child.transform.rotation
        }

        -- MeshRenderer 수집 (비활성화된 것도 포함)
        local meshRenderers = child:GetComponentsInChildren(typeof(CS.UnityEngine.MeshRenderer), true)
        local tempMeshes = {}
        if meshRenderers and meshRenderers.Length > 0 then
            for j = 0, meshRenderers.Length - 1 do
                tempMeshes[#tempMeshes + 1] = meshRenderers[j]
            end
        end
        poolMeshRenderers[index] = tempMeshes

        -- Collider 수집 (비활성화된 것도 포함)
        local colliders = child:GetComponentsInChildren(typeof(CS.UnityEngine.Collider), true)
        local tempColliders = {}
        if colliders and colliders.Length > 0 then
            for j = 0, colliders.Length - 1 do
                tempColliders[#tempColliders + 1] = colliders[j]
            end
        end
        poolColliders[index] = tempColliders

        -- VivenGrabbableModule 수집 (선택적)
        local grabbable = child:GetComponent("VivenGrabbableModule")
        poolGrabbables[index] = grabbable
        if grabbable then
            allGrabbableModules[#allGrabbableModules + 1] = grabbable
        end

        -- available 풀에 추가
        table.insert(pool.available, index)

        -- Target 스크립트 초기화
        if targetScript then
            targetScript.SetSpawnManager(self)
            targetScript.SetPoolIndex(index)
            if gameManager then
                targetScript.SetGameManager(gameManager)
            end
        end

        -- 비활성화
        SetPoolObjectVisible(index, false)
    end
end

---@details 모든 Grabbable의 콜라이더 상태 갱신
function FlushAllGrabbables()
    if isPoolInitializing then return end

    for i = 1, #allGrabbableModules do
        local grabbable = allGrabbableModules[i]
        if grabbable then
            local success, err = pcall(function()
                grabbable:FlushInteractableCollider()
            end)
        end
    end
end

---@details 풀 오브젝트 가시성 설정
---@param poolIndex number 풀 인덱스
---@param visible boolean 가시성 여부
function SetPoolObjectVisible(poolIndex, visible)
    local obj = poolObjects[poolIndex]
    if not obj then return end

    FlushAllGrabbables()

    -- MeshRenderer 활성화/비활성화
    local meshRenderers = poolMeshRenderers[poolIndex]
    if meshRenderers then
        for _, mr in ipairs(meshRenderers) do
            mr.enabled = visible
        end
    end

    -- Collider 활성화/비활성화
    local colliders = poolColliders[poolIndex]
    if colliders then
        for _, col in ipairs(colliders) do
            col.enabled = visible
        end
    end

    -- 숨김 위치로 이동 (비활성화 시)
    if not visible then
        obj.transform.position = HIDE_POSITION
    end
end

---@details 풀에서 오브젝트 가져오기
---@return GameObject|nil, Target|nil, number
function GetFromPool()
    if #pool.available == 0 then
        return nil, nil, -1
    end

    local index = pool.available[1]
    table.remove(pool.available, 1)
    table.insert(pool.inUse, index)

    local obj = poolObjects[index]
    local script = poolScripts[index]

    return obj, script, index
end

---@details 풀로 오브젝트 반환
---@param poolIndex number 풀 내 인덱스
function ReturnToPool(poolIndex)
    -- inUse에서 제거
    for i = #pool.inUse, 1, -1 do
        if pool.inUse[i] == poolIndex then
            table.remove(pool.inUse, i)
            break
        end
    end

    -- 이미 available에 있는지 확인
    for _, idx in ipairs(pool.available) do
        if idx == poolIndex then
            return
        end
    end

    -- available에 추가
    table.insert(pool.available, poolIndex)

    -- 오브젝트 처리
    local obj = poolObjects[poolIndex]
    if obj then
        -- 강제 릴리즈
        local grabbable = poolGrabbables[poolIndex]
        if grabbable then
            grabbable:Release()
        end

        -- 비활성화
        SetPoolObjectVisible(poolIndex, false)
    end
end

---@details 모든 오브젝트를 풀로 반환
function ReturnAllToPool()
    local inUseCopy = {}
    for _, index in ipairs(pool.inUse) do
        table.insert(inUseCopy, index)
    end

    for _, index in ipairs(inUseCopy) do
        ReturnToPool(index)
    end

    activeTargets = {}
end

--endregion

--region Public Functions

---@details 스폰 설정 초기화
---@param _ any self
---@param settings table 설정 {spawnInterval, maxTargetCount}
function InitSpawn(_, settings)
    if settings then
        spawnInterval = settings.spawnInterval or 2.0
        maxTargetCount = settings.maxTargetCount or 10
    end

    FlushAllGrabbables()
    ReturnAllToPool()
end

---@details 스폰 시작
---@param _ any self
function StartSpawning(_)
    if isSpawning then return end

    isSpawning = true
    isPaused = false

    if spawnCoroutine then
        self:StopCoroutine(spawnCoroutine)
    end

    spawnCoroutine = self:StartCoroutine(util.cs_generator(function()
        while isSpawning do
            if not isPaused then
                TrySpawnTarget()
            end
            coroutine.yield(WaitForSeconds(spawnInterval))
        end
    end))
end

---@details 스폰 정지
---@param _ any self
function StopSpawning(_)
    isSpawning = false
    isPaused = false

    if spawnCoroutine then
        self:StopCoroutine(spawnCoroutine)
        spawnCoroutine = nil
    end
end

---@details 스폰 일시정지
---@param _ any self
function PauseSpawning(_)
    isPaused = true
end

---@details 스폰 재개
---@param _ any self
function ResumeSpawning(_)
    isPaused = false
end

---@details 모든 타겟 제거
function ClearAllTargets()
    ReturnAllToPool()
end

---@details 타겟이 파괴될 때 호출
---@param _ any self
---@param targetObject GameObject 파괴된 타겟
---@param poolIndex number 풀 인덱스
function OnTargetDestroyed(_, targetObject, poolIndex)
    for i = #activeTargets, 1, -1 do
        if activeTargets[i].object == targetObject then
            table.remove(activeTargets, i)
            break
        end
    end

    if poolIndex and poolIndex > 0 then
        ReturnToPool(poolIndex)
    end
end

--endregion

--region Spawning Logic

---@details 타겟 스폰 시도
---@return boolean 스폰 성공 여부
function TrySpawnTarget()
    if #activeTargets >= maxTargetCount then
        return false
    end

    if #pool.available == 0 then
        return false
    end

    return SpawnTarget()
end

---@details 타겟 스폰
---@return boolean 스폰 성공 여부
function SpawnTarget()
    local targetObject, targetScript, poolIndex = GetFromPool()

    if not targetObject then
        Debug.Log("[WARNING] SpawnTarget: 풀에서 오브젝트를 가져오지 못함")
        return false
    end

    -- 스폰 위치 설정
    local spawnPos = SpawnPoint.transform.position

    -- 위치 설정
    targetObject.transform.position = spawnPos
    targetObject.transform.rotation = CS.UnityEngine.Quaternion.identity

    -- 활성화
    SetPoolObjectVisible(poolIndex, true)

    -- Target 스크립트 리셋
    if targetScript then
        targetScript:ResetTarget(spawnPos, poolIndex)
    end

    -- Rigidbody 속도 초기화
    local rb = targetObject:GetComponent(typeof(CS.UnityEngine.Rigidbody))
    if rb then
        rb.linearVelocity = Vector3.zero
        rb.angularVelocity = Vector3.zero
    end

    -- 활성 목록에 추가
    table.insert(activeTargets, {
        object = targetObject,
        poolIndex = poolIndex
    })

    return true
end

--endregion

--region Getters

---@details 활성 타겟 수 반환
---@return number
function GetActiveTargetCount()
    return #activeTargets
end

---@details 스폰 상태 반환
---@return boolean
function IsSpawning()
    return isSpawning
end

---@details 풀 상태 반환 (디버그용)
---@return table
function GetPoolStatus()
    return {
        available = #pool.available,
        inUse = #pool.inUse,
        total = #poolObjects
    }
end

---@details 스폰 간격 설정
---@param interval number 간격 (초)
function SetSpawnInterval(interval)
    spawnInterval = interval
end

---@details 최대 타겟 수 설정
---@param count number 최대 수
function SetMaxTargetCount(count)
    maxTargetCount = count
end

--endregion
