---공 파괴 스크립트 (바구니)
---트리거 영역에 들어온 공을 파괴 (Viven SDK 버전)

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

---@type number
---@details 공 파괴 시 획득 점수
scorePerBall = 10
--endregion

--region Variables
---@type ConveyorGameManager
local gameManager = nil

---@type number
---@details 파괴된 공 개수
local destroyedCount = 0
--endregion

--region Unity Lifecycle
function awake()
    if GameManagerObject ~= nil then
        gameManager = GameManagerObject:GetLuaComponent("ConveyorGameManager")
    end
end

function start()
    destroyedCount = 0
end
--endregion

--region Trigger Events
---@details 트리거 영역에 진입했을 때 호출
---@param other Collider 충돌한 콜라이더
function onTriggerEnter(other)
    -- Rigidbody가 있는 오브젝트만 처리 (공)
    local rb = other.gameObject:GetComponent(typeof(CS.UnityEngine.Rigidbody))

    if rb ~= nil then
        local ballName = other.gameObject.name

        -- 파괴 카운트 증가
        destroyedCount = destroyedCount + 1

        -- 게임 매니저에 점수 추가
        if gameManager ~= nil then
            gameManager.AddScore(scorePerBall)
        end

        -- 공 파괴
        GameObject.Destroy(other.gameObject)
    end
end
--endregion

--region Public Functions
---@details 파괴된 공 개수 반환
function GetDestroyedCount()
    return destroyedCount
end

---@details 파괴 카운트 리셋
function ResetDestroyedCount()
    destroyedCount = 0
end

---@details 점수 설정
---@param score number 공당 점수
function SetScorePerBall(score)
    scorePerBall = score
end
--endregion
