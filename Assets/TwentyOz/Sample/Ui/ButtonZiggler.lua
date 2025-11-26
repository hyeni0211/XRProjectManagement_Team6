-- 본 예제에서는 ButtonZiggler 오브젝트를 클릭하면 Shake 함수가 호출되어 Shake 효과를 줍니다.
-- 또한 Shake 함수가 호출될 때마다 Shake 카운트를 증가시키고 Room Prop을 통해 Shake 카운트를 전달합니다.
-- RoomProp은 다른 사용자들이 같은 Room에 접속하여 Shake 카운트를 확인할 수 있도록 합니다.

local TargetTextTmp
local count

function awake()
    -- Shake 함수를 Button 오브젝트의 onClick 이벤트에 등록합니다.
    self:GetComponent(typeof(Button)).onClick:AddListener(Shake)
    TargetTextTmp = TargetText.gameObject:GetComponent(typeof(TMP_Text))
    
    -- RoomProp을 통해 Shake 카운트를 전달하기 위해 callback을 등록합니다. 
    Room.RegisterRoomPropChanged("Shake", onChangedShakeProp)
    
    count = tonumber(Room.GetRoomProp("Shake"))
    if(count ~= nil) then
        -- RoomProp을 통해 Shake 카운트를 전달받아 초기화합니다.
        ChangedShakeCount(count)
    else
        count = 0
        -- Shake 카운트가 없을 경우 초기화합니다.
        Room.SetRoomProp("Shake", 0)
    end
end

---@details Shake 함수는 Shake 효과를 주는 함수입니다.
function Shake()
    self:DoShake(1, 100, 20)
    count = count + 1
    Room.SetRoomProp("Shake", count)
end

---@details RoomProp이 변경될 때마다 호출되는 함수입니다.
function onChangedShakeProp(value)
    if(value == nil) then
        return
    end
    ChangedShakeCount(value)
end

---@details Shake 카운트를 변경하는 함수입니다.
function ChangedShakeCount(value)
    TargetTextTmp.text = "Shake Count: (" .. value .. ")"
end