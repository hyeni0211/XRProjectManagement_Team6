--- ShootingGun: 슈팅 건 스크립트
--- VR에서 총을 잡고 발사하는 기능

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
---@details 총알 발사 위치 (Shoot Start Point)
ShootPoint = checkInject(ShootPoint)

---@type GameObject
---@details 총알 풀 부모 오브젝트
BulletPool = checkInject(BulletPool)

---@type GameObject
---@details 게임 매니저 오브젝트
GameManagerObject = NullableInject(GameManagerObject)

---@type GameObject
---@details 발사 사운드 AudioSource 오브젝트
ShootSoundObject = NullableInject(ShootSoundObject)

---@type number
---@details 총알 발사 속도
bulletSpeed = 30.0

---@type number
---@details 발사 간격 (초)
shootCooldown = 0.2

--endregion

--region Variables

local util = require 'xlua.util'

---@type VivenGrabbableModule
---@details 잡기 모듈
local grabbableModule = nil

---@type ConveyorGameManager
---@details 게임 매니저 참조
local gameManager = nil

---@type AudioSource
---@details 발사 사운드
local shootSound = nil

---@type boolean
---@details 현재 잡힌 상태
local isGrabbed = false

---@type boolean
---@details 현재 트리거 누름 상태
local isTriggerPressed = false

---@type number
---@details 마지막 발사 시간
local lastShootTime = 0

---@type boolean
---@details 왼손/오른손 여부
local isLeftHand = false

--endregion

--region Pool Variables

---@type table
---@details 풀 테이블
local bulletPool = { available = {}, inUse = {} }

---@type table
---@details 풀 오브젝트 배열
local bulletObjects = {}

---@type table
---@details 풀 스크립트 배열
local bulletScripts = {}

---@type table
---@details MeshRenderer 배열의 배열
local bulletMeshRenderers = {}

---@type table
---@details Collider 배열의 배열
local bulletColliders = {}

---@type Vector3
---@details 숨김 위치
local HIDE_POSITION = nil

---@type boolean
---@details 초기화 완료 여부
local isInitialized = false

--endregion

--region Unity Lifecycle

function awake()
    -- 컴포넌트 가져오기
    grabbableModule = self:GetComponent("VivenGrabbableModule")

    -- 게임 매니저 참조
    if GameManagerObject then
        gameManager = GameManagerObject:GetLuaComponent("ConveyorGameManager")
    end

    -- 사운드 참조
    if ShootSoundObject then
        shootSound = ShootSoundObject:GetComponent(typeof(CS.UnityEngine.AudioSource))
    end

    -- 숨김 위치
    HIDE_POSITION = Vector3(0, -9999, 0)

    -- 총알 풀 초기화
    InitializeBulletPool()

    isInitialized = true
end

function start()
    lastShootTime = 0
end

function onEnable()
    -- 이벤트 리스너 등록
    if grabbableModule then
        grabbableModule.onGrabEvent:AddListener(OnGrab)
        grabbableModule.onReleaseEvent:AddListener(OnRelease)
    end
end

function onDisable()
    -- 이벤트 리스너 해제
    if grabbableModule then
        grabbableModule.onGrabEvent:RemoveListener(OnGrab)
        grabbableModule.onReleaseEvent:RemoveListener(OnRelease)
    end
end

function update()
    if not isGrabbed then return end

    -- 트리거 입력 확인
    CheckTriggerInput()
end

--endregion

--region Input

---@details 트리거 입력 확인
function CheckTriggerInput()
    -- XR 컨트롤러 트리거 입력 확인
    local triggerValue = 0

    if isLeftHand then
        triggerValue = XR.GetLeftTriggerValue()
    else
        triggerValue = XR.GetRightTriggerValue()
    end

    -- 트리거 누름 감지 (임계값 0.5)
    if triggerValue > 0.5 then
        if not isTriggerPressed then
            isTriggerPressed = true
            OnTriggerPressed()
        end
    else
        isTriggerPressed = false
    end
end

---@details 트리거 눌렸을 때
function OnTriggerPressed()
    TryShoot()
end

--endregion

--region Grab Events

---@details 잡기 이벤트
function OnGrab()
    isGrabbed = true

    -- 어느 손으로 잡았는지 확인
    if grabbableModule then
        isLeftHand = grabbableModule.isLeftHandGrabbing
    end

    -- 잡기 햅틱
    PlayGrabHaptic()

    Debug.Log("총 잡음 - " .. (isLeftHand and "왼손" or "오른손"))
end

---@details 놓기 이벤트
function OnRelease()
    isGrabbed = false
    isTriggerPressed = false

    Debug.Log("총 놓음")
end

--endregion

--region Shooting

---@details 발사 시도
function TryShoot()
    local currentTime = Time.time

    -- 쿨다운 확인
    if currentTime - lastShootTime < shootCooldown then
        return
    end

    lastShootTime = currentTime

    -- 발사
    Shoot()
end

