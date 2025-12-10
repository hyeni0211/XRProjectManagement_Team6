# Viven IStep 게임 플로우 생성

타임라인 기반 단계별 게임 진행 시스템을 설정합니다.

## IStep 클래스 개요

IStep은 타임라인의 각 단계를 나타내는 클래스입니다.
- 오브젝트 활성화/비활성화 관리
- 단계 시작/완료/리셋/스킵 콜백
- 클리어 조건 정의

## IStep 생성

```lua
IStep = ImportLuaScript(IStep)

local step = IStep:new(
    activeObjects,      -- 활성화할 오브젝트 테이블
    inactiveObjects,    -- 비활성화할 오브젝트 테이블
    time,               -- 타임라인 시간 (초)
    stepNumber,         -- 스텝 번호
    clearCondition,     -- 클리어 조건 문자열
    onStepStart,        -- 시작 콜백 테이블
    onStepReset,        -- 리셋 콜백 테이블
    onStepSkip          -- 스킵 콜백 테이블
)
```

## 사용 예시

### 기본 스텝 생성
```lua
IStep = ImportLuaScript(IStep)

---@type table
local steps = {}

function initSteps()
    -- 스텝 1: 재료 잡기
    steps[1] = IStep:new(
        { ingredientObject, guideObject },  -- 활성화
        { previousObject },                  -- 비활성화
        0.0,                                 -- 타임라인 0초
        1,                                   -- 스텝 1
        "grabIngredient",                    -- 클리어 조건
        { onStep1Start },                    -- 시작 콜백
        { onStep1Reset },                    -- 리셋 콜백
        nil                                  -- 스킵 콜백 없음
    )

    -- 스텝 2: 재료 자르기
    steps[2] = IStep:new(
        { knifeObject, cuttingBoardObject },
        { guideObject },
        5.0,                                 -- 타임라인 5초
        2,
        "cutIngredient",
        { onStep2Start },
        nil,
        nil
    )
end
```

### 매개변수가 있는 콜백
```lua
-- 콜백 함수
function setToolActive(tool, isActive)
    tool:SetActive(isActive)
end

-- 스텝 생성 (매개변수 포함)
steps[3] = IStep:new(
    nil, nil, 10.0, 3, "useToolComplete",
    { { setToolActive, toolObject, true } },   -- {함수, 매개변수1, 매개변수2}
    { { setToolActive, toolObject, false } },
    nil
)
```

## IStep 메서드

### OnStepStart()
스텝 시작 시 호출됩니다.
1. inactiveObjects 비활성화 (Grabbable이면 Release 호출)
2. activeObjects 활성화
3. onStepStart 콜백 실행

```lua
function startStep(stepIndex)
    steps[stepIndex]:OnStepStart()
end
```

### OnStepClear()
스텝 완료 시 호출됩니다.
```lua
function clearStep(stepIndex)
    steps[stepIndex]:OnStepClear()
end
```

### OnStepReset()
스텝 리셋 시 호출됩니다.
```lua
function resetStep(stepIndex)
    steps[stepIndex]:OnStepReset()
end
```

### OnStepSkip()
스텝 스킵 시 호출됩니다.
```lua
function skipStep(stepIndex)
    steps[stepIndex]:OnStepSkip()
end
```

## 타임라인 매니저 연동

```lua
---@type TimelineManager
local timelineManager = nil

function awake()
    timelineManager = TimelineManagerObject:GetLuaComponent("TimelineManager")
end

-- 타임라인 시작
function startGame()
    timelineManager.StartTimeline(gameTimeline)
end

-- 타임라인 일시정지
function pauseGame()
    timelineManager.PauseTimeline()
end

-- 타임라인 재개
function resumeGame()
    timelineManager.ResumeTimeline()
end

-- 타임라인 중지
function stopGame()
    timelineManager.StopTimeline()
end

-- 특정 시간으로 이동
function skipToTime(time)
    timelineManager.SkipTimeline(time)
end

-- 일시정지 상태 확인
function isPaused()
    return timelineManager.IsTimelinePaused()
end
```

## 클리어 조건 처리

```lua
---@type number
local currentStepIndex = 1

function checkClearCondition(condition)
    local currentStep = steps[currentStepIndex]

    if currentStep.clearCondition == condition then
        currentStep:OnStepClear()

        -- 다음 스텝으로 이동
        currentStepIndex = currentStepIndex + 1
        if steps[currentStepIndex] ~= nil then
            steps[currentStepIndex]:OnStepStart()
        end
    end
end

-- 사용 예시
function onIngredientGrabbed()
    checkClearCondition("grabIngredient")
end

function onIngredientCut()
    checkClearCondition("cutIngredient")
end
```

사용자의 요청에 따라 적절한 IStep 구조와 게임 플로우를 설계해주세요.
