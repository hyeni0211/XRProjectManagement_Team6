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
---@details 총알 발사 속도 (5~10 권장)
bulletSpeed = 8.0

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

---@type boolean
---@details 이벤트 기반 트리거 사용 여부
local useEventBasedTrigger = false

---@type boolean
---@details 홀드(연사) 상태
local isHolding = false

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

---@type table
---@details 총알 발사 시간 기록 {poolIndex = fireTime}
local bulletFireTimes = {}

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

---@type number
---@details 총알 자동 반환 시간
local bulletAutoReturnTime = 3.0

function update()
    -- 발사된 총알 자동 반환 체크
    CheckAndReturnBullets()
end

---@details 발사된 총알 체크 및 자동 반환
function CheckAndReturnBullets()
    if #bulletPool.inUse == 0 then return end

    local currentTime = Time.time
    local toReturn = {}

    -- 반환할 총알 수집
    for _, poolIndex in ipairs(bulletPool.inUse) do
        local bulletObj = bulletObjects[poolIndex]

        if bulletObj then
            local shouldReturn = false

            -- 1. 발사 시간 체크 (ShootingGun이 자체 기록)
            local fireTime = bulletFireTimes[poolIndex]
            if fireTime and (currentTime - fireTime) > bulletAutoReturnTime then
                shouldReturn = true
            end

            -- 2. 위치 체크 (범위 밖이면 반환)
            local pos = bulletObj.transform.position
            if pos.y < -50 or pos.y > 100 or math.abs(pos.x) > 100 or math.abs(pos.z) > 100 then
                shouldReturn = true
            end

            if shouldReturn then
                table.insert(toReturn, poolIndex)
            end
        end
    end

    -- 수집된 총알 반환
    for _, poolIndex in ipairs(toReturn) do
        ReturnBulletToPool(poolIndex)
        bulletFireTimes[poolIndex] = nil
    end
end

function onEnable()
    -- 이벤트 리스너 등록
    if grabbableModule then
        grabbableModule.onGrabEvent:AddListener(OnGrab)
        grabbableModule.onReleaseEvent:AddListener(OnRelease)

        -- Viven SDK 트리거 이벤트 (objectShortClickAction = 짧게 클릭)
        if grabbableModule.objectShortClickAction then
            grabbableModule.objectShortClickAction:AddListener(OnTriggerClick)
            useEventBasedTrigger = true
        end

        -- 길게 누르기 이벤트 (연사용)
        if grabbableModule.objectHoldActionStart then
            grabbableModule.objectHoldActionStart:AddListener(OnHoldStart)
        end
        if grabbableModule.objectHoldActionEnd then
            grabbableModule.objectHoldActionEnd:AddListener(OnHoldEnd)
        end
    end
end

function onDisable()
    -- 이벤트 리스너 해제
    if grabbableModule then
        grabbableModule.onGrabEvent:RemoveListener(OnGrab)
        grabbableModule.onReleaseEvent:RemoveListener(OnRelease)

        -- Viven SDK 트리거 이벤트 해제
        if grabbableModule.objectShortClickAction then
            grabbableModule.objectShortClickAction:RemoveListener(OnTriggerClick)
        end
        if grabbableModule.objectHoldActionStart then
            grabbableModule.objectHoldActionStart:RemoveListener(OnHoldStart)
        end
        if grabbableModule.objectHoldActionEnd then
            grabbableModule.objectHoldActionEnd:RemoveListener(OnHoldEnd)
        end
    end
end

function update()
    if not isGrabbed then return end
    if useEventBasedTrigger then return end -- 이벤트 기반이면 폴링 불필요

    -- 트리거 입력 확인 (폴링 방식)
    CheckTriggerInput()
end

--endregion

--region Input

---@details 트리거 입력 확인 (Unity Input System 사용)
function CheckTriggerInput()
    -- Unity Input System을 통한 트리거 입력 확인
    local triggerValue = 0

    -- pcall로 안전하게 API 호출 시도
    local success, result = pcall(function()
        if isLeftHand then
            -- 왼손 트리거
            return CS.UnityEngine.Input.GetAxis("XRI_Left_Trigger")
        else
            -- 오른손 트리거
            return CS.UnityEngine.Input.GetAxis("XRI_Right_Trigger")
        end
    end)

    if success and result then
        triggerValue = result
    else
        -- 폴백: 마우스 클릭으로 테스트 (에디터용)
        if CS.UnityEngine.Input.GetMouseButton(0) then
            triggerValue = 1.0
        end
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

---@details 짧게 클릭 이벤트 (Viven SDK objectShortClickAction)
function OnTriggerClick()
    if not isGrabbed then return end
    TryShoot()
end

---@details 길게 누르기 시작 (Viven SDK objectHoldActionStart) - 연사 시작
function OnHoldStart()
    if not isGrabbed then return end
    isHolding = true
    TryShoot()
end

---@details 길게 누르기 종료 (Viven SDK objectHoldActionEnd) - 연사 종료
function OnHoldEnd()
    isHolding = false
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
end

---@details 놓기 이벤트
function OnRelease()
    isGrabbed = false
    isTriggerPressed = false
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
        return
    end

    -- 총알 위치 및 방향 설정
    local shootPos = ShootPoint.transform.position
    local shootDir = ShootPoint.transform:TransformDirection(Vector3.forward)

    -- 총알 위치/회전 설정
    bulletObj.transform.position = shootPos
    bulletObj.transform.rotation = ShootPoint.transform.rotation

    -- 총알 활성화
    SetBulletVisible(poolIndex, true)

    -- 발사 시간 기록 (자동 반환용)
    bulletFireTimes[poolIndex] = Time.time

    -- Rigidbody로 발사 (Impulse - 한 번에 빵!)
    local rb = bulletObj:GetComponent(typeof(CS.UnityEngine.Rigidbody))
    if rb then
        rb.linearVelocity = Vector3.zero
        rb.angularVelocity = Vector3.zero
        -- Impulse: 질량 고려한 순간적인 힘 (F = m * v)
        rb:AddForce(shootDir * bulletSpeed * rb.mass, CS.UnityEngine.ForceMode.Impulse)
    end

    -- 총알 스크립트 Fire 호출 (타이머용)
    if bulletScript then
        bulletScript:Fire(shootDir, bulletSpeed, poolIndex)
    end

    -- 사운드 및 햅틱
    PlayShootSound()
    PlayShootHaptic()
end

--endregion

--region Bullet Pool

---@details 총알 풀 초기화
function InitializeBulletPool()
    if not BulletPool then
        Debug.Log("[ERROR] BulletPool이 연결되지 않았습니다!")
        return
    end

    bulletObjects = {}
    bulletScripts = {}
    bulletMeshRenderers = {}
    bulletColliders = {}
    bulletPool.available = {}
    bulletPool.inUse = {}

    local missingScripts = 0

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
        else
            missingScripts = missingScripts + 1
        end

        -- 비활성화
        SetBulletVisible(index, false)
    end

    -- 초기화 결과 요약 (1회만 출력)
    if missingScripts > 0 then
        -- Debug.Log("[WARNING] Bullet 스크립트 없는 오브젝트: " .. missingScripts .. "개")
    end
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