---@details 총알 발사
function Shoot()
    -- 총알 풀에서 가져오기
    local bulletObj, bulletScript, poolIndex = GetBulletFromPool()

    if not bulletObj then
        Debug.LogWarning("총알 풀이 비어있습니다!")
        return
    end

    -- 총알 위치 및 방향 설정
    local shootPos = ShootPoint.transform.position
    local shootDir = ShootPoint.transform.forward

    bulletObj.transform.position = shootPos
    bulletObj.transform.rotation = ShootPoint.transform.rotation

    -- 총알 활성화
    SetBulletVisible(poolIndex, true)

    -- 총알 발사
    if bulletScript then
        bulletScript:Fire(shootDir, bulletSpeed, poolIndex)
    else
        -- 스크립트가 없으면 직접 Rigidbody로 발사
        local rb = bulletObj:GetComponent(typeof(CS.UnityEngine.Rigidbody))
        if rb then
            rb.linearVelocity = shootDir * bulletSpeed
        end
    end

    -- 사운드 재생
    PlayShootSound()

    -- 햅틱 피드백
    PlayShootHaptic()

    Debug.Log("총알 발사!")
end

--endregion

--region Bullet Pool

---@details 총알 풀 초기화
function InitializeBulletPool()
    if not BulletPool then return end

    bulletObjects = {}
    bulletScripts = {}
    bulletMeshRenderers = {}
    bulletColliders = {}
    bulletPool.available = {}
    bulletPool.inUse = {}

    for i = 0, BulletPool.transform.childCount - 1 do
        local child = BulletPool.transform:GetChild(i).gameObject
        local index = i + 1

        bulletObjects[index] = child

        -- Bullet 스크립트 가져오기
        local bulletScript = child:GetLuaComponent("Bullet")
        bulletScripts[index] = bulletScript

        -- MeshRenderer 수집
        local meshRenderers = child:GetComponentsInChildren(typeof(CS.UnityEngine.MeshRenderer))
        local tempMeshes = {}
        for j = 0, meshRenderers.Length - 1 do
            tempMeshes[#tempMeshes + 1] = meshRenderers[j]
        end
        bulletMeshRenderers[index] = tempMeshes

        -- Collider 수집
        local colliders = child:GetComponentsInChildren(typeof(CS.UnityEngine.Collider))
        local tempColliders = {}
        for j = 0, colliders.Length - 1 do
            tempColliders[#tempColliders + 1] = colliders[j]
        end
        bulletColliders[index] = tempColliders

        -- available에 추가
        table.insert(bulletPool.available, index)

        -- Bullet 스크립트 초기화
        if bulletScript then
            bulletScript.SetGun(self)
            bulletScript.SetPoolIndex(index)
        end

        -- 비활성화
        SetBulletVisible(index, false)
    end

    Debug.Log("총알 풀 초기화 완료 - 크기: " .. #bulletObjects)
end

---@details 총알 가시성 설정
---@param poolIndex number 풀 인덱스
---@param visible boolean 가시성
function SetBulletVisible(poolIndex, visible)
    local obj = bulletObjects[poolIndex]
    if not obj then return end

    -- MeshRenderer 토글
    local meshRenderers = bulletMeshRenderers[poolIndex]
    if meshRenderers then
        for _, mr in ipairs(meshRenderers) do
            mr.enabled = visible
        end
    end

    -- Collider 토글
    local colliders = bulletColliders[poolIndex]
    if colliders then
        for _, col in ipairs(colliders) do
            col.enabled = visible
        end
    end

    -- 숨김 위치 이동
    if not visible then
        obj.transform.position = HIDE_POSITION
    end
end

---@details 풀에서 총알 가져오기
---@return GameObject|nil, Bullet|nil, number
function GetBulletFromPool()
    if #bulletPool.available == 0 then
        return nil, nil, -1
    end

    local index = bulletPool.available[1]
    table.remove(bulletPool.available, 1)
    table.insert(bulletPool.inUse, index)

    return bulletObjects[index], bulletScripts[index], index
end

---@details 총알을 풀로 반환
---@param poolIndex number 풀 인덱스
function ReturnBulletToPool(poolIndex)
    -- inUse에서 제거
    for i = #bulletPool.inUse, 1, -1 do
        if bulletPool.inUse[i] == poolIndex then
            table.remove(bulletPool.inUse, i)
            break
        end
    end

    -- 이미 available에 있는지 확인
    for _, idx in ipairs(bulletPool.available) do
        if idx == poolIndex then
            return
        end
    end

    -- available에 추가
    table.insert(bulletPool.available, poolIndex)

    -- 비활성화
    SetBulletVisible(poolIndex, false)
end

--endregion

--region Effects

---@details 발사 사운드 재생
function PlayShootSound()
    if shootSound then
        shootSound:Play()
    end
end

---@details 발사 햅틱 피드백
function PlayShootHaptic()
    if isLeftHand then
        XR.StartControllerVibration(true, 0.7, 0.1)
    else
        XR.StartControllerVibration(false, 0.7, 0.1)
    end
end

---@details 잡기 햅틱 피드백
function PlayGrabHaptic()
    if isLeftHand then
        XR.StartControllerVibration(true, 0.3, 0.05)
    else
        XR.StartControllerVibration(false, 0.3, 0.05)
    end
end

--endregion

--region Public Functions

---@details 잡힌 상태 반환
---@return boolean
function IsGrabbed()
    return isGrabbed
end

---@details 총알 속도 설정
---@param speed number 속도
function SetBulletSpeed(speed)
    bulletSpeed = speed
end

---@details 쿨다운 설정
---@param cooldown number 초
function SetShootCooldown(cooldown)
    shootCooldown = cooldown
end

--endregion
