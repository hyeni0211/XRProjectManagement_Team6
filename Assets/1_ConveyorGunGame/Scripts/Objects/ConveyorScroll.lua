---컨베이어 텍스처 스크롤 스크립트
---머티리얼 텍스처 오프셋을 이동시켜 움직이는 효과 (Viven SDK 버전)

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
---@details 스크롤 속도
scrollSpeed = 0.5

---@type number
---@details 스크롤 방향 X (0 = 비활성화)
scrollDirectionX = 0

---@type number
---@details 스크롤 방향 Y (1 = 기본 방향)
scrollDirectionY = 1
--endregion

--region Variables
---@type Material
local material = nil

---@type Vector2
local currentOffset = nil

---@type boolean
local isScrolling = true
--endregion

--region Unity Lifecycle
function awake()
    -- MeshRenderer에서 머티리얼 가져오기
    local meshRenderer = self:GetComponent(typeof(CS.UnityEngine.MeshRenderer))
    if meshRenderer ~= nil then
        material = meshRenderer.material
    end

    -- 초기 오프셋 설정
    currentOffset = Vector2.new(0, 0)
end

function start()
    if material == nil then
        Debug.LogWarning("ConveyorScroll: MeshRenderer 또는 Material이 없습니다.")
    else
        Debug.Log("컨베이어 스크롤 시작 - 속도: " .. scrollSpeed)
    end
end

function update()
    if material == nil or not isScrolling then
        return
    end

    -- 오프셋 업데이트
    local deltaX = scrollDirectionX * scrollSpeed * Time.deltaTime
    local deltaY = scrollDirectionY * scrollSpeed * Time.deltaTime

    currentOffset.x = currentOffset.x + deltaX
    currentOffset.y = currentOffset.y + deltaY

    -- 오프셋 적용 (BaseMap 텍스처)
    material:SetTextureOffset("_BaseMap", currentOffset)
end

function onDisable()
    -- 오프셋 초기화
    if material ~= nil then
        material:SetTextureOffset("_BaseMap", Vector2.new(0, 0))
    end
end
--endregion

--region Settings
---@details 스크롤 속도 설정
---@param speed number 속도
function SetScrollSpeed(speed)
    scrollSpeed = speed
end

---@details 스크롤 속도 반환
function GetScrollSpeed()
    return scrollSpeed
end

---@details 스크롤 방향 설정
---@param x number X축 방향
---@param y number Y축 방향
function SetScrollDirection(x, y)
    scrollDirectionX = x
    scrollDirectionY = y
end

---@details 스크롤 일시정지/재개
---@param enabled boolean 스크롤 활성화 여부
function SetScrolling(enabled)
    isScrolling = enabled
end

---@details 스크롤 중 여부 반환
function GetIsScrolling()
    return isScrolling
end

---@details 오프셋 초기화
function ResetOffset()
    currentOffset = Vector2.new(0, 0)
    if material ~= nil then
        material:SetTextureOffset("_BaseMap", currentOffset)
    end
end
--endregion
