--- Target: 타겟 오브젝트 스크립트
--- 총알에 맞으면 파괴되고 점수 획득

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
---@details 스폰 매니저 오브젝트
SpawnManagerObject = NullableInject(SpawnManagerObject)

---@type GameObject
---@details 게임 매니저 오브젝트
GameManagerObject = NullableInject(GameManagerObject)

---@type number
---@details 타겟 점수 값
scoreValue = 10

---@type string
---@details 공 종류 (GolfBall, BasketBall, etc.) - 에디터에서 자동 설정
ballType = nil

--endregion

--region Variables

---@type TargetSpawnManager
---@details 스폰 매니저 참조
local spawnManager = nil

---@type ConveyorGameManager
---@details 게임 매니저 참조
local gameManager = nil

---@type number
---@details 풀 내 인덱스
local poolIndex = -1

---@type boolean
---@details 이미 피격됨 여부 (중복 판정 방지)
local isHit = false

---@type string
---@details 실제 공 종류 (awake에서 이름으로부터 추출 또는 Injection에서)
local myBallType = nil

---@type Vector3
---@details 스폰 위치
local spawnPosition = nil

---@type Rigidbody
---@details Rigidbody 참조
local rigidbody = nil

--endregion

--region Unity Lifecycle

function awake()
    -- 스폰 매니저 참조
    if SpawnManagerObject then
        spawnManager = SpawnManagerObject:GetLuaComponent("TargetSpawnManager")
    end

    -- 게임 매니저 참조
    if GameManagerObject then
        gameManager = GameManagerObject:GetLuaComponent("ConveyorGameManager")
    end

    -- Rigidbody 참조
    rigidbody = self:GetComponent(typeof(CS.UnityEngine.Rigidbody))

    -- 스폰 위치 저장
    spawnPosition = self.transform.position

    -- 공 종류 결정 (Injection > 이름에서 추출)
    myBallType = ExtractBallTypeFromName()
end

---@details 오브젝트 이름에서 공 종류 추출
---@return string 공 종류 (예: GolfBall, BasketBall)
function ExtractBallTypeFromName()
    -- Injection에서 먼저 확인
    if ballType and ballType ~= "" then
        return ballType
    end

    -- 오브젝트 이름에서 추출 (예: "GolfBall_0" -> "GolfBall")
    local objName = self.gameObject.name
    local ballTypes = {
        "GolfBall",
        "BasketBall",
        "RugbyBall",
        "VolleyBall",
        "BowlingBall",
        "BeachBall",
        "BaseBall",
        "SoccerBall",
        "TennisBall"
    }

    for _, bt in ipairs(ballTypes) do
        if string.find(objName, bt) then
            return bt
        end
    end

    -- 기본값
    return "Unknown"
end

function start()
    isHit = false
end

function onEnable()
    isHit = false
end

function onDisable()
    -- 상태 초기화
    isHit = false
end

--endregion

--region Collision Detection

---@details 트리거 진입 이벤트 (총알 충돌)
---@param other Collider 충돌한 콜라이더
function onTriggerEnter(other)
    if isHit then return end

    -- Bullet 태그 또는 이름 확인
    local otherName = other.gameObject.name
    local otherTag = other.gameObject.tag

    if otherTag == "Bullet" or string.find(otherName, "Bullet") or string.find(otherName, "bullet") then
        OnHitByBullet(other.gameObject)
    end
end

---@details 충돌 이벤트 (물리 충돌)
---@param collision Collision 충돌 정보
function onCollisionEnter(collision)
    if isHit then return end

    local otherName = collision.gameObject.name
    local otherTag = collision.gameObject.tag

    if otherTag == "Bullet" or string.find(otherName, "Bullet") or string.find(otherName, "bullet") then
        OnHitByBullet(collision.gameObject)
    end
end

---@details 총알에 맞았을 때 처리
---@param bulletObject GameObject 총알 오브젝트
function OnHitByBullet(bulletObject)
    if isHit then return end

    isHit = true

    -- 가이드 시스템: 공 종류 체크하여 점수 계산
    if gameManager then
        -- 새로운 가이드 시스템 사용
        if gameManager.OnTargetHit then
            local isCorrect = gameManager.OnTargetHit(myBallType)
            Debug.Log("타겟 피격! 공 종류: " .. (myBallType or "Unknown") .. " / 정답: " .. tostring(isCorrect))
        else
            -- 기존 방식 (호환용)
            gameManager.AddScore(scoreValue)
            Debug.Log("타겟 피격! 점수: " .. scoreValue)
        end
    end

    -- 히트 이펙트 (선택적)
    PlayHitEffect()

    -- 풀로 반환
    ReturnToPool()
end

--endregion

--region Effects

---@details 피격 햅틱 피드백
function PlayHitHaptic()
    -- 양손 컨트롤러 진동
    XR.StartControllerVibration(false, 0.5, 0.1) -- 오른손
    XR.StartControllerVibration(true, 0.5, 0.1)  -- 왼손
end

---@details 피격 이펙트 (확장 가능)
function PlayHitEffect()
    -- TODO: 파티클 이펙트, 사운드 등 추가 가능
end

--endregion

--region Pool Management

---@details 풀로 반환
function ReturnToPool()
    -- 스폰 매니저에 알림
    if spawnManager and spawnManager.OnTargetDestroyed then
        spawnManager:OnTargetDestroyed(self.gameObject, poolIndex)
    else
        -- spawnManager가 nil이면 직접 찾기
        local smObj = CS.UnityEngine.GameObject.Find("SpawnManager")
        if smObj then
            local sm = smObj:GetLuaComponent("TargetSpawnManager")
            if sm and sm.OnTargetDestroyed then
                sm:OnTargetDestroyed(self.gameObject, poolIndex)
            end
        end
    end
end

---@details 바구니(끝)에서 파괴될 때 (점수 감소)
function OnReachEnd()
    if isHit then return end

    isHit = true

    -- 점수 감소 또는 HP 감소
    if gameManager and gameManager.OnTargetMissed then
        gameManager.OnTargetMissed()
    end

    Debug.Log("타겟 놓침!")

    ReturnToPool()
end

--endregion

--region Public Functions

---@details 타겟 리셋
---@param _ any self
---@param position Vector3 스폰 위치
---@param index number 풀 인덱스
function ResetTarget(_, position, index)
    spawnPosition = position or self.transform.position
    poolIndex = index or -1

    isHit = false

    -- Rigidbody 초기화
    if rigidbody then
        rigidbody.linearVelocity = Vector3.zero
        rigidbody.angularVelocity = Vector3.zero
    end
end

---@details 점수 값 반환
---@return number
function GetScoreValue()
    return scoreValue
end

---@details 점수 값 설정
---@param value number 점수
function SetScoreValue(value)
    scoreValue = value
end

---@details 피격 여부 반환
---@return boolean
function IsHit()
    return isHit
end

---@details 풀 인덱스 반환
---@return number
function GetPoolIndex()
    return poolIndex
end

---@details 풀 인덱스 설정
---@param index number
function SetPoolIndex(index)
    poolIndex = index
end

---@details 스폰 매니저 설정
---@param manager TargetSpawnManager
function SetSpawnManager(manager)
    spawnManager = manager
end

---@details 게임 매니저 설정
---@param manager ConveyorGameManager
function SetGameManager(manager)
    gameManager = manager
end

---@details 공 종류 반환
---@return string
function GetBallType()
    return myBallType
end

---@details 공 종류 설정
---@param bt string 공 종류
function SetBallType(bt)
    myBallType = bt
end

--endregion
