--- GameUIManager: 게임 UI 관리 스크립트
--- 점수, 타이머, 게임 상태 UI 표시

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
---@details 점수 텍스트 오브젝트 (TMP_Text)
ScoreTextObject = NullableInject(ScoreTextObject)

---@type GameObject
---@details 타이머 텍스트 오브젝트 (TMP_Text)
TimerTextObject = NullableInject(TimerTextObject)

---@type GameObject
---@details 명중 횟수 텍스트 오브젝트 (TMP_Text)
HitCountTextObject = NullableInject(HitCountTextObject)

---@type GameObject
---@details 콤보 텍스트 오브젝트 (TMP_Text)
ComboTextObject = NullableInject(ComboTextObject)

---@type GameObject
---@details 메시지 텍스트 오브젝트 (TMP_Text)
MessageTextObject = NullableInject(MessageTextObject)

---@type number
---@details UI 업데이트 간격 (초)
updateInterval = 0.1

--endregion

--region Variables

local util = require 'xlua.util'

---@type ConveyorGameManager
---@details 게임 매니저 참조
local gameManager = nil

---@type TMP_Text
---@details 점수 텍스트 참조
local scoreText = nil

---@type TMP_Text
---@details 타이머 텍스트 참조
local timerText = nil

---@type TMP_Text
---@details 명중 횟수 텍스트 참조
local hitCountText = nil

---@type TMP_Text
---@details 콤보 텍스트 참조
local comboText = nil

---@type TMP_Text
---@details 메시지 텍스트 참조
local messageText = nil

---@type number
---@details 마지막 업데이트 시간
local lastUpdateTime = 0

---@type number
---@details 현재 콤보
local currentCombo = 0

---@type number
---@details 마지막 점수 (변화 감지용)
local lastScore = 0

---@type any
---@details 메시지 숨기기 코루틴
local hideMessageRoutine = nil

--endregion

--region Unity Lifecycle

function awake()
    -- 게임 매니저 참조
    if GameManagerObject then
        gameManager = GameManagerObject:GetLuaComponent("ConveyorGameManager")
    end

    -- UI 텍스트 참조
    if ScoreTextObject then
        scoreText = ScoreTextObject:GetComponent(typeof(CS.TMPro.TMP_Text))
    end

    if TimerTextObject then
        timerText = TimerTextObject:GetComponent(typeof(CS.TMPro.TMP_Text))
    end

    if HitCountTextObject then
        hitCountText = HitCountTextObject:GetComponent(typeof(CS.TMPro.TMP_Text))
    end

    if ComboTextObject then
        comboText = ComboTextObject:GetComponent(typeof(CS.TMPro.TMP_Text))
    end

    if MessageTextObject then
        messageText = MessageTextObject:GetComponent(typeof(CS.TMPro.TMP_Text))
    end
end

function start()
    -- 초기 UI 설정
    lastScore = 0
    currentCombo = 0
    UpdateAllUI()
    HideMessage()
end

function update()
    -- 일정 간격으로 UI 업데이트
    if Time.time - lastUpdateTime >= updateInterval then
        lastUpdateTime = Time.time
        UpdateAllUI()
        CheckScoreChange()
    end
end

function onDisable()
    -- 코루틴 정리
    StopHideMessageRoutine()
end

--endregion

--region UI Update

---@details 모든 UI 업데이트
function UpdateAllUI()
    UpdateScoreUI()
    UpdateTimerUI()
    UpdateHitCountUI()
    UpdateComboUI()
end

---@details 점수 UI 업데이트
function UpdateScoreUI()
    if not scoreText or not gameManager then return end

    local score = gameManager.GetScore()
    scoreText.text = string.format("%d", score)
end

---@details 타이머 UI 업데이트
function UpdateTimerUI()
    if not timerText or not gameManager then return end

    local remainingTime = gameManager.GetRemainingTime()
    local minutes = math.floor(remainingTime / 60)
    local seconds = remainingTime % 60

    timerText.text = string.format("%02d:%02d", minutes, seconds)

    -- 마지막 10초는 빨간색
    if remainingTime <= 10 then
        timerText.color = CS.UnityEngine.Color.red
    else
        timerText.color = CS.UnityEngine.Color.white
    end
end

---@details 명중 횟수 UI 업데이트
function UpdateHitCountUI()
    if not hitCountText or not gameManager then return end

    local hitCount = gameManager.GetHitCount()
    local missCount = gameManager.GetMissCount()

    hitCountText.text = string.format("Hit: %d | Miss: %d", hitCount, missCount)
end

---@details 콤보 UI 업데이트
function UpdateComboUI()
    if not comboText then return end

    if currentCombo >= 2 then
        comboText.text = string.format("x%d COMBO!", currentCombo)
        comboText.gameObject:SetActive(true)
    else
        comboText.gameObject:SetActive(false)
    end
end

--endregion

--region Score Change Detection

---@details 점수 변화 감지
function CheckScoreChange()
    if not gameManager then return end

    local currentScore = gameManager.GetScore()

    if currentScore > lastScore then
        -- 점수 증가 = 명중
        currentCombo = currentCombo + 1
        OnScoreIncrease(currentScore - lastScore)
    elseif currentScore < lastScore then
        -- 점수 감소 = 미스
        currentCombo = 0
        OnScoreDecrease(lastScore - currentScore)
    end

    lastScore = currentScore
end

---@details 점수 증가 시 호출
---@param amount number 증가량
function OnScoreIncrease(amount)
    -- 콤보 메시지 표시
    if currentCombo >= 3 then
        ShowMessage("COMBO x" .. currentCombo .. "!", 1.5)
    elseif currentCombo >= 5 then
        ShowMessage("EXCELLENT!", 2.0)
    elseif currentCombo >= 10 then
        ShowMessage("INCREDIBLE!", 2.5)
    end

    Debug.Log("점수 증가: +" .. amount .. " (콤보: " .. currentCombo .. ")")
end

---@details 점수 감소 시 호출
---@param amount number 감소량
function OnScoreDecrease(amount)
    ShowMessage("MISS!", 1.0)
    Debug.Log("점수 감소: -" .. amount)
end

--endregion

--region Message Display

---@details 메시지 표시
---@param msg string 메시지 내용
---@param duration number 표시 시간 (초)
function ShowMessage(msg, duration)
    if not messageText then return end

    StopHideMessageRoutine()

    messageText.text = msg
    messageText.gameObject:SetActive(true)

    -- 일정 시간 후 숨기기
    hideMessageRoutine = self:StartCoroutine(util.cs_generator(function()
        coroutine.yield(WaitForSeconds(duration))
        HideMessage()
    end))
end

---@details 메시지 숨기기
function HideMessage()
    if messageText then
        messageText.gameObject:SetActive(false)
    end
end

---@details 메시지 숨기기 코루틴 정지
function StopHideMessageRoutine()
    if hideMessageRoutine then
        self:StopCoroutine(hideMessageRoutine)
        hideMessageRoutine = nil
    end
end

--endregion

--region Public Functions

---@details 콤보 리셋
function ResetCombo()
    currentCombo = 0
    UpdateComboUI()
end

---@details 콤보 증가
function AddCombo()
    currentCombo = currentCombo + 1
    UpdateComboUI()
end

---@details 현재 콤보 반환
---@return number
function GetCombo()
    return currentCombo
end

---@details 게임 매니저 설정
---@param manager ConveyorGameManager
function SetGameManager(manager)
    gameManager = manager
end

---@details UI 업데이트 간격 설정
---@param interval number 초
function SetUpdateInterval(interval)
    updateInterval = interval
end

--endregion
