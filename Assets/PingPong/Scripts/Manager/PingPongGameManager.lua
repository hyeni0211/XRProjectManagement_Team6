---탁구 게임 매니저
---게임 시작, 점수, 난이도 관리

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
---@details 공 발사 기계 오브젝트
BallLauncherObject = checkInject(BallLauncherObject)

---@type GameObject
---@details 점수 텍스트 오브젝트 (TMP_Text가 있는 GameObject)
ScoreTextObject = NullableInject(ScoreTextObject)

---@type GameObject
---@details 게임 시작 버튼 오브젝트
StartButtonObject = NullableInject(StartButtonObject)

---@type GameObject
---@details 난이도 선택 드롭다운 오브젝트
DifficultyDropdownObject = NullableInject(DifficultyDropdownObject)

---@type GameObject
---@details 시작 UI 패널 (시작 버튼, 드롭다운 - 게임 전 표시)
StartUIPanel = NullableInject(StartUIPanel)

---@type GameObject
---@details 게임 UI 패널 (점수 텍스트 - 게임 중 표시)
GameUIPanel = NullableInject(GameUIPanel)
--endregion

--region Variables
---@type BallLauncher
local ballLauncher = nil

---@type TMP_Text
---@details 점수 텍스트 컴포넌트
local scoreText = nil

---@type Button
---@details 게임 시작 버튼 컴포넌트
local startButton = nil

---@type TMP_Dropdown
---@details 난이도 드롭다운 컴포넌트
local difficultyDropdown = nil

---@type number
---@details 현재 점수
local score = 0

---@type number
---@details 현재 난이도 (1: 쉬움, 2: 보통, 3: 어려움)
local difficulty = 1

---@type boolean
---@details 게임 진행 중 여부
local isGameRunning = false

---@type table
---@details 난이도별 설정
local difficultySettings = {
    [1] = { ballSpeed = 3, launchInterval = 3.0, machineSpeed = 1 },   -- 쉬움
    [2] = { ballSpeed = 5, launchInterval = 2.0, machineSpeed = 2 },   -- 보통
    [3] = { ballSpeed = 8, launchInterval = 1.0, machineSpeed = 3 }    -- 어려움
}
--endregion

--region Unity Lifecycle
function awake()
    ballLauncher = BallLauncherObject:GetLuaComponent("BallLauncher")

    -- 점수 UI 초기화
    if ScoreTextObject ~= nil then
        scoreText = ScoreTextObject:GetComponent(typeof(TMP_Text))
    end

    -- 시작 버튼 초기화
    if StartButtonObject ~= nil then
        startButton = StartButtonObject:GetComponent(typeof(Button))
        startButton.onClick:AddListener(OnClickStartButton)
    end

    -- 난이도 드롭다운 초기화
    if DifficultyDropdownObject ~= nil then
        difficultyDropdown = DifficultyDropdownObject:GetComponent(typeof(TMP_Dropdown))
        difficultyDropdown.onValueChanged:AddListener(OnDifficultyChanged)
    end
end

function start()
    -- 기본 난이도로 시작
    SetDifficulty(1)

    -- 초기 점수 UI 표시
    score = 0
    UpdateScoreUI()

    -- 시작 UI 표시, 게임 UI 숨김 (자동 시작 안 함)
    ShowStartUI(true)
    ShowGameUI(false)
end

function onDisable()
    -- 이벤트 리스너 정리
    if startButton ~= nil then
        startButton.onClick:RemoveListener(OnClickStartButton)
    end
    if difficultyDropdown ~= nil then
        difficultyDropdown.onValueChanged:RemoveListener(OnDifficultyChanged)
    end
end
--endregion

--region Game Control
---@details 게임 시작
function StartGame()
    if isGameRunning then return end

    isGameRunning = true
    score = 0

    -- 발사 기계 시작
    if ballLauncher ~= nil then
        Debug.Log("런처가 시작되어야 함.")
        ballLauncher.StartLaunching()
    end

    Debug.Log("탁구 게임 시작! 난이도: " .. difficulty)
end

---@details 게임 정지
function StopGame()
    if not isGameRunning then return end

    isGameRunning = false

    -- 발사 기계 정지
    if ballLauncher ~= nil then
        ballLauncher.StopLaunching()
    end

    Debug.Log("탁구 게임 종료! 최종 점수: " .. score)
end

---@details 게임 리셋
function ResetGame()
    StopGame()
    score = 0
    SetDifficulty(1)
end
--endregion

--region Score
---@details 점수 추가
---@param points number 추가할 점수
function AddScore(points)
    score = score + points
    Debug.Log("점수: " .. score)

    -- UI 업데이트 (UI가 있으면)
    UpdateScoreUI()
end

---@details 점수 UI 업데이트
function UpdateScoreUI()
    if scoreText ~= nil then
        scoreText.text = tostring(score)
    end
end

---@details 현재 점수 반환
function GetScore()
    return score
end
--endregion

--region Difficulty
---@details 난이도 설정
---@param level number 난이도 (1-3)
function SetDifficulty(level)
    if level < 1 then level = 1 end
    if level > 3 then level = 3 end

    difficulty = level
    local settings = difficultySettings[difficulty]

    -- 발사 기계에 설정 적용
    if ballLauncher ~= nil then
        ballLauncher.SetBallSpeed(settings.ballSpeed)
        ballLauncher.SetLaunchInterval(settings.launchInterval)
        ballLauncher.SetMachineSpeed(settings.machineSpeed)
    end

    Debug.Log("난이도 설정: " .. difficulty)
end

---@details 현재 난이도 반환
function GetDifficulty()
    return difficulty
end

---@details 난이도별 설정 반환
function GetDifficultySettings()
    return difficultySettings[difficulty]
end
--endregion

--region Getters
function GetIsGameRunning()
    return isGameRunning
end
--endregion

--region UI Events
---@details 시작 버튼 클릭 이벤트
function OnClickStartButton()
    if isGameRunning then return end

    -- 시작 UI 숨기고, 게임 UI 표시, 게임 시작
    ShowStartUI(false)
    ShowGameUI(true)
    StartGame()

    Debug.Log("시작 버튼 클릭!")
end

---@details 난이도 드롭다운 변경 이벤트
---@param index number 선택된 인덱스 (0부터 시작)
function OnDifficultyChanged(index)
    -- Dropdown index는 0부터 시작, 난이도는 1부터 시작
    local newDifficulty = index + 1
    SetDifficulty(newDifficulty)

    Debug.Log("난이도 변경: " .. newDifficulty)
end

---@details 시작 UI 패널 표시/숨기기
---@param show boolean 표시 여부
function ShowStartUI(show)
    if StartUIPanel ~= nil then
        StartUIPanel:SetActive(show)
    end
end

---@details 게임 UI 패널 표시/숨기기
---@param show boolean 표시 여부
function ShowGameUI(show)
    if GameUIPanel ~= nil then
        GameUIPanel:SetActive(show)
    end
end
--endregion
