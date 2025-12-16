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

---@type table
---@details 소속 총 Lua 스크립트 테이블
local ownerGunScript = nil

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

    -- ShootingGun Lua 스크립트 찾기
    FindOwnerGunScript()
end

---@details ShootingGun Lua 스크립트 테이블 찾기
function FindOwnerGunScript()
    if ownerGunScript then return end

    -- 방법 1: 씬에서 이름으로 찾기
    local gunNames = {
        "Gun_Cosmic_Retro_Blaster_1",
        "Gun_Cosmic_Retro_Blaster",
        "ShootingGun",
        "Gun"
    }

    for _, name in ipairs(gunNames) do
        local gunObj = GameObject.Find(name)
        if gunObj then
            local script = gunObj:GetLuaComponent("ShootingGun")
            if script and script.ReturnBulletToPool then
                ownerGunScript = script
                return
            end
        end
    end

    -- 방법 2: 부모 계층에서 찾기
    local current = self.transform.parent
    while current do
        local script = current.gameObject:GetLuaComponent("ShootingGun")
        if script and script.ReturnBulletToPool then
            ownerGunScript = script
            return
        end
        current = current.parent
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

    -- 자동 반환 시간 체크
    if Time.time - fireTime > autoReturnTime then
        ForceReturnToPool()
        return
    end

    -- 범위 밖 체크 (너무 멀리 날아가면 반환)
    local pos = self.transform.position
    if pos.y < -50 or pos.y > 100 or math.abs(pos.x) > 100 or math.abs(pos.z) > 100 then
        ForceReturnToPool()
    end
end

function fixedUpdate()
    -- 물리 엔진이 알아서 처리하도록 비워둠
    -- (매 프레임 속도 강제 설정하면 kinematic처럼 동작함)
end

--endregion

--region Collision Detection

---@details 타겟인지 확인
---@param objName string 오브젝트 이름
---@param objTag string 오브젝트 태그
---@return boolean
function IsTarget(objName, objTag)
    -- 태그로 확인
    if objTag == "Target" then return true end

    -- 이름으로 확인 (Ball 종류들)
    if string.find(objName, "Ball") or string.find(objName, "ball") then return true end
    if string.find(objName, "Target") or string.find(objName, "target") then return true end

    return false
end

---@details 트리거 진입 이벤트
---@param other Collider 충돌한 콜라이더
function onTriggerEnter(other)
    if not isFired then return end

    local otherName = other.gameObject.name
    local otherTag = other.gameObject.tag

    -- 타겟 충돌 확인 (Ball 포함)
    if IsTarget(otherName, otherTag) then
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

    -- Gun/Bullet 무시 (자기 자신이나 총과 충돌 무시)
    if string.find(otherName, "Gun") or string.find(otherName, "Bullet") or otherTag == "Bullet" then
        return
    end

    -- 타겟 충돌 확인 (Ball 포함)
    if IsTarget(otherName, otherTag) then
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
    -- 타겟에 힘 가하기 (튕겨나가게)
    local targetRb = targetObject:GetComponent(typeof(CS.UnityEngine.Rigidbody))
    if targetRb and fireDirection then
        local hitForce = fireDirection * fireSpeed * 2.0
        targetRb:AddForce(hitForce, CS.UnityEngine.ForceMode.Impulse)
    end

    -- 타겟 스크립트에 히트 알림
    local targetScript = targetObject:GetLuaComponent("Target")
    if targetScript and targetScript.OnHitByBullet then
        targetScript.OnHitByBullet(self.gameObject)
    end

    PlayHitEffect()
    ReturnToPool()
end

---@details 장애물 히트 처리
---@param obstacleObject GameObject 히트된 장애물
function OnHitObstacle(obstacleObject)
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

---@details 강제 풀 반환 (isFired 체크 안함)
function ForceReturnToPool()
    isFired = false

    -- 코루틴 정지
    StopAutoReturnRoutine()

    -- Rigidbody 정지
    if rigidbody then
        rigidbody.linearVelocity = Vector3.zero
        rigidbody.angularVelocity = Vector3.zero
    end

    -- ownerGun이 없으면 다시 찾기
    if not ownerGunScript then
        FindOwnerGunScript()
    end

    -- 소유 총에 반환 알림
    if ownerGunScript and ownerGunScript.ReturnBulletToPool then
        ownerGunScript.ReturnBulletToPool(poolIndex)
    else
        -- ShootingGun에서 update로 처리하도록 숨기기만 함
        HideBullet()
    end
end

---@details 총알 직접 숨김 처리
function HideBullet()
    self.transform.position = Vector3(0, -9999, 0)

    -- MeshRenderer 비활성화
    local meshRenderers = self:GetComponentsInChildren(typeof(CS.UnityEngine.MeshRenderer))
    if meshRenderers then
        for i = 0, meshRenderers.Length - 1 do
            meshRenderers[i].enabled = false
        end
    end

    -- Collider 비활성화
    local colliders = self:GetComponentsInChildren(typeof(CS.UnityEngine.Collider))
    if colliders then
        for i = 0, colliders.Length - 1 do
            colliders[i].enabled = false
        end
    end
end

---@details 풀로 반환
function ReturnToPool()
    if not isFired then return end
    ForceReturnToPool()
end

---@details 자동 반환 코루틴 시작
function StartAutoReturnRoutine()
    StopAutoReturnRoutine()

    autoReturnRoutine = self:StartCoroutine(util.cs_generator(function()
        coroutine.yield(WaitForSeconds(autoReturnTime))
        if isFired then
            ForceReturnToPool()
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

    -- ShootingGun에서 이미 AddForce로 발사함
    -- 여기서는 타이머만 시작

    -- 자동 반환 코루틴 시작
    StartAutoReturnRoutine()
end

---@details 소유 총 설정 (VivenLuaBehaviour에서 Lua 스크립트 추출)
---@param gunComponent VivenLuaBehaviour 총 컴포넌트
function SetGun(gunComponent)
    -- gunComponent는 VivenLuaBehaviour (C# 컴포넌트)
    -- gameObject에서 Lua 스크립트 테이블을 가져옴
    if gunComponent and gunComponent.gameObject then
        ownerGunScript = gunComponent.gameObject:GetLuaComponent("ShootingGun")
    end
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
