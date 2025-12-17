-- SyncVar 테스트용 Lua 스크립트
-- VivenCustomSyncView 컴포넌트가 필요합니다

-- 동기화할 변수들 미리 선언
testNumber = nil
testString = nil
testBool = nil
playerName = nil

-- 최초 동기화 시 작업
function initializeSync()
    -- 서버 혹은 플레이어로부터 동기화 데이터를 처음 받아올때 호출됩니다. 
    -- 해당 구현을 통해 최초 동기화를 위한 작업을 구현해 주세요. 
    print("[SyncVar Test] Sync 초기화 작업")
    printCurrentValues()
end

function awake()
    -- SyncVar 등록 (값 변경 시 콜백 함수와 함께)
    testNumber = SyncView:CreateLuaSyncVar("testNumber", 0)
    testNumber:AddListener(onNumberChanged)  -- 콜백 함수 등록
    testString = SyncView:CreateLuaSyncVar("testString", "초기값")
    testString:AddListener(onStringChanged)  -- 콜백 함수 등록
    testBool = SyncView:CreateLuaSyncVar("testBool", false)
    testBool:AddListener(onBoolChanged)  -- 콜백 함수 등록
    playerName = SyncView:CreateLuaSyncVar("playerName", "Anonymous")
    playerName:AddListener(onPlayerNameChanged)  -- 콜백 함수 등록

    print("[SyncVar Test] SyncVar 등록 완료")
end

-- SyncView 초기화 완료 시 호출
function onSyncViewInitialized()
    print("[SyncVar Test] SyncView 초기화 완료!")
    print("현재 값들:")
    print("[SyncVar Test] 소유권: " .. tostring(SyncView.IsMine))

    printCurrentValues()
end 

-- 소유권 변경 시 호출
function onOwnershipChanged(isMine)
    print("[SyncVar Test] 소유권 변경: " .. tostring(isMine))
    if isMine then
        print("소유권을 획득했습니다! 값을 변경할 수 있습니다.")
    else
        print("소유권을 잃었습니다. 읽기 전용 모드입니다.")
    end
end

-- 각 변수별 변경 콜백 함수들
function onNumberChanged(oldValue, newValue)
    print("[SyncVar] Number 변경: " .. tostring(oldValue) .. " → " .. tostring(newValue))
end

function onStringChanged(oldValue, newValue)
    print("[SyncVar] String 변경: '" .. tostring(oldValue) .. "' → '" .. tostring(newValue) .. "'")
end

function onBoolChanged(oldValue, newValue)
    print("[SyncVar] Bool 변경: " .. tostring(oldValue) .. " → " .. tostring(newValue))
end

function onPlayerNameChanged(oldValue, newValue)
    print("[SyncVar] PlayerName 변경: '" .. tostring(oldValue) .. "' → '" .. tostring(newValue) .. "'")
end

-- 테스트용 함수들 (소유권이 있을 때만 작동)
function incrementNumber()
    if not SyncView.IsMine then
        print("[SyncVar Test] 소유권이 없어서 값을 변경할 수 없습니다!")
        return
    end

    testNumber.Value = testNumber.Value + 1
    print("[SyncVar Test] Number 증가: " .. tostring(testNumber:Get()))
end

function changeString()
    if not SyncView.IsMine then
        print("[SyncVar Test] 소유권이 없어서 값을 변경할 수 없습니다!")
        return
    end

    local messages = {"안녕하세요", "테스트중", "동기화확인", "SyncVar작동"}
    testString:Set(messages[math.random(1, #messages)] .. "_" .. tostring(math.random(100, 999)))
    print("[SyncVar Test] String 변경: " .. testString:Get())
end

function toggleBool()
    if not SyncView.IsMine then
        print("[SyncVar Test] 소유권이 없어서 값을 변경할 수 없습니다!")
        return
    end

    testBool.Value = not testBool.Value
    print("[SyncVar Test] Bool 토글: " .. tostring(testBool.Value))
end

function changePlayerName()
    if not SyncView.IsMine then
        print("[SyncVar Test] 소유권이 없어서 값을 변경할 수 없습니다!")
        return
    end

    playerName = "Player_" .. tostring(math.random(1000, 9999))
    print("[SyncVar Test] PlayerName 변경: " .. playerName)
end

-- 현재 값들을 출력하는 함수
function printCurrentValues()
    print("=== 현재 SyncVar 값들 ===")
    print("testNumber: " .. tostring(testNumber.Value))
    print("testString: " .. tostring(testString.Value))
    print("testBool: " .. tostring(testBool.Value))
    print("playerName: " .. tostring(playerName.Value))
    print("소유권: " .. tostring(SyncView.IsMine))
    print("========================")
end

-- 소유권 요청
function requestOwnership()
    print("[SyncVar Test] 소유권 요청!")
    SyncView:RequestOwnership()
end

-- 자동 테스트 (소유권이 있을 때만 실행)
function autoTest()
    if not SyncView.IsMine then
        print("[SyncVar Test] 자동 테스트를 위해서는 소유권이 필요합니다!")
        return
    end

    print("[SyncVar Test] 자동 테스트 시작!")

    -- 3초마다 값들을 변경
    Timer.Every(3, function()
        if SyncView.IsMine then
            incrementNumber()
            changeString()
            toggleBool()
        end
    end)
end

-- Update 함수에서 키 입력 처리 (테스트용)
function update()
    -- 새로운 Input System 사용
    if Keyboard.current.digit1Key.wasPressedThisFrame then
        incrementNumber()
    elseif Keyboard.current.digit2Key.wasPressedThisFrame then
        changeString()
    elseif Keyboard.current.digit3Key.wasPressedThisFrame then
        toggleBool()
    elseif Keyboard.current.digit4Key.wasPressedThisFrame then
        changePlayerName()
    elseif Keyboard.current.pKey.wasPressedThisFrame then
        printCurrentValues()
    elseif Keyboard.current.oKey.wasPressedThisFrame then
        requestOwnership()
    elseif Keyboard.current.tKey.wasPressedThisFrame then
        autoTest()
    end
end

-- 도움말 출력
function start()
    print("=== SyncVar 테스트 도움말 ===")
    print("1키: Number 증가")
    print("2키: String 변경")
    print("3키: Bool 토글")
    print("4키: PlayerName 변경")
    print("P키: 현재 값들 출력")
    print("O키: 소유권 요청")
    print("T키: 자동 테스트 시작")
    print("=============================")
end