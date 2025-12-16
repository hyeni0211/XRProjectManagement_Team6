--- ConveyorGameManager: 컨베이어 건 게임 매니저
--- 게임 시작, 점수, 상태, 타이머 관리 (Viven SDK 버전)

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
---@details 타겟 스폰 매니저 오브젝트
SpawnManagerObject = NullableInject(SpawnManagerObject)

---@type GameObject
---@details 점수 텍스트 오브젝트 (TMP_Text)
ScoreTextObject = NullableInject(ScoreTextObject)

---@type GameObject
---@details 타이머 텍스트 오브젝트 (TMP_Text)
TimerTextObject = NullableInject(TimerTextObject)

---@type GameObject
---@details 시작 UI 패널
StartUIPanel = NullableInject(StartUIPanel)

---@type GameObject
---@details 게임 UI 패널
GameUIPanel = NullableInject(GameUIPanel)

---@type GameObject
---@details 게임 오버 UI 패널
GameOverUIPanel = NullableInject(GameOverUIPanel)

---@type GameObject
---@details 시작 버튼 오브젝트
StartButtonObject = NullableInject(StartButtonObject)

---@type GameObject
---@details 재시작 버튼 오브젝트
RestartButtonObject = NullableInject(RestartButtonObject)

---@type GameObject
---@details 최종 점수 텍스트 오브젝트 (TMP_Text)
FinalScoreTextObject = NullableInject(FinalScoreTextObject)

---@type GameObject
---@details 명중 사운드 오브젝트
HitSoundObject = NullableInject(HitSoundObject)

---@type GameObject
---@details 미스 사운드 오브젝트
MissSoundObject = NullableInject(MissSoundObject)

---@type GameObject
---@details 가이드 텍스트 오브젝트 (현재 맞춰야 할 공 종류 표시)
GuideTextObject = NullableInject(GuideTextObject)

-- 공 종류별 가이드 오브젝트들 (씬에서 자동 Injection)
BallGuide_GolfBall = NullableInject(BallGuide_GolfBall)
BallGuide_BasketBall = NullableInject(BallGuide_BasketBall)
BallGuide_RugbyBall = NullableInject(BallGuide_RugbyBall)
BallGuide_VolleyBall = NullableInject(BallGuide_VolleyBall)
BallGuide_BowlingBall = NullableInject(BallGuide_BowlingBall)
BallGuide_BeachBall = NullableInject(BallGuide_BeachBall)
BallGuide_BaseBall = NullableInject(BallGuide_BaseBall)
BallGuide_SoccerBall = NullableInject(SoccerBall)
BallGuide_TennisBall = NullableInject(BallGuide_TennisBall)

---@type number
---@details 게임 시간 (초)
gameTime = 60

---@type number
---@details 스폰 간격 (초)
spawnInterval = 2.0

---@type number
---@details 최대 타겟 수
maxTargetCount = 10

---@type number
---@details 타겟 놓침 시 감점
missedPenalty = 5

--endregion

--region Variables

local util = require 'xlua.util'

---@type TargetSpawnManager
---@details 스폰 매니저 참조
local spawnManager = nil

---@type TMP_Text
---@details 점수 텍스트 참조
local scoreText = nil

---@type TMP_Text
---@details 타이머 텍스트 참조
local timerText = nil

---@type TMP_Text
---@details 최종 점수 텍스트 참조
local finalScoreText = nil

---@type Button
---@details 시작 버튼 참조
local startButton = nil

---@type Button
---@details 재시작 버튼 참조
local restartButton = nil

---@type AudioSource
---@details 명중 사운드 참조
local hitSound = nil

---@type AudioSource
---@details 미스 사운드 참조
local missSound = nil

---@type number
---@details 현재 점수
local score = 0

---@type number
---@details 명중 횟수
local hitCount = 0

---@type number
---@details 미스 횟수
local missCount = 0

---@type boolean
---@details 게임 진행 중 여부
local isGameRunning = false

---@type boolean
---@details 게임 일시정지 여부
local isGamePaused = false

---@type number
---@details 남은 시간
local remainingTime = 0

