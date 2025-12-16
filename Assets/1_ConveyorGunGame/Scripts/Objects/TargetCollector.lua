--- TargetCollector: 컨베이어 벨트 끝 공 수거
--- 놓친 공을 감지하고 풀로 반환

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
---@details 게임 매니저 오브젝트
GameManagerObject = NullableInject(GameManagerObject)

---@type GameObject
---@details 스폰 매니저 오브젝트
SpawnManagerObject = NullableInject(SpawnManagerObject)

--endregion

--region Variables

---@type ConveyorGameManager
---@details 게임 매니저 참조
local gameManager = nil

---@type TargetSpawnManager
---@details 스폰 매니저 참조
local spawnManager = nil

--endregion

--region Unity Lifecycle

function awake()
    -- 게임 매니저 참조
    if GameManagerObject then
        gameManager = GameManagerObject:GetLuaComponent("ConveyorGameManager")
    end

    -- 스폰 매니저 참조
    if SpawnManagerObject then
        spawnManager = SpawnManagerObject:GetLuaComponent("TargetSpawnManager")
    end
end

function start()
    -- 초기화 완료
end

--endregion

--region Trigger Events

---@details 트리거 진입 이벤트
---@param other Collider 충돌한 콜라이더
function onTriggerEnter(other)
    -- Target 태그 또는 이름 확인
    local otherTag = other.gameObject.tag
    local otherName = other.gameObject.name

    local isTarget = (otherTag == "Target") or
                     string.find(otherName, "Ball") or
                     string.find(otherName, "ball")

    if not isTarget then return end

    -- Target 스크립트에서 OnReachEnd 호출
    local targetScript = other.gameObject:GetLuaComponent("Target")
    if targetScript and targetScript.OnReachEnd then
        targetScript.OnReachEnd()
    else
        -- Target 스크립트가 없으면 직접 처리
        CollectTarget(other.gameObject)
    end
end

---@details 타겟 직접 수거 처리
---@param targetObject GameObject 타겟 오브젝트
function CollectTarget(targetObject)
    -- 게임 매니저에 놓침 알림
    if gameManager and gameManager.OnTargetMissed then
        gameManager.OnTargetMissed()
    end

    -- 스폰 매니저에 반환 알림
    if spawnManager and spawnManager.OnTargetDestroyed then
        -- poolIndex 찾기
        local targetScript = targetObject:GetLuaComponent("Target")
        local poolIndex = -1
        if targetScript and targetScript.GetPoolIndex then
            poolIndex = targetScript.GetPoolIndex()
        end

        spawnManager:OnTargetDestroyed(targetObject, poolIndex)
    end
end

--endregion
