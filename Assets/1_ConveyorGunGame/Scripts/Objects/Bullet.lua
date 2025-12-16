--- Bullet: 총알 스크립트
--- 발사, 충돌 감지, 풀 반환 처리

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
---@details 히트 이펙트 오브젝트 (선택적)
HitEffectObject = NullableInject(HitEffectObject)

---@type number
---@details 자동 반환 시간 (초)
autoReturnTime = 3.0

--endregion

--region Variables

local util = require 'xlua.util'

---@type Rigidbody
---@details Rigidbody 참조
local rigidbody = nil

---@type ConveyorGameManager
---@details 게임 매니저 참조
local gameManager = nil

---@type ShootingGun
---@details 소속 총 참조
local ownerGun = nil

---@type number
---@details 풀 내 인덱스
local poolIndex = -1

---@type boolean
---@details 발사 상태
local isFired = false

---@type number
---@details 발사 시간
local fireTime = 0

---@type any
---@details 자동 반환 코루틴
local autoReturnRoutine = nil

---@type Vector3
---@details 발사 방향
local fireDirection = nil

---@type number
---@details 발사 속도
local fireSpeed = 0

--endregion

--region Unity Lifecycle

function awake()
    -- Rigidbody 가져오기
    rigidbody = self:GetComponent(typeof(CS.UnityEngine.Rigidbody))

    -- 게임 매니저 참조
    if GameManagerObject then
        gameManager = GameManagerObject:GetLuaComponent("ConveyorGameManager")
    end
end

function start()
    isFired = false
end

function onEnable()
    -- 상태 초기화
end

function onDisable()
    -- 코루틴 정리
    StopAutoReturnRoutine()
    isFired = false
end

function update()
    if not isFired then return end

    -- 자동 반환 시간 체크 (코루틴 백업)
    if Time.time - fireTime > autoReturnTime then
        ReturnToPool()
    end
end

function fixedUpdate()
    if not isFired then return end

    -- 속도 유지 (Rigidbody가 있는 경우)
    if rigidbody and fireDirection then
        rigidbody.linearVelocity = fireDirection * fireSpeed
    end
end

--endregion

--region Collision Detection

---@details 트리거 진입 이벤트
---@param other Collider 충돌한 콜라이더
function onTriggerEnter(other)
    if not isFired then return end

    local otherName = other.gameObject.name
    local otherTag = other.gameObject.tag

    -- 타겟 충돌 확인
    if otherTag == "Target" or string.find(otherName, "Target") or string.find(otherName, "target") then
        OnHitTarget(other.gameObject)
        return
    end

    -- 벽/바닥 충돌 확인 (선택적)
    if otherTag == "Wall" or otherTag == "Ground" or otherTag == "Obstacle" then
        OnHitObstacle(other.gameObject)
        return
    end
end

---@details 충돌 이벤트 (물리 충돌)
---@param collision Collision 충돌 정보
function onCollisionEnter(collision)
    if not isFired then return end

    local otherName = collision.gameObject.name
    local otherTag = collision.gameObject.tag

    -- 타겟 충돌 확인
    if otherTag == "Target" or string.find(otherName, "Target") or string.find(otherName, "target") then
        OnHitTarget(collision.gameObject)
        return
    end

    -- 벽/바닥 충돌
    if otherTag == "Wall" or otherTag == "Ground" or otherTag == "Obstacle" then
        OnHitObstacle(collision.gameObject)
        return
    end
end

---@details 타겟 히트 처리
---@param targetObject GameObject 히트된 타겟
function OnHitTarget(targetObject)
    Debug.Log("총알이 타겟 맞춤: " .. targetObject.name)

    -- 히트 이펙트 재생
    PlayHitEffect()

    -- 타겟의 피격 처리는 Target.lua에서 onTriggerEnter로 처리됨
    -- 여기서는 총알만 풀로 반환

    ReturnToPool()
end

---@details 장애물 히트 처리
---@param obstacleObject GameObject 히트된 장애물
function OnHitObstacle(obstacleObject)
    Debug.Log("총알이 장애물 맞춤: " .. obstacleObject.name)

    ReturnToPool()
end

--endregion

--region Effects

---@details 히트 이펙트 재생
function PlayHitEffect()
    if HitEffectObject then
        -- 이펙트 위치 설정 및 재생
        HitEffectObject.transform.position = self.transform.position

        local particleSystem = HitEffectObject:GetComponent(typeof(CS.UnityEngine.ParticleSystem))
        if particleSystem then
            particleSystem:Play()
        end
    end
end

--endregion

--region Pool Management

---@details 풀로 반환
function ReturnToPool()
    if not isFired then return end

    isFired = false

    -- 코루틴 정지
    StopAutoReturnRoutine()

    -- Rigidbody 정지
    if rigidbody then
        rigidbody.linearVelocity = Vector3.zero
        rigidbody.angularVelocity = Vector3.zero
    end

    -- 소유 총에 반환 알림
    if ownerGun and ownerGun.ReturnBulletToPool then
        ownerGun.ReturnBulletToPool(poolIndex)
    end

    Debug.Log("총알 풀 반환: " .. poolIndex)
end

---@details 자동 반환 코루틴 시작
function StartAutoReturnRoutine()
    StopAutoReturnRoutine()

    autoReturnRoutine = self:StartCoroutine(util.cs_generator(function()
        coroutine.yield(WaitForSeconds(autoReturnTime))
        if isFired then
            Debug.Log("총알 자동 반환 (시간 초과)")
            ReturnToPool()
        end
    end))
end

---@details 자동 반환 코루틴 정지
function StopAutoReturnRoutine()
    if autoReturnRoutine then
        self:StopCoroutine(autoReturnRoutine)
        autoReturnRoutine = nil
    end
end

--endregion

--region Public Functions

---@details 총알 발사
---@param _ any self
---@param direction Vector3 발사 방향
---@param speed number 발사 속도
---@param index number 풀 인덱스
function Fire(_, direction, speed, index)
    isFired = true
    fireDirection = direction
    fireSpeed = speed
    fireTime = Time.time

    if index then
        poolIndex = index
    end

    -- Rigidbody 속도 설정
    if rigidbody then
        rigidbody.linearVelocity = direction * speed
    end

    -- 자동 반환 코루틴 시작
    StartAutoReturnRoutine()

    Debug.Log("총알 발사! 속도: " .. speed)
end

---@details 소유 총 설정
---@param gun ShootingGun 총 스크립트
function SetGun(gun)
    ownerGun = gun
end

---@details 풀 인덱스 설정
---@param index number 인덱스
function SetPoolIndex(index)
    poolIndex = index
end

---@details 풀 인덱스 반환
---@return number
function GetPoolIndex()
    return poolIndex
end

---@details 발사 상태 반환
---@return boolean
function IsFired()
    return isFired
end

---@details 게임 매니저 설정
---@param manager ConveyorGameManager
function SetGameManager(manager)
    gameManager = manager
end

---@details 자동 반환 시간 설정
---@param time number 초
function SetAutoReturnTime(time)
    autoReturnTime = time
end

--endregion