---@type Coroutine
---@details 게임 타이머 코루틴
local gameTimerRoutine = nil

---@type TMP_Text
---@details 가이드 텍스트 참조
local guideText = nil

---@type string
---@details 현재 타겟 공 종류
local currentTargetBallType = nil

---@type table
---@details 공 종류 목록
local BALL_TYPES = {
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

---@type table
---@details 공 종류 한글 이름 (UI 표시용)
local BALL_TYPE_NAMES = {
    GolfBall = "골프공",
    BasketBall = "농구공",
    RugbyBall = "럭비공",
    VolleyBall = "배구공",
    BowlingBall = "볼링공",
    BeachBall = "비치볼",
    BaseBall = "야구공",
    SoccerBall = "축구공",
    TennisBall = "테니스공"
}

---@type table
---@details 공 종류별 가이드 오브젝트 참조
local ballGuideObjects = {}

---@type number
---@details 가이드 변경 간격 (초)
local guideChangeInterval = 5.0

---@type Coroutine
---@details 가이드 변경 코루틴
local guideChangeRoutine = nil

---@type number
---@details 정답 점수 (가이드에 맞는 공)
local correctHitScore = 10

---@type number
---@details 오답 점수 (가이드와 다른 공)
local wrongHitScore = 0

--endregion

--region Unity Lifecycle

function awake()
    -- 스폰 매니저 참조
    if SpawnManagerObject then
        spawnManager = SpawnManagerObject:GetLuaComponent("TargetSpawnManager")
    end

    -- UI 텍스트 참조
    if ScoreTextObject then
        scoreText = ScoreTextObject:GetComponent(typeof(CS.TMPro.TMP_Text))
    end

    if TimerTextObject then
        timerText = TimerTextObject:GetComponent(typeof(CS.TMPro.TMP_Text))
    end

    if FinalScoreTextObject then
        finalScoreText = FinalScoreTextObject:GetComponent(typeof(CS.TMPro.TMP_Text))
    end

    -- 버튼 참조
    if StartButtonObject then
        startButton = StartButtonObject:GetComponent(typeof(CS.UnityEngine.UI.Button))
    end

    if RestartButtonObject then
        restartButton = RestartButtonObject:GetComponent(typeof(CS.UnityEngine.UI.Button))
    end

    -- 사운드 참조
    if HitSoundObject then
        hitSound = HitSoundObject:GetComponent(typeof(CS.UnityEngine.AudioSource))
    end

    if MissSoundObject then
        missSound = MissSoundObject:GetComponent(typeof(CS.UnityEngine.AudioSource))
    end

    -- 가이드 텍스트 참조
    if GuideTextObject then
        guideText = GuideTextObject:GetComponent(typeof(CS.TMPro.TMP_Text))
    end

    -- 공 종류별 가이드 오브젝트 참조
    ballGuideObjects = {
        GolfBall = BallGuide_GolfBall,
        BasketBall = BallGuide_BasketBall,
        RugbyBall = BallGuide_RugbyBall,
        VolleyBall = BallGuide_VolleyBall,
        BowlingBall = BallGuide_BowlingBall,
        BeachBall = BallGuide_BeachBall,
        BaseBall = BallGuide_BaseBall,
        SoccerBall = BallGuide_SoccerBall,
        TennisBall = BallGuide_TennisBall
    }
end

function start()
    -- 초기화
    ResetGameStats()
    UpdateAllUI()

    -- UI 초기 상태
    ShowStartUI(true)
    ShowGameUI(false)
    ShowGameOverUI(false)
end

function onEnable()
    -- 버튼 이벤트 등록
    if startButton then
        startButton.onClick:AddListener(OnClickStartButton)
    end

    if restartButton then
        restartButton.onClick:AddListener(OnClickRestartButton)
    end
end

function onDisable()
    -- 이벤트 리스너 정리
    if startButton then
        startButton.onClick:RemoveListener(OnClickStartButton)
    end

    if restartButton then
        restartButton.onClick:RemoveListener(OnClickRestartButton)
    end

    -- 코루틴 정리
    StopGameTimer()
end

--endregion

--region Game Control

---@details 게임 시작
function StartGame()
    if isGameRunning then return end

    isGameRunning = true
    isGamePaused = false
    ResetGameStats()
    remainingTime = gameTime
    UpdateAllUI()

    -- 첫 번째 타겟 공 종류 선택
    SelectRandomTargetBallType()
    StartGuideChangeRoutine()

    -- 스폰 매니저 초기화 및 시작
    if spawnManager then
        spawnManager:InitSpawn({
            spawnInterval = spawnInterval,
            maxTargetCount = maxTargetCount
        })
        spawnManager:StartSpawning()
    end

    -- 타이머 시작
    StartGameTimer()

    -- 햅틱 피드백
    XR.StartControllerVibration(false, 0.3, 0.1)
    XR.StartControllerVibration(true, 0.3, 0.1)

    Debug.Log("컨베이어 건 게임 시작!")
end

---@details 게임 종료
function StopGame()
    if not isGameRunning then return end

    isGameRunning = false
    isGamePaused = false

    -- 스포너 정지
    if spawnManager then
        spawnManager:StopSpawning()
        spawnManager:ClearAllTargets()
    end

    -- 타이머 정지
    StopGameTimer()

    -- 가이드 변경 코루틴 정지
    StopGuideChangeRoutine()

    -- 게임 오버 UI 표시
    ShowStartUI(false)
    ShowGameUI(false)
    ShowGameOverUI(true)
    UpdateFinalScoreUI()

    -- 햅틱 피드백
    XR.StartControllerVibration(false, 0.5, 0.2)
    XR.StartControllerVibration(true, 0.5, 0.2)

    Debug.Log("컨베이어 건 게임 종료! 최종 점수: " .. score .. " (명중: " .. hitCount .. ", 미스: " .. missCount .. ")")
end

---@details 게임 일시정지
function PauseGame()
    if not isGameRunning or isGamePaused then return end

    isGamePaused = true

    if spawnManager then
        spawnManager:PauseSpawning()
    end

    Debug.Log("게임 일시정지")
end

---@details 게임 재개
function ResumeGame()
    if not isGameRunning or not isGamePaused then return end

    isGamePaused = false

    if spawnManager then
        spawnManager:ResumeSpawning()
    end

    Debug.Log("게임 재개")
end

---@details 게임 리셋
function ResetGame()
    StopGame()
    ResetGameStats()
    UpdateAllUI()

    -- UI 초기화
    ShowStartUI(true)
    ShowGameUI(false)
    ShowGameOverUI(false)
end

---@details 게임 통계 리셋
function ResetGameStats()
    score = 0
    hitCount = 0
    missCount = 0
    remainingTime = gameTime
end

--endregion

--region Timer

---@details 게임 타이머 시작
function StartGameTimer()
    StopGameTimer()

    gameTimerRoutine = self:StartCoroutine(util.cs_generator(function()
        while remainingTime > 0 and isGameRunning do
            coroutine.yield(WaitForSeconds(1))

            if not isGamePaused then
                remainingTime = remainingTime - 1
                UpdateTimerUI()

                -- 마지막 10초 햅틱
                if remainingTime <= 10 and remainingTime > 0 then
                    XR.StartControllerVibration(false, 0.1, 0.05)
                    XR.StartControllerVibration(true, 0.1, 0.05)
                end
            end
        end

        if isGameRunning then
            StopGame()
        end
    end))
end

---@details 게임 타이머 정지
function StopGameTimer()
    if gameTimerRoutine then
        self:StopCoroutine(gameTimerRoutine)
        gameTimerRoutine = nil
    end
end

--endregion

--region Score

---@details 타겟 히트 처리 (가이드 체크 포함)
---@param ballType string 맞춘 공의 종류
---@return boolean 정답 여부
function OnTargetHit(ballType)
    if not isGameRunning then return false end

    local isCorrect = (ballType == currentTargetBallType)

    if isCorrect then
        -- 정답: 가이드에 맞는 공
        score = score + correctHitScore
        hitCount = hitCount + 1
        UpdateScoreUI()

        -- 명중 사운드
        PlayHitSound()

        -- 강한 햅틱 피드백
        XR.StartControllerVibration(false, 0.5, 0.1)
        XR.StartControllerVibration(true, 0.5, 0.1)

        Debug.Log("정답! +" .. correctHitScore .. " (" .. ballType .. " = " .. currentTargetBallType .. ")")
    else
        -- 오답: 가이드와 다른 공
        score = score + wrongHitScore
        missCount = missCount + 1
        UpdateScoreUI()

        -- 미스 사운드
        PlayMissSound()

        -- 약한 햅틱 피드백
        XR.StartControllerVibration(false, 0.2, 0.05)
        XR.StartControllerVibration(true, 0.2, 0.05)

        Debug.Log("오답! +" .. wrongHitScore .. " (" .. ballType .. " ≠ " .. currentTargetBallType .. ")")
    end

    return isCorrect
end

---@details 점수 추가 (기존 방식, 호환용)
---@param points number 추가할 점수
function AddScore(points)
    if not isGameRunning then return end

    score = score + points
    hitCount = hitCount + 1
    UpdateScoreUI()

    -- 명중 사운드
    PlayHitSound()

    -- 햅틱 피드백
    XR.StartControllerVibration(false, 0.4, 0.08)
    XR.StartControllerVibration(true, 0.4, 0.08)

    Debug.Log("점수 획득! +" .. points .. " (총: " .. score .. ")")
end

---@details 타겟 놓침 처리
function OnTargetMissed()
    if not isGameRunning then return end

    missCount = missCount + 1

    -- 감점 적용
    score = math.max(0, score - missedPenalty)
    UpdateScoreUI()

    -- 미스 사운드
    PlayMissSound()

    -- 햅틱 피드백 (약하게)
    XR.StartControllerVibration(false, 0.2, 0.05)
    XR.StartControllerVibration(true, 0.2, 0.05)

    Debug.Log("타겟 놓침! -" .. missedPenalty .. " (총: " .. score .. ")")
end

---@details 현재 점수 반환
---@return number
function GetScore()
    return score
end

---@details 명중 횟수 반환
---@return number
function GetHitCount()
    return hitCount
end

---@details 미스 횟수 반환
---@return number
function GetMissCount()
    return missCount
end

---@details 명중률 반환
---@return number 0~100
function GetAccuracy()
    local total = hitCount + missCount
    if total == 0 then return 0 end
    return math.floor((hitCount / total) * 100)
end

--endregion

--region UI

---@details 모든 UI 업데이트
function UpdateAllUI()
    UpdateScoreUI()
    UpdateTimerUI()
end

---@details 점수 UI 업데이트
function UpdateScoreUI()
    if scoreText then
        scoreText.text = tostring(score)
    end
end

---@details 타이머 UI 업데이트
function UpdateTimerUI()
    if timerText then
        local minutes = math.floor(remainingTime / 60)
        local seconds = remainingTime % 60
        timerText.text = string.format("%02d:%02d", minutes, seconds)
    end
end

---@details 최종 점수 UI 업데이트
function UpdateFinalScoreUI()
    if finalScoreText then
        local accuracy = GetAccuracy()
        finalScoreText.text = string.format(
            "점수: %d\n명중: %d | 미스: %d\n명중률: %d%%",
            score, hitCount, missCount, accuracy
        )
    end
end

---@details 시작 UI 표시/숨기기
---@param show boolean 표시 여부
function ShowStartUI(show)
    if StartUIPanel then
        StartUIPanel:SetActive(show)
    end
end

---@details 게임 UI 표시/숨기기
---@param show boolean 표시 여부
function ShowGameUI(show)
    if GameUIPanel then
        GameUIPanel:SetActive(show)
    end
end

---@details 게임 오버 UI 표시/숨기기
---@param show boolean 표시 여부
function ShowGameOverUI(show)
    if GameOverUIPanel then
        GameOverUIPanel:SetActive(show)
    end
end

--endregion

--region UI Events

---@details 시작 버튼 클릭
function OnClickStartButton()
    if isGameRunning then return end

    ShowStartUI(false)
    ShowGameUI(true)
    ShowGameOverUI(false)
    StartGame()

    Debug.Log("시작 버튼 클릭!")
end

---@details 재시작 버튼 클릭
function OnClickRestartButton()
    ShowStartUI(false)
    ShowGameUI(true)
    ShowGameOverUI(false)
    ResetGameStats()
    StartGame()

    Debug.Log("재시작 버튼 클릭!")
end

--endregion

--region Sound

---@details 명중 사운드 재생
function PlayHitSound()
    if hitSound then
        hitSound:Play()
    end
end

---@details 미스 사운드 재생
function PlayMissSound()
    if missSound then
        missSound:Play()
    end
end

--endregion

--region Settings

---@details 게임 시간 설정
---@param time number 게임 시간 (초)
function SetGameTime(time)
    gameTime = time
end

---@details 스폰 간격 설정
---@param interval number 간격 (초)
function SetSpawnInterval(interval)
    spawnInterval = interval
end

---@details 최대 타겟 수 설정
---@param count number 최대 수
function SetMaxTargetCount(count)
    maxTargetCount = count
end

---@details 미스 페널티 설정
---@param penalty number 감점
function SetMissedPenalty(penalty)
    missedPenalty = penalty
end

---@details 남은 시간 반환
---@return number
function GetRemainingTime()
    return remainingTime
end

---@details 게임 진행 중 여부 반환
---@return boolean
function IsGameRunning()
    return isGameRunning
end

---@details 게임 일시정지 여부 반환
---@return boolean
function IsGamePaused()
    return isGamePaused
end

--endregion

--region Guide System

---@details 랜덤으로 타겟 공 종류 선택
function SelectRandomTargetBallType()
    local randomIndex = math.random(1, #BALL_TYPES)
    currentTargetBallType = BALL_TYPES[randomIndex]

    -- 가이드 UI 업데이트
    UpdateGuideUI()

    Debug.Log("새 타겟 공 종류: " .. currentTargetBallType .. " (" .. (BALL_TYPE_NAMES[currentTargetBallType] or currentTargetBallType) .. ")")
end

---@details 가이드 UI 업데이트
function UpdateGuideUI()
    -- 가이드 텍스트 업데이트
    if guideText and currentTargetBallType then
        local displayName = BALL_TYPE_NAMES[currentTargetBallType] or currentTargetBallType
        guideText.text = displayName
    end

    -- 모든 공 가이드 오브젝트 비활성화 후 현재 타겟만 활성화
    for ballType, guideObj in pairs(ballGuideObjects) do
        if guideObj then
            local shouldShow = (ballType == currentTargetBallType)
            guideObj:SetActive(shouldShow)
        end
    end
end

---@details 가이드 변경 코루틴 시작
function StartGuideChangeRoutine()
    StopGuideChangeRoutine()

    guideChangeRoutine = self:StartCoroutine(util.cs_generator(function()
        while isGameRunning do
            coroutine.yield(WaitForSeconds(guideChangeInterval))

            if isGameRunning and not isGamePaused then
                SelectRandomTargetBallType()

                -- 가이드 변경 시 약한 햅틱 피드백
                XR.StartControllerVibration(false, 0.15, 0.05)
                XR.StartControllerVibration(true, 0.15, 0.05)
            end
        end
    end))
end

---@details 가이드 변경 코루틴 정지
function StopGuideChangeRoutine()
    if guideChangeRoutine then
        self:StopCoroutine(guideChangeRoutine)
        guideChangeRoutine = nil
    end
end

---@details 현재 타겟 공 종류 반환
---@return string
function GetCurrentTargetBallType()
    return currentTargetBallType
end

---@details 공 종류가 현재 타겟과 일치하는지 확인
---@param ballType string 확인할 공 종류
---@return boolean
function IsCorrectBallType(ballType)
    return ballType == currentTargetBallType
end

---@details 가이드 변경 간격 설정
---@param interval number 간격 (초)
function SetGuideChangeInterval(interval)
    guideChangeInterval = interval
end

---@details 정답/오답 점수 설정
---@param correct number 정답 점수
---@param wrong number 오답 점수
function SetHitScores(correct, wrong)
    correctHitScore = correct
    wrongHitScore = wrong
end

--endregion
