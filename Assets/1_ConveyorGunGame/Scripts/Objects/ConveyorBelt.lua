---컨베이어 벨트 스크립트
---트리거 영역에 있는 Rigidbody에 힘을 가해 이동시킴 (Viven SDK 버전)

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

---@type number
---@details 컨베이어 벨트 속도
speed = 0.8

---@type Vector3
---@details 이동 방향 (로컬 좌표)
direction = Vector3(0, 0, 1)
--endregion

--region Variables
---@type boolean
---@details 디버그 로그 출력 여부
local enableDebugLog = false

---@type number
---@details 힘 배율
local forceMultiplier = 2.0
--endregion

--region Unity Lifecycle
function awake()
    -- direction이 주입되지 않았으면 기본값 설정
    if direction == nil then
        direction = Vector3(0, 0, 1)
    end
end

function start()
    -- 초기화 완료
end
--endregion

--region Trigger Events
---@details 트리거 영역에 머무는 동안 호출
---@param other Collider 충돌한 콜라이더
function onTriggerStay(other)
    -- Rigidbody 확인
    local rb = other.gameObject:GetComponent(typeof(CS.UnityEngine.Rigidbody))

    if rb ~= nil then
        -- 디버그 로그
        if enableDebugLog then
            Debug.Log(other.gameObject.name .. " 공 감지 성공! 힘을 가하는 중.")
        end

        -- Rigidbody가 정지 상태면 깨움
        if rb:IsSleeping() then
            rb:WakeUp()
        end

        -- 월드 방향으로 변환
        local worldDirection = self.transform:TransformDirection(direction)

        -- 힘 적용 (Acceleration 모드)
        rb:AddForce(worldDirection.normalized * speed * forceMultiplier, CS.UnityEngine.ForceMode.Acceleration)
    end
end
--endregion

--region Settings
---@details 속도 설정
---@param newSpeed number 새 속도
function SetSpeed(newSpeed)
    speed = newSpeed
end

---@details 속도 반환
function GetSpeed()
    return speed
end

---@details 방향 설정
---@param newDirection Vector3 새 방향
function SetDirection(newDirection)
    direction = newDirection
end

---@details 방향 반환
function GetDirection()
    return direction
end

---@details 디버그 로그 활성화/비활성화
---@param enable boolean 활성화 여부
function SetDebugLog(enable)
    enableDebugLog = enable
end

---@details 힘 배율 설정
---@param multiplier number 배율
function SetForceMultiplier(multiplier)
    forceMultiplier = multiplier
end
--endregion
